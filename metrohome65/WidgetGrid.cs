using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SmartDeviceProject1
{

    class PageControl : Control
    {
        public virtual void SetScrollPosition(Point Location) { }
        public virtual Point GetScrollPosition() { return new Point(0, 0); }
        public virtual void ClickAt(Point Location) { }
        public virtual void ShowPopupMenu(Point Location) { }
    }

    class WidgetGrid : PageControl
    {
        private int CellWidth = 90;

        private int CellHeight = 90;

        private int CellSpacingVer = 9;

        private int CellSpacingHor = 9;

        /// <summary>
        /// widgets list
        /// </summary>
        private List<WidgetWrapper> Widgets;

        public WidgetGrid()
        {
            Widgets = new List<WidgetWrapper>();
            Initialize();
            CreatePopupMenu();
        }

        private ContextMenu _mnuWidgetActions;
        private WidgetWrapper _mnuWidgetSender;

        private void CreatePopupMenu()
        {
            _mnuWidgetActions = new ContextMenu();

            MenuItem menuSettings = new System.Windows.Forms.MenuItem();
            menuSettings.Text = "Setings";
            menuSettings.Click += ShowWidgetSettings;

            MenuItem menuMove = new System.Windows.Forms.MenuItem();
            menuMove.Text = "Move";
            menuMove.Click += MoveWidget;

            MenuItem menuDelete = new System.Windows.Forms.MenuItem();
            menuDelete.Text = "Delete";
            menuDelete.Click += DeleteWidget;

            _mnuWidgetActions.MenuItems.Add(menuSettings);
            _mnuWidgetActions.MenuItems.Add(menuMove);
            _mnuWidgetActions.MenuItems.Add(menuDelete);
        }

        public override void ShowPopupMenu(Point Location)
        {
            _mnuWidgetSender = GetWidgetAtPos(Location);
            if (_mnuWidgetSender != null)
                _mnuWidgetActions.Show(this, Location);
        }

        /// <summary>
        /// Initialize stored grid state - widgets set and settings
        /// </summary>
        private void Initialize()
        {
            DebugFill();

            PaintBuffer();
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="Widget"></param>
        private WidgetWrapper AddWidget(Point Position, Size Size, IWidget Widget)
        {
            WidgetWrapper Wrapper = new WidgetWrapper(Size, Position, Widget);

            Wrapper.ScreenRect.X = Position.X * (CellWidth + CellSpacingHor) + CellSpacingHor;
            Wrapper.ScreenRect.Y = Position.Y * (CellHeight + CellSpacingVer) + CellSpacingVer;
            Wrapper.ScreenRect.Width = Size.Width * (CellWidth + CellSpacingHor) - CellSpacingHor;
            Wrapper.ScreenRect.Height = Size.Height * (CellHeight + CellSpacingVer) - CellSpacingVer;

            Widgets.Add(Wrapper);

            int Height = 0;
            foreach (WidgetWrapper wsInfo in Widgets)
                Height = Math.Max(Height, wsInfo.ScreenRect.Bottom);
            this.Height = Height + 50;

            return Wrapper;
        }

        private Bitmap DoubleBuffer = null;

        private void PaintBuffer()
        {
            DoubleBuffer = new Bitmap(this.Width, this.Height);
            Graphics g = Graphics.FromImage(DoubleBuffer);

            // paing background
            Brush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            g.FillRectangle(bgBrush, 0, 0, this.Width, this.Height);

            // paint widgets
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                wsInfo.Paint(g, wsInfo.ScreenRect);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(DoubleBuffer, 0, this.TopOffset);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PaintBuffer();
        }

        private int TopOffset = 0;

        public override void SetScrollPosition(Point Location) 
        {
            this.TopOffset = Location.Y;
            Invalidate();
        }

        public override Point GetScrollPosition() 
        { 
            return new Point(0, TopOffset); 
        }

        private WidgetWrapper GetWidgetAtPos(Point Location)
        {
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                if (wsInfo.ScreenRect.Contains(
                    new Point(Location.X, Location.Y + TopOffset)))
                  return wsInfo;
            }
            return null;
        }

        /// <summary>
        /// Handle click event.
        /// Check each widget for containing event location,
        /// and then call widget's click event handler
        /// </summary>
        /// <param name="Location"></param>
        public override void ClickAt(Point Location)
        {
            WidgetWrapper TargetWidget = GetWidgetAtPos(Location);
            if (TargetWidget != null)
                TargetWidget.OnClick(new Point(Location.X - TargetWidget.ScreenRect.Left, Location.Y - TargetWidget.ScreenRect.Top));
        }

        private void ShowWidgetSettings(object sender, EventArgs e)
        {
            MessageBox.Show("settings");
        }

        private void DeleteWidget(object sender, EventArgs e)
        {
            MessageBox.Show("delete");
        }

        private void MoveWidget(object sender, EventArgs e)
        {
            MessageBox.Show("move");
        }


        // fill grid with debug values
        private void DebugFill()
        {
            ShortcutWidget PhoneWidget = new ShortcutWidget();
            PhoneWidget.Caption = "Phone";
            PhoneWidget.IconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
              "\\icons\\phone.png";
            PhoneWidget.CommandLine = @"\Windows\TaskMgr.exe";
            AddWidget(new Point(0, 0), new Size(2, 2), PhoneWidget);

            ShortcutWidget SMSWidget = new ShortcutWidget();
            SMSWidget.Caption = "SMS";
            SMSWidget.IconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
              "\\icons\\mail.png";
            SMSWidget.CommandLine = "";
            AddWidget(new Point(0, 2), new Size(2, 2), SMSWidget);

            ShortcutWidget PeopleWidget = new ShortcutWidget();
            PeopleWidget.Caption = "People";
            PeopleWidget.CommandLine = "";
            AddWidget(new Point(2, 0), new Size(2, 2), PeopleWidget);

            IconWidget AnalogClockWidget = new IconWidget();
            AddWidget(new Point(2, 2), new Size(2, 2), AnalogClockWidget);

            DigitalClockWidget ClockWidget = new DigitalClockWidget();
            AddWidget(new Point(0, 4), new Size(4, 2), ClockWidget).
                bgColor = Color.FromArgb(30, 30, 30);

            IconWidget BTWidget = new IconWidget();
            AddWidget(new Point(0, 6), new Size(1, 1), BTWidget);

            IconWidget WiFiWidget = new IconWidget();
            AddWidget(new Point(0, 7), new Size(1, 1), WiFiWidget);

            DigitalClockWidget ClockWidget2 = new DigitalClockWidget();
            AddWidget(new Point(0, 8), new Size(4, 2), ClockWidget2).
                bgColor = Color.FromArgb(30, 30, 30);
        }

    }
}
