using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    public class TextBox2 : TextElement
    {
        private readonly FleuxControlPage _settingsPage;

        public TextBox2(FleuxControlPage settingsPage) : base("")
        {
            _settingsPage = settingsPage;

            Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                          MetroTheme.PhoneTextBoxFontBrush);
            AutoSizeMode = AutoSizeModeOptions.None;

            TapHandler += point => OnTap();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBrush);
            drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height);

            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            base.Draw(drawingGraphics);
        }

        // Tap handler for textbox.
        // When user taps on textbox, show new page with system inputbox and virtual keyboard
        private bool OnTap()
        {
            var inputPage = new TextBoxInputPage();
            _settingsPage.Control.AnimateExit();
            inputPage.TheForm.Owner = _settingsPage.TheForm;
            inputPage.TheForm.Show();
            return true;
        }


    }
}
