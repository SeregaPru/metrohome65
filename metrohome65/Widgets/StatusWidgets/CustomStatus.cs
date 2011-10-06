﻿using System;
using System.Drawing;
using System.Resources;
using System.IO;
using MetroHome65.Routines;
using OpenNETCF.Drawing;

namespace MetroHome65.Widgets.StatusWidget
{
    public class CustomStatus
    {
        public CustomStatus()
        {
            UpdateStatus();
        }

        public virtual void PaintStatus(Graphics g, Rectangle Rect)
        {
            //
        }

        /// <summary>
        /// Check if status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public virtual bool UpdateStatus()
        {
            return false;
        }

        /// <summary>
        /// Switch status to another - on <--> off
        /// </summary>
        public virtual void ChangeStatus()
        {
            //
        }

        protected enum DrawStatus {
            dsOff,
            dsOn,
            dsError
        }

        private OpenNETCF.Drawing.Imaging.ImagingFactory _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img;

        private static int IconSize = ScreenRoutines.Scale(48);
        private static int BarSize = ScreenRoutines.Scale(11);

        protected virtual void PaintStatus(Graphics g, Rectangle Rect,
            DrawStatus DrawStatus, string IconName, string Caption)
        {
            SolidBrush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            System.Drawing.Color CaptionColor;
            String IconPostfix;
            String BarPostfix;

            switch (DrawStatus) {
                case DrawStatus.dsOn :
                    CaptionColor = System.Drawing.Color.White;
                    IconPostfix = "_on";
                    BarPostfix = "_on";
                    break;
                case DrawStatus.dsOff :
                    CaptionColor = System.Drawing.Color.DarkGray;
                    IconPostfix = "_off";
                    BarPostfix = "_off";
                    break;
                default:
                    CaptionColor = System.Drawing.Color.White;
                    IconPostfix = "_on";
                    BarPostfix = "_error";
                    break;
            }

            // draw status indicator
            DrawResourceImage(g, 
                Rect.Left, Rect.Bottom - BarSize,
                Rect.Width, BarSize, "bar" + BarPostfix);

            // draw additional caption
            int CaptionHeigth = 0;
            if (!Caption.Equals(String.Empty))
            {
                Font captionFont = new System.Drawing.Font("Segoe UI Light", 7, FontStyle.Bold);
                captionBrush.Color = CaptionColor;
                SizeF CaptionSize = g.MeasureString(Caption, captionFont);
                g.DrawString(Caption, captionFont, captionBrush, 
                    Rect.Left + (Rect.Width - CaptionSize.Width) / 2 , Rect.Top + 44);
                CaptionHeigth = 16;
            }

            // draw main status icon
            DrawResourceImage(g, 
              (Rect.Left + Rect.Right - IconSize) / 2, 
              Rect.Top + (Rect.Height - IconSize - BarSize - CaptionHeigth) / 2, 
              IconSize, IconSize,
              IconName + IconPostfix);
        }

        private void DrawResourceImage(Graphics g, int x, int y, int Width, int Height, string IconName)
        {
            String IconPath = "StatusWidgets.icons." + 
                ((ScreenRoutines.IsQVGA) ? "small." : "big.") + 
                IconName + ".png";
            try
            {
                OpenNETCF.Drawing.Imaging.StreamOnFile IconStream = new OpenNETCF.Drawing.Imaging.StreamOnFile(this.GetType().Assembly.GetManifestResourceStream(IconPath));
                _factory.CreateImageFromStream(IconStream, out _img);

                OpenNETCF.Drawing.Imaging.ImageInfo ImageInfo;
                int tmp = _img.GetImageInfo(out ImageInfo);
                OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                    x, y, Width, Height);

                IntPtr hdc = g.GetHdc();
                _img.Draw(hdc, ImgRect, null);
                g.ReleaseHdc(hdc);
            }
            catch (Exception e)
            {
            }
        }

    }

}