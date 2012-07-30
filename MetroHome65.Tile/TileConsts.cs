using MetroHome65.Routines.Screen;

namespace MetroHome65.Tile
{

    public abstract class TileTheme
    {
        // 1x1 tile size 
        public int TileSize { get; protected set; }

        // spacing between tiles 
        public int TileSpacing { get; protected set; }

        // tile canvas left padding
        public int TilesPaddingLeft { get; protected set; }

        // tile canvas top padding
        public int TilesPaddingTop { get; protected set; }

        public bool ArrowVisible { get; protected set; }

        public int ArrowPadding { get; protected set; }

        public int ArrowPosX { get; protected set; }
    }


    /// <summary>
    /// settings for tiles for theme Windows Phone 7
    /// </summary>
    public class TileThemeWP7 : TileTheme
    {
        public TileThemeWP7()
        {
            // real WP7 size is 80,5px or 173 for 2x2 tile)
            TileSize = 81;

            TileSpacing = 12;

            TilesPaddingLeft = 28;

            // 93 is real padding in WP7, 36 is WM top bar height
            TilesPaddingTop = 93 - ScreenConsts.TopBarSize;

            ArrowPadding = 24;

            ArrowPosX = TileSize * 4 + TileSpacing * 3 + TilesPaddingLeft + ArrowPadding;
        }
    }

    /// <summary>
    /// settings for tiles for theme Windows Phone 7
    /// </summary>
    public class TileThemeWindows8 : TileTheme
    {
        public TileThemeWindows8()
        {
            // real Win8 small tile size is ???px, ??? for medium, ??? for big )
            TileSize = 90;
            TileSpacing = 12;

            TilesPaddingLeft = 5;
            TilesPaddingTop = 5 + ScreenConsts.TopBarSize;

            ArrowPadding = -100;
            ArrowPosX = 0;
        }
    }

}
