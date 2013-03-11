using System;
using System.Drawing;


namespace MetroHome65.Routines
{
    /// <summary>
    /// Wrapper class for drawing transparent images
    /// </summary>
    public class AlphaImage
    {
        private OpenNETCF.Drawing.Imaging.IImage _img;
        private String _imagePath = "";

        public AlphaImage(String imagePath)
        {
            ImagePath = imagePath;
        }

        public AlphaImage(System.IO.Stream stream)
        {
            try
            {
                var iconStream = new OpenNETCF.Drawing.Imaging.StreamOnFile(stream);
                var factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
                factory.CreateImageFromStream(iconStream, out _img);
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetBtnImg")
            }
        }

        public AlphaImage(string resourceName, System.Reflection.Assembly assembly)
        {
            try
            {
                var iconStream = new OpenNETCF.Drawing.Imaging.StreamOnFile(assembly.GetManifestResourceStream(resourceName));
                var factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
                factory.CreateImageFromStream(iconStream, out _img);
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetBtnImg")
            }
        }

        public String ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (_imagePath == value) return;

                _imagePath = value;
                try
                {
                    if (!String.IsNullOrEmpty(_imagePath))
                    {
                        var factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
                        factory.CreateImageFromFile(_imagePath, out _img);
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

        public Size Size { get
            {
                OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                _img.GetImageInfo(out imageInfo);
                return new Size((int) imageInfo.Width, (int) imageInfo.Height);
            } 
        }

        public void PaintIcon(Graphics g, int x, int y)
        {
            // draw icon from external image file
            if (_img == null) return;

            try
            {
                OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                _img.GetImageInfo(out imageInfo);

                var hdc = g.GetHdc();
                OpenNETCF.Drawing.Imaging.RECT imgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                    x, y, (int)imageInfo.Width, (int)imageInfo.Height);
                _img.Draw(hdc, imgRect, null);
                g.ReleaseHdc(hdc);
            }
            catch
            {
                //!! write to log  (e.StackTrace, "PaintIcon")
            }
        }

        public void PaintIcon(Graphics g, Rectangle rect)
        {
            // draw icon from external image file
            if (_img == null) return;

            try
            {
                OpenNETCF.Drawing.Imaging.ImageInfo imageInfo;
                _img.GetImageInfo(out imageInfo);

                var hdc = g.GetHdc();
                OpenNETCF.Drawing.Imaging.RECT imgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                    (rect.Left + rect.Right - (int)imageInfo.Width) / 2,
                    rect.Top + (rect.Height - (int)imageInfo.Height) / 2,
                    (int)imageInfo.Width, (int)imageInfo.Height);
                _img.Draw(hdc, imgRect, null);
                g.ReleaseHdc(hdc);
            }
            catch
            {
                //!! write to log  (e.StackTrace, "PaintIcon")
            }
        }


        /// <summary>
        /// Paints image as backgroud for rect.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rect"></param>
        public void PaintBackground(Graphics g, Rectangle rect)
        {
            // if button image is set, draw button image
            if (_img == null) return;

            try
            {
                var hdc = g.GetHdc();
                var imgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(rect.Left, rect.Top, rect.Width, rect.Height);
                _img.Draw(hdc, imgRect, null);
                g.ReleaseHdc(hdc);
            }
            catch
            {
                //!! write to log  (e.StackTrace, "PaintBackground")
            }
        }

        public void PaintBackgroundAlpha(Graphics g, Rectangle rect, byte alpha)
        {
            //!! see _factory.CreateBitmapFromImage

            var buffer = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            var buffergx = Graphics.FromImage(buffer);

            var bufferRect = new Rectangle(0, 0, rect.Width, rect.Height);
            if (_img == null) return;

            var hdcBuffer = buffergx.GetHdc();
            try
            {
                OpenNETCF.Drawing.Imaging.RECT imgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                    bufferRect.Left, bufferRect.Top, bufferRect.Width, bufferRect.Height);
                _img.Draw(hdcBuffer, imgRect, null);
                buffergx.ReleaseHdc(hdcBuffer);
            }
            catch
            { }

            var hdcDst = g.GetHdc();
            hdcBuffer = buffergx.GetHdc();
            var blendFunction = new BlendFunction
                                    {
                                        BlendOp = (byte) BlendOperation.AcSrcOver,
                                        BlendFlags = (byte) BlendFlags.Zero,
                                        SourceConstantAlpha = alpha,
                                        AlphaFormat = (byte) AlphaFormat.AcSrcAlpha
                                    };
            // Only supported blend operation
            // Documentation says put 0 here
            // Constant alpha factor

            DrawingAPI.AlphaBlend(hdcDst, rect.Left, rect.Top, rect.Width, rect.Height, hdcBuffer,
                                  0, 0, rect.Width, rect.Height, blendFunction);

            g.ReleaseHdc(hdcDst);          // Required cleanup to GetHdc()
            buffergx.ReleaseHdc(hdcBuffer);       // Required cleanup to GetHdc()
        }


        public static void DrawAlphaImage(Graphics g, Image img, Rectangle rect, byte alpha)
        {
            var hdcDst = g.GetHdc();
            var gSrc = Graphics.FromImage(img);
            var hdcSrc = gSrc.GetHdc();

            var blendFunction = new BlendFunction
                                    {
                                        BlendOp = (byte) BlendOperation.AcSrcOver,
                                        BlendFlags = (byte) BlendFlags.Zero,
                                        SourceConstantAlpha = alpha,
                                        AlphaFormat = 0
                                    };
            // Only supported blend operation
            // Documentation says put 0 here
            // Constant alpha factor
            // AlphaFormat.AC_SRC_ALPHA;
            //!!blendFunction.AlphaFormat = (byte)AlphaFormat.AC_SRC_ALPHA;

            DrawingAPI.AlphaBlend(hdcDst, rect.Left, rect.Top, rect.Width, rect.Height, hdcSrc,
                0, 0, img.Width, img.Height, blendFunction);

            g.ReleaseHdc(hdcDst);          // Required cleanup to GetHdc()
            gSrc.ReleaseHdc(hdcSrc);       // Required cleanup to GetHdc()
        }
    }



}
