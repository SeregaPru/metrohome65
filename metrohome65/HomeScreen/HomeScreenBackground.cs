using System;
using System.Drawing;
using Fleux.UIElements;

namespace MetroHome65.HomeScreen
{
    public class HomeScreenBackground : UIElement
    {
        private readonly Image _image;

        private readonly Color _bgColor;

        public HomeScreenBackground(MainSettings mainSettings)
        {
            Size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

            _bgColor = mainSettings.ThemeColor;
            _image = PrepareBgImage(mainSettings.ThemeImage.Trim());
        }

        private Image PrepareBgImage(string ThemeImageFileName)
        {
            if (String.IsNullOrEmpty(ThemeImageFileName))
                return null;

            var srcImage = new Bitmap(ThemeImageFileName);

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
        }

    }
}
