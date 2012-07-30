using System;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    public class TextBox : UIElement
    {
        private readonly FleuxControlPage _settingsPage;

        private string _text;


        public bool MultiLine { get; set; }


        public string Text
        {
            get { return this._text; }

            set
            {
                this._text = value;
                this.Update();
            }
        }

        public TextStyle Style { get; set; }


        public TextBox(FleuxControlPage settingsPage)
        {
            _settingsPage = settingsPage;
            _text = "";
            MultiLine = false;

            Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                          MetroTheme.PhoneTextBoxFontBrush);

            TapHandler += point => OnTap();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBrush);
            drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height);

            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            drawingGraphics.Style(this.Style);

            if (MultiLine)
                drawingGraphics.DrawMultiLineText(this._text, this.Size.Width, this.Size.Height);
            else
                drawingGraphics.DrawText(this._text);
            
        }

        // Tap handler for textbox.
        // When user taps on textbox, show new page with system inputbox and virtual keyboard
        private bool OnTap()
        {
            var inputPage = new TextInputForm
                                {
                                    Value = Text, 
                                    Owner = _settingsPage.TheForm,
                                    MultiLine = this.MultiLine,
                                };
            inputPage.OnValueChanged += () => SetText(inputPage.Value);
            inputPage.Show();
            return true;
        }

        private void SetText(string value)
        {
            Text = value;
            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        // event triggered when Text changes
        public event EventHandler TextChanged;

    }
}
