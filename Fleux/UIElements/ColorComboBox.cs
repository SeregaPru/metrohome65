using System.Drawing;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;

namespace Fleux.UIElements
{
    public class ColorItem
    {
        public Color Color;
        public string Caption;

        public ColorItem(Color color, string caption)
        {
            Color = color;
            Caption = caption;
        }
    }

    public class ColorComboBox: ComboBox
    {
        private void DrawColor(IDrawingGraphics drawingGraphics, object arg)
        {
            drawingGraphics.Color(((ColorItem) arg).Color);
            drawingGraphics.FillRectangle(1, 9, 35, 44);
        }

        protected override UIElement BuildCustomItem(object arg, bool forList)
        {
            var canvas = new Canvas() { Size = new Size(Size.Width - 10, PopupItemHeight), };
            canvas.AddElement(
                new TextElement(((ColorItem)arg).Caption)
                {
                    Style = ((forList) && ((ColorItem)arg == Items[SelectedIndex])) ?
                        new TextStyle(
                            MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                            MetroTheme.PhoneAccentBrush) :
                        new TextStyle(
                            MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                            MetroTheme.PhoneTextBoxFontBrush),
                    Size = new Size(Size.Width - Padding - PopupItemHeight, PopupItemHeight),
                    Location = new Point(55, 0),
                }
            );
            canvas.AddElement(
                new DelegateUIElement()
                {
                    DrawingAction = graphics => DrawColor(graphics, arg),
                    Size = new Size(PopupItemHeight, PopupItemHeight),
                    Location = new Point(0, 0),
                }
                );
            return canvas;
        }
    }
}
