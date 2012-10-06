using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements.Events;

namespace Fleux.UIElements
{
    public class PopupPage : Canvas
    {
        protected ApplicationBar _appBar;

        public event Action OnClose;

        protected internal virtual void CreateAppBar()
        {
            _appBar = new ApplicationBar
            {
                Size = new Size(Size.Width, 48 + 2 * 10),
                Location = new Point(0, Size.Height - 48 - 2 * 10)
            };
            _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Fleux.Images.back.bmp"));
            _appBar.ButtonTap += OnAppBarButtonTap;
            AddElement(_appBar);
        }

        protected internal virtual void InitializeControls() { }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            ParentControl.RemoveElement(this);
            OnClose();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(MetroTheme.PhoneBackgroundBrush);
            drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height);

            base.Draw(drawingGraphics);
        }
    }


    public class FontPopupPage : PopupPage
    {
        private const int Padding = 20;
        protected internal override void InitializeControls()
        {
            var stackPanel = new StackPanel { Size = new Size(10, 10), };

            stackPanel.AddElement(new ComboBox()
            {
                
                Size = new Size(Size.Width - 2 * Padding, 50),
                Items = new List<object>() { 
                    MetroTheme.PhoneFontFamilyNormal,
                    MetroTheme.PhoneFontFamilyLight,
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontFamilySemiBold,
                },
                Style = MetroTheme.PhoneTextNormalStyle,
            }
                );
            stackPanel.AddElement(new Canvas() { Size = new Size(100, 100), });

            var scroller = new SolidScrollViewer
                {
                    Content = stackPanel,
                    Location = new Point(Padding, Padding),
                    Size = Size = new Size(this.Size.Width - Padding, this.Size.Height - _appBar.Size.Height - Padding),
                    ShowScrollbars = true,
                    HorizontalScroll = false,
                    VerticalScroll = true,
                };

            this.AddElement(scroller);
        }
    }


    public class PopupEdit: Canvas
    {
        #region Fields

        private readonly FleuxControlPage _settingsPage;

        private bool _droppedDown = false;

        private string _fontFamily = MetroTheme.PhoneFontFamilyNormal;
        private int _fontSize = 12;
        private Color _fontColor = MetroTheme.PhoneForegroundBrush;

        private PopupPage _popupPage;

        #endregion


        #region Properties

        #endregion


        #region Methods

        public PopupEdit(FleuxControlPage settingsPage)
        {
            _settingsPage = settingsPage;

            TapHandler += p =>
                {
                    DropDown();
                    return true;
                };
        }

        private PopupPage CreatePopup()
        {
            var popupPage = new FontPopupPage
                {
                    Size = _settingsPage.Size
                };
            popupPage.CreateAppBar();
            popupPage.InitializeControls();
            return popupPage;
        }

        private void DropDown()
        {
            if (_droppedDown) return;

            _droppedDown = true;

            _popupPage = CreatePopup();

            _popupPage.OnClose += CloseUp;

            //_settingsPage.Control.AddElement(_popupPage.AnimateHorizontalEntrance(false));
            //_settingsPage.Control.AnimateEntrance();

            _settingsPage.Control.AddElement(_popupPage);
            Update();
        }

        private void CloseUp()
        {
            if (! _droppedDown) return;

            _droppedDown = false;

            //_settingsPage.Control.AnimateExit();
            Update();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            // field background
            //drawingGraphics.Color(MetroTheme.PhoneBackgroundBrush);
            //drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height);

            // field border
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            // font example 
            drawingGraphics.Style(new TextStyle(_fontFamily, _fontSize, _fontColor));
            drawingGraphics.MoveRelX(4).DrawText(_fontFamily);

            base.Draw(drawingGraphics);
        }

        #endregion

    }
}
