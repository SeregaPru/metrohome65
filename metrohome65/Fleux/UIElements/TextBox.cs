namespace Fleux.UIElements
{
    using System;
    using System.Drawing;
    using Core;
    using Core.GraphicsHelpers;
    using Styles;

    public class TextBox : UIElement
    {
        protected bool m_TextboxTextChanged;
        protected string m_Text;
        protected System.Windows.Forms.TextBox m_InternalTextBox;
        protected System.Windows.Forms.Form m_HostForm;
        protected bool m_ShowCaret;
#if !WindowsCE
        protected Fleux.Controls.FleuxControl m_ParentFleuxControl;
#endif
        private System.Windows.Forms.Timer m_UpdateCaretTimer;

#if !WindowsCE
        public TextBox(System.Windows.Forms.Form hostForm, Fleux.Controls.FleuxControl parentControl)
#else
        public TextBox(System.Windows.Forms.Form hostForm)
#endif
        {
#if !WindowsCE
            m_ParentFleuxControl = parentControl;
#endif

            this.Size = new Size(200, 80);
            this.Style = MetroTheme.PhoneTextLargeStyle;
            m_HostForm = hostForm;
            m_InternalTextBox = new System.Windows.Forms.TextBox();
            m_InternalTextBox.Text = m_Text;
            m_InternalTextBox.TextChanged += new EventHandler(m_InternalTextBox_TextChanged);
            m_InternalTextBox.GotFocus += new EventHandler(m_InternalTextBox_GotFocus);
            m_InternalTextBox.LostFocus += new EventHandler(m_InternalTextBox_LostFocus);
            m_ShowCaret = false;
            m_TextboxTextChanged = false;
            m_UpdateCaretTimer = new System.Windows.Forms.Timer();
            m_UpdateCaretTimer.Interval = 500;
            m_UpdateCaretTimer.Enabled = false;
            m_UpdateCaretTimer.Tick += new EventHandler(m_UpdateCaretTimer_Tick);
        }

        public bool Focused { get { return m_InternalTextBox.Focused; } }

        public override bool Tap(Point p)
        {
            // On touching the Fleux Textbox, a hidden Windows Forms TextBox is added to 
            // the control, in order to have SIP shown automatically

            // Move caret at the end of the current text content
            m_InternalTextBox.SelectionStart = m_InternalTextBox.TextLength;
            m_InternalTextBox.SelectionLength = 0;

            // Add the internal TextBox to the form; move it behind the form, and then move focus to it,
            // in order to automatically show System Virtual Keyboard
            m_HostForm.Controls.Add(m_InternalTextBox);
            m_InternalTextBox.SendToBack(); 
            m_HostForm.SelectNextControl(m_HostForm.Controls[0], true, false, false, false);

            return true;
        }

        private void m_UpdateCaretTimer_Tick(object sender, EventArgs e)
        {
            if (m_InternalTextBox.Focused)
            {
                m_ShowCaret = !m_ShowCaret;
                this.Update();
            }
        }

        private void m_InternalTextBox_LostFocus(object sender, EventArgs e)
        {
            if (m_InternalTextBox.Focused)
            {
//#if !WindowsCE
//                m_ParentFleuxControl.VirtualKeyboard.Enabled = false;
//#endif
                m_UpdateCaretTimer.Enabled = false;
                m_ShowCaret = false;
                this.Update();
                m_HostForm.Controls.Remove(m_InternalTextBox);
                m_HostForm.SelectNextControl(m_InternalTextBox, true, false, false, false);
            }
        }

        public void Deactivate()
        {
            if (m_InternalTextBox.Focused)
            {
//#if !WindowsCE
//                m_ParentFleuxControl.VirtualKeyboard.Enabled = false;
//#endif
                m_UpdateCaretTimer.Enabled = false;
                m_ShowCaret = false;
                this.Update();
                m_HostForm.Controls.Remove(m_InternalTextBox);
                m_HostForm.SelectNextControl(m_InternalTextBox, true, false, false, false);
            }
        }

        private void m_InternalTextBox_GotFocus(object sender, EventArgs e)
        {
//#if !WindowsCE
//            m_ParentFleuxControl.VirtualKeyboard.Enabled = true;
//#endif
            m_UpdateCaretTimer.Enabled = true;
        }

        private void m_InternalTextBox_TextChanged(object sender, EventArgs e)
        {
            // Handle two-way updates (both from key press, so this flag would be TRUE, and both
            // from internal updates, so this flag would be FALSE)
            if (!m_TextboxTextChanged)
            {
                m_TextboxTextChanged = true;
                Text = m_InternalTextBox.Text;
            }
            
            m_UpdateCaretTimer.Enabled = true;
        }

        public enum AutoSizeModeOptions
        {
            None,
            OneLineAutoHeight,
            WrapText,
        }

        public AutoSizeModeOptions AutoSizeMode { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }

        public string Text
        {
            get
            {
                return m_Text;
            }

            set
            {
                // Handle two-way updates (both from key press, so this flag would be TRUE, and both
                // from internal updates, so this flag would be FALSE)
                if (!m_TextboxTextChanged)
                    m_InternalTextBox.Text = value;
                else
                    m_TextboxTextChanged = false;

                m_Text = value;
                if (this.AutoSizeMode != AutoSizeModeOptions.None)
                {
                    this.Relayout();
                }
                this.Update();
            }
        }

        public System.Windows.Forms.TextBox InternalTextBox
        {
            get
            {
                return m_InternalTextBox;
            }
        }


        public TextStyle Style { get; set; }

        public override void ResizeForWidth(int width)
        {
            if (this.AutoSizeMode != AutoSizeModeOptions.None)
            {
                this.Size = new Size(width, 10); // Height will be calculated later
                this.Relayout();
            }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Style(this.Style);

            drawingGraphics.Color(MetroTheme.PhoneTextBoxBrush);
            drawingGraphics.FillRectangle(0, 0, this.Size.Width, this.Size.Height);

            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.DrawRectangle(0, 0, this.Size.Width, this.Size.Height);

            drawingGraphics.Color(MetroTheme.PhoneBackgroundBrush);

            switch (this.AutoSizeMode)
            {
                case AutoSizeModeOptions.None:
                case AutoSizeModeOptions.OneLineAutoHeight:
                    if (m_ShowCaret)
                        drawingGraphics.DrawText(String.Format("{0}|", m_Text));
                    else
                        drawingGraphics.DrawText(m_Text);
                    break;
                case AutoSizeModeOptions.WrapText:
                    drawingGraphics.DrawMultiLineText(m_Text, this.Size.Width, this.Size.Height);
                    break;
            }
        }

        private void Relayout()
        {
            if (this.AutoSizeMode != AutoSizeModeOptions.None)
            {
                var height = 0;
                switch (this.AutoSizeMode)
                {
                    case AutoSizeModeOptions.OneLineAutoHeight:
                        height = FleuxApplication.DummyDrawingGraphics.Style(this.Style)
                            .CalculateMultilineTextHeight("q", this.Size.Width);
                        break;
                    case AutoSizeModeOptions.WrapText:
                        height = FleuxApplication.DummyDrawingGraphics.Style(this.Style)
                            .CalculateMultilineTextHeight(this.m_Text, this.Size.Width);
                        break;
                }
                this.Size = new Size(this.Size.Width, height);
            }
        }
    }
}
