using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    public enum EEventMask : uint
    {
        fnevCriticalError = 0x00000001,
        fnevNewMail = 0x00000002,
        fnevObjectCreated = 0x00000004,
        fnevObjectDeleted = 0x00000008,
        fnevObjectModified = 0x00000010,
        fnevObjectMoved = 0x00000020,
        fnevObjectCopied = 0x00000040,
        fnevSearchComplete = 0x00000080,
        fnevTableModified = 0x00000100,
        fnevStatusObjectModified = 0x00000200,
        fnevReservedForMapi = 0x40000000,
        fnevExtended = 0x80000000,
    };

    public delegate void NewMessageEventHandler(IMAPIMessageID newMessageID, EMessageFlags flags);
    public delegate void MessageEventHandler(IMAPIMessageID newMessageID, IMAPIMessageID oldMessageID, EEventMask messageFlags);
    public delegate void FolderEventHandler(IMAPIFolderID newFolderID, IMAPIFolderID oldFolderID, EEventMask flags);

    public interface IMAPIMsgStore : IMAPIProp
    {
        IMAPIMsgStoreID StoreID { get; }
        MAPI Parent { get; }

        IMAPIFolder OpenFolder(IMAPIFolderID id);
        IMAPIMessage OpenMessage(IMAPIMessageID id);

        IMAPIFolderID RootFolder { get; }
        IMAPIFolderID ReceiveFolder { get; }
        IMAPIFolderID SentMailFolder { get; }
        IMAPIFolderID OutboxFolder { get; }
        IMAPIFolderID TrashFolder { get; }

        void DisplayComposeDialog(string recipients);
        void DisplayComposeDialog();
        void DisplayComposeDialog(IMAPIContact[] recipients);

        EEventMask EventNotifyMask { get; set; }
        event NewMessageEventHandler NewMessage;
        event MessageEventHandler MessageEvent;
        event FolderEventHandler FolderEvent;
    }
}
