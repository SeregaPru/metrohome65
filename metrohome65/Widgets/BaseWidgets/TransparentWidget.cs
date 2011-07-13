using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using MetroHome65.Routines;
using MetroHome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Base class for abstract transparent widget.
    /// </summary>
    public abstract class TransparentWidget : BaseWidget
    {
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;
        private int _TileColor = System.Drawing.Color.DeepSkyBlue.ToArgb();
        private String _TileImage = "";

        protected override Boolean GetTransparent() { return true; }


        /// <summary>
        /// backround button or solid box color
        /// </summary>
        [WidgetParameter]
        public int TileColor
        {
            get { return _TileColor; }
            set {
                if (_TileColor != value)
                {
                    _TileColor = value;
                    NotifyPropertyChanged("TileColor");
                }
            }
        }


        /// <summary>
        /// widget background image
        /// </summary>
        [WidgetParameter]
        public String TileImage
        {
            get { return _TileImage; }
            set { SetTileImage(value); }
        }

        public void SetTileImage(String ImagePath)
        {
            if (_TileImage != ImagePath)
            {
                _TileImage = ImagePath;
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

                NotifyPropertyChanged("TileImage");
            }
        }


        public override void Paint(Graphics g, Rectangle Rect)
        {
            PaintBackground(g, Rect);
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
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(Rect.Left, Rect.Top, Rect.Width, Rect.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                    return;
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "PaintBackground")
                }
            }

            // if image is not set, draw solid box with specified color
            Brush bgBrush = new System.Drawing.SolidBrush(Color.FromArgb(_TileColor));
            g.FillRectangle(bgBrush, Rect.Left, Rect.Top, Rect.Width, Rect.Height);
        }


        public override List<Control> EditControls
        {
            get
            {
                List<Control> Controls = base.EditControls;

                Settings_color ColorControl = new Settings_color();
                ColorControl.Value = _TileColor;
                Controls.Add(ColorControl);

                Settings_image ImgControl = new Settings_image();
                ImgControl.Caption = "Button background";
                ImgControl.Value = TileImage;
                Controls.Add(ImgControl);

                BindingManager BindingManager = new BindingManager();
                BindingManager.Bind(this, "TileColor", ColorControl, "Value");
                BindingManager.Bind(this, "TileImage", ImgControl, "Value");

                return Controls;
            }
        }

    }

}
