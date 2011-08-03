using System;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using MetroHome65.Widgets;
using MetroHome65.Routines;

namespace MetroHome65.Pages
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
    public class WidgetWrapper
    {
        public static int CellWidth = ScreenRoutines.Scale(81);
        public static int CellHeight = CellWidth;
        public static int CellSpacingHor = ScreenRoutines.Scale(12);
        public static int CellSpacingVer = CellSpacingHor;

        private IWidget _Widget = null;
        private Point _Location = new Point(0, 0);
        private Size _Size = new Size(1, 1);
        private bool _Moving = false;


        [XmlIgnore]
        public WidgetGrid WidgetGrid = null;


        public WidgetWrapper()
        {
        }

        public WidgetWrapper(Size Size, Point Location, String WidgetName)
        {
            this.WidgetClass = WidgetName;
            this.Size = Size;
            this.Location = Location;
        }

        /// <summary>
        /// Widget location in grid
        /// </summary>
        public Point Location { get { return _Location; } set { SetLocation(value); } }

        public WidgetWrapper SetLocation(Point Location)
        {
            _Location = Location;
            CalcScreenPosition();
            return this;
        }


        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Size Size { 
            get { return _Size; } 
            set { SetSize(value); } 
        }

        /// <summary>
        /// Sets widget size in grid cells.
        /// Checks if specified size contain in widget's possible sizes.
        /// If not, size will be first available widget size.
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public WidgetWrapper SetSize(Size Size)
        {
            if (_Widget == null) 
                return this;

            Boolean SizeOk = false;
            if ((_Widget.Sizes != null) && (_Widget.Sizes.Length > 0))
            {
                foreach (Size size in _Widget.Sizes)
                {
                    if (size.Equals(Size))
                    {
                        _Size = Size;
                        SizeOk = true;
                        break;
                    }
                }
            }

            if (!SizeOk) 
                if ((_Widget.Sizes != null) && (_Widget.Sizes.Length > 0))
                {
                    _Size = _Widget.Sizes[0];
                    //!! write to log
                }
                else
                {
                    _Size = new Size(2, 2);
                    //!! write to log
                }

            CalcScreenPosition();
            return this;
        }

        public void CalcScreenPosition()
        {
            ScreenRect.X = _Location.X * (CellWidth + CellSpacingHor);
            ScreenRect.Y = _Location.Y * (CellHeight + CellSpacingVer);
            ScreenRect.Width = _Size.Width * (CellWidth + CellSpacingHor) - CellSpacingHor;
            ScreenRect.Height = _Size.Height * (CellHeight + CellSpacingVer) - CellSpacingVer;
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


        /// <summary>
        /// Widget absolute position on screen and size (in pixels) 
        /// </summary>
        [XmlIgnore]
        public Rectangle ScreenRect = new Rectangle(0, 0, 0, 0);

        
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

        public void Paint(Graphics g, Rectangle Rect)
        {
            Region prevRegion = g.Clip;
            g.Clip = new Region(Rect);

            if (Widget != null)
            {
                Widget.Paint(g, Rect);
            }
            else
            {
                Pen Pen = new Pen(Color.Gray, 1);
                Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawRectangle(Pen, Rect);
                g.DrawString("Widget\nnot\nfound", new System.Drawing.Font("Verdana", 8, FontStyle.Regular),
                    new SolidBrush(Color.Yellow), Rect.X + 5, Rect.Y + 5);
            }
            g.Clip = prevRegion;
        }

        public void OnClick(Point Location)
        {
            if (Widget != null)
                Widget.OnClick(Location);
        }

        public void OnDblClick(Point Location)
        {
            if (Widget != null)
                Widget.OnClick(Location);
        }


        /// <summary>
        /// Flag when widget is in moving mode
        /// </summary>
        [XmlIgnore]
        public bool Moving {
            get { return _Moving; }
            set
            {
                if (_Moving != value)
                {
                    _Moving = value;
                    Active = !_Moving;
                }
            }
        }

    }
}
