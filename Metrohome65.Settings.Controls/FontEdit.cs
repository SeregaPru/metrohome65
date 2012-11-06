using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;

namespace Metrohome65.Settings.Controls
{

    public class FontEdit: Canvas
    {
        #region Fields

        private TextStyle _textStyle = new TextStyle(
                MetroTheme.PhoneFontFamilyNormal,
                12,
                MetroTheme.PhoneForegroundBrush
            );

        #endregion

        public TextStyle Value
        {
            get { return _textStyle; }
            set
            {
                _textStyle = Value;
                Relayout();
                Update();
            }
        }

        private void Relayout()
        {
            var height = FleuxApplication.DummyDrawingGraphics.Style(_textStyle)
                .CalculateMultilineTextHeight(Caption(), 1000);
            var width = FleuxApplication.DummyDrawingGraphics.Style(_textStyle)
                .CalculateTextWidth(Caption()) + 10;

            this.Size = new Size(width, height);
        }

        #region Methods

        public FontEdit()
        {
            TapHandler += p =>
                {
                    DropDown();
                    return true;
                };
        }

        private void DropDown()
        {
            var popupPage = new FontPopupPage(_textStyle);
            popupPage.TheForm.Closing += (sender, args) => Relayout();
            popupPage.TheForm.Show();
        }

        private void ApplySettings(object sender, TextStyle settings)
        {
            this._textStyle = settings;
            Update();
        }


        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            // field border
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            // font example 
            drawingGraphics.Style(new TextStyle(_textStyle.FontFamily, _textStyle.FontSize, _textStyle.Foreground));
            drawingGraphics.MoveRelX(4).DrawText(Caption());

            base.Draw(drawingGraphics);
        }

        private string Caption()
        {
            return _textStyle.FontFamily;
        }

        #endregion
    }



    public class FontPopupPage : FleuxControlPage
    {
        private readonly TextStyle _textStyle;
        
        private TextElement _example;


        public FontPopupPage(TextStyle textStyle)
            : base(true)
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
                appBar.ButtonTap += (sender, args) => Close();
                Control.AddElement(appBar.AnimateHorizontalEntrance(false));

                var stackPanel = new StackPanel { Size = new Size(SettingsConsts.MaxWidth, 10), };

                // buttons for selecting font family
                stackPanel.AddElement(
                    new TextElement("Font family") { AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight, }
                );

                var fonts = new List<string>()
                    {
                        MetroTheme.PhoneFontFamilyNormal,
                        MetroTheme.PhoneFontFamilyLight,
                        MetroTheme.PhoneFontFamilySemiLight,
                        MetroTheme.PhoneFontFamilySemiBold,
                    };
                var fontBindingManager = new BindingManager() { MultiBind = true, };
                foreach (var font in fonts)
                {
                    var button = new ToggleButton(font)
                        {
                            Size = new Size(SettingsConsts.MaxWidth, 50),
                        };
                    stackPanel.AddElement(button);
                    fontBindingManager.Bind(_textStyle, "FontFamily", button, "Value", true);
                    stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, 10), });
                }

                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, 20) });

                // buttons for selecting font size
                stackPanel.AddElement(
                    new TextElement("Font size") { AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight, }
                );

                var sizePanel = new Canvas()
                {
                    Size = new Size(SettingsConsts.MaxWidth, 10),
                };
                stackPanel.AddElement(sizePanel);

                var sizeBindingManager = new BindingManager() { MultiBind = true, };
                var sizes = new List<int>
                    {
                         8, 10, 11, 12, 14, 
                        16, 18, 20, 22, 24, 
                        26, 28, 30, 32, 34, 
                        36, 38, 42, 46, 50, 
                    };
                var i = 0;
                foreach (var size in sizes)
                {
                    var button = new ToggleButton(size)
                            {
                                Location = new Point((i%5)*90, (i/5)*60),
                                Size = new Size(80, 50),
                            };
                    sizeBindingManager.Bind(_textStyle, "FontSize", button, "Value", true);
                    sizePanel.AddElement(button);
                    i++;
                }

                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, 20)});


                // font color
                var fontColor = new ColorSettingsControl(false)
                    {
                        Size = new Size(SettingsConsts.MaxWidth, 50),
                        Caption = "Font color",
                    };
                fontBindingManager.Bind(_textStyle, "Foreground", fontColor, "Value", true);
                stackPanel.AddElement(fontColor);

                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, 20) });


                // example text block
                _example = new TextElement("Example")
                    {
                        AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                        Style = _textStyle,
                    };
                _textStyle.PropertyChanged += (sender, args) => _example.Update();
                stackPanel.AddElement(_example);


                var scroller = new SolidScrollViewer
                {
                    Content = stackPanel,
                    Location = new Point(SettingsConsts.PaddingHor, SettingsConsts.PaddingHor),
                    Size = new Size(this.Size.Width - SettingsConsts.PaddingHor, this.Size.Height - appBar.Size.Height - SettingsConsts.PaddingHor),
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

    }



    public class ToggleButton : Button, INotifyPropertyChanged
    {
        private object _value;
        private readonly object _object;
        private bool _selected;

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;

                var selected = (_value != null) && (_value.Equals(_object));
                if (_selected == selected) return;
                _selected = selected;
                UpdateStyle();
            }
        }


        public ToggleButton(object text) : base(text.ToString())
        {
            _object = text;
            
            UpdateStyle();
        }

        public override bool Tap(Point p)
        {
            Value = _object;
            OnPropertyChanged("Value");

            return base.Tap(p);
        }

        private void UpdateStyle()
        {
            if ((_value != null) && (_value.Equals(_object)))
            {
                BackgroundColor = MetroTheme.PhoneForegroundBrush;
                Style = new TextStyle(
                    MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                    MetroTheme.PhoneAccentBrush);
            }
            else
            {
                BackgroundColor = Color.DarkGray;
                Style = new TextStyle(
                    MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                    MetroTheme.PhoneBackgroundBrush);
            }
            BorderColor = BackgroundColor;

            Update();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
