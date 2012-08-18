using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    internal class MAPIFolder : MAPIProp, IMAPIFolder
    {
        private cemapi.IMAPIFolder folder;
        private cemapi.IMAPITable hierarchy = null;
        private cemapi.IMAPITable contents = null;
        private MAPIMsgStore parentStore;
        private IMAPIFolderID folderId;

        public MAPIFolder(MAPIMsgStore parentStore, cemapi.IMAPIFolder folder, IMAPIFolderID folderId) : base(folder, parentStore.Session)
        { this.folder = folder; this.parentStore = parentStore; this.folderId = folderId; }

        public override void Dispose()
        {
#warning MAPIFolder dispose not done!
        }

        public IMAPIMsgStore ParentStore { get { return this.parentStore; } }
        public IMAPIFolderID ParentFolder 
        { 
            get 
            {
                return new MAPIFolderID(this.folder.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_PARENT_ENTRYID })[0].AsBinary, this.parentStore);
            } 
        }

        private cemapi.IMAPITable Hierarchy
        {
            get
            {
                // Can't seek so may as well always get new one
                this.hierarchy = this.folder.GetHierarchyTable();
                return this.hierarchy;
            }
        }
        private cemapi.IMAPITable Contents
        {
            get
            {
                if (this.contents == null)
                    this.contents = this.folder.GetContentsTable();
                return this.contents;
            }
        }

        public int NumSubFolders { get { return (int)this.Hierarchy.GetRowCount(); } }

        public int NumSubItems { get { return (int)this.Contents.GetRowCount(); } }

        public IMAPIFolderID[] GetSubFolders(int maxNum)
        {
            cemapi.IMAPITable hierarchy = this.Hierarchy;
            hierarchy.SetColumns(new cemapi.PropTags[] { cemapi.PropTags.PR_ENTRYID });

            int available = this.NumSubFolders;
            if (maxNum > available)
                maxNum = (int)available;
            IMAPIFolderID[] folders = new IMAPIFolderID[maxNum];

            int current = 0;
            while (maxNum > 0)
            {
                int num = maxNum > cemapi.MaxQueryRowCount ? cemapi.MaxQueryRowCount : maxNum;
                cemapi.SRow[] rows = hierarchy.QueryRows(num);
                for (int i = 0; i < num; i++)
                    folders[i + current] = new MAPIFolderID(rows[i].propVals[0].AsBinary, this.parentStore);
                    //folders[i + current] = new MAPIFolder(this.parentStore, this, this.folder.OpenEntryAsFolder(rows[i].propVals[0].AsBinary));

                current += cemapi.MaxQueryRowCount;
                maxNum -= num;
            }
            return folders;
        }

        public void SeekMessages(int position)
        {
            if (this.contents != null)
                this.contents.SeekRow((uint)position);
        }
        public IMAPIMessage[] GetNextMessages(int maxNum)
        {
            cemapi.IMAPITable contents = this.Contents;
            contents.SetColumns(new cemapi.PropTags[] { cemapi.PropTags.PR_ENTRYID });

            List<IMAPIMessage> msgs = new List<IMAPIMessage>(maxNum);

            while (maxNum > 0)
            {
                int num = maxNum > cemapi.MaxQueryRowCount ? cemapi.MaxQueryRowCount : maxNum;
                cemapi.SRow[] rows = contents.QueryRows(num);
                int len = rows.Length;
                if (!(len > 0))
                    break;
                for (int i = 0; i < len; i++)
                {
                    byte[] msgId = rows[i].propVals[0].AsBinary;
                    msgs.Add(new MAPIMessage(this.folder.OpenEntryAsMessage(msgId), new MAPIMessageID(msgId, this.FolderID, this.parentStore)));
                }

                maxNum -= len;
            }
            return msgs.ToArray();
        }

        public void SortMessagesByDeliveryTime(TableSortOrder order)
        {
            this.Contents.SortRows(cemapi.PropTags.PR_MESSAGE_DELIVERY_TIME, order);
        }

        public void SortMessagesBySenderName(TableSortOrder order)
        {
            this.Contents.SortRows(cemapi.PropTags.PR_SENDER_NAME_W, order);
        }

        public void SortMessagesBySize(TableSortOrder order)
        {
            this.Contents.SortRows(cemapi.PropTags.PR_CONTENT_LENGTH, order);
        }

        public IMAPIFolderID FolderID { get { return this.folderId; } }

        public void DeleteMessages(IMAPIMessageID[] msgs)
        {
            int num = msgs.Length;
            List<byte[]> msgIds = new List<byte[]>(num);
            for (int i = 0; i < num; ++i)
                msgIds.Add(msgs[i].AsByteArray);
            this.folder.DeleteMessages(msgIds);
        }
        public void DeleteMessage(IMAPIMessageID id)
        {
            this.folder.DeleteMessage(id.AsByteArray);
        }

        public void EmptyFolder()
        {
            this.folder.EmptyFolder();
        }

        /*#region Operators
        public static bool operator ==(MAPIFolder a, MAPIFolder b)
        {
            if (((object)a) == null || ((object)b) == null)
                return false;
            return a.EntryID.Equals(b.EntryID);
        }

        public static bool operator !=(MAPIFolder a, MAPIFolder b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return ((MAPIFolder)obj == this);
        }

        public override int GetHashCode()
        {
            return this.EntryID.GetHashCode();
        }
        #endregion*/

        public bool Equals(IMAPIFolderID folder)
        {
            return folder.Equals(this.FolderID);
        }
    }

    internal class MAPIFolderID : EntryID, IMAPIFolderID
    {
        private MAPIMsgStore parent;
        public MAPIFolderID(byte[] id, MAPIMsgStore parent) : base(id, parent.Session) { this.parent = parent; }
        private MAPIFolderID(IEntryID  id, MAPIMsgStore parent) : base(id, parent.Session) { this.parent = parent; }
        public static MAPIFolderID BuildFromID(IEntryID id, MAPIMsgStore parent)
        {
            if (id == null)
                return null;
            return new MAPIFolderID(id, parent);
        }

        public IMAPIMsgStore Parent { get { return this.parent; } }

        public IMAPIFolder OpenFolder()
        {
            return this.parent.OpenFolder(this);
        }

        public bool Equals(IMAPIFolderID folder)
        {
            return Equals((IEntryID)folder);
        }
    }
}
