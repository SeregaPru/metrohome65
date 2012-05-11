using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    public interface IMAPIMessageID : IEntryID, IEquatable<IMAPIMessageID>
    {
        IMAPIMessage OpenMessage();
        IMAPIFolderID ParentFolder { get; }
        IMAPIMsgStore ParentStore { get ; }
    }
}
