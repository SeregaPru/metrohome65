using System;
using System.Drawing;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;

namespace MetroHome65.WPLock.Controls
{
    public class TextElementAdvanced : UIElement
    {
        protected string text;

        public TextElementAdvanced(string text)
        {
            this.text = text;
            this.Style = MetroTheme.PhoneTextNormalStyle;
        }

        public enum AutoSizeModeOptions
        {
            None,
            OneLineAutoHeight,
            WrapText,
            Left,
            Center,
            Right,
        }

        public AutoSizeModeOptions AutoSizeMode { get; set; }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                if (this.AutoSizeMode != AutoSizeModeOptions.None)
                {
                    this.Relayout();
                }
                this.Update();
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

            switch (this.AutoSizeMode)
            {
                case AutoSizeModeOptions.None:
                case AutoSizeModeOptions.Left:
                case AutoSizeModeOptions.OneLineAutoHeight:
                    drawingGraphics.DrawText(this.text);
                    break;
                case AutoSizeModeOptions.WrapText:
                    drawingGraphics.DrawMultiLineText(this.text, this.Size.Width, this.Size.Height);
                    break;
                case AutoSizeModeOptions.Center:
                    drawingGraphics.DrawCenterText(this.text,this.Size.Width);
                    break;
                case AutoSizeModeOptions.Right:
                    drawingGraphics.DrawRightText(this.text);
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
                            .CalculateMultilineTextHeight(this.text, this.Size.Width);
                        break;
                }
                this.Size = new Size(this.Size.Width, height);
            }
        }

    }
}
