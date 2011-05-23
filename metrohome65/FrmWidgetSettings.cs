using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65.Pages
{
    public partial class FrmWidgetSettings : Form
    {
        private WidgetWrapper _Widget = null;

        public FrmWidgetSettings()
        {
            InitializeComponent();
        }

        public WidgetWrapper Widget { set { SetWidget(value); } }

        private void SetWidget(WidgetWrapper value)
        {
            _Widget = value;
            FillWidgetTypes();
            FillSizes();
        }

        private void FillWidgetTypes()
        {
            cbType.Items.Clear();
        }

        private void FillSizes()
        {
            cbSize.Items.Clear();

            String SizeStr = "";
            foreach (Size Size in _Widget.Widget.Sizes)
            {
                SizeStr = Size.Width + " x " + Size.Height;
                cbSize.Items.Add(SizeStr);
            }
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuApply_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}