﻿using System;
using System.Drawing;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using TinyIoC;

namespace MetroHome65.HomeScreen
{
    public class HomeScreenBackground : UIElement
    {
        private Image _image;

        public HomeScreenBackground()
        {
            Size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

            MetroTheme.PropertyChanged += OnThemeSettingsChanged;

            SetImage(MetroTheme.PhoneBackgroundImage);
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
                var scaleX = 1.0*Size.Width/srcImage.Width;
                var scaleY = 1.0*Size.Height/srcImage.Height;
                var scale = Math.Max(scaleX, scaleY);
                var destRect = new Rectangle(0, 0, (int) Math.Round(srcImage.Width*scale),
                                             (int) Math.Round(srcImage.Height*scale));

                image = new Bitmap(Size.Width, Size.Height);
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
                drawingGraphics.Graphics.DrawImage(_image, 0, 0);
        }

        private void SetImage(string imagePath)
        {
            if (_image != null)
                _image.Dispose();

            _image = PrepareBgImage(imagePath);
        }

        private void OnThemeSettingsChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhoneBackgroundImage")
            {
                SetImage(MetroTheme.PhoneBackgroundImage);
                Update();
            }
        }

    }
}
