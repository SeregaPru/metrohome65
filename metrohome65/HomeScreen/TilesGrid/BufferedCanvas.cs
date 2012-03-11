using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.NativeHelpers;
using Fleux.UIElements;
using MetroHome65.Routines;
using BitmapData = System.Drawing.Imaging.BitmapData;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public class BufferedCanvas : Canvas
    {
        #region Fields

        private bool _needRepaint;

        private DoubleBuffer _buffer;

        #endregion


        #region Properties

        public bool FreezeUpdate { get; set; }

        #endregion


        #region override methods
    
        public override void AddElement(UIElement element)
        {
            base.AddElement(element);
            _needRepaint = true;
        }

        public override void AddElementAt(int index, UIElement element)
        {
            base.AddElementAt(index, element);
            _needRepaint = true;
        }

        protected override void OnUpdated(UIElement element)
        {
            if (!FreezeUpdate)
            {
                // repaint in buffer only changed element
                if (_buffer != null)
                {
                    _buffer.Graphics.Clip = new Region(element.Bounds);

                    _buffer.Clear();
                    element.Draw(_buffer.DrawingGraphics.CreateChild(
                        element.Location, element.TransformationScaling, element.TransformationCenter));

                    _buffer.Graphics.ResetClip();
                }

                this.Updated(this);
            }
        }

        private void UpdateBuffer()
        {
            _buffer.Clear();
            Children
                .ToList()
                .ForEach(e =>
                {
                    //!! todo не порождать дочерние графики рисовать прямо на одном
                    //_buffer.DrawingGraphics.MoveRel(e.Location.X, e.Location.Y);
                    //e.Draw(_buffer.DrawingGraphics);
                    //_buffer.DrawingGraphics.MoveRel(- e.Location.X, - e.Location.Y);
                    e.Draw(_buffer.DrawingGraphics.CreateChild(e.Location, e.TransformationScaling, e.TransformationCenter));
                });
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (drawingGraphics == null)
                return;

            if (_buffer == null)
                _buffer = new DoubleBuffer(Size);

            if ((_needRepaint) && (!FreezeUpdate))
            {
                UpdateBuffer();
                _needRepaint = false;
            }

            if (_buffer != null)
                //DrawBuffer(drawingGraphics);
                drawingGraphics.DrawImage(_buffer.Image, 0, 0);

        }

        private void DrawBuffer(IDrawingGraphics drawingGraphics)
        {
            var hSrc = _buffer.Graphics.GetHdc();
            var hDst = drawingGraphics.Graphics.GetHdc();

            /*
            var blendFunction = new BlendFunction
            {
                BlendOp = (byte)BlendOperation.AcSrcOver,
                BlendFlags = (byte)BlendFlags.Zero,
                SourceConstantAlpha = 100,
                AlphaFormat = 0
            };

            DrawingAPI.AlphaBlend(hDst, 0, -drawingGraphics.VisibleRect.Top, Size.Width, Size.Height, hSrc,
                0, 0, Size.Width, Size.Height, blendFunction);

             */

            /*
            DrawingAPI.BitBlt(hDst, 0, -drawingGraphics.VisibleRect.Top, Size.Width, Size.Height, 
                hSrc, 0, 0, 
                DrawingAPI.SRCCOPY
                );
            */
            /*
            BitmapData a = _buffer.Image.LockBits(new Rectangle(0, 0, Size.Width, Size.Height), ImageLockMode.ReadOnly,
                                           (PixelFormat) PixelFormatID.PixelFormat32bppARGB);
            var factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
            IImage image;
            factory.CreateImageFromBuffer(a.Scan0, a., BufferDisposalFlag.BufferDisposalFlagNone, image);

            _buffer.Image.UnlockBits(a);
            */

            _buffer.Graphics.ReleaseHdc(hSrc);
            drawingGraphics.Graphics.ReleaseHdc(hDst);
        }

        #endregion


        #region methods

        public void DeleteElement(UIElement element)
        {
            element.Parent = null;
            element.Updated = null;
            Children.Remove(element);
            _needRepaint = true;
        }

        #endregion

    }
}

