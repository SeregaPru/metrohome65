using MetroHome65.Routines;

namespace MetroHome65.Tile
{
    public static class TileConsts
    {
        // 1x1 tile size (real WP7 size is 80,5px or 173 for 2x2 tile)
        public const int TileSize = 81;

        // spacing between tiles (as in WP7)
        public const int TileSpacing = 12;

        // tile canvas left padding (as in WP7)
        public const int TilesPaddingLeft = 28;

        // tile canvas left padding
        // 93 is real padding in WP7, 36 is WM top bar height
        public const int TilesPaddingTop = 93 - ScreenConsts.TopBarSize;

        public const int ArrowPadding = 24;

        public const int ArrowPosX = TileConsts.TileSize * 4 + TileConsts.TileSpacing * 3 + TileConsts.TilesPaddingLeft + ArrowPadding;

    }
}
