using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsMobile.Gestures;

namespace SmartDeviceProject1
{
      public partial class MainForm : Form
      {

        private PageControl PageControl = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetPageControl(new WidgetGrid());
        }

        /// <summary>
        /// When app activated, switch on widgets update,
        /// and bring focus to current control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Activated(object sender, EventArgs e)
        {
            this.BringToFront();
            this.Focus();

            this.PageControl.BringToFront();
            this.PageControl.Focus();

            if (this.PageControl != null)
                this.PageControl.Active = true;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (this.PageControl != null)
                this.PageControl.Active = false;
        }

        private void SetPageControl(PageControl APageControl)
        {
            // unlink previous control
            if (this.PageControl != null)
            {
                gestureRecognizer.TargetControl = null;
                this.PageControl.Resize += null;
                this.Controls.Remove(this.PageControl);
            }

            this.PageControl = APageControl;

            this.PageControl.Location = new Point(0, 0);
            this.PageControl.Size = new Size(
                Math.Max(this.PageControl.Width, this.Width),
                Math.Max(this.PageControl.Height, this.Height));

            this.Controls.Add(this.PageControl);
            this.PageControl.Resize += new EventHandler(PageControl_Resize);
            gestureRecognizer.TargetControl = this.PageControl;
            UpdateScrollSize();
        }

        void PageControl_Resize(object sender, EventArgs e)
        {
            UpdateScrollSize();
        }

        private void UpdateScrollSize()
        {
            physics.Extent = PageControl.Size;
            physics.ViewportSize = this.Size;
        }

        private Point last;
        private Point offset;

        private void ScrollTo(Point Location)
        {
            this.PageControl.SetScrollPosition(Location);
            if (! this.physics.IsAnimating)
                this.physics.Origin = this.PageControl.GetScrollPosition().Negate();
        }

        private void gestureRecognizer1_Select(object sender, GestureEventArgs e)
        {
            this.PageControl.ClickAt(e.Location);            
        }

        private void gestureRecognizer_Hold(object sender, GestureEventArgs e)
        {
            this.PageControl.ShowPopupMenu(e.Location);
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
                this.last = e.Location;
                this.offset = this.PageControl.GetScrollPosition().Negate();
                return;
            }

            Point delta = e.Location.Subtract(this.last);
            this.offset = this.offset.Subtract(delta);
            this.last = e.Location;

            ScrollTo(this.offset.Negate());
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

    }
}