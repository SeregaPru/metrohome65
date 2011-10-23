using System;
using System.Drawing;
using System.Resources;
using System.IO;
using MetroHome65.Routines;

namespace MetroHome65.Widgets.StatusWidget
{
    public class CustomStatus
    {
        public CustomStatus()
        {
            UpdateStatus();
        }

        public virtual void PaintStatus(Graphics g, Rectangle Rect) { }

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
        public virtual void ChangeStatus() { }

        protected enum DrawStatus {
            dsOff,
            dsOn,
            dsError
        }

        private static int _IconSize = (ScreenRoutines.IsQVGA) ? 32 : 48;
        private static string _IconPrefix = (ScreenRoutines.IsQVGA) ? "small." : "big.";
        private static int _BarSize = (ScreenRoutines.IsQVGA) ? 4 : 10;
        private static int _CaptionPosY = (ScreenRoutines.IsQVGA) ? 25 : 44;

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
                Rect.Left, Rect.Bottom - _BarSize, Rect.Width, _BarSize, "bar" + BarPostfix);

            // draw additional caption
            int CaptionHeigth = 0;
            if (!Caption.Equals(String.Empty))
            {
                Font captionFont = new System.Drawing.Font("Segoe UI Light", 7, FontStyle.Bold);
                captionBrush.Color = CaptionColor;
                SizeF CaptionSize = g.MeasureString(Caption, captionFont);
                g.DrawString(Caption, captionFont, captionBrush,
                    Rect.Left + (Rect.Width - CaptionSize.Width) / 2, Rect.Top + _CaptionPosY);
                CaptionHeigth = ScreenRoutines.Scale(16);
            }

            // draw main status icon
            DrawResourceImage(g, 
              (Rect.Left + Rect.Right - _IconSize) / 2,
              Rect.Top + (Rect.Height - _IconSize - _BarSize - CaptionHeigth) / 2,
              _IconSize, _IconSize,
              IconName + IconPostfix);
        }

        private void DrawResourceImage(Graphics g, int x, int y, int Width, int Height, string IconName)
        {
            String IconPath = "StatusWidgets.icons." + _IconPrefix + IconName + ".png";
            AlphaImage.FromResource(IconPath).PaintIcon(g, new Rectangle(x, y, Width, Height));
        }

    }

}
