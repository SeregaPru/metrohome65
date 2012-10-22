using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines.Screen;

namespace Metrohome65.Settings.Controls
{

    public class FontPopupPage : FleuxControlPage
    {
        private const int PaddingHor = 20;
        private const int PaddingVer = 20;

        public event CustomSettingsPage<TextStyle>.ApplySettingsHandler OnApplySettings;

        private readonly TextStyle _textStyle;


        public FontPopupPage(TextStyle textStyle) : base(true)
        {
            _textStyle = textStyle;
            CreateControls();
        }

        private void CreateControls()
        {
            ScreenRoutines.CursorWait();
            try
            {
                Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

                var appBar = new ApplicationBar
                {
                    Size = new Size(Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Size.Height - 48 - 2 * 10)
                };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.back.bmp"));
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.ok.bmp"));
                appBar.ButtonTap += OnAppBarButtonTap;
                Control.AddElement(appBar.AnimateHorizontalEntrance(false));

                var stackPanel = new StackPanel { Size = new Size(this.Size.Width - PaddingHor, 10), };

                stackPanel.AddElement(new ComboBox()
                    {
                        Items = new List<object>() { 
                            MetroTheme.PhoneFontFamilyNormal,
                            MetroTheme.PhoneFontFamilyLight,
                            MetroTheme.PhoneFontFamilySemiLight,
                            MetroTheme.PhoneFontFamilySemiBold,
                        },
                        Style = MetroTheme.PhoneTextNormalStyle,
                        Size = new Size(Size.Width - 2 * PaddingHor, 50),
                        SelectedIndex = 0,
                    }
                );

                stackPanel.AddElement(new Canvas() { Size = new Size(10, 10), });

                var sourceItems = new BindingList<object>()
                    {
                        "8",
                        "10",
                        "11",
                        "12",
                        "14",
                        "16",
                        "18",
                        "20",
                        "22",
                        "24",
                        "24",
                        "26",
                        "28",
                        "30",
                        "38",
                    };
                stackPanel.AddElement(new ListElement()
                    {
                        Size = new Size(200, 250),
                        DataTemplateSelector = item => item1 => new TextElement((string)item1)
                            {
                                Style = new TextStyle(
                                    MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                                    Color.DeepPink),
                                Size = new Size(190, 50),
                            }, 
                        SourceItems = sourceItems,
                    }
                );


                var scroller = new SolidScrollViewer
                    {
                        Content = stackPanel,
                        Location = new Point(PaddingHor, PaddingVer),
                        Size = new Size(this.Size.Width - PaddingHor, this.Size.Height - appBar.Size.Height - PaddingVer),
                        ShowScrollbars = true,
                        HorizontalScroll = false,
                        VerticalScroll = true,
                    };

                Control.AddElement(scroller);
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }


        protected virtual void ApplySettings()
        {
            ScreenRoutines.CursorWait();
            try
            {
                if (OnApplySettings != null)
                    OnApplySettings(null, _textStyle);
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }


        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 1) // ok button
            {
                ApplySettings();
                Close();
            }
            else
                // close button
                Close();
        }

  
    }


    public class PopupEdit: Canvas
    {
        #region Fields

        private TextStyle _textStyle = new TextStyle(
                MetroTheme.PhoneFontFamilyNormal,
                12,
                MetroTheme.PhoneForegroundBrush
            );

        private FontPopupPage _popupPage;

        #endregion


        #region Properties

        #endregion


        #region Methods

        public PopupEdit()
        {
            TapHandler += p =>
                {
                    DropDown();
                    return true;
                };
        }

        private FontPopupPage CreatePopup()
        {
            var popupPage = new FontPopupPage(_textStyle);
            return popupPage;
        }

        private void DropDown()
        {
            _popupPage = CreatePopup();
            _popupPage.OnApplySettings += ApplySettings;
            
            _popupPage.TheForm.Show();
        }

        private void ApplySettings(object sender, TextStyle settings)
        {
            this._textStyle = settings;
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
            drawingGraphics.Style(new TextStyle(_textStyle.FontFamily, _textStyle.FontSize, _textStyle.Foreground));
            drawingGraphics.MoveRelX(4).DrawText(_textStyle.FontFamily);

            base.Draw(drawingGraphics);
        }

        #endregion

    }
}
