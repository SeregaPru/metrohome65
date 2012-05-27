using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    internal class MAPIMessage : MAPIProp, IMAPIMessage
    {
        private cemapi.IMessage msg;
        private IMAPIMessageID msgId;
        private string subject = null, body = null;
        private UInt64 deliveryTime = 0;
        private EMessageStatus status = 0;
        private EMessageFlags flags = 0;
        private bool flagsDone = false, statusDone = false, senderDone = false;
        private IMAPIContact sender = null;

        public MAPIMessage(cemapi.IMessage msg, IMAPIMessageID msgId) : base(msg, (msgId.ParentStore as MAPIMsgStore).Session) { this.msg = msg; this.msgId = msgId; }

        public override void Dispose()
        {
#warning do MAPIMessage dispose
        }

        public IMAPIMessageID MessageID { get { return this.msgId; } }

        #region Message Properties
        public void PopulateProperties(EMessageProperties properties)
        {
            List<cemapi.PropTags> tags = new List<cemapi.PropTags>();
            if ((properties & EMessageProperties.Subject) != 0)
                tags.Add(cemapi.PropTags.PR_SUBJECT);
            if ((properties & EMessageProperties.DeliveryTime) != 0)
                tags.Add(cemapi.PropTags.PR_MESSAGE_DELIVERY_TIME);
            if ((properties & EMessageProperties.Status) != 0)
                tags.Add(cemapi.PropTags.PR_MSG_STATUS);
            if ((properties & EMessageProperties.Body) != 0)
                tags.Add(cemapi.PropTags.PR_BODY);
            if ((properties & EMessageProperties.Sender) != 0)
                tags.AddRange(new cemapi.PropTags[] { cemapi.PropTags.PR_SENDER_EMAIL_ADDRESS, cemapi.PropTags.PR_SENDER_NAME });
            if ((properties & EMessageProperties.Flags) != 0)
                tags.Add(cemapi.PropTags.PR_MESSAGE_FLAGS);

            cemapi.IPropValue[] props = this.msg.GetProps(tags.ToArray());

            int len = props.Length;
            for (int i = 0; i < props.Length; i++)
            {
                switch (tags[i])
                {
                    case cemapi.PropTags.PR_SUBJECT:
                        this.subject = props[i].AsString;
                        break;
                    case cemapi.PropTags.PR_MESSAGE_DELIVERY_TIME:
                        if (props[i].Type != null)
                            this.deliveryTime = props[i].AsUInt64;
                        else
                            this.deliveryTime = 0xFFFFFFFFFFFFFFFF;
                        break;
                    case cemapi.PropTags.PR_MSG_STATUS:
                        this.status = (EMessageStatus)props[i].AsInt32;
                        this.statusDone = true;
                        break;
                    case cemapi.PropTags.PR_BODY:
                        this.body = props[i].AsString;
                        break;
                    case cemapi.PropTags.PR_SENDER_EMAIL_ADDRESS: // sender
                        if (props[i].Tag != cemapi.PropTags.PR_ERROR)
                        {
                            this.sender = new MAPIContact(props[i].AsString, props[i + 1].AsString, this.msgId.ParentFolder.Parent);
                            this.senderDone = true;
                        }
                        else
                        {
                            this.sender = null;
                            this.senderDone = false;
                        }
                        i++;
                        break;
                    case cemapi.PropTags.PR_MESSAGE_FLAGS:
                        this.flagsDone = true;
                        this.flags = (EMessageFlags)props[i].AsInt32;
                        break;
                }
            }
        }

        public void InvalidateProperties(EMessageProperties properties)
        {
            if ((properties & EMessageProperties.Body) != 0)
                this.body = null;
            if ((properties & EMessageProperties.Subject) != 0)
                this.subject = null;
            if ((properties & EMessageProperties.DeliveryTime) != 0)
                this.deliveryTime = 0;
            if ((properties & EMessageProperties.Status) != 0)
                this.statusDone = false;
            if ((properties & EMessageProperties.Sender) != 0)
                this.senderDone = false;
            if ((properties & EMessageProperties.Flags) != 0)
                this.flagsDone = false;
        }

        public string Body
        {
            get
            {
                if (this.body == null)
                    this.body = this.msg.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_BODY })[0].AsString;
                return this.body;
            }
        }

        public string Subject
        {
            get
            {
                if (this.subject == null)
                {
                    cemapi.IPropValue val = this.msg.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_SUBJECT })[0];
                    if (val.Tag != cemapi.PropTags.PR_SUBJECT)
                        this.subject = "";
                    else
                        this.subject = val.AsString;
                }
                return this.subject;
            }
        }

        public UInt64 SystemDeliveryTime
        {
            get
            {
                if (this.deliveryTime == 0)
                {
                    cemapi.IPropValue p = this.msg.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_MESSAGE_DELIVERY_TIME })[0];
                    if (p.Type != null)
                        this.deliveryTime = p.AsUInt64;
                    else
                        this.deliveryTime = 0xFFFFFFFFFFFFFFFF;
                }

                return this.deliveryTime;
            }
        }

        public DateTime LocalDeliveryTime 
        { 
            get 
            {
                if (this.SystemDeliveryTime == 0 || this.SystemDeliveryTime == 0xFFFFFFFFFFFFFFFF)
                    return new DateTime(0);
                return (new DateTime((long)this.SystemDeliveryTime, DateTimeKind.Utc)).ToLocalTime().AddYears(1600); 
            } 
        }

        public EMessageStatus Status
        {
            get
            {
                if (this.statusDone == false)
                {
                    this.status = (EMessageStatus)this.msg.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_MSG_STATUS })[0].AsInt32;
                    this.statusDone = true;
                }
                return this.status;
            }
        }

        public EMessageFlags Flags
        {
            get
            {
                if (this.flagsDone == false)
                {
                    this.flags = (EMessageFlags)this.msg.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_MESSAGE_FLAGS })[0].AsInt32;
                    this.flagsDone = true;
                }
                return this.flags;
            }
            set
            {
                this.flagsDone = this.msg.SetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_MESSAGE_FLAGS }, new object[] { ((uint)value) });
            }
        }

        public IMAPIContact Sender
        {
            get
            {
                if (!this.senderDone)
                {
                    cemapi.IPropValue[] props = this.msg.GetProps(new cemapi.PropTags[] { cemapi.PropTags.PR_SENDER_EMAIL_ADDRESS, cemapi.PropTags.PR_SENDER_NAME });
                    if (props[0].Tag != cemapi.PropTags.PR_ERROR)
                    {
                        this.sender = new MAPIContact(props[0].AsString, props[1].AsString, this.msgId.ParentFolder.Parent);
                        this.senderDone = true;
                    }
                    else
                    {
                        this.sender = null;
                        this.senderDone = false;
                    }
                }
                return this.sender;
            }
        }

        public IMAPIContact[] Recipients
        {
            get
            {
                cemapi.IMAPITable recipients = this.msg.GetRecipientTable();
                if (recipients == null)
                    return new IMAPIContact[0];
                // Fixed columns:
                // [0] - PR_ROWID
                // [1] - PR_DISPLAY_NAME
                // [2] - PR_EMAIL_ADDRESS
                // [3] - PR_ADDRTYPE
                // [4] - PR_RECIPIENT_TYPE

                IMAPIContact[] contacts = new IMAPIContact[recipients.GetRowCount()];
                int count, offset = 0;
                do
                {
                    cemapi.SRow[] rows = recipients.QueryRows(cemapi.MaxQueryRowCount);
                    count = rows.Length;
                    for (int i = 0; i < count; i++)
                        contacts[i + offset] = new MAPIContact(rows[i].propVals[2].AsString, rows[i].propVals[1].AsString, this.msgId.ParentFolder.Parent);
                    offset += count;
                }
                while (count >= cemapi.MaxQueryRowCount);

                return contacts;
            }
        }
        #endregion

        /*#region Operators
        public static bool operator ==(MAPIMessage a, MAPIMessage b)
        {
            if (a == null || b == null)
                return false;
            return a.EntryID == b.EntryID;
        }

        public static bool operator !=(MAPIMessage a, MAPIMessage b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return ((MAPIMessage)obj == this);
        }

        public override int GetHashCode()
        {
            return this.EntryID.GetHashCode();
        }
        #endregion*/
    }

    internal class MAPIMessageID : EntryID, IMAPIMessageID
    {
        private IMAPIFolderID parentFolder;
        private IMAPIMsgStore parentStore;
        public MAPIMessageID(byte[] id, IMAPIFolderID parentFolder, IMAPIMsgStore parentStore) : base(id, (parentStore as MAPIMsgStore).Session) { this.parentFolder = parentFolder; this.parentStore = parentStore; }
        private MAPIMessageID(IEntryID id, IMAPIFolderID parentFolder, IMAPIMsgStore parentStore) : base(id, (parentStore as MAPIMsgStore).Session) { this.parentFolder = parentFolder; this.parentStore = parentStore; }
        public static MAPIMessageID BuildFromID(IEntryID id, IMAPIFolderID parentFolder, IMAPIMsgStore parentStore)
        {
            if (id == null)
                return null;
            return new MAPIMessageID(id, parentFolder, parentStore);
        }

        public IMAPIFolderID ParentFolder { get { return this.parentFolder; } }
        public IMAPIMsgStore ParentStore { get { return this.parentStore; } }

        public IMAPIMessage OpenMessage()
        {
            return this.parentStore.OpenMessage(this);
        }

        public bool Equals(IMAPIMessageID folder)
        {
            return Equals((IEntryID)folder);
        }
    }
}
