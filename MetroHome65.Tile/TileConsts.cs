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
        public int TilesPaddingTop { get; set; }

        public int ArrowPosY { get; protected set; }

        public int ArrowPrevPosX { get; protected set; }

        public int ArrowNextPosX { get; protected set; }
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

            ArrowPosY = TilesPaddingTop;
            ArrowPrevPosX = 24;
            ArrowNextPosX = TileSize * 4 + TileSpacing * 3 + TilesPaddingLeft + ArrowPrevPosX;
        }
    }

    /// <summary>
    /// settings for tiles for theme Windows Phone 7
    /// </summary>
    public class TileThemeWindows8 : TileTheme
    {
        public TileThemeWindows8()
        {
            // real Win8 small tile size is 105px
            TileSize = 105;

            TileSpacing = 12;
            TilesPaddingLeft = TileSpacing;
            TilesPaddingTop = TileSpacing;

            ArrowPosY = 93 - ScreenConsts.TopBarSize;
            ArrowPrevPosX = 24;
            ArrowNextPosX = -1000; // hide arrow in tiles screen
        }
    }

}
