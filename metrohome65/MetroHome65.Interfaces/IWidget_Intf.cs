using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.UIElements;

namespace MetroHome65.Interfaces
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
        Size GridSize { set; }

        /// <summary>
        /// force repaint tile
        /// </summary>
        void ForceUpdate();

        /// <summary>
        /// Handler for click event
        /// </summary>
        /// <param name="location">
        ///   Coordinates of click event, 
        ///   relative to tile's left upper corner
        /// </param>
        bool OnClick(Point location);

        bool OnDblClick(Point location);

        bool DoExitAnimation { get; }

        ICollection<UIElement> EditControls(FleuxControlPage settingsPage);
    }

}
