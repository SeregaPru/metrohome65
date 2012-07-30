using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Base class for abstract transparent widget.
    /// </summary>
    public abstract class TransparentWidget : BaseWidget
    {
        private int _tileColor = Color.Empty.ToArgb();
        private String _tileImage = "";
        private AlphaImage _bgImage;

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
            set
            {
                SetTileImage(value);
            }
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
        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            // if button image is set, draw button image
            if (_bgImage != null)
            {
                _bgImage.PaintBackground(g, rect);
            }
            else
            {
                // if image is not set, draw solid box with specified color
                var tileColor = (_tileColor == Color.Empty.ToArgb()) ? MetroTheme.PhoneAccentBrush : Color.FromArgb(_tileColor);
                g.FillRectangle(new SolidBrush(tileColor), rect);
            }
        }


        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var colorControl = new ColorSettingsControl(true)
                                   {
                                       Caption = "Tile color",
                                       ARGBValue = _tileColor,
                                   };
            controls.Add(colorControl);
            bindingManager.Bind(this, "TileColor", colorControl, "ARGBValue");

            var imgControl = new ImageSettingsControl()
                                 {
                                     Caption = "Button background", 
                                     Value = TileImage,
                                 };
            controls.Add(imgControl);
            bindingManager.Bind(this, "TileImage", imgControl, "Value");

            return controls;
        }

    }

}
