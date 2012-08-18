using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Fleux.Core.NativeHelpers;


namespace MetroHome65.Routines
{
    /// <summary>
    /// Wrapper class for drawing transparent images
    /// </summary>
    public class AlphaImage
    {
        private IImageWrapper _img;
        private String _imagePath = "";

        public AlphaImage(String imagePath)
        {
            ImagePath = imagePath;
        }

        public AlphaImage(string resourceName, System.Reflection.Assembly assembly)
        {
            try
            {
                var factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
                IImage img;

                IImage imagingResource;
                using (var strm = (MemoryStream)assembly.GetManifestResourceStream(resourceName))
                {
                    var pbBuf = strm.GetBuffer();
                    var cbBuf = (uint)strm.Length;
                    factory.CreateImageFromBuffer(pbBuf, cbBuf, BufferDisposalFlag.BufferDisposalFlagNone, out imagingResource);
                }
                _img = new IImageWrapper(imagingResource);
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "SetBtnImg")
            }
        }

        ~AlphaImage()
        {
            if (_img != null)
                _img.Dispose();
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
                        var factory = (IImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("327ABDA8-072B-11D3-9D7B-0000F81EF32E")));
                        IImage img;
                        factory.CreateImageFromFile(_imagePath, out img);
                        _img = new IImageWrapper(img);
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
                return _img.Size;
            } 
        }

        public void PaintIcon(Graphics g, int x, int y)
        {
            // draw icon from external image file
            if (_img == null) return;

            try
            {
                var hdc = g.GetHdc();
                var imgRect = new Rectangle(
                    x, y, _img.Size.Width, _img.Size.Height);
                _img.Draw(hdc, imgRect, imgRect);
                g.ReleaseHdc(hdc);
            }
            catch (Exception e)
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
                var hdc = g.GetHdc();
                var imgRect = new Rectangle(
                    (rect.Left + rect.Right - _img.Size.Width) / 2, 
                    rect.Top + (rect.Height - _img.Size.Height) / 2,
                    _img.Size.Width, _img.Size.Height);
                _img.Draw(hdc, imgRect, imgRect);
                g.ReleaseHdc(hdc);
            }
            catch (Exception e)
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
                var imgRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
                _img.Draw(hdc, imgRect, imgRect);
                g.ReleaseHdc(hdc);
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "PaintBackground")
            }
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
        AcSrcOver = 0x00
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
        AcSrcAlpha = 0x01
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
