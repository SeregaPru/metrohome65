using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    public interface IMAPIMsgStores
    {

        /*IMAPIMsgStore DefaultStore { get; }*/

        IMAPIMsgStore[] Stores { get; }

        string[] StoreNames { get; }

    }
}
