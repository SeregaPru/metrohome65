using System.Collections.Generic;
using System.ComponentModel;

namespace Fleux.Styles
{
    using System;
    using System.Drawing;
    using Core.GraphicsHelpers;

    public delegate void ThemeChangedEventHandler(PropertyChangedEventArgs e);

    /// <summary>
    /// Use as reference http://msdn.microsoft.com/en-us/library/ff769552(VS.92).aspx
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder",
        Justification = "Reviewed. Suppression is OK here.")]
    public static class MetroTheme 
    {
        #region Brushes/Colors

        // Foreground color to single-out items of interest
        private static Color _phoneAccentBrush = Color.FromArgb(40, 160, 220);
        public static Color PhoneAccentBrush
        {
            get { return _phoneAccentBrush; }
            set { SetField(ref _phoneAccentBrush, value, "PhoneAccentBrush"); }
        }

        private static Color _phoneForegroundBrush = Color.White;
        public static Color PhoneForegroundBrush
        {
            get { return _phoneForegroundBrush; }
            set { SetField(ref _phoneForegroundBrush, value, "PhoneForegroundBrush"); }
        }

        private static Color _phoneBackgroundBrush = Color.Black;
        public static Color PhoneBackgroundBrush
        {
            get { return _phoneBackgroundBrush; }
            set { SetField(ref _phoneBackgroundBrush, value, "PhoneBackgroundBrush"); }
        }

        public static Color PhoneInactiveBrush
        {
            get { return Color.Black; }
        }

        public static Color PhoneTextBoxBrush
        {
            get { return _phoneForegroundBrush; }
        }

        // GIANNI added
        public static Color PhoneTextBoxBorderBrush
        {
            get { return Color.Gray; }
        }
        
        public static Color PhoneSubtleBrush
        {
            get { return Color.FromArgb(153, 153, 153); }
        }

        public static Color PhoneContrastForegroundBrush
        {
            get { return Color.White; }
        }

        #endregion

        #region Font Families

        // TODO: public static string PhoneFontFamilyNormal { get { return "Segoe WP"; } }
        public static string PhoneFontFamilyNormal
        {
            get { return "Segoe WP"; }
        }

        public static string PhoneFontFamilyLight
        {
            get { return "Segoe WP Light"; }
        }

        public static string PhoneFontFamilySemiLight
        {
            get { return "Segoe WP SemiLight"; }
        }

        public static string PhoneFontFamilySemiBold
        {
            get { return "Segoe WP Semibold"; }
        }

        #endregion

        #region Font Size

        ////public static int PhoneFontSizeSmall { get { return 14; } }
        ////public static int PhoneFontSizeNormal { get { return 15; } }
        ////public static int PhoneFontSizeMedium { get { return 17; } }
        ////public static int PhoneFontSizeMediumLarge { get { return 19; } }
        ////public static int PhoneFontSizeLarge { get { return 24; } }
        ////public static int PhoneFontSizeExtraLarge { get { return 32; } }
        ////public static int PhoneFontSizeExtraExtraLarge { get { return 54; } }
        ////public static int PhoneFontSizeHuge { get { return 140; } }

        public static int PhoneFontSizeSmall
        {
#if WindowsCE
            get { return 16; }
#else
            get { return 8; }
#endif
        }

        public static int PhoneFontSizeNormal
        {
#if WindowsCE
            get { return 24; }
#else
            get { return 12; }
#endif
        }

        public static int PhoneFontSizeMedium
        {
#if WindowsCE
            get { return 26; }
#else
            get { return 13; }
#endif
        }

        public static int PhoneFontSizeMediumLarge
        {
#if WindowsCE
            get { return 32; }
#else
            get { return 16; }
#endif
        }

        public static int PhoneFontSizeLarge
        {
#if WindowsCE
            get { return 38; }
#else
            get { return 19; }
#endif
        }

        public static int PhoneFontSizeExtraLarge
        {
#if WindowsCE
            get { return 50; }
#else
            get { return 25; }
#endif
        }

        public static int PhoneFontSizeExtraExtraLarge
        {
#if WindowsCE
            get { return 62; }
#else
            get { return 31; }
#endif
        }

        public static int PhoneFontSizeHuge
        {
#if WindowsCE
            get { return 186; }
#else
            get { return 93; }
#endif
        }

        #endregion

        #region Thickness

        public static ThicknessStyle PhoneHorizontalMargin
        {
#if WindowsCE
            get { return new ThicknessStyle(24, 0, 0); }
#else
            get { return new ThicknessStyle(12, 0, 0); }
#endif
        }

        public static ThicknessStyle PhoneVerticalMargin
        {
#if WindowsCE
            get { return new ThicknessStyle(0, 24, 0); }
#else
            get { return new ThicknessStyle(0, 12, 0); }
#endif
        }

        public static ThicknessStyle PhoneMargin
        {
#if WindowsCE
            get { return new ThicknessStyle(24, 0, 0); }
#else
            get { return new ThicknessStyle(12, 0, 0); }
#endif
        }

        public static ThicknessStyle PhoneTouchTargetOverhang
        {
#if WindowsCE
            get { return new ThicknessStyle(24, 0, 0); }
#else
            get { return new ThicknessStyle(12, 0, 0); }
#endif
        }

        public static ThicknessStyle PhoneTouchTargetLargeOverhang
        {
#if WindowsCE
            get { return new ThicknessStyle(24, 40, 0); }
#else
            get { return new ThicknessStyle(12, 20, 0); }
#endif
        }

        public static ThicknessStyle PhoneBorderThickness
        {
#if WindowsCE
            get { return new ThicknessStyle(6, 0, 0); }
#else
            get { return new ThicknessStyle(3, 0, 0); }
#endif
        }

        public static ThicknessStyle PhoneStrokeThickness
        {
#if WindowsCE
            get { return new ThicknessStyle(6, 0, 0); }
#else
            get { return new ThicknessStyle(3, 0, 0); }
#endif
        }

        #endregion

        #region TextStyles

        public static TextStyle PhoneTextBlockBase
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilyNormal,
                    MetroTheme.PhoneFontSizeSmall,
                    MetroTheme.PhoneTextBoxBrush,
                    MetroTheme.PhoneHorizontalMargin);
            }
        }

        public static TextStyle PhoneTextNormalStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilyNormal,
                    MetroTheme.PhoneFontSizeNormal,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextTitle1Style
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeExtraExtraLarge,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextTitle2Style
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeLarge,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextTitle3Style
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeMedium,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextLargeStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeLarge,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextExtraLargeStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeExtraLarge,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextGroupHeaderStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeLarge,
                    MetroTheme.PhoneSubtleBrush);
            }
        }

        public static TextStyle PhoneTextSmallStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilyNormal,
                    MetroTheme.PhoneFontSizeSmall,
                    MetroTheme.PhoneSubtleBrush);
            }
        }

        public static TextStyle PhoneTextContrastStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiBold,
                    MetroTheme.PhoneFontSizeNormal,
                    MetroTheme.PhoneContrastForegroundBrush);
            }
        }

        public static TextStyle PhoneTextAccentStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiBold,
                    MetroTheme.PhoneFontSizeNormal,
                    MetroTheme.PhoneAccentBrush);
            }
        }

        #endregion

        #region additional text styles

        public static TextStyle PhoneTextPageTitle1Style
        {
#if WindowsCE
            get
            {
                var r = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 16, MetroTheme.PhoneForegroundBrush);
                r.Thickness = new ThicknessStyle(0, 40, 0);
                return r;
            }
#else
            get
            {
                var r = new TextStyle(MetroTheme.PhoneFontFamilySemiBold, 8, MetroTheme.PhoneForegroundBrush);
                r.Thickness = new ThicknessStyle(0, 20, 0);
                return r;
            }
#endif
        }

        public static TextStyle PhoneTextPageTitle2Style
        {
#if WindowsCE
            get
            {
                var r = new TextStyle(MetroTheme.PhoneFontFamilySemiLight, 50, MetroTheme.PhoneForegroundBrush);
                r.Thickness = new ThicknessStyle(0, 40, 0);
                return r;
            }
#else
            get
            {
                var r = new TextStyle(MetroTheme.PhoneFontFamilySemiLight, 25, MetroTheme.PhoneForegroundBrush);
                r.Thickness = new ThicknessStyle(0, 20, 0);
                return r;
            }
#endif
        }


        public static TextStyle TileTextStyle
        {
#if WindowsCE
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiBold,
                    14,
                    Color.White);
            }
#else
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilySemiBold,
                    8,
                    Color.White);
            }
#endif
        }

        #endregion

        #region Panorama

        public static TextStyle PhoneTextPanoramaTitleStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilyLight,
                    65,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        public static TextStyle PhoneTextPanoramaSectionTitleStyle
        {
            get
            {
                return new TextStyle(
                    MetroTheme.PhoneFontFamilyLight,
                    MetroTheme.PhoneFontSizeExtraLarge,
                    MetroTheme.PhoneForegroundBrush);
            }
        }

        // TitleHeader
        public static Action<IDrawingGraphics, string, string> DrawPanoramaTitleAction
        {
            get
            {
                return (g, title, subtitle) => g
                                                   .Style(MetroTheme.PhoneTextPanoramaTitleStyle).Bold(false)
                                                   .MoveX(0).MoveY(-90).DrawText(title)
                                                   .MoveX(0).MoveY(g.Bottom).Style(
                                                       MetroTheme.PhoneTextPanoramaSectionTitleStyle).DrawText(subtitle)
                                                   .MoveY(g.Bottom + 20);
            }
        }

        public static Color PanoramaNormalBrush
        {
            get { return Color.FromArgb(255, 255, 255); }   
        }

        #endregion


        #region NotifyPropertyChanged

        public static event ThemeChangedEventHandler PropertyChanged;

        private static void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        private static bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

    }
}