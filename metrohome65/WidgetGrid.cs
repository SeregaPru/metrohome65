using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MetroHome65.Widgets;
using MetroHome65.Routines;
using Microsoft.WindowsMobile.Gestures;

namespace MetroHome65.Pages
{

    public partial class WidgetGrid : UserControl, IPageControl
    {
        private int TopOffset = 0;
        private MetroHome65.Main.IHost _Host;

        private String _CoreDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        private String SettingsFile() { return _CoreDir + "\\widgets.xml"; }

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
            if (this.BackColor != value)
            {
                this.BackColor = value;
                _WidgetsContainer.BackColor = value;
                RepaintGrid(false);
            }
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
            //!!Wrapper.WidgetGrid = this;

            if (Wrapper.Widget is IWidgetUpdatable)
                (Wrapper.Widget as IWidgetUpdatable).WidgetUpdate += new WidgetUpdateEventHandler(OnWidgetUpdate);

            if (DoRealign)
            {
                RealignWidgets();
                WriteSettings();
            }

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
                    RepaintWidget(wsInfo);
                    break;
                }
        }

        private void RepaintWidget(WidgetWrapper Widget)
        {
            Widget.Paint(_graphics, true);

            Rectangle Rect = new Rectangle(
                Widget.ScreenRect.X, Widget.ScreenRect.Y, Widget.ScreenRect.Width, Widget.ScreenRect.Height);
            _WidgetsImage.Invalidate(Rect);
        }

        private void RepaintGrid(bool BufferOnly)
        {
            // paing background
            Brush bgBrush = new System.Drawing.SolidBrush(this.BackColor);
            _graphics.FillRectangle(bgBrush, 0, 0, _DoubleBuffer.Width, _DoubleBuffer.Height);

            // paint widgets
            foreach (WidgetWrapper wsInfo in Widgets)
                wsInfo.Paint(_graphics, false);

            if (!BufferOnly)
                _WidgetsImage.Invalidate();
        }


        /// <summary>
        /// Update may be change widgets screen positions
        /// </summary>
        private void RealignWidgets()
        {
            try
            {
                int MaxRow = 0;
                foreach (WidgetWrapper wsInfo in Widgets)
                    MaxRow = Math.Max(MaxRow, wsInfo.Location.Y + wsInfo.Size.Height);

                object[,] cells = new object[MaxRow+1, 4];
                foreach (WidgetWrapper wsInfo in Widgets)
                {
                    for (int y = 0; y < wsInfo.Size.Height; y++)
                        for (int x = 0; x < wsInfo.Size.Width; x++)
                            cells[wsInfo.Location.Y + y, Math.Min(3, wsInfo.Location.X + x)] = wsInfo;
                }

                // looking for empty rows and delete them - shift widgets 1 row top
                for (int row = MaxRow; row >= 0; row--)
                {
                    if ((cells[row, 0] == null) && (cells[row, 1] == null) && (cells[row, 2] == null) && (cells[row, 3] == null))
                    {
                        foreach (WidgetWrapper wsInfo in Widgets)
                            if (wsInfo.Location.Y > row)
                                wsInfo.Location = new Point(wsInfo.Location.X, wsInfo.Location.Y - 1);
                    }
                }


                // calc max image dimensions for widgets grid
                int WidgetsHeight = 0;
                int WidgetsWidth = 0;
                foreach (WidgetWrapper wsInfo in Widgets)
                {
                    WidgetsHeight = Math.Max(WidgetsHeight, wsInfo.ScreenRect.Bottom);
                    WidgetsWidth = Math.Max(WidgetsWidth, wsInfo.ScreenRect.Right);
                }
                WidgetsHeight += 50; // add padding at bottom and blank spaces at top and bottom


                // change size of internal bitmap and repaint it
                _graphics.Dispose();
                _graphics = null;
                _DoubleBuffer.Dispose();
                _DoubleBuffer = null;

                _DoubleBuffer = new Bitmap(WidgetsWidth, WidgetsHeight);
                _graphics = Graphics.FromImage(_DoubleBuffer);

                RepaintGrid(true);

                if (_WidgetsImage.Image != null)
                    _WidgetsImage.Image.Dispose();
                _WidgetsImage.Image = _DoubleBuffer;
                _WidgetsImage.Size = _DoubleBuffer.Size;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
                //!! write to log  (e.StackTrace, "ReadSettings")
            }

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

            bool TargetCellIsEmpty = false;
            while (!TargetCellIsEmpty)
            {
                object[,] cells = new object[100, 4]; 
                foreach (WidgetWrapper wsInfo in Widgets)
                {
                    for (int y = 0; y < wsInfo.Size.Height; y++)
                        for (int x = 0; x < wsInfo.Size.Width; x++)
                            cells[wsInfo.Location.Y + y, wsInfo.Location.X + x] = wsInfo;
                }

                TargetCellIsEmpty = true;
                for (int y = TargetCell.Y; y < Math.Min(TargetCell.Y + MovingWidget.Size.Height, 100); y++)
                    for (int x = TargetCell.X; x < Math.Min(TargetCell.X + MovingWidget.Size.Width, 4); x++)
                        if ((cells[y, x] != null) && (!cells[y, x].Equals(MovingWidget)))
                        {
                            TargetCellIsEmpty = false;
                            break;
                        }

                if (!TargetCellIsEmpty)
                {
                    foreach (WidgetWrapper wsInfo in Widgets)
                    {
                        if ((wsInfo.Location.Y + wsInfo.Size.Height - 1) >= TargetCell.Y)
                            wsInfo.Location = new Point(wsInfo.Location.X, wsInfo.Location.Y + 1);
                    }
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

                foreach (WidgetWrapper Wrapper in Widgets)
                {
                    //!!Wrapper.WidgetGrid = this;
                    if (Wrapper.Widget is IWidgetUpdatable)
                        (Wrapper.Widget as IWidgetUpdatable).WidgetUpdate += new WidgetUpdateEventHandler(OnWidgetUpdate);
                    Wrapper.AfterDeserialize();
                }

            }
            catch (Exception e)
            {
                //!!MessageBox.Show(e.StackTrace);
                //!! write to log  (e.StackTrace, "ReadSettings")
                DebugFill();
                WriteSettings();
            }

            RealignWidgets();
        }

        /// <summary>
        /// Store widgets position and specific settings to XML file
        /// </summary>
        private void WriteSettings()
        {
            foreach (WidgetWrapper Widget in Widgets)
                Widget.BeforeSerialize();

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
                ClickAtMoveMode(TargetWidget, Location);
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
            WidgetWrapper TargetWidget = GetWidgetAtPos(Location);

            if (TargetWidget == null)
            {
                ShowMainPopupMenu(Location);
                return;
            }

            if (! MoveMode)
            {
                if (! ShowWidgetPopupMenu(TargetWidget, Location))
                    MovingWidget = TargetWidget;
            }
        }

        /// <summary>
        /// Fill popup menu for widget grid with grid settings
        /// </summary>
        /// <param name="Menu"></param>
        private void ShowMainPopupMenu(Point Location)
        {
            ContextMenu MainMenu = new ContextMenu();

            MenuItem menuAddWidget = new System.Windows.Forms.MenuItem();
            menuAddWidget.Text = "Add widget";
            menuAddWidget.Click += AddWidget_Click;
            MainMenu.MenuItems.Add(menuAddWidget);

            // add separator
            MenuItem Separator = new System.Windows.Forms.MenuItem();
            Separator.Text = "-";
            MainMenu.MenuItems.Add(Separator);

            MenuItem menuExit = new System.Windows.Forms.MenuItem();
            menuExit.Text = "Exit";
            menuExit.Click += Exit_Click;
            MainMenu.MenuItems.Add(menuExit);

            MainMenu.Show(this, Location);
        }

        /// <summary>
        /// Fill popup menu with widget specific actions.
        /// If widget has its own actions they are added to standard widget menu
        /// </summary>
        /// <param name="Widget"></param>
        /// <param name="Menu"></param>
        private bool ShowWidgetPopupMenu(WidgetWrapper Widget, Point Location)
        {
            // fill menu with widget customn actions
            if ((Widget.Widget != null) && (Widget.Widget.MenuItems != null))
            {
                _mnuWidgetSender = Widget;
                ContextMenu WidgetMenu = new ContextMenu();

                foreach (String Item in Widget.Widget.MenuItems)
                {
                    MenuItem CustomItem = new System.Windows.Forms.MenuItem();
                    CustomItem.Text = Item;
                    CustomItem.Click += OnCustomMenuClick;
                    WidgetMenu.MenuItems.Add(CustomItem);
                }

                // add separator
                MenuItem Separator = new System.Windows.Forms.MenuItem();
                Separator.Text = "-";
                WidgetMenu.MenuItems.Add(Separator);

                // fill menu with standart actions
                MenuItem menuMove = new System.Windows.Forms.MenuItem();
                menuMove.Text = "Customize";
                menuMove.Click += MoveWidget_Click;
                WidgetMenu.MenuItems.Add(menuMove);

                WidgetMenu.Show(this, Location);
                return true;
            }
            else
                return false;
        }

        private void OnCustomMenuClick(object sender, EventArgs e)
        {
            if ((sender is MenuItem) && (_mnuWidgetSender != null))
                _mnuWidgetSender.Widget.OnMenuItemClick((sender as MenuItem).Text);
        }

        /// <summary>
        /// Delete current widget from grid.
        /// Then widgets are realigned, if deleted widget was alone on its row.
        /// </summary>
        private void DeleteWidget()
        {
            WidgetWrapper DeletingWidget = MovingWidget;
            MovingWidget = null;
            Widgets.Remove(DeletingWidget);
            RealignWidgets();
            WriteSettings();
        }

        private void buttonUnpin_Click(object sender, EventArgs e)
        {
            if (MoveMode)
                DeleteWidget();
        }

        private void MoveWidget_Click(object sender, EventArgs e)
        {
            if (_mnuWidgetSender != null)
                MovingWidget = _mnuWidgetSender;
        }

        /// <summary>
        /// Shows widget settings dialog and applies changes in widget settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWidgetSettings(WidgetWrapper Widget)
        {
            FrmWidgetSettings WidgetSettingsForm = new FrmWidgetSettings();
            WidgetSettingsForm.Widget = Widget;
            WidgetSettingsForm.Owner = null;

            // when show setting dialog, stop selected widget animation
            WidgetWrapper prevMovingWidget = MovingWidget;
            MovingWidget = null;
            Size PrevSize = Widget.Size;

            if (WidgetSettingsForm.ShowDialog() == DialogResult.OK)
            {
                Widget.Paint(_graphics, true); // repaint widget with new style
                if (! PrevSize.Equals(Widget.Size))
                    RealignWidgets(); // if widget size changed - realign widgets
                WriteSettings();
            }
            else
                MovingWidget = prevMovingWidget;
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            if (MoveMode)
                ShowWidgetSettings(_MovingWidget);
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


        private void WidgetGrid_Resize(object sender, EventArgs e)
        {
            _WidgetsContainer.Size = new Size(
                WidgetWrapper.CellWidth * 4 + WidgetWrapper.CellSpacingHor * 3, 
                this.Height - _WidgetsContainer.Top);

            panelButtons.Location = new Point(
                this.Width - (this.Width - _WidgetsContainer.Left - _WidgetsContainer.Width) / 2 - panelButtons.Width / 2, _WidgetsContainer.Top);

            UpdateGridSize();
        }


        // fill grid with debug values
        private void DebugFill()
        {
            Widgets.Clear();

            String IconsDir = _CoreDir + "\\icons\\" + 
                ((ScreenRoutines.IsQVGA) ? "small\\small-" : ""); 

            AddWidget(new Point(0, 0), new Size(2, 2), "MetroHome65.Widgets.SMSWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "SMS").
                SetParameter("IconPath", IconsDir + "message.png").
                SetParameter("TileColor", Color.Orange.ToArgb());

            AddWidget(new Point(0, 2), new Size(2, 2), "MetroHome65.Widgets.PhoneWidget", false).
                SetParameter("CommandLine", @"\Windows\cprog.exe").
                SetParameter("Caption", "Phone").
                SetParameter("IconPath", IconsDir + "phone.png").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(2, 0), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\addrbook.lnk").
                SetParameter("Caption", "Contacts").
                SetParameter("IconPath", IconsDir + "contacts.png").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button darkgray.png");

            AddWidget(new Point(2, 2), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\iexplore.exe").
                SetParameter("Caption", "Internet Explorer").
                SetParameter("IconPath", IconsDir + "iexplore.png").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button blue.png");

            AddWidget(new Point(0, 4), new Size(4, 2), "MetroHome65.Widgets.DigitalClockWidget", false).
                SetParameter("TileImage", _CoreDir + "\\buttons\\bg2.png");

            AddWidget(new Point(0, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\camera.exe").
                SetParameter("IconPath", @"\Windows\camera.exe").
                SetParameter("Caption", "").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button gray.png");

            AddWidget(new Point(1, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wrlsmgr.exe").
                SetParameter("IconPath", @"\Windows\wrlsmgr.exe").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.DarkGreen.ToArgb());

            AddWidget(new Point(2, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\taskmgr.exe").
                SetParameter("IconPath", @"\Windows\taskmgr.exe").
                SetParameter("Caption", "").
                SetParameter("TileImage", _CoreDir + "\\buttons\\button blue.png"); ;

            AddWidget(new Point(3, 6), new Size(1, 1), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\calendar.exe").
                SetParameter("IconPath", @"\Windows\calendar.exe").
                SetParameter("Caption", "").
                SetParameter("TileColor", Color.Maroon.ToArgb());

            AddWidget(new Point(0, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddWidget(new Point(2, 7), new Size(2, 2), "MetroHome65.Widgets.ContactWidget", false);

            AddWidget(new Point(0, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\MobileCalculator.exe").
                SetParameter("Caption", "Calculator").
                SetParameter("IconPath", IconsDir + "calc.png");

            AddWidget(new Point(2, 9), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\wmplayer.exe").
                SetParameter("Caption", "Media player").
                SetParameter("IconPath", IconsDir + "media.png");

            AddWidget(new Point(0, 11), new Size(2, 2), "MetroHome65.Widgets.ShortcutWidget", false).
                SetParameter("CommandLine", @"\Windows\fexplore.exe").
                SetParameter("Caption", "Explorer").
                SetParameter("IconPath", IconsDir + "folder.png");

            AddWidget(new Point(2, 11), new Size(2, 2), "MetroHome65.Widgets.EMailWidget", false).
                SetParameter("CommandLine", @"\Windows\tmail.exe").
                SetParameter("Caption", "E-mail").
                SetParameter("IconPath", IconsDir + "mail.png");
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

        private bool MoveMode { get { return _MovingWidget != null; } }

        private WidgetWrapper _MovingWidget = null;
        private WidgetWrapper MovingWidget
        {
            get { return _MovingWidget;  }
            set {
                if (_MovingWidget != value)
                {
                    buttonSettings.Visible = (value != null);
                    buttonUnpin.Visible = (value != null);

                    if (value == null)
                    {
                        _ResizeTimer = null;

                        _MovingWidget.Moving = false;
                        WriteSettings();

                        RepaintWidget(_MovingWidget);

                        _MovingWidget = null;
                        _MovingWidgetImg.Dispose();
                        _MovingWidgetImg = null;
                                                
                        _BGBrush = null;
                    }
                    else
                    {
                        _MovingWidget = value;
                        _MovingWidget.Moving = true;

                        _MovingWidgetImg = CropImage(_DoubleBuffer, _MovingWidget.ScreenRect);
                        _BGBrush = new System.Drawing.SolidBrush(this.BackColor);

                        _ResizeTimer = new System.Threading.Timer(OnResizeTimer, null, 0, 300);
                        /*
                        _ResizeTimer = new System.Windows.Forms.Timer();
                        _ResizeTimer.Tick += OnResizeTimer;
                        _ResizeTimer.Interval = 400;
                        _ResizeTimer.Enabled = true;
                         */
                    }

                }
            }
        }

        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
 
        Bitmap _MovingWidgetImg = null;
        //!!private System.Windows.Forms.Timer _ResizeTimer;
        private System.Threading.Timer _ResizeTimer;
        private int _deltaX = 0;
        private int _deltaY = -2;
        private int _deltaXInc = 1;
        private int _deltaYInc = -1;
        private Brush _BGBrush = null;

        private void OnResizeTimer(object state)
        //!!private void OnResizeTimer(object sender, EventArgs e)
        {
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

            if (_MovingWidget == null)
                return;

            Rectangle _MovingWidgetRect = new Rectangle(_MovingWidget.ScreenRect.Left, _MovingWidget.ScreenRect.Top,
                _MovingWidget.ScreenRect.Width, _MovingWidget.ScreenRect.Height);
            _MovingWidgetRect.Inflate(_deltaX, _deltaY);

            if ((_deltaX >= 0) || (_deltaX <= -5))
                _deltaXInc = -_deltaXInc;
            _deltaX += _deltaXInc;
            if ((_deltaY >= 0) || (_deltaY <= -5))
                _deltaYInc = -_deltaYInc;
            _deltaY += _deltaYInc;

            // paing background
            Rectangle OutRect = new Rectangle(
                _MovingWidget.ScreenRect.Left - WidgetWrapper.CellSpacingHor + 1,
                _MovingWidget.ScreenRect.Top - WidgetWrapper.CellSpacingVer + 1,
                _MovingWidget.ScreenRect.Width + WidgetWrapper.CellSpacingHor - 1,
                _MovingWidget.ScreenRect.Height + WidgetWrapper.CellSpacingVer - 1);
            _graphics.FillRectangle(_BGBrush, OutRect);

            // paint widgets
            _graphics.DrawImage(_MovingWidgetImg, _MovingWidgetRect, 
                new Rectangle(0, 0, _MovingWidgetImg.Width, _MovingWidgetImg.Height), GraphicsUnit.Pixel); 

            _WidgetsImage.Invalidate(OutRect);
        }

        private void ClickAtMoveMode(WidgetWrapper TargetWidget, Point Location)
        {
            if (TargetWidget == MovingWidget)
                MovingWidget = null;
            else
            {
                MoveWidgetTo(Location);
                RepaintMovingWidget();
            }
        }

        #endregion

    }
}
