using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal class EntryID : IEntryID
    {
        private byte[] id;
        private int hashcode = 0;
        private cemapi.IMAPISession owner;

        //public EntryID(IEntryID id, cemapi.IMAPISession owner) { this.owner = owner; this.id = id.AsByteArray; }
        public EntryID(byte[] id, cemapi.IMAPISession owner) {this.id = id; this.owner = owner; }
        protected EntryID(IEntryID id, cemapi.IMAPISession owner) { this.id = id.AsByteArray; this.owner = owner; }

        public byte[] AsByteArray { get { return this.id; } }

        public static EntryID BuildFromPtr(IntPtr ptr, cemapi.IMAPISession owner)
        {
            int cbEntryID = Marshal.ReadInt32(ptr);
            if (!(cbEntryID > 0))
                return null;
            byte[] id = new byte[cbEntryID];
            IntPtr lpEntryID = Marshal.ReadIntPtr((IntPtr)((uint)ptr + Marshal.SizeOf(typeof(int))));
            for (int i = 0; i < cbEntryID; i++)
                id[i] = Marshal.ReadByte(lpEntryID, i);
            return new EntryID(id, owner);
        }

        #region Operator overloads
        public static bool operator ==(EntryID a, EntryID b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EntryID a, EntryID b)
        {
            return !(a == b);
        }

        public bool Equals(IEntryID id)
        {
            if (id == null)
                return false;
            return this.owner.CompareEntryIDs(id.AsByteArray, this.AsByteArray);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            EntryID i = obj as EntryID;
            if (i == null)
                return false;
            return this.owner.CompareEntryIDs(i.AsByteArray, this.AsByteArray);
        }

        public override int GetHashCode()
        {
            if (this.hashcode == 0)
            {
                foreach (byte b in this.id)
                    this.hashcode ^= b;
            }
            return this.hashcode;
        }
        #endregion
    }
}
