﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetroHome65.Widgets;

namespace MetroHome65.Pages
{

    class WidgetGrid : CustomPageControl, IPageControl
    {
        private static int PaddingHor = 30; //todo comment
        private static int PaddingVer = 5; //todo comment

        private String _CoreDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private String SettingsFile() { return _CoreDir + "\\settings.xml"; }

        private List<WidgetWrapper> Widgets;
        private ContextMenu _mnuWidgetActions;
        private WidgetWrapper _mnuWidgetSender;
        private int TopOffset = 0;
        private Boolean _Active = false;

        PictureBox _PictureBoxArrow = new PictureBox();
        private Bitmap _DoubleBuffer = null;
        private Graphics _graphics = null;
        private PictureBox _WidgetsImage = new PictureBox();
        private Panel _WidgetsContainer = new Panel();

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WidgetGrid
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Name = "WidgetGrid";
            this.Size = new System.Drawing.Size(200, 200);
            this.ResumeLayout(false);
        }

        public WidgetGrid() : base()
        {
            Widgets = new List<WidgetWrapper>();

            _mnuWidgetActions = new ContextMenu();

            _DoubleBuffer = new Bitmap(1, 1);
            _graphics = Graphics.FromImage(_DoubleBuffer);

            _WidgetsImage.Location = new Point(0, 0);

            _WidgetsContainer.Controls.Add(_WidgetsImage);
            _WidgetsContainer.Location = new Point(PaddingHor, PaddingVer);
            this.Controls.Add(_WidgetsContainer);

            _PictureBoxArrow.Size = Properties.Resources.arrow_right_white.Size;
            _PictureBoxArrow.Image = Properties.Resources.arrow_right_white;
            _PictureBoxArrow.Click += new EventHandler(_PictureBoxArrow_Click);
            this.Controls.Add(_PictureBoxArrow);

            ReadSettings();
        }

        protected override void SetActive(Boolean Active) 
        {
            if (_Active == Active)
                return;

            // start updatable widgets
            foreach (WidgetWrapper wsInfo in Widgets)
                wsInfo.Active = Active;
        }

        public override void SetBackColor(Color value) 
        { 
            this.BackColor = value;
            _WidgetsContainer.BackColor = value;
            PaintBuffer();
        }

        private void FillPopupMenu(IWidget Widget)
        {
            _mnuWidgetActions.MenuItems.Clear();

            // fill menu with widget customn actions
            if (Widget.MenuItems != null)
            {
                foreach (String Item in Widget.MenuItems)
                {
                    MenuItem CustomItem = new System.Windows.Forms.MenuItem();
                    CustomItem.Text = Item;
                    CustomItem.Click += OnCustomMenuClick;
                    _mnuWidgetActions.MenuItems.Add(CustomItem);
                }

                // add separator
                MenuItem Separator = new System.Windows.Forms.MenuItem();
                Separator.Text = "-";
                _mnuWidgetActions.MenuItems.Add(Separator);
            }

            // fill menu with standart actions
            MenuItem menuSettings = new System.Windows.Forms.MenuItem();
            menuSettings.Text = "Setings";
            menuSettings.Click += ShowWidgetSettings;
            _mnuWidgetActions.MenuItems.Add(menuSettings);

            MenuItem menuMove = new System.Windows.Forms.MenuItem();
            menuMove.Text = "Move";
            menuMove.Click += MoveWidget;
            _mnuWidgetActions.MenuItems.Add(menuMove);

            MenuItem menuDelete = new System.Windows.Forms.MenuItem();
            menuDelete.Text = "Delete";
            menuDelete.Click += DeleteWidget;
            _mnuWidgetActions.MenuItems.Add(menuDelete);
        }

        public override Boolean ShowPopupMenu(Point Location)
        {
            _mnuWidgetSender = GetWidgetAtPos(Location);
            if ((_mnuWidgetSender != null) && (_mnuWidgetSender.Widget != null))
            {
                FillPopupMenu(_mnuWidgetSender.Widget);
                _mnuWidgetActions.Show(this, Location);
                return true;
            }
            return false;
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="Widget"></param>
        private WidgetWrapper AddWidget(Point Position, Size Size, String WidgetName)
        {
            WidgetWrapper Wrapper = new WidgetWrapper(Size, Position, WidgetName);

            Widgets.Add(Wrapper);

            if (Wrapper.Widget is IWidgetUpdatable)
                (Wrapper.Widget as IWidgetUpdatable).WidgetUpdate += new WidgetUpdateEventHandler(OnWidgetUpdate);

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
                {
                    wsInfo.Paint(_graphics, wsInfo.ScreenRect);

                    Rectangle Rect = new Rectangle(
                        wsInfo.ScreenRect.X, wsInfo.ScreenRect.Y, wsInfo.ScreenRect.Width, wsInfo.ScreenRect.Height);
                    _WidgetsImage.Invalidate(Rect);
                }
        }

        private void PaintBuffer()
        {
            // paing background
            Brush bgBrush = new System.Drawing.SolidBrush(this.BackColor);
            _graphics.FillRectangle(bgBrush, 0, 0, _DoubleBuffer.Width, _DoubleBuffer.Height);

            // paint widgets
            foreach (WidgetWrapper wsInfo in Widgets)
                wsInfo.Paint(_graphics, new Rectangle(
                    wsInfo.ScreenRect.Left, wsInfo.ScreenRect.Top, wsInfo.ScreenRect.Width, wsInfo.ScreenRect.Height));
        }


        /// <summary>
        /// Update may be change widgets screen positions
        /// </summary>
        private void RealignWidgets()
        {
            object[,] cells = new object[100, 4]; //!! todo calc y dim
            int MaxRow = 0;
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                for (int y = 0; y < wsInfo.Size.Height; y++)
                    for (int x = 0; x < wsInfo.Size.Width; x++)
                        cells[wsInfo.Location.Y + y, wsInfo.Location.X + x] = wsInfo;
                MaxRow = Math.Max(MaxRow, wsInfo.Location.Y + wsInfo.Size.Height);
            }

            // looking for empty rows and delete them - shift widgets 1 row top
            for (int row = MaxRow; row >= 0; row--)
            {
                if ((cells[row, 0] == null) && (cells[row, 1] == null) && (cells[row, 2] == null) && (cells[row, 3] == null))
                {
                    foreach (WidgetWrapper wsInfo in Widgets)
                    {
                        if (wsInfo.Location.Y > row)
                            wsInfo.Location = new Point(wsInfo.Location.X, wsInfo.Location.Y - 1);
                    }
                }
            }


            // write new widgets position after realign
            WriteSettings();

            // calc max image dimensions for widgets grid
            int WidgetsHeight = 0;
            int WidgetsWidth = 0;
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                WidgetsHeight = Math.Max(WidgetsHeight, wsInfo.ScreenRect.Bottom);
                WidgetsWidth = Math.Max(WidgetsWidth, wsInfo.ScreenRect.Right);
            }
            WidgetsHeight += 10; // add padding at bottom and blank spaces at top and bottom


            // change size of internal bitmap and repaint it
            _graphics.Dispose();
            _DoubleBuffer.Dispose();

            _DoubleBuffer = new Bitmap(WidgetsWidth, WidgetsHeight);
            _graphics = Graphics.FromImage(_DoubleBuffer);

            PaintBuffer();

            if (_WidgetsImage.Image != null)
                _WidgetsImage.Image.Dispose();
            _WidgetsImage.Image = _DoubleBuffer;
            _WidgetsImage.Size = _DoubleBuffer.Size;

            //!! todo - известить глав форму чтоб пересчитала скролл
        }


        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        private void WriteSettings()
        {
            /*
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(Widgets.GetType());
                System.IO.TextWriter writer =
                  new System.IO.StreamWriter(SettingsFile(), false);
                serializer.Serialize(writer, Widgets);
                writer.Close();
            }
            catch
            {
                //!! write to log  (e.StackTrace, "WriteSettings")
            }
            */
        }


        /// <summary>
        /// Read widgets settings from XML file
        /// </summary>
        private void ReadSettings()
        {
            Widgets.Clear();

            try
            {                
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(Widgets.GetType());
                System.IO.TextReader reader =
                  new System.IO.StreamReader(SettingsFile());
                Widgets = (List<WidgetWrapper>)serializer.Deserialize(reader);
                reader.Close();
                
                DebugFill();
            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "ReadSettings")
                DebugFill();
            }

            RealignWidgets();
        }

     
        public override void SetScrollPosition(Point Location) 
        {
            this.TopOffset = Location.Y;

            _WidgetsImage.Top = this.TopOffset;
            _WidgetsContainer.Refresh();
        }
        
        public override Point GetScrollPosition() { return new Point(0, TopOffset); }

        public override Size GetExtentSize() { return _DoubleBuffer.Size; }

        public override Size GetViewportSize() { return new Size(this.Width, this.Height - PaddingVer); }

        
        private WidgetWrapper GetWidgetAtPos(Point Location)
        {
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                if (wsInfo.ScreenRect.Contains(
                    new Point(Location.X - PaddingHor, Location.Y - TopOffset - PaddingVer)))
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
                TargetWidget.OnClick(new Point(
                    Location.X - PaddingHor - TargetWidget.ScreenRect.Left,
                    Location.Y - PaddingVer - TopOffset - TargetWidget.ScreenRect.Top));
        }

        public override void DblClickAt(Point Location)
        {
            WidgetWrapper TargetWidget = GetWidgetAtPos(Location);
            if (TargetWidget != null)
                TargetWidget.OnDblClick(new Point(
                    Location.X - PaddingHor - TargetWidget.ScreenRect.Left,
                    Location.Y - PaddingVer - TopOffset - TargetWidget.ScreenRect.Top));
        }

        /// <summary>
        /// Shows widget settings dialog and applies changes in widget settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWidgetSettings(object sender, EventArgs e)
        {
            FrmWidgetSettings WidgetSettingsForm = new FrmWidgetSettings();
            WidgetSettingsForm.Widget = _mnuWidgetSender;
            WidgetSettingsForm.Owner = (Form)this.Parent;
            if (WidgetSettingsForm.ShowDialog() == DialogResult.OK)
                RealignWidgets();

        }

        /// <summary>
        /// Delete current widget from grid.
        /// Then widgets are realigned, if deleted widget was alone on its row.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteWidget(object sender, EventArgs e)
        {
            if (_mnuWidgetSender != null)
            {
                Widgets.Remove(_mnuWidgetSender);
                RealignWidgets();
            }
        }

        private void MoveWidget(object sender, EventArgs e)
        {
            MessageBox.Show("move");
        }

        private void OnCustomMenuClick(object sender, EventArgs e)
        {
            if (sender is MenuItem)
                MessageBox.Show((sender as MenuItem).Text);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _WidgetsContainer.Size = new Size(
                WidgetWrapper.CellWidth * 4 + WidgetWrapper.CellSpacingHor * 3, 
                this.Height - PaddingVer);

            _PictureBoxArrow.Location = new Point(
                this.Width - (this.Width - PaddingHor - _WidgetsContainer.Width)/2 - _PictureBoxArrow.Width/2, PaddingVer);
        }


        void _PictureBoxArrow_Click(object sender, EventArgs e)
        {
            OnChangePage(EventArgs.Empty);
        }


        // fill grid with debug values
        private void DebugFill()
        {
            Widgets.Clear();

            BaseWidget newWidget;

            newWidget = new SMSWidget();
            AddWidget(new Point(0, 0), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\Windows\tMail.exe").
                SetParameter("Caption", "SMS").
                SetParameter("IconPath", _CoreDir + "\\icons\\mail.png").
                SetColor(Color.Orange);

            newWidget = new PhoneWidget();
            AddWidget(new Point(0, 2), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\windows\cprog.exe").
                SetParameter("Caption", "Phone").
                SetParameter("IconPath", _CoreDir + "\\icons\\phone.png").
                SetButtonImage(_CoreDir + "\\buttons\\button gray.png");

            newWidget = new ShortcutWidget();
            AddWidget(new Point(2, 0), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\Windows\Start Menu\Programs\Contacts.lnk").
                SetParameter("Caption", "Contacts").
                SetParameter("IconPath", _CoreDir + "\\icons\\contacts.png").
                SetButtonImage(_CoreDir + "\\buttons\\button gray.png");

            newWidget = new ShortcutWidget();
            AddWidget(new Point(2, 2), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\Windows\iexplore.exe").
                SetParameter("Caption", "Internet Explorer").
//                SetParameter("IconPath", _CoreDir + "\\icons\\iexplore.png").
                SetButtonImage(_CoreDir + "\\buttons\\button blue.png");

            newWidget = new DigitalClockWidget();
            AddWidget(new Point(0, 4), new Size(4, 2), newWidget.ToString()).
                SetButtonImage(_CoreDir + "\\buttons\\bg5.png");

            newWidget = new ShortcutWidget();
            AddWidget(new Point(0, 6), new Size(1, 1), newWidget.ToString()).
                SetButtonImage(_CoreDir + "\\buttons\\button gray.png");

            newWidget = new ShortcutWidget();
            AddWidget(new Point(1, 6), new Size(1, 1), newWidget.ToString()).
                SetColor(Color.DarkGreen);

            newWidget = new ShortcutWidget();
            AddWidget(new Point(2, 6), new Size(1, 1), newWidget.ToString()).
                SetButtonImage(_CoreDir + "\\buttons\\button blue.png"); ;

            newWidget = new ShortcutWidget();
            AddWidget(new Point(3, 6), new Size(1, 1), newWidget.ToString()).
                SetColor(Color.Maroon);

            newWidget = new ContactWidget();
            AddWidget(new Point(0, 7), new Size(2, 2), newWidget.ToString());

            newWidget = new ContactWidget();
            AddWidget(new Point(2, 7), new Size(2, 2), newWidget.ToString());

            newWidget = new ShortcutWidget();
            AddWidget(new Point(0, 9), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\Windows\MobileCalculator.exe").
                SetParameter("Caption", "Calculator").
                SetParameter("IconPath", _CoreDir + "\\icons\\calc.png");

            newWidget = new ShortcutWidget();
            AddWidget(new Point(2, 9), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\Windows\wmplayer.exe").
                SetParameter("Caption", "Media player").
                SetParameter("IconPath", _CoreDir + "\\icons\\media.png");

            newWidget = new ShortcutWidget();
            AddWidget(new Point(0, 11), new Size(2, 2), newWidget.ToString()).
                SetParameter("CommandLine", @"\Windows\fexplore.exe").
                SetParameter("Caption", "Explorer").
                SetParameter("IconPath", _CoreDir + "\\icons\\fexplore.png");

            RealignWidgets();
        }

    }
}
