using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    public interface IMAPIFolderID : IEntryID, IEquatable<IMAPIFolderID>
    {
        IMAPIFolder OpenFolder();
        IMAPIMsgStore Parent { get; }
    }
}
