using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MetroHome65.Pages
{
    class CustomPageControl : UserControl, IPageControl
    {
        public virtual void SetScrollPosition(Point Location) { }

        public virtual Point GetScrollPosition() { return new Point(0, 0); }

        public virtual Size GetViewportSize() { return this.Size; }
        public virtual Size GetExtentSize() {return this.Size; }

        public virtual void ClickAt(Point Location) { }

        public virtual void DblClickAt(Point Location) { }

        public virtual Boolean ShowPopupMenu(Point Location) { return false; }

        public Boolean Active { set { SetActive(value); } }

        protected virtual void SetActive(Boolean value) { }

        public virtual Control GetControl() { return this; }

        public virtual void SetBackColor(Color value) { this.BackColor = value; }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CustomPageControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.Black;
            this.Name = "CustomPageControl";
            this.ResumeLayout(false);
        }


        public event EventHandler ChangePage = null;

        public void OnChangePage(EventArgs e)
        {
            if (this.ChangePage != null)
                this.ChangePage(this, e);
        }
    }
}
