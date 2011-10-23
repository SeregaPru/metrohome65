using System;
using System.Text;
using System.Drawing;
using OpenNETCF.Drawing;


namespace MetroHome65.Routines
{
    /// <summary>
    /// Wrapper class for drawing transparent images
    /// </summary>
    public class AlphaImage
    {
        private OpenNETCF.Drawing.Imaging.IImage _img = null;
        private String _ImagePath = "";

        public AlphaImage(String ImagePath)
        {
            this.ImagePath = ImagePath;
        }

        private AlphaImage(OpenNETCF.Drawing.Imaging.IImage img)
        {
            this._img = img;
        }

        public static AlphaImage FromResource(String ResourcePath)
        {
            OpenNETCF.Drawing.Imaging.IImage img;
            OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
            _factory.CreateImageFromStream(new OpenNETCF.Drawing.Imaging.StreamOnFile(ResourcePath), out img);
            return new AlphaImage(img);
        }

        public String ImagePath
        {
            get { return _ImagePath; }
            set
            {
                if (_ImagePath != value)
                {
                    _ImagePath = value;
                    try
                    {
                        if (!String.IsNullOrEmpty(_ImagePath))
                        {
                            OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
                            _factory.CreateImageFromFile(_ImagePath, out _img);
                        }
                        else
                            _img = null;
                    }
                    catch (Exception e)
                    {
                        //!! write to log  (e.StackTrace, "SetBtnImg")
                    }
                }
            }
        }


        /// <summary>
        /// Paints image as backgroud for rect.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        public void PaintBackground(Graphics g, Rectangle Rect)
        {
            // if button image is set, draw button image
            if (_img != null)
            {
                try
                {
                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(Rect.Left, Rect.Top, Rect.Width, Rect.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                    return;
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "PaintBackground")
                }
            }
        }

        public void PaintIcon(Graphics g, Rectangle Rect)
        {
            // draw icon from external image file
            if (_img != null)
            {
                try
                {
                    OpenNETCF.Drawing.Imaging.ImageInfo ImageInfo;
                    int tmp = _img.GetImageInfo(out ImageInfo);

                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                        (Rect.Left + Rect.Right - (int)ImageInfo.Width) / 2,
                        Rect.Top + (Rect.Height - (int)ImageInfo.Height) / 2,
                        (int)ImageInfo.Width, (int)ImageInfo.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                }
                catch (Exception e)
                {
                    //!! write to log  (e.StackTrace, "PaintIcon")
                }
            }
        }

    }
}
