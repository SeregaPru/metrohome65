﻿using System;
using System.Drawing;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.UIElements;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{
    public class FlatButton : UIElement
    {
        //private Bitmap _image;
        private AlphaImage _image;

        public string ResourceName
        {
            set
            {
                try
                {
                    //_image = ResourceManager.Instance.GetBitmapFromEmbeddedResource(value);
                    _image = new AlphaImage(value, this.GetType().Assembly);

                    this.Size = _image.Size.ToPixels(); // scale original image according to screen factor
                }
                catch (Exception) { }
                Update();
            }
        }

        public FlatButton(string resourceName)
        {
            ResourceName = resourceName;
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            //drawingGraphics.DrawText("*"); return;

            if (FleuxApplication.ScaleToLogic(1) == 1) // no need scale
                _image.PaintIcon(drawingGraphics.Graphics, drawingGraphics.CalculateX(0), drawingGraphics.CalculateY(0));
            else
                _image.PaintBackground(drawingGraphics.Graphics, new Rectangle(
                    drawingGraphics.CalculateX(0), drawingGraphics.CalculateY(0), Size.Width, Size.Height));

            /*
            Color transparentKeyColor = Color.Black; // DEFAULT BLACK, IF IT IS NOT POSSIBLE TO READ FROM IMAGE
            if (_image != null)
                transparentKeyColor = _image.GetPixel(0, 0);
            drawingGraphics.DrawImage(_image, 0, 0, Size.Width, Size.Height, transparentKeyColor);
            */
        }

    }
}