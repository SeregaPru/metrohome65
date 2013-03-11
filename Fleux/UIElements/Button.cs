namespace Fleux.UIElements
{
    using System.Drawing;
    using Core;
    using Core.GraphicsHelpers;
    using Styles;

    public class Button : UIElement
    {
        #region Private fields
        protected bool m_pressed;
        protected string m_text;
        #endregion

        #region Constructor
        public Button(string text)
        {
            m_text = text;

            Style = MetroTheme.PhoneTextNormalStyle;
            BackgroundColor = MetroTheme.PhoneBackgroundBrush;
            BorderColor = MetroTheme.PhoneForegroundBrush;

            Size = new Size(100, 25);
        }
        #endregion

        #region Properties
        public enum AutoSizeModeOptions
        {
            None,
            OneLineAutoHeight,
            WrapText,
        }

        public AutoSizeModeOptions AutoSizeMode { get; set; }

        public string Text
        {
            get
            {
                return this.m_text;
            }

            set
            {
                this.m_text = value;
                if (this.AutoSizeMode != AutoSizeModeOptions.None)
                {
                    this.Relayout();
                }
                this.Update();
            }
        }

        public TextStyle Style { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }

        #endregion

        #region Methods - OVERRIDE
        public override void ResizeForWidth(int width)
        {
            if (this.AutoSizeMode != AutoSizeModeOptions.None)
            {
                this.Size = new Size(width, 10); // Height will be calculated later
                this.Relayout();
            }
        }

        public override UIElement Pressed(Point p)
        {
            m_pressed = true;
            this.Update();
            return this;
        }

        public override void Released()
        {
            m_pressed = false;
            this.Update();
            base.Released();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (m_pressed)
            {
                drawingGraphics.Color(BorderColor);
                drawingGraphics.FillRectangle(0, 0, this.Size.Width, this.Size.Height);

                drawingGraphics.Style(new TextStyle(Style.FontFamily, Style.FontSize, BackgroundColor));
            }
            else
            {
                drawingGraphics.Color(BackgroundColor);
                drawingGraphics.FillRectangle(0, 0, this.Size.Width, this.Size.Height);

                drawingGraphics.Color(this.BorderColor);
                drawingGraphics.PenWidth(3);
                drawingGraphics.DrawRectangle(0, 0, this.Size.Width, this.Size.Height);
                drawingGraphics.PenWidth(1);

                drawingGraphics.Style(this.Style);
            }

            switch (this.AutoSizeMode)
            {
                case AutoSizeModeOptions.None:
                case AutoSizeModeOptions.OneLineAutoHeight:
                    {
                        int width = drawingGraphics.CalculateTextWidth(m_text);
                        drawingGraphics.MoveX((this.Size.Width - width) / 2).DrawText(m_text);
                        break;
                    }
                case AutoSizeModeOptions.WrapText:
                    drawingGraphics.DrawMultiLineText(m_text, this.Size.Width, this.Size.Height);
                    break;
            }
        }
        #endregion

        #region Private methods
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
                            .CalculateMultilineTextHeight(this.m_text, this.Size.Width);
                        break;
                }
                this.Size = new Size(this.Size.Width, height);
            }
        }
        #endregion
    }
}
