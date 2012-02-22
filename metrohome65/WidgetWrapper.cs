using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Fleux.UIElements;
using Fleux.Core.GraphicsHelpers;
using MetroHome65.Widgets;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{
  
    /// <summary>
    /// Container for widget, middle layer between widget grid and widget plugin.
    /// </summary>
    public class WidgetWrapper : UIElement
    {
        #region Fields

        public static int CellWidth = ScreenRoutines.Scale(81);
        public static int CellHeight = CellWidth;
        public static int CellSpacingHor = ScreenRoutines.Scale(12);
        public static int CellSpacingVer = CellSpacingHor;

        private IWidget _widget;
        private Point _gridPosition = new Point(0, 0);
        private Size _gridSize = new Size(1, 1);
        private bool _moving = false;

        // double buffer
        private Bitmap _doubleBuffer;
        private Graphics _graphics;
        private bool _needRepaint = true;

        private readonly List<PropertyInfo> _propertyInfos = new List<PropertyInfo>();

        private Timer _movingTimer;

        #endregion


        #region Methods

        // empty constructor for deserialize
        public WidgetWrapper() : base() {}

        public WidgetWrapper(Size aGridSize, Point aGridPosition, String aWidgetName)
            : base()
        {
            WidgetClass = aWidgetName;
            GridSize = aGridSize;
            GridPosition = aGridPosition;
        }

        ~WidgetWrapper()
        {
            ClearBuffer();
        }

        /// <summary>
        /// Widget location in grid
        /// </summary>
        public Point GridPosition { 
            get { return _gridPosition; } 
            set {
                if (_gridPosition == value) return;

                _gridPosition = value;
                CalcScreenPosition();
            }
        }


        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Size GridSize { get { return _gridSize; } set { SetGridSize(value); } }

        /// <summary>
        /// Sets widget size in grid cells.
        /// Checks if specified size contain in widget's possible sizes.
        /// If not, size will be first available widget size.
        /// </summary>
        /// <param name="value"> </param>
        /// <returns></returns>
        private void SetGridSize(Size value)
        {
            if (_widget == null)
                return;

            var sizeOk = false;
            if ((_widget.Sizes != null) && (_widget.Sizes.Length > 0))
            {
                foreach (var possibleSize in _widget.Sizes)
                {
                    if (possibleSize.Equals(value))
                    {
                        _gridSize = value;
                        sizeOk = true;
                        break;
                    }
                }
            }

            if (!sizeOk) 
                if ((_widget.Sizes != null) && (_widget.Sizes.Length > 0))
                {
                    _gridSize = _widget.Sizes[0];
                }
                else
                {
                    _gridSize = new Size(2, 2);
                }

            _widget.Size = _gridSize;

            CalcScreenPosition();
        }

        private void CalcScreenPosition()
        {
            var screenRect = GetScreenRect();
            Location = screenRect.Location;
            Size = screenRect.Size;
        }

        public Rectangle GetScreenRect()
        {
            return new Rectangle(
                _gridPosition.X * (CellWidth + CellSpacingHor) + 30,
                _gridPosition.Y * (CellHeight + CellSpacingVer) + 5,
                _gridSize.Width * (CellWidth + CellSpacingHor) - CellSpacingHor,
                _gridSize.Height * (CellHeight + CellSpacingVer) - CellSpacingVer);
        }

        private void FillWidgetProperties()
        {
            _propertyInfos.Clear();

            if (_widget == null) return;

            // get widget properties
            foreach (var propertyInfo in (_widget).GetType().GetProperties())
                if (propertyInfo.GetCustomAttributes(typeof(WidgetParameterAttribute), true).Length > 0)
                {
                    _propertyInfos.Add(propertyInfo);
                }
        }

        #endregion


        #region Properties

        public IWidget Widget { get { return _widget; } }

        /// <summary>
        /// Property for serialize widget class name.
        /// When deserialized Widget will be created
        /// </summary>
        public String WidgetClass
        {
            // return Widget's class name
            get { return (_widget != null) ? (Widget).GetType().ToString() : ""; }

            // create new widget instance by class name
            set
            {
                if (WidgetClass == value) return;

                _widget = PluginManager.GetInstance().CreateWidget(value);
                FillWidgetProperties();

                if (Widget is IUpdatable)
                    (Widget as IUpdatable).OnUpdate += OnWidgetUpdate;

                TapHandler += p => { OnClick(p); return true; };
                DoubleTapHandler += p => { OnDblClick(p); return true; };

                Active = true;
            }
        }


        /// <summary>
        /// Swithes off widget activity when application goes to background
        /// </summary>
        public Boolean Active { 
            set {
                if (_widget is IUpdatable)
                {
                    (_widget as IUpdatable).Active = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// prepare struct for serialization and store settings
        /// </summary>
        /// <returns></returns>
        public WidgetWrapperSettings SerializeSettings()
        {
            var settings = new WidgetWrapperSettings
                               {
                                   Size = GridSize,
                                   Location = GridPosition,
                                   WidgetClass = WidgetClass,
                                   Parameters = new List<StoredParameter>()
                               };

            foreach (var prop in _propertyInfos)
                settings.Parameters.Add(new StoredParameter(prop.Name, prop.GetValue(Widget, null).ToString()));

            return settings;
        }

        /// <summary>
        /// parse settings struct read from settings file and apply these settings to widget
        /// </summary>
        /// <param name="settings"></param>
        public void DeserializeSettings(WidgetWrapperSettings settings)
        {
            WidgetClass = settings.WidgetClass;
            GridSize = settings.Size;
            GridPosition = settings.Location;

            foreach (var param in settings.Parameters)
                SetParameter(param.Name, param.Value);
        }

        public WidgetWrapper SetParameter(String name, object value)
        {
            foreach (var propertyInfo in _propertyInfos.Where(propertyInfo => propertyInfo.Name == name))
            {
                propertyInfo.SetValue(Widget, Convert.ChangeType(value, propertyInfo.PropertyType, null), null);
                //propertyInfo.SetValue((object)Widget, Value, null);
                break;
            }
            return this;
        }

        private void ClearBuffer()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_doubleBuffer != null)
            {
                _doubleBuffer.Dispose();
                _doubleBuffer = null;
            }
        }

        private void PaintBuffer()
        {
            ClearBuffer();

            _doubleBuffer = new Bitmap(Size.Width, Size.Height);
            _graphics = Graphics.FromImage(_doubleBuffer);
            var paintRect = new Rectangle(0, 0, _doubleBuffer.Width, _doubleBuffer.Height);

            if (Widget != null)
            {
                Widget.Paint(_graphics, paintRect);
            }
            else
            {
                var pen = new Pen(Color.Gray, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                _graphics.DrawRectangle(pen, paintRect);
                _graphics.DrawString("Widget\nnot\nfound", new Font("Verdana", 8, FontStyle.Regular),
                    new SolidBrush(Color.Yellow), paintRect.X + 5, paintRect.Y + 5);
            }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (_needRepaint)
            {
                PaintBuffer();
                _needRepaint = false;
            }

            if (drawingGraphics == null)
                return;

            // for moving mode - change size
            if (_moving)
            {
                var paintRect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
                paintRect.Inflate(_deltaX, _deltaY);
                drawingGraphics.DrawImage(_doubleBuffer, paintRect);
            }
            else
            {
                drawingGraphics.Graphics.DrawImage(_doubleBuffer, -drawingGraphics.VisibleRect.Left, -drawingGraphics.VisibleRect.Top);
            }
        }

        public void ForceUpdate()
        {
            _needRepaint = true;
            Update();
            System.Windows.Forms.Application.DoEvents();
        }

        /// <summary>
        /// Handler for event, triggered by widget, when it needs to be repainted
        /// </summary>
        private void OnWidgetUpdate()
        {
            ForceUpdate();
        }

        public bool OnClick(Point clickLocation)
        {
            return (Widget != null) && Widget.OnClick(clickLocation);
        }

        public bool OnDblClick(Point clickLocation)
        {
            return (Widget != null) && Widget.OnClick(clickLocation);
        }

        /// <summary>
        /// Flag when widget is in moving mode
        /// </summary>
        public bool Moving {
            get { return _moving; }
            set {
                if (_moving != value)
                {
                    _moving = value;

                    if (value)
                    {
                        if (_movingTimer == null)
                        {
                            _movingTimer = new Timer() { Interval = 300 };
                            _movingTimer.Tick += (s, e) => RepaintMovingWidget();
                        }
                        _movingTimer.Enabled = true;
                    }
                    else
                    {
                        if (_movingTimer != null)
                            _movingTimer.Enabled = false;
                        Update();
                    }
                }
            }
        }

        private int _deltaX = 0;
        private int _deltaY = -2;
        private int _deltaXInc = 2;
        private int _deltaYInc = -2;


        private void RepaintMovingWidget()
        {
            if (!_moving)
                return;

            if ((_deltaX >= 0) || (_deltaX <= -5))
                _deltaXInc = -_deltaXInc;
            _deltaX += _deltaXInc;
            if ((_deltaY >= 0) || (_deltaY <= -5))
                _deltaYInc = -_deltaYInc;
            _deltaY += _deltaYInc;

            // paint widget
            Update();

            //System.Windows.Forms.Application.DoEvents();
        }


    }
}
