using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OpenNETCF.Drawing;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace SmartDeviceProject1
{

    public class BaseWidget : IWidget
    {
        private EventHandlerList _Events = new EventHandlerList();
        private static readonly object _EventWidgetUpdate = new object();

        protected virtual Size[] GetSizes() { return null; }
        public Size[] Sizes { get { return GetSizes(); } }

        protected virtual String[] GetMenuItems() { return null; } 
        public String[] MenuItems { get { return GetMenuItems(); } }
        
        protected virtual Boolean GetTransparent() { return false; }
        public Boolean Transparent { get { return GetTransparent(); } }
        
        public virtual void Paint(Graphics g, Rectangle Rect) { }

        public virtual void OnClick(Point Location)
        {
            MessageBox.Show(String.Format("click at widget at {0}:{1}", 
                Location.X, Location.Y));
        }

        public virtual void MenuItemClick(String ItemName) { }

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
        [WidgetParameter]
        public String Caption { get { return _Caption; } set { _Caption = value; } }
        private String _Caption = "";
       
        /// <summary>
        /// relative or absolute path to icon file.
        /// icon format must be transparent PNG
        /// </summary>
        [WidgetParameter]
        public String IconPath {
            get { return _IconPath; }
            set {
                try
                {
                    _IconPath = value;
                    if (_IconPath != "")
                        _factory.CreateImageFromFile(value, out _img);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, "SetIconPath");
                }
            }
        }
        private String _IconPath = "";

        protected void PaintIcon(Graphics g, Rectangle Rect)
        {
            // draw icon
            if (_img != null)
            {
                try
                {
                    Size IconSize = new Size(92, 90);

                    IntPtr hdc = g.GetHdc();
                    OpenNETCF.Drawing.Imaging.RECT ImgRect = OpenNETCF.Drawing.Imaging.RECT.FromXYWH(
                        (Rect.Left + Rect.Right - IconSize.Width) / 2, 
                        (Rect.Top + Rect.Bottom - IconSize.Height) / 2, IconSize.Width, IconSize.Height);
                    _img.Draw(hdc, ImgRect, null);
                    g.ReleaseHdc(hdc);
                }
                catch (Exception e) {
                    MessageBox.Show(e.StackTrace, "PaintIcon");
                }
            }
        }

        protected void PaintCaption(Graphics g, Rectangle Rect)
        {
            // draw caption
            if (Caption != "")
            {
                Font captionFont = new System.Drawing.Font("Helvetica", 9, FontStyle.Regular);
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
        /// parameter "CommandLine" - relative or absolute path to application with parameters.
        /// </summary>
        [WidgetParameter]
        public String CommandLine { 
            get { return _CommandLine; } 
            set { _CommandLine = value; } 
        }
        private String _CommandLine = "";

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
            _Timer.Interval = 2000;
            _Timer.Enabled = true;
        }

        public void StopUpdate()
        {
            if (_Timer != null)
                _Timer.Enabled = false;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            _ShowPoints = !_ShowPoints;
            OnWidgetUpdate();
        }

    }


    public class ContactWidget : BaseWidget
    {
        protected override String[] GetMenuItems() 
        { 
            String[] Items = { "Call", "Send SMS" };
            return Items; 
        }
        
        public override void MenuItemClick(String ItemName) 
        {
            MessageBox.Show(ItemName);
        }

        public override void Paint(Graphics g, Rectangle Rect)
        {
            Brush bgBrush = new System.Drawing.SolidBrush(Color.Red);
            g.FillRectangle(bgBrush, Rect.Left, Rect.Top, Rect.Width, Rect.Height);
        }
    }


    public class PhoneWidget : ShortcutWidget
    {
        public override void Paint(Graphics g, Rectangle Rect)
        {
            PaintIcon(g, new Rectangle(Rect.Left - 15, Rect.Top, Rect.Width, Rect.Height));
            PaintCaption(g, Rect);
            PaintCalls(g, Rect);
        }

        private void PaintCalls(Graphics g, Rectangle Rect)
        {
            // draw missing calls
            String MissingCalls = "0";

            Font captionFont = new System.Drawing.Font("Helvetica", 22, FontStyle.Regular);
            Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
            g.DrawString(MissingCalls, captionFont, captionBrush,
                Rect.Right - g.MeasureString(MissingCalls, captionFont).Width - 20, 
                (Rect.Bottom + Rect.Top - g.MeasureString(MissingCalls, captionFont).Height) / 2 - 2);
        }

    }

}
