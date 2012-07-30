using System.Drawing.Imaging;

namespace Fleux.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Core;

    public class DoubleBufferedControl : Control
    {
        protected Graphics controlGr;
        protected Bitmap offBmp;
        protected Graphics offGr;
        protected bool offUpdated;
        protected bool resizing;

        public virtual void Draw(Action<Graphics> drawAction)
        {
            if (!IsDisposed && this.offGr != null)
            {
                drawAction(this.offGr);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (this)
            {
                this.CreateGraphicBuffers();
                if (this.offBmp != null)
                {
                    if (!this.offUpdated)
                    {
                        this.Draw(new PaintEventArgs(this.offGr, ClientRectangle));
                        this.offUpdated = true;
                    }

                    this.controlGr.DrawImage(this.offBmp, 0, 0);
                }
                else
                {
                    this.DrawBackground(e);
                }
            }
        }

        protected virtual void Draw(PaintEventArgs e)
        {
        }

        protected virtual void DrawBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected virtual void CreateGraphicBuffers()
        {
            if (this.offBmp != null)
            {
                this.ReleaseGraphicBuffers();
            }

            this.controlGr = CreateGraphics();

            if (Height > 0 && Width > 0)
            {
                if (this.offBmp == null)
                {
                    if (this.controlGr == null)
                    {
                        this.controlGr = CreateGraphics();
                    }

                    this.offBmp = new Bitmap(Width, Height, PixelFormat.Format16bppRgb565);
                    this.offGr = Graphics.FromImage(this.offBmp);
                    this.offUpdated = false;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.ReleaseGraphicBuffers();
            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected virtual void ReleaseGraphicBuffers()
        {
            lock (this)
            {
                if (this.offBmp != null)
                {
                    // Dispose resources
                    this.offGr.Dispose();
                    this.offBmp.Dispose();
                    this.offBmp = null;
                    this.offGr = null;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!this.resizing)
            {
                lock (this)
                {
                    this.resizing = true;
                    this.ReleaseGraphicBuffers();
                    this.resizing = false;
                }
            }
        }

        protected virtual void ForcedInvalidate()
        {
            if (!IsDisposed && this.offBmp != null)
            {
                this.offUpdated = false;
                this.Draw(new PaintEventArgs(this.offGr, new Rectangle(0, 0, this.offBmp.Width, this.offBmp.Height)));
                this.controlGr.DrawImage(this.offBmp, 0, 0);
            }
        }
    }
}