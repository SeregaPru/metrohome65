using System;
using System.Collections;
using System.Collections.Generic;

namespace MetroHome65.Interfaces
{
    public interface IPluginManager
    {
        // creates tile of selected type
        ITile CreateTile(String tileName);

        IEnumerable GetTileTypes();
    }
}
