using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MAPIdotnet
{
    internal abstract class MAPIProp : IMAPIProp
    {
        private cemapi.IMAPIProp instance;
        private string displayName = null;
        private IEntryID entryID = null;
        private cemapi.IMAPISession parent;

        public MAPIProp(cemapi.IMAPIProp instance, cemapi.IMAPISession parent) { this.instance = instance; this.parent = parent; }

        public abstract void Dispose();

        public string DisplayName
        {
            get
            {
                if (this.displayName == null)
                {
                    cemapi.IPropValue[] props = this.instance.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_DISPLAY_NAME });
                    this.displayName = props[0].AsString;
                }
                return this.displayName;
            }
        }

        public override string ToString() { return this.DisplayName; }

        public IEntryID EntryID
        {
            get
            {
                if (this.entryID == null)
                {
                    cemapi.IPropValue[] props = this.instance.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_ENTRYID });
                    this.entryID = new EntryID(props[0].AsBinary, this.parent);
                }
                return this.entryID;
            }
        }

        public Icon Icon
        {
            get
            {
                cemapi.IPropValue[] props = this.instance.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_ICON });
                return new Icon(new System.IO.MemoryStream(props[0].AsBinary));
            }
        }

        /*public bool Equals(IMAPIProp p)
        {
            if (p == null)
                return false;
            return this.EntryID.Equals(p.EntryID);
        }

        public override bool Equals(object obj)
        {
            MAPIProp p = obj as MAPIProp;
            if (p == null)
                return false;

            return this.EntryID.Equals(p.EntryID);
        }*/


    }
}
