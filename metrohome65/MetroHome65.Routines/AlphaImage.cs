using System;
using System.Text;
using System.Drawing;
using OpenNETCF.Drawing;
using System.Runtime.InteropServices;


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

        public AlphaImage(System.IO.Stream Stream)
        {
            try
            {
                OpenNETCF.Drawing.Imaging.StreamOnFile IconStream = new OpenNETCF.Drawing.Imaging.StreamOnFile(Stream);
                OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
                _factory.CreateImageFromStream(IconStream, out _img);
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetBtnImg")
            }
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

        public void PaintBackgroundAlpha(Graphics g, Rectangle Rect, byte Alpha)
        {
            //!! see _factory.CreateBitmapFromImage

            Bitmap buffer = new Bitmap(Rect.Width, Rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            Graphics buffergx = Graphics.FromImage(buffer);
            IntPtr hdcBuffer;

            Rectangle BufferRect = new Rectangle(0, 0, Rect.Width, Rect.Height);
            if (_img != null)
            {
                hdcBuffer = buffergx.GetHdc();
                try
                {
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                        BufferRect.Left, BufferRect.Top, BufferRect.Width, BufferRect.Height);
                    _img.Draw(hdcBuffer, ImgRect, null);
                    buffergx.ReleaseHdc(hdcBuffer);
                }
                catch (Exception e) { }

                IntPtr hdcDst = g.GetHdc();
                hdcBuffer = buffergx.GetHdc();
                BlendFunction blendFunction = new BlendFunction();
                blendFunction.BlendOp = (byte)BlendOperation.AC_SRC_OVER;   // Only supported blend operation
                blendFunction.BlendFlags = (byte)BlendFlags.Zero;           // Documentation says put 0 here
                blendFunction.SourceConstantAlpha = Alpha;                 // Constant alpha factor
                blendFunction.AlphaFormat = (byte)AlphaFormat.AC_SRC_ALPHA;

                DrawingAPI.AlphaBlend(hdcDst, Rect.Left, Rect.Top, Rect.Width, Rect.Height, hdcBuffer,
                    0, 0, Rect.Width, Rect.Height, blendFunction);

                g.ReleaseHdc(hdcDst);          // Required cleanup to GetHdc()
                buffergx.ReleaseHdc(hdcBuffer);       // Required cleanup to GetHdc()
            }

        }


        public static void DrawAlphaImage(Graphics g, Image Img, Rectangle Rect, byte Alpha)
        {
            IntPtr hdcDst = g.GetHdc();
            Graphics gSrc = Graphics.FromImage(Img);
            IntPtr hdcSrc = gSrc.GetHdc();

            BlendFunction blendFunction = new BlendFunction();
            blendFunction.BlendOp = (byte)BlendOperation.AC_SRC_OVER;   // Only supported blend operation
            blendFunction.BlendFlags = (byte)BlendFlags.Zero;           // Documentation says put 0 here
            blendFunction.SourceConstantAlpha = Alpha;                 // Constant alpha factor
            blendFunction.AlphaFormat = (byte)0; // AlphaFormat.AC_SRC_ALPHA;
            //!!blendFunction.AlphaFormat = (byte)AlphaFormat.AC_SRC_ALPHA;

            DrawingAPI.AlphaBlend(hdcDst, Rect.Left, Rect.Top, Rect.Width, Rect.Height, hdcSrc,
                0, 0, Img.Width, Img.Height, blendFunction);

            g.ReleaseHdc(hdcDst);          // Required cleanup to GetHdc()
            gSrc.ReleaseHdc(hdcSrc);       // Required cleanup to GetHdc()
        }
    }


    // These structures, enumerations and p/invoke signatures come from
    // wingdi.h in the Windows Mobile 5.0 Pocket PC SDK

    public struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    public enum BlendOperation : byte
    {
        AC_SRC_OVER = 0x00
    }

    public enum BlendFlags : byte
    {
        Zero = 0x00
    }

    public enum SourceConstantAlpha : byte
    {
        Transparent = 0x00,
        Opaque = 0xFF
    }

    public enum AlphaFormat : byte
    {
        AC_SRC_ALPHA = 0x01
    }

    public class DrawingAPI
    {
        [DllImport("coredll.dll")]
        extern public static bool AlphaBlend(IntPtr hdcDest, Int32 nXDest, Int32 nYDest, Int32 nWidthDst, Int32 nHeightDst, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, Int32 nWidthSrc, Int32 nHeightSrc, BlendFunction blendFunction);

        [DllImport("coredll.dll")]
        extern public static bool BitBlt(IntPtr hdcDest, Int32 nXDest, Int32 nYDest, Int32 nWidth, Int32 nHeight, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, UInt32 dwRop);

        public const UInt32 SRCCOPY = 0x00CC0020;
    }


}
