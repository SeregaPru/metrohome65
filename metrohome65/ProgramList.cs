using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65.Pages
{
    public partial class ProgramList : UserControl, IPageControl
    {
        public ProgramList()
        {
            InitializeComponent();
        }

        // IPageControl
        public void SetScrollPosition(Point Location) { }
        public Point GetScrollPosition() { return new Point(0, 0); }

        public Size GetViewportSize() { return this.Size; }
        public Size GetExtentSize() { return this.Size; }

        public void ClickAt(Point Location) { }
        public void DblClickAt(Point Location) { }
        public Boolean ShowPopupMenu(Point Location) { return false; }

        public Boolean Active { set { } }

        public void SetBackColor(Color value) { }

        public Control GetControl() { return this; }


        public event EventHandler ChangePage = null;

        public void OnChangePage(EventArgs e)
        {
            if (this.ChangePage != null)
                this.ChangePage(this, e);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            OnChangePage(EventArgs.Empty);
        }

    }
}
