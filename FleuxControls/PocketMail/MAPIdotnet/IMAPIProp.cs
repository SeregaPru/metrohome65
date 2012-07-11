using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MAPIdotnet
{
    public interface IMAPIProp : IDisposable
    {
        string DisplayName { get; }

        IEntryID EntryID { get; }
        
        Icon Icon { get; }
    }
}
