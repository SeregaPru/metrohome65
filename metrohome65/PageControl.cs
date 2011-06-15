using System;
using System.Windows.Forms;
using System.Drawing;

namespace MetroHome65.Pages
{
    public interface IPageControl 
    {     
        Boolean Active { set; }

        void SetBackColor(Color value);

        void SetHost(MetroHome65.Main.IHost Host);

        Control GetControl();
    }
}
