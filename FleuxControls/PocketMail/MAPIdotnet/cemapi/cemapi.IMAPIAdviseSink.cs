using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.WindowsCE.Forms;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        internal class NEWMAIL_NOTIFICATION
        {
            public NEWMAIL_NOTIFICATION(IEntryID entryID, IEntryID parentID, EMessageFlags flags)
            { this.entryID = entryID; this.parentID = parentID; this.flags = flags; }
            public IEntryID entryID, parentID;
            public EMessageFlags flags;
        }
        internal class OBJECT_NOTIFICATION
        {
            public OBJECT_NOTIFICATION(IEntryID entryID, IEntryID parentID, IEntryID oldID, IEntryID oldParentID, ObjectType objType)
            { this.entryID = entryID; this.parentID = parentID; this.oldID = oldID; this.oldParentID = oldParentID; this.objType = objType; }
            public IEntryID entryID, parentID, oldID, oldParentID;
            public ObjectType objType;
        }

        public delegate void AdviseNewMailHandler(EEventMask eventMask, NEWMAIL_NOTIFICATION o);
        public delegate void AdviseObjectHandler(EEventMask eventMask, OBJECT_NOTIFICATION o);
        private delegate void OnAdviseCallbackHandler(IntPtr store, IntPtr lpNotifications);

        internal interface IMAPIAdviseSink : IMAPIUnknown, IDisposable
        {
            void Unadvise();

            event AdviseNewMailHandler NewMail;
            event AdviseObjectHandler ObjectNotification;
        }

        private class MAPIAdviseSink : MAPIUnknown, IMAPIAdviseSink
        {
            [DllImport("MAPIlib.dll")]
            private static extern HRESULT MAPIlibUnadvise(IntPtr adviser);

            [DllImport("MAPIlib.dll")]
            private static extern HRESULT MAPIlibAdvise(IntPtr storePtr, EEventMask eventMask, OnAdviseCallbackHandler callback, ref IntPtr adviser);

            private IMsgStore store;
            private IntPtr adviserPtr;
            private OnAdviseCallbackHandler callbackHandler;
            private IMAPISession owner;

            public MAPIAdviseSink(IMsgStore store, EEventMask eventMask, IMAPISession owner)
            {
                this.store = store;
                IntPtr ptr = IntPtr.Zero;
                this.callbackHandler = OnNotifyCallback;
                HRESULT hr = MAPIlibAdvise(store.Ptr, eventMask, this.callbackHandler, ref ptr);
                if (hr != HRESULT.S_OK)
                    throw new Exception("MAPIlibAdvise failed: " + hr.ToString());
                this.adviserPtr = ptr;
                this.owner = owner;
            }

            public void Unadvise()
            {
                if (this.adviserPtr == IntPtr.Zero)
                    return;
                HRESULT hr = MAPIlibUnadvise(this.adviserPtr);
                if (hr != HRESULT.S_OK)
                    throw new Exception("MAPIlibUnadvise failed: " + hr.ToString());
                this.adviserPtr = IntPtr.Zero;
            }

            public override void Dispose()
            {
                Unadvise();
                this.NewMail = null;
                this.ObjectNotification = null;
            }

            private void OnNotifyCallback(IntPtr store, IntPtr lpNotifications)
            {
                EEventMask eventType = (EEventMask)Marshal.ReadInt32(lpNotifications);
                uint sPtr = ((uint)(lpNotifications)) + 8;
                switch (eventType)
                {
                    case EEventMask.fnevNewMail:
                        {
                            if (this.NewMail == null)
                                break;
                            NEWMAIL_NOTIFICATION n = new NEWMAIL_NOTIFICATION(
                                EntryID.BuildFromPtr((IntPtr)sPtr, this.owner),
                                EntryID.BuildFromPtr((IntPtr)(sPtr + 4 * 2), this.owner),
                                (EMessageFlags)Marshal.ReadInt32((IntPtr)(sPtr + 4 * 4)));
                            this.NewMail(eventType, n);
                        }
                        break;
                    case EEventMask.fnevObjectCopied:
                    case EEventMask.fnevObjectCreated:
                    case EEventMask.fnevObjectDeleted:
                    case EEventMask.fnevObjectModified:
                    case EEventMask.fnevObjectMoved:
                    case EEventMask.fnevSearchComplete:
                        {
                            if (this.ObjectNotification == null)
                                break;
                            OBJECT_NOTIFICATION o = new OBJECT_NOTIFICATION(
                                EntryID.BuildFromPtr((IntPtr)sPtr, this.owner),
                                EntryID.BuildFromPtr((IntPtr)(sPtr + 4 * 3), this.owner),
                                EntryID.BuildFromPtr((IntPtr)(sPtr + 4 * 5), this.owner),
                                EntryID.BuildFromPtr((IntPtr)(sPtr + 4 * 7), this.owner),
                                (ObjectType)Marshal.ReadInt32((IntPtr)(sPtr + 4 * 2)));
                            this.ObjectNotification(eventType, o);
                        }
                        break;
                }

            }

            /*private class MessageCatcher : MessageWindow
            {
                public const int WM_CUSTOMMSG = 0x0400;

                private uint wParam;
                private MAPIAdviseSink parent;

                public MessageCatcher(MAPIAdviseSink parent, uint wParam)
                { this.wParam = wParam; this.parent = parent; }

                protected override void WndProc(ref Microsoft.WindowsCE.Forms.Message m)
                {
                    base.WndProc(ref m);
                    if (m.Msg == WM_CUSTOMMSG && ((uint)m.WParam) == this.wParam)
                    {
                        EEventMask mask = (EEventMask)Marshal.ReadInt32(m.LParam);
                        uint intSize = (uint)Marshal.SizeOf(typeof(int));
                        uint sPtr = ((uint)(m.LParam)) + intSize;
                        switch (mask)
                        {
                            case EEventMask.fnevNewMail:
                                {
                                    if (this.parent.NewMail == null)
                                        break;
                                    NEWMAIL_NOTIFICATION n = new NEWMAIL_NOTIFICATION();
                                    n.entryID = new EntryID((IntPtr)sPtr);
                                    n.parentID = new EntryID((IntPtr)(sPtr + intSize * 2));
                                    n.flags = (EMessageFlags)Marshal.ReadInt32((IntPtr)(sPtr + intSize * 4));

                                    cemapi.IMAPIFolder parentFolder = this.parent.store.OpenEntryAsFolder(n.parentID.AsByteArray);
                                    cemapi.IMessage newMsg = parentFolder.OpenEntryAsMessage(n.entryID.AsByteArray);

                                    this.parent.NewMail(mask, newMsg, parentFolder);
                                }
                                break;
                            case EEventMask.fnevObjectCopied:
                            case EEventMask.fnevObjectCreated:
                            case EEventMask.fnevObjectDeleted:
                            case EEventMask.fnevObjectModified:
                            case EEventMask.fnevObjectMoved:
                            case EEventMask.fnevSearchComplete:
                                {
                                    OBJECT_NOTIFICATION o = new OBJECT_NOTIFICATION();
                                    o.entryID = new EntryID((IntPtr)sPtr);
                                    o.objType = (ObjectType)Marshal.ReadInt32((IntPtr)(sPtr + intSize * 2));
                                    o.parentID = new EntryID((IntPtr)(sPtr + intSize * 3));
                                    o.oldID = new EntryID((IntPtr)(sPtr + intSize * 5));
                                    o.oldParentID = new EntryID((IntPtr)(sPtr + intSize * 7));
                                    if (this.parent.ObjectNotification != null)
                                        this.parent.ObjectNotification(mask, o);
                                }
                                break;
                        }
                    }
                }
            }*/

            public event AdviseNewMailHandler NewMail;
            public event AdviseObjectHandler ObjectNotification;
        }
    }
}
