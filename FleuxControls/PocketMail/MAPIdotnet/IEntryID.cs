using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    public interface IEntryID : IEquatable<IEntryID>
    {
        byte[] AsByteArray { get; }
    }
}
