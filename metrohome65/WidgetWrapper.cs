using System;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace SmartDeviceProject1
{
    [XmlType("param")]
    public class StoredParameter
    {
        public String Name;
        public String Value;
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
        public static int CellSpacingVer = 12;
        public static int CellSpacingHor = 12;
        public static int PaddingVer = 12;
        public static int PaddingHor = 38;

        /// <summary>
        /// Widget position in grid
        /// </summary>
        public Point Position = new Point(0, 0);

        /// <summary>
        /// Widget size in cells
        /// </summary>
        public Size Size = new Size(1, 1);

        /// <summary>
        /// Swithes off widget activity when application goes to background
        /// </summary>
        [XmlIgnore]
        public Boolean Active { 
            set {
                if (Widget is IWidgetUpdatable)
                {
                    if (value)
                        (Widget as IWidgetUpdatable).StartUpdate();
                    else
                        (Widget as IWidgetUpdatable).StopUpdate();
                }
            }
        }

        /// <summary>
        /// backround button or solid box color
        /// </summary>
        public Color Color { get { return _Color; } set { SetColor(value); } }
        private Color _Color = System.Drawing.Color.Blue;
        public WidgetWrapper SetColor(Color Color)
        {
            _Color = Color;
            return this;
        }

        public WidgetWrapper SetButtonImage(String ImagePath)
        {
            try
            {
                if ((ImagePath != "") && (ImagePath != null))
                    _factory.CreateImageFromFile(ImagePath, out _img);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace, "SetBtnImg");
            }

            return this;
        }

        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;


        [XmlIgnore]
        public IWidget Widget { get { return _Widget; } }
        private IWidget _Widget = null;

        /// <summary>
        /// Property for serialize widget class name.
        /// When deserialized Widget will be created
        /// </summary>
        public String WidgetClass {
            get { return ((object)Widget).GetType().ToString(); } 
            set { 
            } 
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
                    propertyInfo.SetValue((object)Widget, Value, null);
            }
            return this;
        }


        public WidgetWrapper(Size Size, Point Position, IWidget Widget)
        {
            this.Size = Size;
            this.Position = Position;
            this._Widget = Widget;

            ScreenRect.X = Position.X * (CellWidth + CellSpacingHor) + PaddingHor;
            ScreenRect.Y = Position.Y * (CellHeight + CellSpacingVer) + PaddingVer;
            ScreenRect.Width = Size.Width * (CellWidth + CellSpacingHor) - CellSpacingHor;
            ScreenRect.Height = Size.Height * (CellHeight + CellSpacingVer) - CellSpacingVer;

            // get widget properties
            foreach (PropertyInfo propertyInfo in ((object)Widget).GetType().GetProperties())                
                if (propertyInfo.GetCustomAttributes(typeof(WidgetParameter), true).Length > 0)
                {
                    _propertyInfos.Add(propertyInfo);
                }                
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
            // if button image is set< draw button image
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
                    MessageBox.Show(e.StackTrace, "PaintBackground");
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
    }
}
