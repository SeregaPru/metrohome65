using System;
using System.Drawing;

namespace MetroHome65.Widgets.StatusWidget
{
    public class RotationStatus : CustomStatus
    {
        private bool _RotationStatus = false; // auto rotate is on

        public RotationStatus() : base()
        {
            UpdateStatus();
        }

        public override void PaintStatus(Graphics g, Rectangle rect)
        {
            DrawStatus DrawStatus = (_RotationStatus) ? DrawStatus.dsOn : DrawStatus.dsOff;
            PaintStatus(g, rect, DrawStatus, "autorotate", "");
        }

        public override bool UpdateStatus()
        {
            bool CurrentStatus = GetRotationStatus();
            if (CurrentStatus != _RotationStatus)
            {
                _RotationStatus = CurrentStatus;
                return true;
            }

            return false;
        }

        private bool GetRotationStatus()
        {
            return true;
        }

    }
}
