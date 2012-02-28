using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;


namespace MetroHome65.Widgets
{

    /// <summary>
    /// Attribute - tile description, that will be diplayed in properties page
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TileInfoAttribute : Attribute
    {
        private readonly String _caption = "";

        public String Caption { get { return _caption; } }


        public TileInfoAttribute() { }

        public TileInfoAttribute(String caption)
        {
            _caption = caption;
        }
    }


    /// <summary>
    /// Signs field or property of tile, that it is user defined parameter,
    /// and it should be stored in tile's user settings
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class TileParameterAttribute : Attribute {
    }


    /// <summary>
    /// Tile base interface
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// Tile possible sizes, in cells
        /// 1x1 1x2 2x2 etc. .. 4x4 is max
        /// </summary>
        Size[] Sizes { get; }

        /// <summary>
        /// Current tile size
        /// </summary>
        Size Size { set; }

        /// <summary>
        /// Additional popup menu items for tile.
        /// They will be shown in tile popup menu, above standart items.
        /// </summary>
        String[] MenuItems { get; }

        /// <summary>
        /// Tile transparency.
        /// Transparent tile draws itself over grid background.
        /// Not transparent tile should draw the whole area.
        /// </summary>
        Boolean Transparent { get; }

        /// <summary>
        /// paint tile's internal area.
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="rect">Drawing area</param>
        void Paint(Graphics g, Rectangle rect);

        /// <summary>
        /// Hanler for custom menu item click
        /// </summary>
        /// <param name="itemName"></param>
        void OnMenuItemClick(String itemName);

        /// <summary>
        /// Handler for click event
        /// </summary>
        /// <param name="location">
        ///   Coordinates of click event, 
        ///   relative to tile's left upper corner
        /// </param>
        bool OnClick(Point location);

        bool OnDblClick(Point location);

        bool AnimateExit { get; } 

        List<Control> EditControls { get; }
    }

}
