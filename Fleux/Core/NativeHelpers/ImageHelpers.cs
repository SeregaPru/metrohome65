using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Fleux.Core.NativeHelpers
{
    public static class ImageHelpers
    {
        [CLSCompliant(false)]
        public static Bitmap IBitmapImageToBitmap(IBitmapImage imageBitmap)
        {
            if (imageBitmap == null)
                throw new ArgumentNullException();
            Size size;
            imageBitmap.GetSize(out size);
            Rectangle rect = new Rectangle(0, 0, size.Width, size.Height);
            BitmapData lockedBitmapData = new BitmapData();
            imageBitmap.LockBits(ref rect, 0U, PixelFormatID.PixelFormat16bppRGB565, out lockedBitmapData);
            Bitmap bitmap = new Bitmap((int)lockedBitmapData.Width, (int)lockedBitmapData.Height, 
                PixelFormat.Format16bppRgb565);
            System.Drawing.Imaging.BitmapData bitmapdata = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format16bppRgb565);
            ImageHelpers.CopyMemory(bitmapdata.Scan0, lockedBitmapData.Scan0, (int)(lockedBitmapData.Height * lockedBitmapData.Stride));
            imageBitmap.UnlockBits(ref lockedBitmapData);
            bitmap.UnlockBits(bitmapdata);
            return bitmap;
        }

        internal static void CopyMemory(IntPtr pDst, IntPtr pSrc, int cbSize)
        {
          if (pDst == IntPtr.Zero || pSrc == IntPtr.Zero || (cbSize <= 0 || pDst.ToInt32() == -1) || pSrc.ToInt32() == -1)
            throw new ArgumentException();
          byte[] numArray = new byte[65536];
          while (cbSize > 65536)
          {
            Marshal.Copy(pSrc, numArray, 0, 65536);
            Marshal.Copy(numArray, 0, pDst, 65536);
            cbSize -= 65536;
            pDst = (IntPtr) ((int) pDst + 65536);
            pSrc = (IntPtr) ((int) pSrc + 65536);
          }
          Marshal.Copy(pSrc, numArray, 0, cbSize);
          Marshal.Copy(numArray, 0, pDst, cbSize);
        }
    }
}
