using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace SmartDeviceProject1
{
    public abstract class BaseWidget
    {
        /// <summary>
        /// widget position in grid
        /// </summary>
        public Point position;

        /// <summary>
        /// widget size in cells
        /// 1x1 1x2 2x2 etc. .. 4x4 is max
        /// </summary>
        public Point size;

        protected BaseWidget()
        {
            position = new Point(0, 0);
            size = new Point(1, 1);
        }

        /// <summary>
        /// paint widget interior.
        /// Must be override in descendant classes.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="rect">Drawing area</param>
        public abstract void Paint(Graphics g, Rectangle rect);

        /// <summary>
        /// click event handler.
        /// </summary>
        /// <param name="Location">Click position relative to widget's top left corner</param>
        public virtual void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("click at widget at pos {0}, {1}", 
                this.position.X, this.position.Y));
        }
    }


    /// <summary>
    /// Base class for abstract transparent widget.
    /// </summary>
    public class TransparentWidget : BaseWidget
    {
        ///!! todo - button style, color
        public Color bgColor = System.Drawing.Color.Blue;

        /// <summary>
        /// Paints button with user defined style and color
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        public override void Paint(Graphics g, Rectangle Rect)
        {
            Brush bgBrush = new System.Drawing.SolidBrush(bgColor);
            g.FillRectangle(bgBrush, Rect.Left, Rect.Top, Rect.Width, Rect.Height);
        }
    }


    /// <summary>
    /// Class for transparent widget with icon and caption.
    /// </summary>
    public class IconWidget : TransparentWidget
    {        
        /// <summary>
        /// relative or absolute path to icon file.
        /// icon format must be transparent PNG
        /// </summary>
        public String IconPath = "";

        /// <summary>
        /// user defined caption for widget
        /// </summary>
        public String Caption = "";

        /// <summary>
        /// Paints icon and caption over standart backround (user defined button)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);

            // draw icon
            if (IconPath != "")
            {
                Bitmap MyBitmap = new Bitmap(IconPath);
                ImageAttributes attrib = new ImageAttributes();
                Color color = MyBitmap.GetPixel(0, 0);
                attrib.SetColorKey(color, color);

                Rectangle rectIconBox = new Rectangle(Rect.Left, Rect.Top, Rect.Width, Rect.Height);
                rectIconBox.Inflate(-20, -20);

                g.DrawImage(MyBitmap, rectIconBox,
                    0, 0, MyBitmap.Width, MyBitmap.Height, GraphicsUnit.Pixel, attrib);
                //!! потом сделать масштабирование из произвольного размера
            }
            
            // draw caption
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Helvetica", 9, FontStyle.Regular);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(Caption, captionFont, captionBrush, 
                    Rect.Left + 10, Rect.Bottom - 5 - g.MeasureString(Caption, captionFont).Height);
            }
        }

        public override void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("Icon widget {0} at pos {1}:{2}",
                this.Caption, Location.X, Location.Y));
        }

    }


    /// <summary>
    /// Widget for start application.
    /// Looks like icon widget - icon with caption. 
    /// </summary>
    public class ShortcutWidget : IconWidget
    {
        /// <summary>
        /// relative or absolute path to application with parameters.
        /// </summary>
        public String CommandLine = "";

        public override void OnClick(Point Location)
        {
            if (CommandLine != "") {
              StartProcess(CommandLine);
            }
        }

        private static void StartProcess(string FileName)
        {
            try
            {
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();                
                
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = FileName;

                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }



    public class DigitalClockWidget : TransparentWidget
    {
        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);

            Brush brushCaption = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            String sTime = DateTime.Now.ToString("hh:mm");
            String sDate = DateTime.Now.DayOfWeek.ToString() + ", " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Day.ToString();

            Font fntTime = new System.Drawing.Font("Verdana", 36, FontStyle.Regular);
            SizeF TimeBox = g.MeasureString(sTime, fntTime);
            Font fntDate = new System.Drawing.Font("Helvetica", 12, FontStyle.Regular);
            SizeF DateBox = g.MeasureString(sDate, fntDate);

            g.DrawString(sTime, fntTime, brushCaption,
                Rect.Left + (Rect.Width - TimeBox.Width) / 2, 
                Rect.Top + (Rect.Height - TimeBox.Height - DateBox.Height) / 2);

            g.DrawString(sDate, fntDate, brushCaption,
                Rect.Left + (Rect.Width - DateBox.Width) / 2,
                Rect.Bottom - (Rect.Height - TimeBox.Height - DateBox.Height) / 2 - DateBox.Height);
        }
    }
}
