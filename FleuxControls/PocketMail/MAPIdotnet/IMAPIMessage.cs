using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    /// <summary>
    /// ORable properties to populate in IMAPIMessage
    /// </summary>
    public enum EMessageProperties
    {
        Body = 0x01,
        Subject = 0x02,
        DeliveryTime = 0x04,
        Sender = 0x08,
        Status = 0x10,
        Flags = 0x20,
    }

    public enum EMessageStatus : uint
    {
        MSGSTATUS_RECTYPE_SMTP                  = 0x00020000,
        MSGSTATUS_RECTYPE_SMS                   = 0x00040000,

// Re-use the above flags for incoming SMS status messages
// which are mutually exclusive from outgoing messages.
        MSGSTATUS_SMSSTATUS_SUCCESS             = 0x00020000,

// To determine if any transport still needs to send the message...
        MSGSTATUS_RECTYPE_ALLTYPES              = (MSGSTATUS_RECTYPE_SMTP | MSGSTATUS_RECTYPE_SMS),

// This flag should be set if only the header has been downloaded.
        MSGSTATUS_HEADERONLY                    = 0x00010000,

// This flag is set if the item should be partially downloaded.
        MSGSTATUS_PARTIAL_DOWNLOAD              = 0x00080000,

// This flag is set if the item is only partially downloaded.
        MSGSTATUS_PARTIAL                       = 0x00100000,

// This flag is set if the message has at least one attachment marked for downloaded.
        MSGSTATUS_REMOTE_DOWNLOAD_ATTACH        = 0x00200000,

// This flag is set if the MIME header needs to be downloaded.
        MSGSTATUS_REMOTE_DOWNLOAD_HEADER        = 0x00400000,

// This flag is set if the message has a TNEF blob attached.
        MSGSTATUS_HAS_TNEF                      = 0x00800000,

// This flag is set if the TNEF of a message needs to be downloaded.
        MSGSTATUS_REMOTE_DOWNLOAD_TNEF          = 0x01000000,

// This flag is set if the attachment has a copy on the server.
        MSGSTATUS_GHOSTED_ATTACH                = 0x02000000,

// This flag is set if there are attachments on the message that
// have not yet been downloaded.
        MSGSTATUS_PENDING_ATTACHMENTS           = 0x04000000,

// This flag is set if the message has PR_CE_MIME_TEXT property
        MSGSTATUS_HAS_PR_CE_MIME_TEXT           = 0x08000000,

// This flag is set if the message has PR_BODY property
        MSGSTATUS_HAS_PR_BODY                   = 0x10000000,

// This flag is set if the message has PR_CE_SMIME_TEXT property
        MSGSTATUS_HAS_PR_CE_SMIME_TEXT          = 0x20000000,

// This flag is set if the message has PR_CE_CRYPT_MIME_TEXT property
        MSGSTATUS_HAS_PR_CE_CRYPT_MIME_TEXT     = 0x40000000,
    }

    public enum EMessageFlags : uint
    {
        MSGFLAG_READ        = 0x00000001,
        MSGFLAG_UNMODIFIED  = 0x00000002,
        MSGFLAG_SUBMIT      = 0x00000004,
        MSGFLAG_UNSENT      = 0x00000008,
        MSGFLAG_HASATTACH   = 0x00000010,
        MSGFLAG_FROMME      = 0x00000020,
        MSGFLAG_ASSOCIATED  = 0x00000040,
        MSGFLAG_RESEND      = 0x00000080,
        MSGFLAG_RN_PENDING  = 0x00000100,
        MSGFLAG_NRN_PENDING = 0x00000200,
    };

    public interface IMAPIMessage : IMAPIProp
    {
        IMAPIMessageID MessageID { get; }

        void PopulateProperties(EMessageProperties properties);
        void InvalidateProperties(EMessageProperties properties);

        string Body { get; }
        string Subject { get; }
        UInt64 SystemDeliveryTime { get; }
        DateTime LocalDeliveryTime { get; }
        EMessageStatus Status { get; }
        EMessageFlags Flags { get; set; }
        IMAPIContact Sender { get; }
        IMAPIContact[] Recipients { get; }
    }
}
