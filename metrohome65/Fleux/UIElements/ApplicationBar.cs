// GIANNI added

namespace Fleux.UIElements
{
    using System;
    using Fleux.UIElements;
    using System.Drawing;
    using Fleux.UIElements.Events;

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
            base.Draw(drawingGraphics);
        }
        #endregion

        #region Private methods

        private void RecalculatePositions()
        {
            // CENTER THE BUTTONS HORIZONTALLY
            int offset = (this.Size.Width - m_CurrentButtonsWidth) / 2;
            if (offset < 0)
                return;

            int prevLocation = offset;
            Children.ForEach(element =>
            {
                element.Location = new Point(prevLocation, 0);
                prevLocation = element.Location.X + element.Size.Width + m_ButtonSpacing;
            });
        }
        #endregion
    }
}
