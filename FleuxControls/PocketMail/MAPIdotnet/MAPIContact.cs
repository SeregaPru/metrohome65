using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MAPIdotnet
{
    internal class MAPIContact : IMAPIContact
    {
        private string name, fullAddress;
        private IMAPIMsgStore store;

        public MAPIContact(string fullAddress, string name, IMAPIMsgStore store)
        { this.fullAddress = fullAddress; this.name = name; this.store = store; }

        public string Name { get { return this.name; } }

        public string FullAddress { get { return this.fullAddress; } }

        public Recipient AsPOOM() { return new Recipient(this.name, new Recipient(this.fullAddress).Address); }

        public IMAPIMsgStore Store { get { return this.store; } }

        #region Operator overloads
        public static bool operator ==(MAPIContact a, MAPIContact b)
        {
            if (a == null)
                return false;
            return a.Equals((IMAPIContact)b);
        }

        public static bool operator !=(MAPIContact a, MAPIContact b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IMAPIContact);
        }

        public bool Equals(IMAPIContact c)
        {
            if (c == null)
                return false;
            return c.Name.Equals(this.name);
        }

        public override int GetHashCode()
        {
            return this.name.GetHashCode();
        }
        #endregion

        public override string ToString() { return this.name; }
    }
}
