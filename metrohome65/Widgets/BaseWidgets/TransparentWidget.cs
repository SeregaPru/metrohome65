using System;
using System.Drawing;
using System.Windows.Forms;
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
        private int _tileColor = Color.DeepSkyBlue.ToArgb();
        private String _tileImage = "";
        private AlphaImage _bgImage;

        protected override Boolean GetTransparent() { return true; }


        /// <summary>
        /// backround button or solid box color
        /// </summary>
        [TileParameter]
        public int TileColor
        {
            get { return _tileColor; }
            set {
                if (_tileColor != value)
                {
                    _tileColor = value;
                    NotifyPropertyChanged("TileColor");
                }
            }
        }


        /// <summary>
        /// widget background image
        /// </summary>
        [TileParameter]
        public String TileImage
        {
            get { return _tileImage; }
            set { SetTileImage(value); }
        }

        public void SetTileImage(String imagePath)
        {
            if (_tileImage == imagePath) return;

            _tileImage = imagePath;
            _bgImage = String.IsNullOrEmpty(imagePath) ? null : new AlphaImage(imagePath);

            NotifyPropertyChanged("TileImage");
        }


        /// <summary>
        /// Paints backgroud for transparent widget.
        /// Style and color are user-defined.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public override void Paint(Graphics g, Rectangle rect)
        {
            // if button image is set, draw button image
            if (_bgImage != null)
            {
                _bgImage.PaintBackground(g, rect);
            }
            else
            {
                // if image is not set, draw solid box with specified color
                Brush bgBrush = new SolidBrush(Color.FromArgb(_tileColor));
                g.FillRectangle(bgBrush, rect.Left, rect.Top, rect.Width, rect.Height);
            }
        }


        public override List<Control> EditControls
        {
            get
            {
                var controls = base.EditControls;

                var colorControl = new Settings_color
                                       {
                                           Value = _tileColor
                                       };
                controls.Add(colorControl);

                var imgControl = new Settings_image
                                     {
                                         Caption = "Button background", 
                                         Value = TileImage
                                     };
                controls.Add(imgControl);

                var bindingManager = new BindingManager();
                bindingManager.Bind(this, "TileColor", colorControl, "Value");
                bindingManager.Bind(this, "TileImage", imgControl, "Value");

                return controls;
            }
        }

    }

}
