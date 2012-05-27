using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MAPIdotnet
{
    internal class MAPIMsgStore : MAPIProp, IMAPIMsgStore
    {
        private cemapi.IMsgStore msgStore;
        private cemapi.IMAPIAdviseSink adviser;
        private MAPI parent;
        private EEventMask advisingOn = 0;
        private cemapi.IMAPISession session;

        public MAPIMsgStore(MAPI parent, cemapi.IMsgStore msgStore, cemapi.IMAPISession session) : base(msgStore, session)
        { this.msgStore = msgStore; this.parent = parent; this.session = session; }

        public override void Dispose()
        {
            this.advisingOn = 0;
            UpdateAdvisors();
        }

        #region Methods
        public IMAPIMsgStoreID StoreID { get { return new MAPIMsgStoreID(this.msgStore.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_ENTRYID })[0].AsBinary, this.parent, this.session); } }
            //return new MAPIFolderID(this.folder.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_PARENT_ENTRYID })[0].AsBinary, this.parentStore);

        public cemapi.IMAPISession Session { get { return this.session; } }

        public MAPI Parent { get { return this.parent; } }

        public void DisplayComposeDialog(string recipients) { MessagingApplication.DisplayComposeForm(this.DisplayName, recipients); }
        public void DisplayComposeDialog() { MessagingApplication.DisplayComposeForm(this.DisplayName); }
        public void DisplayComposeDialog(IMAPIContact[] recipients)
        {
            StringBuilder recips = new StringBuilder();
            int len = recipients.Length;
            for (int i = 0; i < (len - 1); ++i)
            {
                recips.Append(recipients[i].FullAddress);
                recips.Append("; ");
            }
            if (len > 0)
                recips.Append(recipients[len - 1].FullAddress);
            MessagingApplication.DisplayComposeForm(this.DisplayName, recips.ToString());
        }

        public override string ToString() { return this.DisplayName; }

        private IMAPIFolderID GetFolder(cemapi.PropTags tag)
        {
            cemapi.IPropValue[] props = this.msgStore.GetProps(new cemapi.PropTags[] { tag });
            if (props.Length != 1)
                throw new Exception("Couldn't get folder prop!");
            return new MAPIFolderID(props[0].AsBinary, this);
        }

        public IMAPIFolder OpenFolder(IMAPIFolderID id)
        {
            return new MAPIFolder(this, this.msgStore.OpenEntryAsFolder((id as MAPIFolderID).AsByteArray), id);
        }
        public IMAPIMessage OpenMessage(IMAPIMessageID id)
        {
            return new MAPIMessage(this.msgStore.OpenEntryAsMessage((id as MAPIMessageID).AsByteArray), id);
        }

        public IMAPIFolderID RootFolder { get { return GetFolder(cemapi.PropTags.PR_IPM_SUBTREE_ENTRYID); } }

        public IMAPIFolderID SentMailFolder { get { return GetFolder(cemapi.PropTags.PR_IPM_SENTMAIL_ENTRYID); } }

        public IMAPIFolderID OutboxFolder { get { return GetFolder(cemapi.PropTags.PR_IPM_OUTBOX_ENTRYID); } }

        public IMAPIFolderID TrashFolder { get { return GetFolder(cemapi.PropTags.PR_IPM_WASTEBASKET_ENTRYID); } }

        public IMAPIFolderID ReceiveFolder { get { return new MAPIFolderID(this.msgStore.GetReceiveFolder(), this); } }
        #endregion

        public EEventMask EventNotifyMask
        {
            get { return this.advisingOn; }
            set
            {
                this.advisingOn = value;
                UpdateAdvisors();
            }
        }
        private void UpdateAdvisors()
        {
            if (this.adviser != null)
                this.adviser.Dispose();
            EEventMask mask = this.advisingOn;
            //if ((this.advisingOn & EEventMask.fnevNewMail) != 0) // Doesn't do anything if you don't have object created!
            //    mask |= EEventMask.fnevObjectCreated;
            this.adviser = this.msgStore.Advise(mask);
            if ((this.advisingOn & EEventMask.fnevNewMail) != 0)
                this.adviser.NewMail += new cemapi.AdviseNewMailHandler(adviser_NewMail);
            this.adviser.ObjectNotification += new cemapi.AdviseObjectHandler(adviser_ObjectNotification);
        }

        private void adviser_NewMail(EEventMask mask, cemapi.NEWMAIL_NOTIFICATION o)
        {
            if (this.NewMessage == null)
                return;
            this.NewMessage(MAPIMessageID.BuildFromID(o.entryID, MAPIFolderID.BuildFromID(o.parentID, this), this), o.flags);
        }

        private void adviser_ObjectNotification(EEventMask mask, cemapi.OBJECT_NOTIFICATION o)
        {
            switch (o.objType)
            {
                case cemapi.ObjectType.MAPI_FOLDER:
                    if (this.FolderEvent != null)
                    {
                        this.FolderEvent(MAPIFolderID.BuildFromID(o.entryID, this),
                            MAPIFolderID.BuildFromID(o.oldID, this),
                            mask);
                    }
                    break;
                case cemapi.ObjectType.MAPI_MESSAGE:
                    if (this.MessageEvent != null)
                    {
                        this.MessageEvent(MAPIMessageID.BuildFromID(o.entryID, MAPIFolderID.BuildFromID(o.parentID, this), this),
                            MAPIMessageID.BuildFromID(o.oldID, MAPIFolderID.BuildFromID(o.oldParentID, this), this),
                            mask);
                    }
                    break;
                default:
                    throw new Exception("Event type \"" + o.objType.ToString() + "\" not implemented yet");
                    //break;
            }
        }

        public event NewMessageEventHandler NewMessage;
        public event MessageEventHandler MessageEvent;
        public event FolderEventHandler FolderEvent;
    }

    internal class MAPIMsgStoreID : EntryID, IMAPIMsgStoreID
    {
        private MAPI parent;
        private cemapi.IMAPISession session;
        public MAPIMsgStoreID(byte[] id, MAPI parent, cemapi.IMAPISession session) : base(id, session) { this.parent = parent; this.session = session; }
        private MAPIMsgStoreID(IEntryID id, MAPI parent, cemapi.IMAPISession session) : base(id, session) { this.parent = parent; this.session = session; }
        public static MAPIMsgStoreID BuildFromID(IEntryID id, MAPI parent, cemapi.IMAPISession session)
        {
            if (id == null)
                return null;
            return new MAPIMsgStoreID(id, parent, session);
        }

        public IMAPIMsgStore OpenMsgStore() { return new MAPIMsgStore(this.parent, this.session.OpenMsgStore(this.AsByteArray), this.session); }

        public MAPI ParentSession { get { return this.parent; } }

        public bool Equals(IMAPIMsgStoreID id) { return this.Equals((IEntryID)id);}
    }
}
