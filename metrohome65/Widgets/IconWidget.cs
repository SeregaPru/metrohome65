using System;
using System.Drawing;
using System.Windows.Forms;
using OpenNETCF.Drawing;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Class for transparent widget with icon and caption.
    /// </summary>
    public class IconWidget : TransparentWidget
    {
        //Эти переменные понядобятся для загрузки изображений при запуске приложения.
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;
        private String _Caption = "";
        private String _IconPath = "";


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
                try
                {
                    _IconPath = value;
                    if (_IconPath != "")
                        _factory.CreateImageFromFile(value, out _img);
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "SetIconPath")
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

        protected void PaintIcon(Graphics g, Rectangle Rect)
        {
            // draw icon
            if (_img != null)
            {
                try
                {
                    Size IconSize = new Size(92, 90);

                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                        (Rect.Left + Rect.Right - IconSize.Width) / 2,
                        (Rect.Top + Rect.Bottom - IconSize.Height - 10) / 2, IconSize.Width, IconSize.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "PaintIcon")
                }
            }
        }


        protected void PaintCaption(Graphics g, Rectangle Rect)
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

    }

}
