﻿using System;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using MetroHome65.Widgets;

namespace MetroHome65.Pages
{
    [XmlType("param")]
    public class StoredParameter
    {
        [XmlAttribute]
        public String Name;

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
    public class WidgetWrapper
    {
        public static int CellWidth = 81;
        public static int CellHeight = 81;
        public static int CellSpacingHor = 12;
        public static int CellSpacingVer = 12;

        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;
        private IWidget _Widget = null;
        private Color _Color = System.Drawing.Color.Blue;
        private Point _Location = new Point(0, 0);
        private Size _Size = new Size(1, 1);
        private String _ButtonImage = "";

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
            UpdateScreenPosition();
            return this;
        }


        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Size Size { get { return _Size; } set { SetSize(value); } }

        /// <summary>
        /// Sets widget size in grid cells.
        /// Checks if specified size contain in widget's possible sizes.
        /// If not, size will be first available widget size.
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public WidgetWrapper SetSize(Size Size)
        {
            Boolean SizeOk = false;
            foreach (Size size in _Widget.Sizes)
            {
                if (size.Equals(Size))
                {
                    _Size = Size;
                    SizeOk = true;
                    break;
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

            UpdateScreenPosition();
            return this;
        }

        private void UpdateScreenPosition()
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
        /// backround button or solid box color
        /// </summary>
        public Color Color { get { return _Color; } set { SetColor(value); } }

        public WidgetWrapper SetColor(Color Color)
        {
            _Color = Color;
            return this;
        }


        public String ButtonImage { get { return _ButtonImage; } set { SetButtonImage(value); } }

        public WidgetWrapper SetButtonImage(String ImagePath)
        {
            if (_ButtonImage != ImagePath)
            {
                _ButtonImage = ImagePath;
                try
                {
                    if ((ImagePath != "") && (ImagePath != null))
                        _factory.CreateImageFromFile(ImagePath, out _img);
                    else
                        _img = null;
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "SetBtnImg")
                }
            }

            return this;
        }


        /// <summary>
        /// Widget absolute position on screen and size (in pixels) 
        /// </summary>
        [XmlIgnore]
        public Rectangle ScreenRect = new Rectangle(0, 0, 0, 0);
        
        private List<PropertyInfo> _propertyInfos = new List<PropertyInfo>();

        public List<StoredParameter> Parameters
        {
            get {
                List<StoredParameter> parameters = new List<StoredParameter>();
                foreach (PropertyInfo prop in _propertyInfos)
                    parameters.Add(new StoredParameter(prop.Name, 
                        (String)prop.GetValue((object)Widget, null)));
                return parameters;
            }
            set {
                foreach (StoredParameter param in value)
                    SetParameter(param.Name, param.Value);
            }
        }

        public WidgetWrapper SetParameter(String Name, object Value)
        {
            foreach (PropertyInfo propertyInfo in _propertyInfos)
            {
                if (propertyInfo.Name == Name)
                {
                    propertyInfo.SetValue((object)Widget, Value, null);
                    break;
                }
            }
            return this;
        }

        public void Paint(Graphics g, Rectangle Rect)
        {
            if (Widget != null)
            {
                if (Widget.Transparent)
                    PaintBackground(g, Rect);
                Widget.Paint(g, Rect);
            }
        }

        /// <summary>
        /// Paints backgroud for transparent widget.
        /// Style and color are user-defined.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        private void PaintBackground(Graphics g, Rectangle Rect)
        {
            // if button image is set, draw button image
            if (_img != null)
            {
                try
                {
                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(Rect.Left, Rect.Top, ScreenRect.Width, ScreenRect.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                    return;
                }
                catch (Exception e) {
                    //!! write to log  (e.StackTrace, "PaintBackground")
                }
            }

            // if image is not set, draw solid box with specified color
            Brush bgBrush = new System.Drawing.SolidBrush(Color);
            g.FillRectangle(bgBrush, Rect.Left, Rect.Top, Rect.Width, Rect.Height);
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
    }
}
