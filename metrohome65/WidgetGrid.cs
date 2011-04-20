using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SmartDeviceProject1
{
    /// <summary>
    /// widget on the screen description
    /// </summary>
    class WidgetScreenInfo
    {
        /// <summary>
        /// screen absolute position and size of widget
        /// </summary>
        public Rectangle Rect = new Rectangle(0, 0, 0, 0);

        public BaseWidget Widget = null;
    }


    class WidgetGrid
    {
        private int CellWidth = 90;

        private int CellHeight = 90;

        private int CellSpacingVer = 9;

        private int CellSpacingHor = 9;

        /// <summary>
        /// widgets list
        /// </summary>
        private List<WidgetScreenInfo> widgets;

        public WidgetGrid()
        {
            widgets = new List<WidgetScreenInfo>();
            Initialize();
        }

        /// <summary>
        /// Initialize stored grid state - widgets set and settings
        /// </summary>
        private void Initialize()
        {
            DebugFill();
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="Widget"></param>
        private void AddWidget(BaseWidget Widget)
        {
            WidgetScreenInfo newWidgetScreenInfo = new WidgetScreenInfo();
            newWidgetScreenInfo.Widget = Widget;
            widgets.Add(newWidgetScreenInfo);

            newWidgetScreenInfo.Rect.X = Widget.position.X * (CellWidth + CellSpacingHor) + CellSpacingHor;
            newWidgetScreenInfo.Rect.Y = Widget.position.Y * (CellHeight + CellSpacingVer) + CellSpacingVer;
            newWidgetScreenInfo.Rect.Width = Widget.size.X * (CellWidth + CellSpacingHor) - CellSpacingHor;
            newWidgetScreenInfo.Rect.Height = Widget.size.Y * (CellHeight + CellSpacingVer) - CellSpacingVer;
        }

        // fill grid with debug values
        private void DebugFill()
        {
            ShortcutWidget PhoneWidget = new ShortcutWidget();
            PhoneWidget.position = new Point(0, 0);
            PhoneWidget.size = new Point(2, 2);
            PhoneWidget.Caption = "Phone";
            PhoneWidget.IconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + 
              "\\icons\\phone.png";
            PhoneWidget.CommandLine = @"\Windows\TaskMgr.exe";
            AddWidget(PhoneWidget);

            ShortcutWidget SMSWidget = new ShortcutWidget();
            SMSWidget.position = new Point(0, 2);
            SMSWidget.size = new Point(2, 2);
            SMSWidget.Caption = "SMS";
            SMSWidget.IconPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
              "\\icons\\mail.png";
            SMSWidget.CommandLine = "";
            AddWidget(SMSWidget);

            ShortcutWidget PeopleWidget = new ShortcutWidget();
            PeopleWidget.position = new Point(2, 0);
            PeopleWidget.size = new Point(2, 2);
            PeopleWidget.Caption = "People";
            PeopleWidget.CommandLine = "";
            AddWidget(PeopleWidget);

            IconWidget AnalogClockWidget = new IconWidget();
            AnalogClockWidget.position = new Point(2, 2);
            AnalogClockWidget.size = new Point(2, 2);
            AddWidget(AnalogClockWidget);

            DigitalClockWidget ClockWidget = new DigitalClockWidget();
            ClockWidget.position = new Point(0, 4);
            ClockWidget.size = new Point(4, 2);
            ClockWidget.bgColor = Color.FromArgb(30, 30, 30);
            AddWidget(ClockWidget);

            IconWidget BTWidget = new IconWidget();
            BTWidget.position = new Point(0, 6);
            BTWidget.size = new Point(1, 1);
            AddWidget(BTWidget);

            IconWidget WiFiWidget = new IconWidget();
            WiFiWidget.position = new Point(0, 7);
            WiFiWidget.size = new Point(1, 1);
            AddWidget(WiFiWidget);
        }

        public void Paint(Graphics g)
        {
            foreach (WidgetScreenInfo wsInfo in widgets) 
            {
                wsInfo.Widget.Paint(g, wsInfo.Rect);
            }
        }

        /// <summary>
        /// Handle click event.
        /// Check each widget for containing event location,
        /// and then call widget's click event handler
        /// </summary>
        /// <param name="Location"></param>
        public void ClickAt(Point Location)
        {
            foreach (WidgetScreenInfo wsInfo in widgets)
            {
                if (wsInfo.Rect.Contains(Location)) 
                {
                    wsInfo.Widget.OnClick(new Point(Location.X - wsInfo.Rect.Left, Location.Y - wsInfo.Rect.Top));
                }
            }
        }
    }
}
