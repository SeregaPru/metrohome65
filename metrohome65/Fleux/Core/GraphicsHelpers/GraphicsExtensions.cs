﻿namespace Fleux.Core.GraphicsHelpers
{
    using System.Drawing;
    using System.Reflection;
    using Core.NativeHelpers;

    public static class GraphicsExtensions
    {
        public static Rectangle Clone(this Rectangle r)
        {
            return new Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static Rectangle TransformToBounds(this Rectangle r)
        {
            return new Rectangle(r.Left, r.Top, r.Right, r.Bottom);
        }

        public static Graphics DrawPng(this Graphics gr, IImageWrapper pngImage, Rectangle destRect)
        {
            var hDc = gr.GetHdc();
            pngImage.Draw(hDc, destRect, new Rectangle(0, 0, pngImage.Size.Width, pngImage.Size.Height));
            gr.ReleaseHdc(hDc);
            return gr;
        }

        public static Graphics DrawPng(this Graphics gr, string resourceName, Rectangle destRect)
        {
            return DrawPng(gr, 
                           ResourceManager.Instance.GetIImageFromEmbeddedResource(resourceName, Assembly.GetCallingAssembly()), 
                           destRect);
        }
    }
}
