using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MAPIdotnet
{
    public interface IMAPIContact : IEquatable<IMAPIContact>
    {
        string Name { get; }
        string FullAddress { get; }

        Recipient AsPOOM();

        IMAPIMsgStore Store { get; }
    }
}
