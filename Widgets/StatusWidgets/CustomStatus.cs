using System;
using System.Drawing;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;

namespace MetroHome65.Widgets.StatusWidgets
{
    public class CustomStatus
    {
        public CustomStatus()
        {
            UpdateStatus();
        }

        public virtual void PaintStatus(Graphics g, Rectangle rect) { }

        /// <summary>
        /// Check if status was changed since last check
        /// </summary>
        /// <returns>True if status was changed</returns>
        public virtual bool UpdateStatus()
        {
            return false;
        }

        /// <summary>
        /// Switch status to another - on / off
        /// </summary>
        public virtual void ChangeStatus() { }

        protected enum DrawStatus {
            Off,
            On,
            Error
        }

        private static readonly int IconSize = (ScreenRoutines.IsQVGA) ? 32 : 48;
        private static readonly string IconPrefix = (ScreenRoutines.IsQVGA) ? "small." : "big.";
        private static readonly int BarSize = (ScreenRoutines.IsQVGA) ? 4 : 10;
        private static readonly int CaptionPosY = (ScreenRoutines.IsQVGA) ? 25 : 44;

        protected void PaintStatus(Graphics g, Rectangle rect,
            DrawStatus drawStatus, string iconName, string caption)
        {
            var captionBrush = new SolidBrush(Color.White);

            Color captionColor;
            String iconPostfix;
            String barPostfix;

            switch (drawStatus) {
                case DrawStatus.On :
                    captionColor = Color.White;
                    iconPostfix = "_on";
                    barPostfix = "_on";
                    break;
                case DrawStatus.Off :
                    captionColor = Color.DarkGray;
                    iconPostfix = "_off";
                    barPostfix = "_off";
                    break;
                default:
                    captionColor = Color.White;
                    iconPostfix = "_on";
                    barPostfix = "_error";
                    break;
            }

            // draw status indicator
            DrawResourceImage(g,
                rect.Left, rect.Bottom - BarSize, rect.Width, BarSize, "bar" + barPostfix);

            // draw additional caption
            var captionHeigth = 0;
            if (!String.IsNullOrEmpty(caption))
            {
                var captionFont = new Font("Segoe WP Light", 7, FontStyle.Bold);
                captionBrush.Color = captionColor;
                var captionSize = g.MeasureString(caption, captionFont);
                g.DrawString(caption, captionFont, captionBrush,
                    rect.Left + (rect.Width - captionSize.Width) / 2, rect.Top + CaptionPosY);
                captionHeigth = ScreenRoutines.Scale(16);
            }

            // draw main status icon
            DrawResourceImage(g, 
              (rect.Left + rect.Right - IconSize) / 2,
              rect.Top + (rect.Height - IconSize - BarSize - captionHeigth) / 2,
              IconSize, IconSize,
              iconName + iconPostfix);
        }

        private void DrawResourceImage(Graphics g, int x, int y, int width, int height, string iconName)
        {
            var iconPath = "StatusWidgets.icons." + IconPrefix + iconName + ".png";
            (new AlphaImage(iconPath, this.GetType().Assembly)).
                PaintBackground(g, new Rectangle(x, y, width, height));
        }

    }

}
