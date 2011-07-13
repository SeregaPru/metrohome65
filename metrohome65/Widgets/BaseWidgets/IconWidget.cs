using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using OpenNETCF.Drawing;
using MetroHome65.Pages;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Class for transparent widget with icon and caption.
    /// </summary>
    public abstract class IconWidget : TransparentWidget
    {
        //Эти переменные понядобятся для загрузки изображений при запуске приложения.
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;
        private String _Caption = "";
        private String _IconPath = "";
        private Size _IconSize = new Size(92, 90);


        /// <summary>
        /// user defined caption for widget
        /// </summary>
        [WidgetParameter]
        public String Caption { get { return _Caption; } set { _Caption = value; } }


        /// <summary>
        /// relative or absolute path to icon file.
        /// icon format must be transparent PNG
        /// </summary>
        [WidgetParameter]
        public String IconPath
        {
            get { return _IconPath; }
            set
            {
                if (_IconPath != value)
                {
                    _IconPath = value;
                    UpdateIconImage();
                }
            }
        }

        protected override Size[] GetSizes() 
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        protected virtual void UpdateIconImage()
        {
            try
            {
                if (_IconPath != "")
                    _factory.CreateImageFromFile(_IconPath, out _img);
                else
                    _img = null;
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetIconPath")
            }
        }

        protected virtual void PaintIcon(Graphics g, Rectangle Rect)
        {
            // draw icon
            if (_img != null)
            {
                try
                {
                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                        (Rect.Left + Rect.Right - _IconSize.Width) / 2,
                        (Rect.Top + Rect.Bottom - _IconSize.Height - 10) / 2, _IconSize.Width, _IconSize.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "PaintIcon")
                }
            }
        }


        protected virtual void PaintCaption(Graphics g, Rectangle Rect)
        {
            // draw caption
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Verdana", 8, FontStyle.Regular);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(Caption, captionFont, captionBrush,
                    Rect.Left + 10, Rect.Bottom - 5 - g.MeasureString(Caption, captionFont).Height);
            }
        }


        /// <summary>
        /// Paints icon and caption over standart backround (user defined button)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);
            PaintIcon(g, Rect);
            PaintCaption(g, Rect);
        }


        public override void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("Icon widget {0} at pos {1}:{2}",
                this.Caption, Location.X, Location.Y));
        }


        public override Control[] EditControls
        {
            get
            {
                Control[] Controls = new Control[2];
                Settings_string EditControl = new Settings_string();
                EditControl.Caption = "Caption";
                EditControl.Value = Caption;
                EditControl.OnValueChanged += new Settings_string.ValueChangedHandler(EditControl_OnValueChanged);
                Controls[0] = EditControl;

                Settings_image ImgControl = new Settings_image();
                ImgControl.Caption = "Icon image";
                ImgControl.Value = IconPath;
                ImgControl.OnValueChanged += new Settings_image.ValueChangedHandler(ImgControl_OnValueChanged);
                Controls[1] = ImgControl;

                return Controls;
            }
        }

        void ImgControl_OnValueChanged(string Value)
        {
            IconPath = Value;
        }

        void EditControl_OnValueChanged(string Value)
        {
            Caption = Value;
        }


    }

}
