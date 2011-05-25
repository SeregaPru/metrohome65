using System;
using System.Windows.Forms;
using System.Drawing;

namespace MetroHome65.Pages
{
    interface IPageControl 
    {
        void SetScrollPosition(Point Location);
        Point GetScrollPosition();

        Size GetViewportSize();
        Size GetExtentSize();

        void ClickAt(Point Location);
        void DblClickAt(Point Location);
        Boolean ShowPopupMenu(Point Location);
        
        Boolean Active { set; }

        void SetBackColor(Color value);

        Control GetControl();

        event EventHandler ChangePage;
    }
}
