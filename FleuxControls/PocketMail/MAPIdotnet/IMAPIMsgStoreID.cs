using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    public interface IMAPIMsgStoreID : IEntryID, IEquatable<IMAPIMsgStoreID>
    {
        IMAPIMsgStore OpenMsgStore();
        MAPI ParentSession { get; }
    }
}
