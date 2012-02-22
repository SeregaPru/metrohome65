using System.Drawing;

namespace MetroHome65.Widgets.StatusWidgets
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
            DrawStatus drawStatus = (_RotationStatus) ? DrawStatus.On : DrawStatus.Off;
            PaintStatus(g, rect, drawStatus, "autorotate", "");
        }

        public override bool UpdateStatus()
        {
            bool currentStatus = GetRotationStatus();
            if (currentStatus != _RotationStatus)
            {
                _RotationStatus = currentStatus;
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
