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

            SetPageControl(new WidgetGrid());
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
            this.PageControl.Width = Math.Max(this.PageControl.Width, this.Width);
            this.PageControl.Height = Math.Max(this.PageControl.Height, this.Height);

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

        private Point last;
        private Point offset;

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

            this.PageControl.SetScrollPosition(this.offset.Negate());
            this.physics.Origin = this.PageControl.GetScrollPosition().Negate();

            this.last = e.Location;
        }

        private void physics_AnimateFrame(object sender, PhysicsAnimationFrameEventArgs e)
        {
//            this.PageControl.SetScrollPosition(new Point(
//                - e.Location.X - this.offset.X,
//                - e.Location.Y - this.offset.Y));

/*            Point delta = e.Location.Subtract(this.last);
            this.offset = this.offset.Subtract(delta);

            this.PageControl.SetScrollPosition(this.offset.Negate());

            this.last = e.Location;
 */
            this.PageControl.SetScrollPosition(e.Location.Negate());
            //!! during animation
            this.physics.Origin = this.PageControl.GetScrollPosition().Negate();
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
                //this.physics.Origin = this.offset.Negate();
                this.physics.Start();
            }
        }

    }
}