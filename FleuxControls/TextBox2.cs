using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Core.GraphicsHelpers;
using Fleux.UIElements;
using Fleux.UIElements.Events;

namespace FleuxControls
{
    public class TextBox2 : UIElement
    {
        private System.Windows.Forms.TextBox _internalTextBox;

        private string _text;
        private Form _hostForm;
        private Timer m_UpdateCaretTimer;


        public string Text
        {
            get { return _text; }

            set
            {
                if (_text == value) return;

                _text = value;
                _internalTextBox.Text = value;
                this.Update();
            }
        }


        public TextBox2(System.Windows.Forms.Form hostForm)
        {
            _hostForm = hostForm;

            _internalTextBox = new System.Windows.Forms.TextBox()
                                   {
                                       Location = new Point(100, 100),
                                       Size = new Size(10, 50),
                                   };
            _hostForm.Controls.Add(_internalTextBox);

            this.SizeChanged += OnSizeChanged;

            m_UpdateCaretTimer = new System.Windows.Forms.Timer();
            m_UpdateCaretTimer.Interval = 50;
            m_UpdateCaretTimer.Tick += new EventHandler(m_UpdateCaretTimer_Tick);
            m_UpdateCaretTimer.Enabled = true;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            _internalTextBox.Size = this.Size;
        }

        public delegate void myDelegate();

        private Point _location;

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(Color.Red);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            _location = new Point(
                drawingGraphics.CalculateX(0), drawingGraphics.CalculateY(0));
        }

        private void PlaceInternalBox(IDrawingGraphics drawingGraphics)
        {
        }

        private void m_UpdateCaretTimer_Tick(object sender, EventArgs e)
        {
            _internalTextBox.BringToFront();
            _internalTextBox.Location = _location;
        }

    }
}
