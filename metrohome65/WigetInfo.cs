using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OpenNETCF.Drawing;
using System.Collections.Generic;
using System.ComponentModel;


namespace SmartDeviceProject1
{

    public class BaseWidget : IWidget
    {
        private EventHandlerList _Events = new EventHandlerList();
        private static readonly object _EventWidgetUpdate = new object();

        protected virtual Size[] GetSizes() { return null; }
        public Size[] Sizes { get { return GetSizes(); } }

        protected virtual Boolean GetTransparent() { return false; }
        public Boolean Transparent { get { return GetTransparent(); } }

        public virtual void Paint(Graphics g, Rectangle Rect) { }

        public virtual void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("click at widget at {0}:{1}", 
                Location.X, Location.Y));
        }

        /// <summary>
        /// Event raised when Widget needs to be updated (repainted)
        /// </summary>
        public event WidgetUpdateEventHandler WidgetUpdate
        {
            add { _Events.AddHandler(_EventWidgetUpdate, value); }
            remove { _Events.RemoveHandler(_EventWidgetUpdate, value); }       
        }

        protected void OnWidgetUpdate()
        {
            var handler = _Events[_EventWidgetUpdate] as WidgetUpdateEventHandler;
            if (handler != null)
            {
                WidgetUpdateEventArgs e = new WidgetUpdateEventArgs(this);
                handler(this, e);
            }
        }
    }


    /// <summary>
    /// Base class for abstract transparent widget.
    /// </summary>
    public class TransparentWidget : BaseWidget
    {
        protected override Boolean GetTransparent() { return true; }
    }


    /// <summary>
    /// Class for transparent widget with icon and caption.
    /// </summary>
    public class IconWidget : TransparentWidget
    {
        //Эти переменные понядобятся для загрузки изображений при запуске приложения.
        private OpenNETCF.Drawing.Imaging.ImagingFactoryClass _factory = new OpenNETCF.Drawing.Imaging.ImagingFactoryClass();
        private OpenNETCF.Drawing.Imaging.IImage _img = null;

        /// <summary>
        /// user defined caption for widget
        /// </summary>
        public String Caption = "";

        private String _IconPath = "";
        
        /// <summary>
        /// relative or absolute path to icon file.
        /// icon format must be transparent PNG
        /// </summary>
        public String IconPath { 
            get { return _IconPath; } 
            set { 
                _IconPath = value;
                try
                {
                    if (_IconPath != "")
                        _factory.CreateImageFromFile(_IconPath, out _img);
                } catch {
                }
            }
        }

        protected void PaintIcon(Graphics g, Rectangle Rect)
        {
            // draw icon
            if (_img != null)
            {
                try
                {
                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(Rect.Left + 45, Rect.Top + 45, 92, 90);
                    _img.Draw(hdc, ImgRect, null);
                }
                catch { }
            }
        }

        protected void PaintCaption(Graphics g, Rectangle Rect)
        {
            // draw caption
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Helvetica", 10, FontStyle.Regular);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(Caption, captionFont, captionBrush,
                    Rect.Left + 10, Rect.Bottom - 5 - g.MeasureString(Caption, captionFont).Height);
            }
        }

        /// <summary>
        /// Paints icon and caption over standart backround (user defined button)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="Rect"></param>
        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);
            PaintIcon(g, Rect);
            PaintCaption(g, Rect);
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
            if (CommandLine != "") 
              StartProcess(CommandLine);
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

    
    public class DigitalClockWidget : TransparentWidget, IWidgetUpdatable
    {
        private Brush _brushCaption;
        private Font _fntTime;
        private Font _fntDate;
        private System.Windows.Forms.Timer _Timer;
        private Boolean _ShowPoints = true;

        public DigitalClockWidget() : base()
        {
            _brushCaption = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            _fntTime = new System.Drawing.Font("Verdana", 36, FontStyle.Regular);
            _fntDate = new System.Drawing.Font("Helvetica", 12, FontStyle.Regular);
        }

        public override void Paint(Graphics g, Rectangle Rect)
        {
            base.Paint(g, Rect);

            String sTime = DateTime.Now.ToString("hh" + (_ShowPoints ? ":" : " ") + "mm");
            String sDate = DateTime.Now.DayOfWeek.ToString() + ", " + DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Day.ToString();

            SizeF TimeBox = g.MeasureString(sTime, _fntTime);
            SizeF DateBox = g.MeasureString(sDate, _fntDate);

            g.DrawString(sTime, _fntTime, _brushCaption,
                Rect.Left + (Rect.Width - TimeBox.Width) / 2, 
                Rect.Top + (Rect.Height - TimeBox.Height - DateBox.Height) / 2);

            g.DrawString(sDate, _fntDate, _brushCaption,
                Rect.Left + (Rect.Width - DateBox.Width) / 2,
                Rect.Bottom - (Rect.Height - TimeBox.Height - DateBox.Height) / 2 - DateBox.Height);
        }

        public void StartUpdate()
        {
            if (_Timer == null)
            {
                _Timer = new System.Windows.Forms.Timer();
                _Timer.Tick += new EventHandler(OnTimer);
            }
            _Timer.Interval = 1000;
            _Timer.Enabled = true;
        }

        public void StopUpdate()
        {
            _Timer.Enabled = false;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            _ShowPoints = !_ShowPoints;
            OnWidgetUpdate();
        }

    }
}
