using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MetroHome65.Routines;

namespace MetroHome65.Widgets.StatusWidgets
{
    [WidgetInfo("Statuses")]
    public class StatusWidget : ShortcutWidget, IUpdatable
    {
        private Timer _updateTimer;

        readonly List<CustomStatus> _statuses = new List<CustomStatus>();

        /// <summary>
        /// minimal width for single status indicator for auto-layout
        /// </summary>
        private static readonly int MinWidth = ScreenRoutines.Scale(49);

        private Size _widgetSize;


        public StatusWidget() : base()
        {
            _statuses.Add(new BatteryStatus());
            _statuses.Add(new BluetoothStatus());
            _statuses.Add(new WiFiStatus());
        }

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(4, 1) 
            };
            return sizes;
        }
        
        public override void Paint(Graphics g, Rectangle rect)
        {
            _widgetSize = rect.Size;

            base.Paint(g, rect);
            PaintStatuses(g);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect) {}

        private void PaintStatuses(Graphics g)
        {
            var pen = new Pen(Color.Gray);

            for (var i = 0; i < _statuses.Count; i++)
            {
                var statusRect = GetStatusRect(i);
                _statuses[i].PaintStatus(g, statusRect);
                if (i < _statuses.Count - 1)
                    g.DrawLine(pen, statusRect.Right, statusRect.Top + 1, statusRect.Right, statusRect.Bottom - 1);
            }
        }

        private Rectangle GetStatusRect(int position)
        {
            var statusWidth = 0;
            var statusCount = StatusesCount();
            while (statusWidth < MinWidth)
            {
                statusWidth = _widgetSize.Width / statusCount;
                statusCount--;
            }

            var statusRect = new Rectangle(
                statusWidth * position + position, 0, 
                statusWidth, _widgetSize.Height);
            return statusRect;
        }

        /// <summary>
        /// Get count of registered statuses
        /// </summary>
        /// <returns></returns>
        private int StatusesCount()
        {
            return _statuses.Count;
        }

        public bool Active
        {
            get { return _updateTimer.Enabled; }
            set
            {
                if (value)
                {
                    StartUpdate();
                }
                else
                {
                    StopUpdate();
                }
            }
        }

        public void StartUpdate()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new Timer() { Interval = 2000 };
                _updateTimer.Tick += (s, e) =>
                {
                    if (UpdateStatuses())
                    {
                        OnWidgetUpdate();
                    }
                };
            }
            _updateTimer.Enabled = true;
        }

        public void StopUpdate()
        {
            if (_updateTimer != null)
                _updateTimer.Enabled = false;
        }

        private bool UpdateStatuses()
        {
            var result = false;
            foreach(var status in _statuses)
            {
                result = status.UpdateStatus();
                if (result) break;
            }
            return result;
        }

        public override bool OnClick(Point location)
        {
            for (var i = 0; i < _statuses.Count; i++)
            {
                if (GetStatusRect(i).Contains(location))
                {
                    _statuses[i].ChangeStatus();
                    OnWidgetUpdate();
                    break;
                }
            }
            return true;
        }

        // no external action - no animation
        public override bool AnimateExit { get { return false; } }

    }

}
