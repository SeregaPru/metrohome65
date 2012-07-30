using System;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Styles;

namespace Metrohome65.Settings.Controls
{
    public partial class TextInputForm : Form
    {
        private bool _active = true;

        public string Value
        {
            get { return textBox.Text; }
            set { textBox.Text = value; } 
        }

        public bool MultiLine
        {
            get { return textBox.Multiline; }
            set
            {
                textBox.Multiline = value;
                textBox.Size = new Size(textBox.Size.Width, textBox.Multiline ? 150 : 50);
            }
        }
    
        public TextInputForm()
        {
            InitializeComponent();
            BackColor = MetroTheme.PhoneBackgroundBrush;
        }

        private void CloseForm()
        {
            _active = false;
            inputPanel.Enabled = false;
            Close();
        }

        private void MenuCancelClick(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void MenuOkClick(object sender, EventArgs e)
        {
            if (OnValueChanged != null)
                OnValueChanged();

            CloseForm();
        }

        private void InputPanelEnabledChanged(object sender, EventArgs e)
        {
            inputPanel.Enabled = _active;
        }

        private void TextBoxGotFocus(object sender, EventArgs e)
        {
            inputPanel.Enabled = true;
        }

        public event Action OnValueChanged;
    }
}