using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Gestures;
using MetroHome65.Pages;

namespace MetroHome65.Main
{
      public partial class MainForm : Form
      {

        private IPageControl _PageControl = null;
        private List<IPageControl> _Pages = new List<IPageControl>();
        private Point _ScrollLast;
        private Point _ScrollOffset;

        public MainForm()
        {
            InitializeComponent();

            this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - this.Top;
            this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AddPage(new MetroHome65.Pages.WidgetGrid());
            AddPage(new MetroHome65.Pages.ProgramList());

            SetPageControl(_Pages[0]);
        }

        private void AddPage(IPageControl Page)
        {
            _Pages.Add(Page);
            Page.ChangePage += new EventHandler(PageControl_ChangePage);
        }


        /// <summary>
        /// When app activated, switch on widgets update,
        /// and bring focus to current control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (this._PageControl != null)
            {
                this._PageControl.Active = true;
                gestureRecognizer.TargetControl = this; //!! _PageControl.GetControl();
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            gestureRecognizer.TargetControl = null;

            if (this._PageControl != null)
                this._PageControl.Active = false;
        }

        private void SetPageControl(IPageControl APageControl)
        {
            // stop animation
            this.physics.Stop();

            // unlink previous control
            if (this._PageControl != null)
            {
                this._PageControl.Active = false;
                this._PageControl.GetControl().Resize += null;
                this.Controls.Remove(this._PageControl.GetControl());
            }

            this._PageControl = APageControl;

            if (this._PageControl != null)
            {
                this._PageControl.SetBackColor(this.BackColor);
                this._PageControl.GetControl().Location = new Point(0, 0);
                this._PageControl.GetControl().Size = new Size(this.Width, this.Height);

                this.Controls.Add(this._PageControl.GetControl());
                this._PageControl.GetControl().Resize += new EventHandler(PageControl_Resize);
                UpdateScrollSize();

                //!!gestureRecognizer.TargetControl = _PageControl.GetControl();

                this._PageControl.Active = true;
            }
        }

        private void UpdateScrollSize()
        {
            physics.Extent = _PageControl.GetExtentSize();
            physics.ViewportSize = _PageControl.GetViewportSize();
        }


        /// <summary>
        /// when page is resised, new scroll size must be assigned
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PageControl_Resize(object sender, EventArgs e)
        {
            UpdateScrollSize();
        }

        /// <summary>
        /// Change active page to another one, next in list.
        /// !! todo - lather do change back and forward
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PageControl_ChangePage(object sender, EventArgs e)
        {
            int CurIndex = _Pages.IndexOf(this._PageControl);
            CurIndex = ((CurIndex == -1) || (CurIndex == _Pages.Count - 1)) ? 0 : CurIndex + 1;
            SetPageControl(_Pages[CurIndex]);
        }

        private void ScrollTo(Point Location)
        {
            this._PageControl.SetScrollPosition(Location);
            if (! this.physics.IsAnimating)
                this.physics.Origin = this._PageControl.GetScrollPosition().Negate();
        }

        private void gestureRecognizer1_Select(object sender, GestureEventArgs e)
        {
            this._PageControl.ClickAt(e.Location);            
        }

        private void gestureRecognizer_DoubleSelect(object sender, GestureEventArgs e)
        {
            this._PageControl.DblClickAt(e.Location);            
        }

        /// <summary>
        /// Hold (long press) handler.
        /// Calls current control's popup menu.
        /// If fails, shows main popup menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gestureRecognizer_Hold(object sender, GestureEventArgs e)
        {
            if (!this._PageControl.ShowPopupMenu(e.Location))
                this.mnuMain.Show(this, e.Location);
        }

        private void gestureRecognizer_Scroll(object sender, GestureScrollEventArgs e)
        {
            physics.Stop();
            physics.Start(e.Angle, e.Velocity);
        }

        private void physics_AnimateFrame(object sender, PhysicsAnimationFrameEventArgs e)
        {
            ScrollTo(e.Location.Negate());
        }

        private void gestureRecognizer_Pan(object sender, GestureEventArgs e)
        {
            this.physics.Stop();
            if ((e.State & GestureState.Begin) == GestureState.Begin)
            {
                this._ScrollLast = e.Location;
                this._ScrollOffset = this._PageControl.GetScrollPosition().Negate();
                return;
            }

            Point delta = e.Location.Subtract(this._ScrollLast);
            this._ScrollOffset = this._ScrollOffset.Subtract(delta);
            this._ScrollLast = e.Location;

            ScrollTo(this._ScrollOffset.Negate());
        }

        private void gestureRecognizer_Begin(object sender, GestureEventArgs e)
        {
            this.physics.Stop();
        }

        private void gestureRecognizer_End(object sender, GestureEventArgs e)
        {
            if (! this.physics.IsAnimating)
            {
                this.physics.Angle = 0;
                this.physics.Velocity = 0;
                this.physics.Start();
            }
        }

        /// <summary>
        /// Menu handler - Close application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}