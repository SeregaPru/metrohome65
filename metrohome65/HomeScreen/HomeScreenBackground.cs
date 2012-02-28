using System;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;

namespace MetroHome65.HomeScreen
{
    public class HomeScreenBackground : UIElement
    {
        private Image _image;

        private Color _bgColor;

        public HomeScreenBackground(MainSettings mainSettings)
        {
            Size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

            mainSettings.PropertyChanged += OnMainSettingsChanged;

            SetBgColor(mainSettings.ThemeColor);
            SetImage(mainSettings.ThemeImage.Trim());
        }

        private Image PrepareBgImage(string imagePath)
        {
            if (String.IsNullOrEmpty(imagePath))
                return null;

            var srcImage = new Bitmap(imagePath);

            // вычисляем общий коэффициент масштабирования изображения с учетом пропорций
            var scaleX = 1.0 * Size.Width / srcImage.Width;
            var scaleY = 1.0 * Size.Height / srcImage.Height;
            var scale = Math.Max(scaleX, scaleY);
            var destRect = new Rectangle(0, 0, (int)Math.Round(srcImage.Width * scale),
                                         (int)Math.Round(srcImage.Height * scale));

            var image = new Bitmap(Size.Width, Size.Height);
            var graphic = Graphics.FromImage(image);
            graphic.DrawImage(srcImage, destRect,
                              new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);
            return image;
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if (_image != null)
                drawingGraphics.Graphics.DrawImage(_image, 0, 0);
            else
            {
                drawingGraphics.Color(_bgColor);
                drawingGraphics.FillRectangle(Bounds);
            }
        }

        private void SetImage(string imagePath)
        {
            _image = PrepareBgImage(imagePath);
        }

        private void SetBgColor(Color color)
        {
            _bgColor = color;
        }

        private void OnMainSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var mainSettings = sender as MainSettings;
            if (mainSettings == null) return;

            if (e.PropertyName == "ThemeImage")
            {
                SetImage(mainSettings.ThemeImage);
                Update();
            }

            if (e.PropertyName == "ThemeColor")
            {
                SetBgColor(mainSettings.ThemeColor);
                Update();
            }
        }

    }
}
