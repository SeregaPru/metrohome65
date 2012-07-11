using System;
using System.Collections;

namespace MetroHome65.Interfaces
{
    public interface IPluginManager
    {
        // creates tile of selected type
        ITile CreateTile(String tileName);

        // creates lockscreen of selected type
        ILockScreen CreateLockScreen(String lockScreenName);

        IEnumerable GetTileTypes();

        IEnumerable GetLockScreenTypes();
    }
}
