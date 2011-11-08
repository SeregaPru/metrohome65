using System;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Animations;
using Fleux.Core.GraphicsHelpers;
using MetroHome65.Widgets;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{
    [XmlType("param")]
    public class StoredParameter
    {
        [XmlAttribute]
        public String Name;

        [XmlAttribute]
        public String Value;

        public StoredParameter() { }

        public StoredParameter(String Name, String Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }

  
    /// <summary>
    /// Container for widget, middle layer between widget grid and widget plugin.
    /// </summary>
    [Serializable]
    public class WidgetWrapper : UIElement
    {
        public static int CellWidth = ScreenRoutines.Scale(81);
        public static int CellHeight = CellWidth;
        public static int CellSpacingHor = ScreenRoutines.Scale(12);
        public static int CellSpacingVer = CellSpacingHor;

        private IWidget _Widget = null;
        private Point _GridPosition = new Point(0, 0);
        private Size _GridSize = new Size(1, 1);
        private bool _Moving = false;

        // double buffer
        private Bitmap _DoubleBuffer = null;
        private Graphics _graphics = null;
        private bool _needRepaint = true;


        //[XmlIgnore]
        //public WidgetGrid WidgetGrid = null;


        public WidgetWrapper() : base()
        {
            //
        }

        public WidgetWrapper(Size AGridSize, Point AGridPosition, String AWidgetName)
            : base()
        {
            this.WidgetClass = AWidgetName;
            this.GridSize = AGridSize;
            this.GridPosition = AGridPosition;

            if (Widget is IWidgetUpdatable)
                (Widget as IWidgetUpdatable).WidgetUpdate += OnWidgetUpdate;

            this.TapHandler += p => { OnClick(p); return true; };
            this.DoubleTapHandler += p => { OnDblClick(p); return true; };

        }

        ~WidgetWrapper()
        {
            ClearBuffer();
        }

        /// <summary>
        /// Widget location in grid
        /// </summary>
        public Point GridPosition { get { return _GridPosition; } set { SetGridPosition(value); } }

        public WidgetWrapper SetGridPosition(Point value)
        {
            _GridPosition = value;
            CalcScreenPosition();
            return this;
        }


        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Size GridSize { get { return _GridSize; } set { SetGridSize(value); } }

        /// <summary>
        /// Sets widget size in grid cells.
        /// Checks if specified size contain in widget's possible sizes.
        /// If not, size will be first available widget size.
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public WidgetWrapper SetGridSize(Size value)
        {
            if (_Widget == null) 
                return this;

            Boolean SizeOk = false;
            if ((_Widget.Sizes != null) && (_Widget.Sizes.Length > 0))
            {
                foreach (Size PossibleSize in _Widget.Sizes)
                {
                    if (PossibleSize.Equals(value))
                    {
                        _GridSize = value;
                        SizeOk = true;
                        break;
                    }
                }
            }

            if (!SizeOk) 
                if ((_Widget.Sizes != null) && (_Widget.Sizes.Length > 0))
                {
                    _GridSize = _Widget.Sizes[0];
                    //!! write to log
                }
                else
                {
                    _GridSize = new Size(2, 2);
                    //!! write to log
                }

            _Widget.Size = _GridSize;

            CalcScreenPosition();
            return this;
        }

        public void CalcScreenPosition()
        {
            Location = new Point(
                _GridPosition.X * (CellWidth + CellSpacingHor) + 30,
                _GridPosition.Y * (CellHeight + CellSpacingVer) + 5);
            Size = new Size(
                _GridSize.Width * (CellWidth + CellSpacingHor) - CellSpacingHor,
                _GridSize.Height * (CellHeight + CellSpacingVer) - CellSpacingVer );
        }


        [XmlIgnore]
        public IWidget Widget { get { return _Widget; } }

        /// <summary>
        /// Property for serialize widget class name.
        /// When deserialized Widget will be created
        /// </summary>
        [XmlAttribute]
        public String WidgetClass
        {
            // return Widget's class name
            get { return (_Widget != null) ? ((object)Widget).GetType().ToString() : ""; }

            // create new widget instance by class name
            set
            {
                if (WidgetClass != value)
                {
                    _Widget = PluginManager.GetInstance().CreateWidget(value);
                    FillWidgetProperties();
                    this.Active = true;
                }
            }
        }


        /// <summary>
        /// Swithes off widget activity when application goes to background
        /// </summary>
        [XmlIgnore]
        public Boolean Active { 
            set {
                if (_Widget is IWidgetUpdatable)
                {
                    if (value)
                        (_Widget as IWidgetUpdatable).StartUpdate();
                    else
                        (_Widget as IWidgetUpdatable).StopUpdate();
                }
            }
        }


        private List<PropertyInfo> _propertyInfos = new List<PropertyInfo>();

        private void FillWidgetProperties()
        {
            _propertyInfos.Clear();

            if (_Widget == null) return;

            // get widget properties
            foreach (PropertyInfo propertyInfo in ((object)_Widget).GetType().GetProperties())
                if (propertyInfo.GetCustomAttributes(typeof(WidgetParameterAttribute), true).Length > 0)
                {
                    _propertyInfos.Add(propertyInfo);
                }
        }


        public List<StoredParameter> Parameters = null;

        public void BeforeSerialize()
        {
            Parameters = new List<StoredParameter>();
            foreach (PropertyInfo prop in _propertyInfos)
                Parameters.Add(new StoredParameter(prop.Name, prop.GetValue((object)Widget, null).ToString()));
        }

        public void AfterDeserialize()
        {
            foreach (StoredParameter param in Parameters)
                SetParameter(param.Name, param.Value);
        }

        public WidgetWrapper SetParameter(String Name, object Value)
        {
            foreach (PropertyInfo propertyInfo in _propertyInfos)
            {
                if (propertyInfo.Name == Name)
                {
                    propertyInfo.SetValue((object)Widget, Convert.ChangeType(Value, propertyInfo.PropertyType, null), null);
                    //propertyInfo.SetValue((object)Widget, Value, null);
                    break;
                }
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
            if (_DoubleBuffer != null)
            {
                _DoubleBuffer.Dispose();
                _DoubleBuffer = null;
            }
        }

        private void PaintBuffer()
        {
            ClearBuffer();

            _DoubleBuffer = new Bitmap(Size.Width, Size.Height);
            _graphics = Graphics.FromImage(_DoubleBuffer);
            Rectangle Rect = new Rectangle(0, 0, _DoubleBuffer.Width, _DoubleBuffer.Height);

            if (Widget != null)
            {
                Widget.Paint(_graphics, Rect);
            }
            else
            {
                Pen Pen = new Pen(Color.Gray, 1);
                Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                _graphics.DrawRectangle(Pen, Rect);
                _graphics.DrawString("Widget\nnot\nfound", new System.Drawing.Font("Verdana", 8, FontStyle.Regular),
                    new SolidBrush(Color.Yellow), Rect.X + 5, Rect.Y + 5);
            }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (_needRepaint)
            {
                PaintBuffer();
                _needRepaint = false;
            }

            drawingGraphics.DrawImage(_DoubleBuffer, 0, 0, Bounds.Width, Bounds.Height);
        }


        public bool OnClick(Point ClickLocation)
        {
            if (Widget != null)
                return Widget.OnClick(ClickLocation);
            else
                return false;
        }

        public bool OnDblClick(Point ClickLocation)
        {
            if (Widget != null)
                return Widget.OnClick(ClickLocation);
            else
                return false;
        }

        /// <summary>
        /// Handler for event, triggered by widget, when it needs to be repainted
        /// </summary>
        /// <param name="sender"></param>
        private void OnWidgetUpdate(object sender, WidgetUpdateEventArgs e)
        {
            this._needRepaint = true;
            this.Update();
        }


        /// <summary>
        /// Flag when widget is in moving mode
        /// </summary>
        [XmlIgnore]
        public bool Moving {
            get { return _Moving; }
            set {
                if (_Moving != value)
                {
                    _Moving = value;
                    Active = !_Moving;
                }
            }
        }

    }
}
