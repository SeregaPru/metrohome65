using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces.Events;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen
{
    public class HomeScreenBackground : UIElement
    {
        private Image _image;

        public HomeScreenBackground()
        {
            Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);

            SetImage(GetImage());
        }

        protected void SetImage(string imagePath)
        {
            if (_image != null)
                _image.Dispose();

            _image = PrepareBgImage(imagePath);
        }

        private Image PrepareBgImage(string imagePath)
        {
            if (String.IsNullOrEmpty(imagePath))
                return null;
            imagePath = imagePath.Trim();
            if (String.IsNullOrEmpty(imagePath))
                return null;

            Bitmap image = null;
            try
            {
                var srcImage = new Bitmap(imagePath);

                // вычисляем общий коэффициент масштабирования изображения с учетом пропорций
                var scaleX = 1.0 * Size.Width / srcImage.Width;
                var scaleY = 1.0 * Size.Height / srcImage.Height;
                var scale = Math.Max(scaleX, scaleY);
                var destRect = new Rectangle(0, 0, (int) Math.Round(srcImage.Width*scale),
                                             (int) Math.Round(srcImage.Height*scale));

                image = new Bitmap(Size.Width, Size.Height, PixelFormat.Format16bppRgb565);
                var graphic = Graphics.FromImage(image);
                graphic.DrawImage(srcImage, destRect,
                                  new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
            } catch { }

            return image;
        }

        /// <summary>
        /// Draw only background image.
        /// If no background image is specified, background color is specified for owner control
        /// </summary>
        /// <param name="drawingGraphics"></param>
        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if (_image != null)
                drawingGraphics.Graphics.DrawImage(_image, drawingGraphics.CalculateX(0), 0);
        }

        protected virtual string GetImage()
        {
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            return mainSettings.ThemeImage;
        }

        protected virtual void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "ThemeImage")
            {
                SetImage(GetImage());
                Update();
            }
        }

    }
}
