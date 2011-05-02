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
        protected virtual void SetActive(Boolean Active) { }
        public Boolean Active { set { SetActive(value); } }
    }

    class WidgetGrid : PageControl
    {
        private List<WidgetWrapper> Widgets;
        private ContextMenu _mnuWidgetActions;
        private WidgetWrapper _mnuWidgetSender;
        private Bitmap _DoubleBuffer = null;
        private Graphics _graphics = null;
        private int TopOffset = 0;

        public WidgetGrid() : base()
        {
            this.ForeColor = Color.Black;
            this.BackColor = Color.Black;

            CreatePopupMenu();

            Widgets = new List<WidgetWrapper>();

            _DoubleBuffer = new Bitmap(this.Width, this.Height);
            _graphics = Graphics.FromImage(_DoubleBuffer);

            ReadSettings();
            PaintBuffer();
        }

        protected override void SetActive(Boolean Active) 
        {
            // start updatable widgets
            foreach (WidgetWrapper wsInfo in Widgets)
                if (wsInfo.Widget is IWidgetUpdatable)
                {
                    if (Active)
                        (wsInfo.Widget as IWidgetUpdatable).StartUpdate();
                    else
                        (wsInfo.Widget as IWidgetUpdatable).StopUpdate();
                }
        }

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
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="Widget"></param>
        private WidgetWrapper AddWidget(Point Position, Size Size, IWidget Widget)
        {
            WidgetWrapper Wrapper = new WidgetWrapper(Size, Position, Widget);

            Widgets.Add(Wrapper);

            if (Widget is IWidgetUpdatable)
                (Widget as IWidgetUpdatable).WidgetUpdate += new WidgetUpdateEventHandler(OnWidgetUpdate);

            return Wrapper;
        }

        /// <summary>
        /// Handler for event, triggered by widget, when it needs to be repainted
        /// </summary>
        /// <param name="sender"></param>
        private void OnWidgetUpdate(object sender, WidgetUpdateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WidgetUpdateEventHandler(OnWidgetUpdate), new object[] { this, e });
                return;
            }

            // repaint only updated widget
            foreach (WidgetWrapper wsInfo in Widgets)
                if (e.Widget == wsInfo.Widget)
                    wsInfo.Paint(_graphics, wsInfo.ScreenRect);
            this.Invalidate();
            //!! todo - repaint on screen only changed part 
        }

        /// <summary>
        /// Update may be change widgets screen positions
        /// </summary>
        private void RealignWidgets()
        {
            int Height = 0;
            foreach (WidgetWrapper wsInfo in Widgets)
                Height = Math.Max(Height, wsInfo.ScreenRect.Bottom);
            this.Height = Height + 50;

            WriteSettings();
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        private void WriteSettings()
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(Widgets.GetType());
                System.IO.TextWriter writer =
                  new System.IO.StreamWriter("myItems.xml", false);
                serializer.Serialize(writer, Widgets);
                writer.Close();
            }
            catch
            { 
            }
        }

        /// <summary>
        /// Read widgets settings from XML file
        /// </summary>
        private void ReadSettings()
        {
            Widgets.Clear();

            try
            {
                /*
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(Widgets.GetType());
                System.IO.TextReader reader =
                  new System.IO.StreamReader("myItems.xml");
                Widgets = (List<WidgetWrapper>)serializer.Deserialize(reader);
                reader.Close();
                 */
                DebugFill();
            }
            catch
            {
                DebugFill();
            }

            RealignWidgets();
        }

        private void PaintBuffer()
        {
            // paing background
            Brush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            _graphics.FillRectangle(bgBrush, 0, 0, this.Width, this.Height);

            // paint widgets
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                wsInfo.Paint(_graphics, wsInfo.ScreenRect);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_DoubleBuffer, 0, this.TopOffset);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _DoubleBuffer = new Bitmap(this.Width, this.Height);
            _graphics = Graphics.FromImage(_DoubleBuffer);

            PaintBuffer();
        }

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
              "\\icons\\Phone.png";
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
