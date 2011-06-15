using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetroHome65.Widgets;
using Microsoft.WindowsMobile.Gestures;

namespace MetroHome65.Pages
{

    public partial class WidgetGrid : UserControl, IPageControl
    {
        private int TopOffset = 0;
        private MetroHome65.Main.IHost _Host;

        private String _CoreDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private String SettingsFile() { return _CoreDir + "\\settings.xml"; }

        private List<WidgetWrapper> Widgets;
        private WidgetWrapper _mnuWidgetSender;
        private Bitmap _DoubleBuffer = null;
        private Graphics _graphics = null;

        public WidgetGrid()
        {
            //!! read widget settings and place user widgets
            Widgets = new List<WidgetWrapper>();

            _DoubleBuffer = new Bitmap(1, 1);
            _graphics = Graphics.FromImage(_DoubleBuffer);

            PluginManager.GetInstance();

            InitializeComponent();

            ReadSettings();
        }

        public virtual Control GetControl() { return this; }

        private Boolean _Active = false;
        public Boolean Active { set { SetActive(value); } }

        protected void SetActive(Boolean Active) 
        {
            if (_Active != Active)
            {
                _Active = Active;
                gestureRecognizer.TargetControl = (_Active) ? this : null;

                // start updatable widgets
                foreach (WidgetWrapper wsInfo in Widgets)
                    wsInfo.Active = _Active;
            }
        }

        public void SetHost(MetroHome65.Main.IHost Host)
        {
            _Host = Host;
        }

        public void SetBackColor(Color value) 
        { 
            this.BackColor = value;
            _WidgetsContainer.BackColor = value;
            PaintBuffer();
        }

        /// <summary>
        /// Fill popup menu with widget specific actions.
        /// If widget has its own actions they are added to standard widget menu
        /// </summary>
        /// <param name="Widget"></param>
        /// <param name="Menu"></param>
        private void FillWidgetPopupMenu(IWidget Widget, ContextMenu Menu)
        {
            // fill menu with widget customn actions
            if ((Widget != null) && (Widget.MenuItems != null))
            {
                foreach (String Item in Widget.MenuItems)
                {
                    MenuItem CustomItem = new System.Windows.Forms.MenuItem();
                    CustomItem.Text = Item;
                    CustomItem.Click += OnCustomMenuClick;
                    Menu.MenuItems.Add(CustomItem);
                }

                // add separator
                MenuItem Separator = new System.Windows.Forms.MenuItem();
                Separator.Text = "-";
                Menu.MenuItems.Add(Separator);
            }

            // fill menu with standart actions
            if (Widget != null)
            {
                MenuItem menuSettings = new System.Windows.Forms.MenuItem();
                menuSettings.Text = "Setings";
                menuSettings.Click += ShowWidgetSettings;
                Menu.MenuItems.Add(menuSettings);
            }

            MenuItem menuMove = new System.Windows.Forms.MenuItem();
            menuMove.Text = "Move";
            menuMove.Click += MoveWidget;
            Menu.MenuItems.Add(menuMove);

            MenuItem menuDelete = new System.Windows.Forms.MenuItem();
            menuDelete.Text = "Delete";
            menuDelete.Click += DeleteWidget;
            Menu.MenuItems.Add(menuDelete);
        }

        /// <summary>
        /// Fill popup menu for widget grid with grid settings
        /// </summary>
        /// <param name="Menu"></param>
        private void FillMainPopupMenu(ContextMenu Menu)
        {
            MenuItem menuAddWidget = new System.Windows.Forms.MenuItem();
            menuAddWidget.Text = "Add widget";
            menuAddWidget.Click += AddWidget_Click;
            Menu.MenuItems.Add(menuAddWidget);

            // add separator
            MenuItem Separator = new System.Windows.Forms.MenuItem();
            Separator.Text = "-";
            Menu.MenuItems.Add(Separator);

            MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "Exit";
            menuExit.Click += Exit_Click;
            Menu.MenuItems.Add(menuExit);
        }

        public Boolean ShowWidgetPopupMenu(Point Location)
        {
            _mnuWidgetActions.MenuItems.Clear();

            _mnuWidgetSender = GetWidgetAtPos(Location);
            if (_mnuWidgetSender != null)
                FillWidgetPopupMenu(_mnuWidgetSender.Widget, _mnuWidgetActions);
            else
                FillMainPopupMenu(_mnuWidgetActions);

            _mnuWidgetActions.Show(this, Location);
            return true;
        }

        private void AddWidget_Click(object sender, EventArgs e)
        {
            AddWidget(new Point(0, 90), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget");
        }

        /// <summary>
        /// add widget to internal collection 
        /// and calc widget screen coordinates
        /// </summary>
        /// <param name="Widget"></param>
        public WidgetWrapper AddWidget(Point Position, Size Size, String WidgetName)
        {
            return AddWidget(Position, Size, WidgetName, true);
        }

        private WidgetWrapper AddWidget(Point Position, Size Size, String WidgetName, bool DoRealign)
        {
            WidgetWrapper Wrapper = new WidgetWrapper(Size, Position, WidgetName);

            Widgets.Add(Wrapper);
            Wrapper.WidgetGrid = this;

            if (Wrapper.Widget is IWidgetUpdatable)
                (Wrapper.Widget as IWidgetUpdatable).WidgetUpdate += new WidgetUpdateEventHandler(OnWidgetUpdate);

            if (DoRealign)
                RealignWidgets();

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
                wsInfo.Paint(_graphics, wsInfo.ScreenRect);
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
            _graphics = null;
            _DoubleBuffer.Dispose();
            _DoubleBuffer = null;

            _DoubleBuffer = new Bitmap(WidgetsWidth, WidgetsHeight);
            _graphics = Graphics.FromImage(_DoubleBuffer);

            PaintBuffer();

            if (_WidgetsImage.Image != null)
                _WidgetsImage.Image.Dispose();
            _WidgetsImage.Image = _DoubleBuffer;
            _WidgetsImage.Size = _DoubleBuffer.Size;

            UpdateGridSize();
        }

        private void MoveWidgetTo(Point Location)
        {
            Point TargetCell = new Point(
                (Location.X - _WidgetsContainer.Left + WidgetWrapper.CellSpacingHor) / (WidgetWrapper.CellWidth + WidgetWrapper.CellSpacingHor),
                (Location.Y - _WidgetsContainer.Top  + WidgetWrapper.CellSpacingVer - TopOffset) / (WidgetWrapper.CellHeight + WidgetWrapper.CellSpacingVer)
                );

            // if widget doesn't fit to new position by width, do not do anything
            if (TargetCell.X + MovingWidget.Size.Width > 4)
                return;

            object[,] cells = new object[100, 4]; //!! todo calc y dim
            bool PlaceEmpty = false;
            while (!PlaceEmpty)
            {
                for (int y = 0; y < 100; y++)
                    for (int x = 0; x < 4; x++)
                        cells[y, x] = null;

                foreach (WidgetWrapper wsInfo in Widgets)
                {
                    for (int y = 0; y < wsInfo.Size.Height; y++)
                        for (int x = 0; x < wsInfo.Size.Width; x++)
                            cells[wsInfo.Location.Y + y, wsInfo.Location.X + x] = wsInfo;
                }

                PlaceEmpty = true;
                for (int y = TargetCell.Y; y < Math.Min(TargetCell.Y + MovingWidget.Size.Height, 100); y++)
                    for (int x = TargetCell.X; x < Math.Min(TargetCell.X + MovingWidget.Size.Width, 4); x++)
                        if ((cells[y, x] != null) && (!cells[y, x].Equals(MovingWidget)))
                        {
                            PlaceEmpty = false;
                            break;
                        }

                if (!PlaceEmpty)
                    foreach (WidgetWrapper wsInfo in Widgets)
                    {
                        if ((wsInfo.Location.Y + wsInfo.Size.Height - 1) >= TargetCell.Y)
                            wsInfo.Location = new Point(wsInfo.Location.X, wsInfo.Location.Y + 1);
                    }
            }

            MovingWidget.Location = TargetCell;

            RealignWidgets();       
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

                foreach (WidgetWrapper Widget in Widgets)
                    Widget.WidgetGrid = this;

            }
            catch (Exception e)
            {
                //!! write to log  (e.StackTrace, "ReadSettings")
                DebugFill();
            }

            RealignWidgets();
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
                  new System.IO.StreamWriter(SettingsFile(), false);
                serializer.Serialize(writer, Widgets);
                writer.Close();
            }
            catch
            {
                //!! write to log  (e.StackTrace, "WriteSettings")
            }
        }
        

        private WidgetWrapper GetWidgetAtPos(Point Location)
        {
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                if (wsInfo.ScreenRect.Contains(
                    new Point(Location.X - _WidgetsContainer.Left, Location.Y - TopOffset - _WidgetsContainer.Top)))
                  return wsInfo;
            }
            return null;
        }

        private WidgetWrapper GetBottomWidgetForPos(Point Location)
        {
            Rectangle WidgetRect;
            foreach (WidgetWrapper wsInfo in Widgets)
            {
                WidgetRect = wsInfo.ScreenRect;
                WidgetRect.Y -= WidgetWrapper.CellSpacingVer;
                WidgetRect.Height += WidgetWrapper.CellSpacingVer;

                if (WidgetRect.Contains(
                    new Point(Location.X - _WidgetsContainer.Left, Location.Y - TopOffset - _WidgetsContainer.Top)))
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
        private void ClickAt(Point Location)
        {
            WidgetWrapper TargetWidget = GetWidgetAtPos(Location);

            // if Move mode is enabled, place selected widget to the new position
            // if we click at moving widget, exit from move mode
            if (MoveMode)
            {
                if (TargetWidget == MovingWidget)
                    MovingWidget = null;
                else
                    MoveWidgetTo(Location);
                return;
            }

            if (TargetWidget != null)
                TargetWidget.OnClick(new Point(
                    Location.X - _WidgetsContainer.Left - TargetWidget.ScreenRect.Left,
                    Location.Y - _WidgetsContainer.Top - TopOffset - TargetWidget.ScreenRect.Top));
        }

        private void DblClickAt(Point Location)
        {
            WidgetWrapper TargetWidget = GetWidgetAtPos(Location);
            if (TargetWidget != null)
                TargetWidget.OnDblClick(new Point(
                    Location.X - _WidgetsContainer.Left - TargetWidget.ScreenRect.Left,
                    Location.Y - _WidgetsContainer.Top - TopOffset - TargetWidget.ScreenRect.Top));
        }

        private void HoldAt(Point Location)
        {
            if (MoveMode)
                return;
                        
            if (!ShowWidgetPopupMenu(Location))
                mnuMain.Show(this, Location);
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
            if (_mnuWidgetSender != null)
            {
                MovingWidget = _mnuWidgetSender;
            }
        }

        private void OnCustomMenuClick(object sender, EventArgs e)
        {
            if (sender is MenuItem)
                MessageBox.Show((sender as MenuItem).Text);
        }


        private void WidgetGrid_Resize(object sender, EventArgs e)
        {
            _WidgetsContainer.Size = new Size(
                WidgetWrapper.CellWidth * 4 + WidgetWrapper.CellSpacingHor * 3, 
                this.Height - _WidgetsContainer.Top);

            buttonNextPage.Location = new Point(
                this.Width - (this.Width - _WidgetsContainer.Left - _WidgetsContainer.Width) / 2 - buttonNextPage.Width / 2, _WidgetsContainer.Top);

            UpdateGridSize();
        }


        private void buttonNextPage_Click(object sender, EventArgs e)
        {
            if (_Host != null)
                _Host.ChangePage(true);
        }

        /// <summary>
        /// Menu handler - Close application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        // fill grid with debug values
        private void DebugFill()
        {
            Widgets.Clear();

            AddWidget(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.SMSWidget", false).
                SetParameter("CommandLine", @"\Windows\tMail.exe").
                SetParameter("Caption", "SMS").
                SetParameter("IconPath", _CoreDir + "\\icons\\mail.png").
                SetColor(Color.Orange);

            AddWidget(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.PhoneWidget", false).
                SetParameter("CommandLine", @"\windows\cprog.exe").
                SetParameter("Caption", "Phone").
                SetParameter("IconPath", _CoreDir + "\\icons\\phone.png").
                SetButtonImage(_CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(2, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\Start Menu\Programs\Contacts.lnk").
                SetParameter("Caption", "Contacts").
                SetParameter("IconPath", _CoreDir + "\\icons\\contacts.png").
                SetButtonImage(_CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\iexplore.exe").
                SetParameter("Caption", "Internet Explorer").
//                SetParameter("IconPath", _CoreDir + "\\icons\\iexplore.png").
                SetButtonImage(_CoreDir + "\\buttons\\button blue.png");

            AddWidget(new Point(0, 4), new Size(4, 2), "MetroHome65.Widgets.DigitalClockWidget", false).
                SetButtonImage(_CoreDir + "\\buttons\\bg5.png");

            AddWidget(new Point(0, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetButtonImage(_CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(1, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetColor(Color.DarkGreen);

            AddWidget(new Point(2, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetButtonImage(_CoreDir + "\\buttons\\button blue.png"); ;

            AddWidget(new Point(3, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetColor(Color.Maroon);

            AddWidget(new Point(0, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddWidget(new Point(2, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddWidget(new Point(0, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\MobileCalculator.exe").
                SetParameter("Caption", "Calculator").
                SetParameter("IconPath", _CoreDir + "\\icons\\calc.png");

            AddWidget(new Point(2, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wmplayer.exe").
                SetParameter("Caption", "Media player").
                SetParameter("IconPath", _CoreDir + "\\icons\\media.png");

            AddWidget(new Point(0, 11), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\fexplore.exe").
                SetParameter("Caption", "Explorer").
                SetParameter("IconPath", _CoreDir + "\\icons\\fexplore.png");
        }


       #region Scroll and Pan

        private int _ScrollLastY;

        private void gestureRecognizer1_Select(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            ClickAt(e.Location);
        }

        private void gestureRecognizer_DoubleSelect(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            DblClickAt(e.Location);
        }

        private void gestureRecognizer_Hold(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            HoldAt(e.Location);
        }

        private void gestureRecognizer_Begin(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            this.physics.Stop();
        }

        private void gestureRecognizer_End(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            if (!this.physics.IsAnimating)
            {
                this.physics.Angle = 0;
                this.physics.Velocity = 0;
                this.physics.Start();
            }
        }

        private void gestureRecognizer_Pan(object sender, Microsoft.WindowsMobile.Gestures.GestureEventArgs e)
        {
            if (MoveMode)
            {
                MoveWidgetTo(e.Location);
                return;
            }

            this.physics.Stop();

            if ((e.State & GestureState.Begin) == GestureState.Begin)
            {
                this._ScrollLastY = e.Location.Y;
                return;
            }

            int delta = e.Location.Y - this._ScrollLastY;
            this._ScrollLastY = e.Location.Y;

            ScrollTo(TopOffset + delta);
        }

        private void gestureRecognizer_Scroll(object sender, Microsoft.WindowsMobile.Gestures.GestureScrollEventArgs e)
        {
            physics.Stop();
            physics.Start(e.Angle, e.Velocity);
        }

        private void ScrollTo(int YOffset)
        {
            this.TopOffset = YOffset;

            _WidgetsImage.Top = this.TopOffset;
            _WidgetsContainer.Refresh();

            if (!this.physics.IsAnimating)
                this.physics.Origin = new Point(0, -TopOffset);
        }

        private void UpdateGridSize()
        {
            physics.Stop();

            physics.Extent = _DoubleBuffer.Size;
            physics.ViewportSize = _WidgetsContainer.Size;

            physics.Start(0, 1);
        }

        private void physics_AnimateFrame(object sender, PhysicsAnimationFrameEventArgs e)
        {
            ScrollTo(- e.Location.Y);
        }

        #endregion


        #region Moving widget

        private WidgetWrapper _MovingWidget = null;
        private WidgetWrapper MovingWidget
        {
            get { return _MovingWidget;  }
            set {
                if (_MovingWidget != value)
                {
                    if (value == null)
                    {
                        _ResizeTimer = null;
                        _MovingWidget.Moving = false;
                        _MovingWidget = null;
                    }
                    else
                    {
                        _MovingWidget = value;
                        _MovingWidget.Moving = true;

                        //_ResizeTimer = new System.Threading.Timer(OnResizeTimer, null, 0, 400);
                        _ResizeTimer = new System.Windows.Forms.Timer();
                        _ResizeTimer.Tick += OnResizeTimer;
                        _ResizeTimer.Interval = 500;
                        _ResizeTimer.Enabled = true;
                    }
                    RealignWidgets();
                }
            }
        }
        private bool MoveMode { get { return _MovingWidget != null; } }

        private System.Windows.Forms.Timer _ResizeTimer;
        private int _deltaX = 0;
        private int _deltaY = 0;
        private int _deltaXInc = 1;
        private int _deltaYInc = -1;

        //private void OnResizeTimer(object state)
        private void OnResizeTimer(object sender, EventArgs e)
        {
            if (_MovingWidget != null)
                RepaintMovingWidget();
        }

        private delegate void OnRepaintMovingWidget();
        private void RepaintMovingWidget()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new OnRepaintMovingWidget(RepaintMovingWidget), null);
                return;
            }

            _MovingWidget.CalcScreenPosition();
            Rectangle _MovingWidgetRect = new Rectangle(_MovingWidget.ScreenRect.Left, _MovingWidget.ScreenRect.Top,
                _MovingWidget.ScreenRect.Width, _MovingWidget.ScreenRect.Height);
            _MovingWidgetRect.Inflate(_deltaX >> 1, _deltaY >> 1);
            _MovingWidgetRect.Offset(_deltaX, -_deltaY);

            if ((_deltaX >= 2) || (_deltaX <= -2))
                _deltaXInc = -_deltaXInc;
            _deltaX += _deltaXInc;
            if ((_deltaY >= 2) || (_deltaY <= -2))
                _deltaYInc = -_deltaYInc;
            _deltaY += _deltaYInc;

            // paing background
            Brush bgBrush = new System.Drawing.SolidBrush(this.BackColor);
            Rectangle OutRect = new Rectangle(
                _MovingWidget.ScreenRect.Left - WidgetWrapper.CellSpacingHor + 1,
                _MovingWidget.ScreenRect.Top - WidgetWrapper.CellSpacingVer + 1,
                _MovingWidget.ScreenRect.Width + WidgetWrapper.CellSpacingHor - 1,
                _MovingWidget.ScreenRect.Height + WidgetWrapper.CellSpacingVer - 1);
            _graphics.FillRectangle(bgBrush, OutRect);

            // paint widgets
            _MovingWidget.Paint(_graphics, _MovingWidgetRect);

            // if Moving mode - draw unpin and settings icons
            if (MoveMode)
            {
                _graphics.DrawIcon(Properties.Resources.unpin,
                    _MovingWidgetRect.Right - Properties.Resources.unpin.Width + 2,
                    _MovingWidgetRect.Top - 2);
            }

            _WidgetsImage.Invalidate(OutRect);

            Application.DoEvents();
        }

        #endregion

    }
}
