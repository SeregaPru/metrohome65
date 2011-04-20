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

        private WidgetGrid Grid = new WidgetGrid();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            //base.OnPaint(e);

            // paing background
            Brush bgBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            e.Graphics.FillRectangle(bgBrush, this.Left, this.Top, this.Width, this.Height);

            // paint widgets
            Grid.Paint(e.Graphics);
        }

        private void gestureRecognizer1_Select(object sender, GestureEventArgs e)
        {
            Grid.ClickAt(new Point(e.X, e.Y));            
        }

        private void gestureRecognizer_Hold(object sender, GestureEventArgs e)
        {
            MessageBox.Show("hold");
        }

        private void gestureRecognizer_Scroll(object sender, GestureScrollEventArgs e)
        {
            MessageBox.Show("scroll");
        }

    }
}