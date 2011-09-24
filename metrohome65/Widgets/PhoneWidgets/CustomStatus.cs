using System;
using System.Drawing;

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

        protected virtual void PaintStatus(Graphics g, Rectangle Rect,
            DrawStatus DrawStatus, string IconName, string Caption)
        {
            SolidBrush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            System.Drawing.Color StatusColor;
            System.Drawing.Color IconColor;

            switch (DrawStatus) {
                case DrawStatus.dsOn :
                    StatusColor = System.Drawing.Color.Green;
                    IconColor = System.Drawing.Color.White;
                    break;
                case DrawStatus.dsOff :
                    StatusColor = System.Drawing.Color.DarkGray;
                    IconColor = System.Drawing.Color.DarkGray;
                    break;
                default :
                    StatusColor = System.Drawing.Color.Red;
                    IconColor = System.Drawing.Color.White;
                    break;
            }

            Font captionFont = new System.Drawing.Font("Segoe UI Light", 12, FontStyle.Bold);
            captionBrush.Color = IconColor;
            g.DrawString(IconName, captionFont, captionBrush, Rect.Left + 3, Rect.Top + 5);

            captionFont = new System.Drawing.Font("Segoe UI Light", 8, FontStyle.Bold);
            captionBrush.Color = IconColor;
            g.DrawString(Caption, captionFont, captionBrush, Rect.Left + 3, Rect.Top + 35);

            captionBrush.Color = StatusColor;
            g.FillRectangle(captionBrush, Rect.Left, Rect.Bottom - 9, Rect.Width, 8);
        }

    }

}
