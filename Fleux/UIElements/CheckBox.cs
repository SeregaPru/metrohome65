namespace Fleux.UIElements
{
    using System;
    using System.Drawing;
    using Core;
    using Core.GraphicsHelpers;
    using Styles;

    public class CheckBox : UIElement
    {
        #region Private fields
        private bool m_pressed;
        private bool m_checked;
        private string m_text;
        #endregion

        #region Constructor
        public CheckBox(string text)
        {
            m_text = text;
            this.Style = MetroTheme.PhoneTextNormalStyle;
            this.Size = new Size(100, 25);

            this.BorderColor = MetroTheme.PhoneForegroundBrush;
            this.BackgroundColor = MetroTheme.PhoneBackgroundBrush;
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

        public bool Checked
        {
            get
            {
                return m_checked;
            }

            set
            {
                if (m_checked == value) return;

                m_checked = value;
                this.Update();

                if (CheckStateChanged != null)
                    CheckStateChanged(this, new EventArgs());
            }
        }

        public TextStyle Style { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }

        // event triggered when Check state changes
        public event EventHandler CheckStateChanged;

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
            m_checked = !m_checked;
            this.Update();
            base.Released();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (m_pressed)
            {
                drawingGraphics.Color(this.BorderColor);
                drawingGraphics.FillRectangle(0, 0, 40, 40);
            }
            else
            if (m_checked)
            {
                drawingGraphics.Color(this.BorderColor);
                drawingGraphics.PenWidth(2);
                drawingGraphics.DrawRectangle(0, 0, 40, 40);

                drawingGraphics.FillRectangle(5, 5, 35, 35);
            }
            else
            {
                // unchecked unpressed
                drawingGraphics.Color(this.BackgroundColor);
                drawingGraphics.FillRectangle(0, 0, 40, 40);

                drawingGraphics.Color(this.BorderColor);
                drawingGraphics.PenWidth(2);
                drawingGraphics.DrawRectangle(0, 0, 40, 40);
            }

            drawingGraphics.Style(this.Style);

            switch (this.AutoSizeMode)
            {
                case AutoSizeModeOptions.None:
                case AutoSizeModeOptions.OneLineAutoHeight:
                    drawingGraphics.MoveX(60).DrawText(m_text);
                    break;
                case AutoSizeModeOptions.WrapText:
                    drawingGraphics.MoveX(60).DrawMultiLineText(m_text, this.Size.Width, this.Size.Height);
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
