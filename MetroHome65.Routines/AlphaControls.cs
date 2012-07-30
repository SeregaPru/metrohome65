using System;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace MetroHome65.Routines
{
    public interface IBackgroundForm
    {
        Image BackgroundImage { get; }
    }


    public static class TransparentHelper
    {
        public static void DrawBackgroundImage(Control Self, Graphics g)
        {
            Control _Parent = Self;
            Rectangle ParentBounds = new Rectangle(0, 0, Self.Bounds.Width, Self.Bounds.Height);

            while (_Parent != null)
            {
                ParentBounds.Offset(_Parent.Location.X, _Parent.Location.Y);
                _Parent = _Parent.Parent;
                if (_Parent is IBackgroundForm)
                {
                    g.DrawImage((_Parent as IBackgroundForm).BackgroundImage, 0, 0, ParentBounds, GraphicsUnit.Pixel);
                    break;
                }
            }
        }
    }



    public class TransparentPanel : System.Windows.Forms.Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            TransparentHelper.DrawBackgroundImage(this, e.Graphics);
        }
    }



    public class TransparentPictureBox : System.Windows.Forms.PictureBox
    {
        public TransparentPictureBox() : base() { }

        public TransparentPictureBox(String ResourcePath, Assembly Assembly) : base()
        {
            _AlphaImage = new AlphaImage(Assembly.GetManifestResourceStream(ResourcePath));
        }

        AlphaImage _AlphaImage = null;

        private String _ImagePath = "";
        public String ImagePath
        {
            get { return _ImagePath; }
            set
            {
                if (_ImagePath != value)
                {
                    _ImagePath = value;
                    _AlphaImage = new AlphaImage(_ImagePath);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            TransparentHelper.DrawBackgroundImage(this, e.Graphics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_AlphaImage != null)
            {
                if (this.SizeMode == PictureBoxSizeMode.Normal)
                    _AlphaImage.PaintBackground(e.Graphics, e.ClipRectangle); //!! todo
                else
                    _AlphaImage.PaintBackground(e.Graphics, e.ClipRectangle);
                return;
            }

            if (Image != null)
            {
                base.OnPaint(e);
                return;

                if (this.SizeMode == PictureBoxSizeMode.Normal)
                    //e.Graphics.DrawImage(Image, e.ClipRectangle.X, e.ClipRectangle.Y);
                    AlphaImage.DrawAlphaImage(e.Graphics, Image, 
                        new Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, Image.Width, Image.Height), 255);
                else
                    AlphaImage.DrawAlphaImage(e.Graphics, Image, e.ClipRectangle, 255);
                
                return;
            }

            base.OnPaint(e);
        }
    }
}
