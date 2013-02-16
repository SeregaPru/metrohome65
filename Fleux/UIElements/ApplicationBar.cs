// GIANNI added

using Fleux.Styles;

namespace Fleux.UIElements
{
    using System;
    using System.Drawing;
    using Events;

    public class ApplicationBar : Canvas
    {
        #region Events
        public event EventHandler<ButtonTapEventArgs> ButtonTap;
        private void OnButtonTap(int id)
        {
            EventHandler<ButtonTapEventArgs> localHandler = ButtonTap;
            if (localHandler != null)
                localHandler(this, new ButtonTapEventArgs(id));
        }
        #endregion

        #region Private fields
        private int m_CurrentButtonsWidth = 0;
        private int m_ButtonSpacing = 30;
        #endregion

        #region Methods
        public bool AddButton(Bitmap buttonIcon)
        {
            if (Children.Count >= 4)
            {
                return false;
            }
            else
            {
                int index = Children.Count;
                AddElement(new ImageButton(buttonIcon) { Size = buttonIcon.Size, TapHandler = p => { OnButtonTap(index); return true; } });
                m_CurrentButtonsWidth += buttonIcon.Width + m_ButtonSpacing;
                RecalculatePositions();
                return true;
            }
        }
        #endregion

        #region OVERRIDE - Methods
        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(GetColor());
            drawingGraphics.FillRectangle(new Rectangle(0, 0, Size.Width, Size.Height));

            base.Draw(drawingGraphics);
        }
        #endregion

        #region Private methods

        private Color GetColor()
        {
            return (MetroTheme.PhoneBackgroundBrush == Color.White) ? 
                Color.FromArgb(222,222,222) : Color.FromArgb(42,42,42);
        }

        private void RecalculatePositions()
        {
            // CENTER THE BUTTONS HORIZONTALLY
            int offset = (this.Size.Width - m_CurrentButtonsWidth) / 2;
            if (offset < 0)
                return;

            int prevLocation = offset;
            Children.ForEach(element =>
            {
                element.Location = new Point(prevLocation, (this.Size.Height - element.Size.Height) / 2);
                prevLocation = element.Location.X + element.Size.Width + m_ButtonSpacing;
            });
        }
        #endregion
    }
}
