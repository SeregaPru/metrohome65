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
    
        public BufferedCanvas()
        {
            SizeChanged += (s, e) => CreateBuffer();
        }

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

        /// <summary>
        /// Handler for update event from child.
        /// Redraw only triggered element, not whole buffer. For 
        /// </summary>
        /// <param name="element"></param>
        protected override void OnUpdated(UIElement element)
        {
            if (!FreezeUpdate)
            {
                // repaint in buffer only changed element
                //RepaintBuffer();

                if (element == this)
                    _needRepaint = true;
                else
                    RepaintElement(element);

                this.Updated(this);
            }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (drawingGraphics == null)
                return;

            if (_buffer == null)
                CreateBuffer();

            if ((_needRepaint) && (!FreezeUpdate))
            {
                RepaintBuffer();
                _needRepaint = false;
            }

            DrawBuffer(drawingGraphics);
        }

        private void DrawBuffer(IDrawingGraphics drawingGraphics)
        {
            var dstTop = (drawingGraphics.VisibleRect.Top > 0) ? 0 : drawingGraphics.VisibleRect.Top;
            var srcTop = (drawingGraphics.VisibleRect.Top > 0) ? drawingGraphics.VisibleRect.Top : 0;
            var height = (drawingGraphics.VisibleRect.Top > 0) ? drawingGraphics.VisibleRect.Height : drawingGraphics.VisibleRect.Height;

            lock (this)
            {
drawingGraphics.DrawImage(_buffer.Image, 0, 0);
return;

                drawingGraphics.DrawImage(_buffer.Image,
                    new Rectangle(0, dstTop, drawingGraphics.VisibleRect.Width, height),
                    new Rectangle(0, srcTop, drawingGraphics.VisibleRect.Width, height));
                return;

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

                DrawingAPI.BitBlt(hDst, 0, dstTop, Size.Width, height, 
                    hSrc, 0, srcTop, 
                    DrawingAPI.SRCCOPY
                    );

                
                //DrawingAPI.BitBlt(hDst, 0, -drawingGraphics.VisibleRect.Top, Size.Width, Size.Height, 
                //    hSrc, 0, 0, 
                //    DrawingAPI.SRCCOPY
                //    );
                
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
        }

        #endregion


        #region methods

        private void RepaintBuffer()
        {
            lock (this)
            {
                if (_buffer == null) return;

                ClearBuffer(new Rectangle(0, 0, _buffer.Image.Width, _buffer.Image.Height));
                Children
                    .ToList()
                    .ForEach(element =>
                                 {
                                     //!! todo не порождать дочерние графики рисовать прямо на одном
                                     //_buffer.DrawingGraphics.MoveRel(e.Location.X, e.Location.Y);
                                     //e.Draw(_buffer.DrawingGraphics);
                                     //_buffer.DrawingGraphics.MoveRel(- e.Location.X, - e.Location.Y);

                                     element.Draw(_buffer.DrawingGraphics.CreateChild(element.Location, element.TransformationScaling, element.TransformationCenter));
                                 });
            }
        }

        private void RepaintElement(UIElement element)
        {
            lock (this)
            {
                if (_buffer == null) return;

                //var oldClip = _buffer.Graphics.Clip;
                //_buffer.Graphics.Clip = new Region(element.Bounds);

                ClearBuffer(element.Bounds);
                element.Draw(_buffer.DrawingGraphics.CreateChild(
                    element.Location, element.TransformationScaling, element.TransformationCenter));

                //_buffer.Graphics.Clip = oldClip;
            }
        }

        private void ClearBuffer(Rectangle rect)
        {
            _buffer.Graphics.FillRectangle(new SolidBrush(Color.Transparent), rect);
        }

        private void CreateBuffer()
        {
            // re-create buffer with new size
            lock (this)
            {
                _buffer = null;
                _buffer = new DoubleBuffer(Size);
            }
            _needRepaint = true;
        }

        #endregion

    }
}

