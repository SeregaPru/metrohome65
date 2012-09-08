using System.Drawing;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;

namespace Fleux.UIElements
{
    public class PopupPanel : Canvas
    {


    }


    public class PopupEdit: Canvas
    {
        #region Fields

        private readonly FleuxControlPage _settingsPage;

        private bool _droppedDown;

        #endregion


        #region Properties

        #endregion


        #region Methods

        public PopupEdit(FleuxControlPage settingsPage)
        {
            _settingsPage = settingsPage;

            TapHandler += p => DropDown();
        }

        private void CreatePopup()
        {
            var popupPage = new Canvas()
                                {
                                    Size = _settingsPage.Size,
                                };
        }

        private bool DropDown()
        {
            if (_droppedDown) return false;

            //......
            _droppedDown = true;

            //Update();

            return true;
        }

        private bool CloseUp()
        {
            if (! _droppedDown) return false;

            //...
            _droppedDown = false;

            Update();
            return true;
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            // field background
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBrush);
            drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height);

            // field border
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            base.Draw(drawingGraphics);
        }

        #endregion

    }
}
