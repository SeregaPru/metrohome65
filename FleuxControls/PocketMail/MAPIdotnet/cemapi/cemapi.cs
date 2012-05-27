using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    /// <summary>
    /// Classes and functions to wrap Native CE MAPI types.
    /// </summary>
    internal static partial class cemapi
    {
        // MAKE_MAPI_E(x) --> 0x80040000 | x
        private enum HRESULT : uint
        {
            S_FALSE = 0x00000001,
            S_OK = 0x00000000,

            E_NOTIMPL = 0x80004001,

            MAPI_E_CALL_FAILED = 0x80004005,
            MAPI_E_NOT_ENOUGH_MEMORY = 0x8007000E,
            MAPI_E_INVALID_PARAMETER = 0x80000003,
            MAPI_E_INTERFACE_NOT_SUPPORTED = 0x80000004,
            MAPI_E_NO_ACCESS = 0x80000009,

            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x80000002,
            E_UNEXPECTED = 0x8000FFFF,
            E_FAIL = 0x80000008,

            MAPI_E_NO_SUPPORT               = 0x80040000 | 0x102,
            MAPI_E_BAD_CHARWIDTH            = 0x80040000 | 0x103,
            MAPI_E_STRING_TOO_LONG          = 0x80040000 | 0x105,
            MAPI_E_UNKNOWN_FLAGS            = 0x80040000 | 0x106,
            MAPI_E_INVALID_ENTRYID          = 0x80040000 | 0x107,
            MAPI_E_INVALID_OBJECT           = 0x80040000 | 0x108,
            MAPI_E_OBJECT_CHANGED           = 0x80040000 | 0x109,
            MAPI_E_OBJECT_DELETED           = 0x80040000 | 0x10A,
            MAPI_E_BUSY                     = 0x80040000 | 0x10B,
            MAPI_E_NOT_ENOUGH_DISK          = 0x80040000 | 0x10D,
            MAPI_E_NOT_ENOUGH_RESOURCES     = 0x80040000 | 0x10E,
            MAPI_E_NOT_FOUND                = 0x80040000 | 0x10F,
            MAPI_E_VERSION                  = 0x80040000 | 0x110,
            MAPI_E_LOGON_FAILED             = 0x80040000 | 0x111,
            MAPI_E_SESSION_LIMIT            = 0x80040000 | 0x112,
            MAPI_E_USER_CANCEL              = 0x80040000 | 0x113,
            MAPI_E_UNABLE_TO_ABORT          = 0x80040000 | 0x114,
            MAPI_E_NETWORK_ERROR            = 0x80040000 | 0x115,
            MAPI_E_DISK_ERROR               = 0x80040000 | 0x116,
            MAPI_E_TOO_COMPLEX              = 0x80040000 | 0x117,
            MAPI_E_BAD_COLUMN               = 0x80040000 | 0x118,
            MAPI_E_EXTENDED_ERROR           = 0x80040000 | 0x119,
            MAPI_E_COMPUTED                 = 0x80040000 | 0x11A,
            MAPI_E_CORRUPT_DATA             = 0x80040000 | 0x11B,
            MAPI_E_UNCONFIGURED             = 0x80040000 | 0x11C,
            MAPI_E_FAILONEPROVIDER          = 0x80040000 | 0x11D,
            MAPI_E_UNKNOWN_CPID             = 0x80040000 | 0x11E,
            MAPI_E_UNKNOWN_LCID             = 0x80040000 | 0x11F,

            MAPI_E_CORRUPT_STORE            = 0x80040000 | 0x600,
            MAPI_E_NOT_IN_QUEUE             = 0x80040000 | 0x601,
            MAPI_E_NO_SUPPRESS              = 0x80040000 | 0x602,
            MAPI_E_COLLISION                = 0x80040000 | 0x604,
            MAPI_E_NOT_INITIALIZED          = 0x80040000 | 0x605,
            MAPI_E_NON_STANDARD             = 0x80040000 | 0x606,
            MAPI_E_NO_RECIPIENTS            = 0x80040000 | 0x607,
            MAPI_E_SUBMITTED                = 0x80040000 | 0x608,
            MAPI_E_HAS_FOLDERS              = 0x80040000 | 0x609,
            MAPI_E_HAS_MESSAGES             = 0x80040000 | 0x60A,
            MAPI_E_FOLDER_CYCLE             = 0x80040000 | 0x60B,
        };

        private enum PT : uint
        {
            PT_UNSPECIFIED = 0,    /* (Reserved for interface use) type doesn't matter to caller */
            PT_NULL = 1,    /* NULL property value */
            PT_I2 = 2,    /* Signed 16-bit value */
            PT_LONG = 3,    /* Signed 32-bit value */
            PT_R4 = 4,    /* 4-byte floating point */
            PT_DOUBLE = 5,    /* Floating point double */
            PT_CURRENCY = 6,    /* Signed 64-bit int (decimal w/    4 digits right of decimal pt) */
            PT_APPTIME = 7,    /* Application time */
            PT_ERROR = 10,    /* 32-bit error value */
            PT_BOOLEAN = 11,    /* 16-bit boolean (non-zero true, */
            // Use PT_BOOLEAN_DESKTOP to be specific instead of using PT_BOOLEAN which is mapped to 2 in addrmapi.h
            PT_BOOLEAN_DESKTOP = 11,    /* 16-bit boolean (non-zero true) */
            PT_OBJECT = 13,    /* Embedded object in a property */
            PT_I8 = 20,    /* 8-byte signed integer */
            PT_STRING8 = 30,    /* Null terminated 8-bit character string */
            PT_UNICODE = 31,    /* Null terminated Unicode string */
            PT_SYSTIME = 64,    /* FILETIME 64-bit int w/ number of 100ns periods since Jan 1,1601 */
            PT_CLSID = 72,    /* OLE GUID */
            PT_BINARY = 258,   /* Uninterpreted (counted byte array) */

            PT_TSTRING = PT_UNICODE
        };

        public enum PropTags : uint
        {
            PR_ERROR = 10,

            // Common non-transmittable
            PR_ENTRYID = PT.PT_BINARY | 0x0FFF << 16,
            PR_OBJECT_TYPE = PT.PT_LONG | 0x0FFE << 16,
            PR_ICON = PT.PT_BINARY | 0x0FFD << 16,
            PR_MINI_ICON = PT.PT_BINARY | 0x0FFC << 16,
            PR_STORE_ENTRYID = PT.PT_BINARY | 0x0FFB << 16,
            PR_STORE_RECORD_KEY = PT.PT_BINARY | 0x0FFA << 16,
            PR_RECORD_KEY = PT.PT_BINARY | 0x0FF9 << 16,
            PR_MAPPING_SIGNATURE = PT.PT_BINARY | 0x0FF8 << 16,
            PR_ACCESS_LEVEL = PT.PT_LONG | 0x0FF7 << 16,
            PR_INSTANCE_KEY = PT.PT_BINARY | 0x0FF6 << 16,
            PR_ROW_TYPE = PT.PT_LONG | 0x0FF5 << 16,
            PR_ACCESS = PT.PT_LONG | 0x0FF4 << 16,

            // Common transmittable
            PR_ROWID = PT.PT_LONG | 0x3000 << 16,
            PR_DISPLAY_NAME = PT.PT_TSTRING | 0x3001 << 16,
            PR_DISPLAY_NAME_W = PT.PT_UNICODE | 0x3001 << 16,
            PR_DISPLAY_NAME_A = PT.PT_STRING8 | 0x3001 << 16,
            PR_ADDRTYPE = PT.PT_TSTRING | 0x3002 << 16,
            PR_ADDRTYPE_W = PT.PT_UNICODE | 0x3002 << 16,
            PR_ADDRTYPE_A = PT.PT_STRING8 | 0x3002 << 16,
            PR_EMAIL_ADDRESS = PT.PT_TSTRING | 0x3003 << 16,
            PR_EMAIL_ADDRESS_W = PT.PT_UNICODE | 0x3003 << 16,
            PR_EMAIL_ADDRESS_A = PT.PT_STRING8 | 0x3003 << 16,
            PR_COMMENT = PT.PT_TSTRING | 0x3004 << 16,
            PR_COMMENT_W = PT.PT_UNICODE | 0x3004 << 16,
            PR_COMMENT_A = PT.PT_STRING8 | 0x3004 << 16,
            PR_DEPTH = PT.PT_LONG | 0x3005 << 16,
            PR_PROVIDER_DISPLAY = PT.PT_TSTRING | 0x3006 << 16,
            PR_PROVIDER_DISPLAY_W = PT.PT_UNICODE | 0x3006 << 16,
            PR_PROVIDER_DISPLAY_A = PT.PT_STRING8 | 0x3006 << 16,
            PR_CREATION_TIME = PT.PT_SYSTIME | 0x3007 << 16,
            PR_LAST_MODIFICATION_TIME = PT.PT_SYSTIME | 0x3008 << 16,
            PR_RESOURCE_FLAGS = PT.PT_LONG | 0x3009 << 16,
            PR_PROVIDER_DLL_NAME = PT.PT_TSTRING | 0x300A << 16,
            PR_PROVIDER_DLL_NAME_W = PT.PT_UNICODE | 0x300A << 16,
            PR_PROVIDER_DLL_NAME_A = PT.PT_STRING8 | 0x300A << 16,
            PR_SEARCH_KEY = PT.PT_BINARY | 0x300B << 16,
            PR_PROVIDER_UID = PT.PT_BINARY | 0x300C << 16,
            PR_PROVIDER_ORDINAL = PT.PT_LONG | 0x300D << 16,

            // Message store specific
            PR_DEFAULT_STORE = PT.PT_BOOLEAN | 0x3400 << 16,
            PR_STORE_SUPPORT_MASK = PT.PT_LONG | 0x340D << 16,
            PR_STORE_STATE = PT.PT_LONG | 0x340E << 16,

            PR_IPM_SUBTREE_SEARCH_KEY = PT.PT_BINARY | 0x3410 << 16,
            PR_IPM_OUTBOX_SEARCH_KEY = PT.PT_BINARY | 0x3411 << 16,
            PR_IPM_WASTEBASKET_SEARCH_KEY = PT.PT_BINARY | 0x3412 << 16,
            PR_IPM_SENTMAIL_SEARCH_KEY = PT.PT_BINARY | 0x3413 << 16,
            PR_MDB_PROVIDER = PT.PT_BINARY | 0x3414 << 16,
            PR_RECEIVE_FOLDER_SETTINGS = PT.PT_OBJECT | 0x3415 << 16,

            PR_VALID_FOLDER_MASK = PT.PT_LONG | 0x35DF << 16,
            PR_IPM_SUBTREE_ENTRYID = PT.PT_BINARY | 0x35E0 << 16,

            PR_IPM_OUTBOX_ENTRYID = PT.PT_BINARY | 0x35E2 << 16,
            PR_IPM_WASTEBASKET_ENTRYID = PT.PT_BINARY | 0x35E3 << 16,
            PR_IPM_SENTMAIL_ENTRYID = PT.PT_BINARY | 0x35E4 << 16,
            PR_VIEWS_ENTRYID = PT.PT_BINARY | 0x35E5 << 16,
            PR_COMMON_VIEWS_ENTRYID = PT.PT_BINARY | 0x35E6 << 16,
            PR_FINDER_ENTRYID = PT.PT_BINARY | 0x35E7 << 16,

            // Message non-transmittable properties
            PR_CURRENT_VERSION                          = PT.PT_I8 |      0x0E00 << 16,
            PR_DELETE_AFTER_SUBMIT                      = PT.PT_BOOLEAN | 0x0E01 << 16,
            PR_DISPLAY_BCC                              = PT.PT_TSTRING | 0x0E02 << 16,
            PR_DISPLAY_BCC_W                            = PT.PT_UNICODE | 0x0E02 << 16,
            PR_DISPLAY_BCC_A                            = PT.PT_STRING8 | 0x0E02 << 16,
            PR_DISPLAY_CC                               = PT.PT_TSTRING | 0x0E03 << 16,
            PR_DISPLAY_CC_W                             = PT.PT_UNICODE | 0x0E03 << 16,
            PR_DISPLAY_CC_A                             = PT.PT_STRING8 | 0x0E03 << 16,
            PR_DISPLAY_TO                               = PT.PT_TSTRING | 0x0E04 << 16,
            PR_DISPLAY_TO_W                             = PT.PT_UNICODE | 0x0E04 << 16,
            PR_DISPLAY_TO_A                             = PT.PT_STRING8 | 0x0E04 << 16,
            PR_PARENT_DISPLAY                           = PT.PT_TSTRING | 0x0E05 << 16,
            PR_PARENT_DISPLAY_W                         = PT.PT_UNICODE | 0x0E05 << 16,
            PR_PARENT_DISPLAY_A                         = PT.PT_STRING8 | 0x0E05 << 16,
            PR_MESSAGE_DELIVERY_TIME                    = PT.PT_SYSTIME | 0x0E06 << 16,
            PR_MESSAGE_FLAGS                            = PT.PT_LONG |    0x0E07 << 16,
            PR_MESSAGE_SIZE                             = PT.PT_LONG |    0x0E08 << 16,
            PR_PARENT_ENTRYID                           = PT.PT_BINARY |  0x0E09 << 16,
            PR_SENTMAIL_ENTRYID                         = PT.PT_BINARY |  0x0E0A << 16,
            PR_CORRELATE                                = PT.PT_BOOLEAN | 0x0E0C << 16,
            PR_CORRELATE_MTSID                          = PT.PT_BINARY |  0x0E0D << 16,
            PR_DISCRETE_VALUES                          = PT.PT_BOOLEAN | 0x0E0E << 16,
            PR_RESPONSIBILITY                           = PT.PT_BOOLEAN | 0x0E0F << 16,
            PR_SPOOLER_STATUS                           = PT.PT_LONG |    0x0E10 << 16,
            PR_TRANSPORT_STATUS                         = PT.PT_LONG |    0x0E11 << 16,
            PR_MESSAGE_RECIPIENTS                       = PT.PT_OBJECT |  0x0E12 << 16,
            PR_MESSAGE_ATTACHMENTS                      = PT.PT_OBJECT |  0x0E13 << 16,
            PR_SUBMIT_FLAGS                             = PT.PT_LONG |    0x0E14 << 16,
            PR_RECIPIENT_STATUS                         = PT.PT_LONG |    0x0E15 << 16,
            PR_TRANSPORT_KEY                            = PT.PT_LONG |    0x0E16 << 16,
            PR_MSG_STATUS                               = PT.PT_LONG |    0x0E17 << 16,
            PR_MESSAGE_DOWNLOAD_TIME                    = PT.PT_LONG |    0x0E18 << 16,
            PR_CREATION_VERSION                         = PT.PT_I8 |      0x0E19 << 16,
            PR_MODIFY_VERSION                           = PT.PT_I8 |      0x0E1A << 16,
            PR_HASATTACH                                = PT.PT_BOOLEAN | 0x0E1B << 16,
            PR_BODY_CRC                                 = PT.PT_LONG |    0x0E1C << 16,
            PR_NORMALIZED_SUBJECT                       = PT.PT_TSTRING | 0x0E1D << 16,
            PR_NORMALIZED_SUBJECT_W                     = PT.PT_UNICODE | 0x0E1D << 16,
            PR_NORMALIZED_SUBJECT_A                     = PT.PT_STRING8 | 0x0E1D << 16,
            PR_RTF_IN_SYNC                              = PT.PT_BOOLEAN | 0x0E1F << 16,
            PR_ATTACH_SIZE                              = PT.PT_LONG |    0x0E20 << 16,
            PR_ATTACH_NUM                               = PT.PT_LONG |    0x0E21 << 16,
            PR_PREPROCESS                               = PT.PT_BOOLEAN | 0x0E22 << 16,

            // Message recipient properties
            PR_CONTENT_INTEGRITY_CHECK                  = PT.PT_BINARY |    0x0C00 << 16,
            PR_EXPLICIT_CONVERSION                      = PT.PT_LONG |      0x0C01 << 16,
            PR_IPM_RETURN_REQUESTED                     = PT.PT_BOOLEAN |   0x0C02 << 16,
            PR_MESSAGE_TOKEN                            = PT.PT_BINARY |    0x0C03 << 16,
            PR_NDR_REASON_CODE                          = PT.PT_LONG |      0x0C04 << 16,
            PR_NDR_DIAG_CODE                            = PT.PT_LONG |      0x0C05 << 16,
            PR_NON_RECEIPT_NOTIFICATION_REQUESTED       = PT.PT_BOOLEAN |   0x0C06 << 16,
            PR_DELIVERY_POINT                           = PT.PT_LONG |      0x0C07 << 16,

            PR_ORIGINATOR_NON_DELIVERY_REPORT_REQUESTED = PT.PT_BOOLEAN |   0x0C08 << 16,
            PR_ORIGINATOR_REQUESTED_ALTERNATE_RECIPIENT = PT.PT_BINARY |    0x0C09 << 16,
            PR_PHYSICAL_DELIVERY_BUREAU_FAX_DELIVERY    = PT.PT_BOOLEAN |   0x0C0A << 16,
            PR_PHYSICAL_DELIVERY_MODE                   = PT.PT_LONG |      0x0C0B << 16,
            PR_PHYSICAL_DELIVERY_REPORT_REQUEST         = PT.PT_LONG |      0x0C0C << 16,
            PR_PHYSICAL_FORWARDING_ADDRESS              = PT.PT_BINARY |    0x0C0D << 16,
            PR_PHYSICAL_FORWARDING_ADDRESS_REQUESTED    = PT.PT_BOOLEAN |   0x0C0E << 16,
            PR_PHYSICAL_FORWARDING_PROHIBITED           = PT.PT_BOOLEAN |   0x0C0F << 16,
            PR_PHYSICAL_RENDITION_ATTRIBUTES            = PT.PT_BINARY |    0x0C10 << 16,
            PR_PROOF_OF_DELIVERY                        = PT.PT_BINARY |    0x0C11 << 16,
            PR_PROOF_OF_DELIVERY_REQUESTED              = PT.PT_BOOLEAN |   0x0C12 << 16,
            PR_RECIPIENT_CERTIFICATE                    = PT.PT_BINARY |    0x0C13 << 16,
            PR_RECIPIENT_NUMBER_FOR_ADVICE              = PT.PT_TSTRING |   0x0C14 << 16,
            PR_RECIPIENT_NUMBER_FOR_ADVICE_W            = PT.PT_UNICODE |   0x0C14 << 16,
            PR_RECIPIENT_NUMBER_FOR_ADVICE_A            = PT.PT_STRING8 |   0x0C14 << 16,
            PR_RECIPIENT_TYPE                           = PT.PT_LONG |      0x0C15 << 16,
            PR_REGISTERED_MAIL_TYPE                     = PT.PT_LONG |      0x0C16 << 16,
            PR_REPLY_REQUESTED                          = PT.PT_BOOLEAN |   0x0C17 << 16,
            //PR_REQUESTED_DELIVERY_METHOD                = PT.PT_LONG |      0x0C18 << 16,
            PR_SENDER_ENTRYID                           = PT.PT_BINARY |    0x0C19 << 16,
            PR_SENDER_NAME                              = PT.PT_TSTRING |   0x0C1A << 16,
            PR_SENDER_NAME_W                            = PT.PT_UNICODE |   0x0C1A << 16,
            PR_SENDER_NAME_A                            = PT.PT_STRING8 |   0x0C1A << 16,
            PR_SUPPLEMENTARY_INFO                       = PT.PT_TSTRING |   0x0C1B << 16,
            PR_SUPPLEMENTARY_INFO_W                     = PT.PT_UNICODE |   0x0C1B << 16,
            PR_SUPPLEMENTARY_INFO_A                     = PT.PT_STRING8 |   0x0C1B << 16,
            PR_TYPE_OF_MTS_USER                         = PT.PT_LONG |      0x0C1C << 16,
            PR_SENDER_SEARCH_KEY                        = PT.PT_BINARY |    0x0C1D << 16,
            PR_SENDER_ADDRTYPE                          = PT.PT_TSTRING |   0x0C1E << 16,
            PR_SENDER_ADDRTYPE_W                        = PT.PT_UNICODE |   0x0C1E << 16,
            PR_SENDER_ADDRTYPE_A                        = PT.PT_STRING8 |   0x0C1E << 16,
            PR_SENDER_EMAIL_ADDRESS                     = PT.PT_TSTRING |   0x0C1F << 16,
            PR_SENDER_EMAIL_ADDRESS_W                   = PT.PT_UNICODE |   0x0C1F << 16,
            PR_SENDER_EMAIL_ADDRESS_A                   = PT.PT_STRING8 |   0x0C1F << 16,

            // Message envelope properties
            PR_ACKNOWLEDGEMENT_MODE                     = PT.PT_LONG |      0x0001 << 16,
            PR_ALTERNATE_RECIPIENT_ALLOWED              = PT.PT_BOOLEAN |   0x0002 << 16,
            PR_AUTHORIZING_USERS                        = PT.PT_BINARY |    0x0003 << 16,
            PR_AUTO_FORWARD_COMMENT                     = PT.PT_TSTRING |   0x0004 << 16,
            PR_AUTO_FORWARD_COMMENT_W                   = PT.PT_UNICODE |   0x0004 << 16,
            PR_AUTO_FORWARD_COMMENT_A                   = PT.PT_STRING8 |   0x0004 << 16,
            PR_AUTO_FORWARDED                           = PT.PT_BOOLEAN |   0x0005 << 16,
            PR_CONTENT_CONFIDENTIALITY_ALGORITHM_ID     = PT.PT_BINARY |    0x0006 << 16,
            PR_CONTENT_CORRELATOR                       = PT.PT_BINARY |    0x0007 << 16,
            PR_CONTENT_IDENTIFIER                       = PT.PT_TSTRING |   0x0008 << 16,
            PR_CONTENT_IDENTIFIER_W                     = PT.PT_UNICODE |   0x0008 << 16,
            PR_CONTENT_IDENTIFIER_A                     = PT.PT_STRING8 |   0x0008 << 16,
            PR_CONTENT_LENGTH                           = PT.PT_LONG |      0x0009 << 16,
            PR_CONTENT_RETURN_REQUESTED                 = PT.PT_BOOLEAN |   0x000A << 16,

            // Message envelope properties
            PR_CONVERSATION_KEY                         = PT.PT_BINARY |    0x000B << 16,

            PR_CONVERSION_EITS                          = PT.PT_BINARY |    0x000C << 16,
            PR_CONVERSION_WITH_LOSS_PROHIBITED          = PT.PT_BOOLEAN |   0x000D << 16,
            PR_CONVERTED_EITS                           = PT.PT_BINARY |    0x000E << 16,
            PR_DEFERRED_DELIVERY_TIME                   = PT.PT_SYSTIME |   0x000F << 16,
            PR_DELIVER_TIME                             = PT.PT_SYSTIME |   0x0010 << 16,
            PR_DISCARD_REASON                           = PT.PT_LONG |      0x0011 << 16,
            PR_DISCLOSURE_OF_RECIPIENTS                 = PT.PT_BOOLEAN |   0x0012 << 16,
            PR_DL_EXPANSION_HISTORY                     = PT.PT_BINARY |    0x0013 << 16,
            PR_DL_EXPANSION_PROHIBITED                  = PT.PT_BOOLEAN |   0x0014 << 16,
            PR_EXPIRY_TIME                              = PT.PT_SYSTIME |   0x0015 << 16,
            PR_IMPLICIT_CONVERSION_PROHIBITED           = PT.PT_BOOLEAN |   0x0016 << 16,
            PR_IMPORTANCE                               = PT.PT_LONG |      0x0017 << 16,
            PR_IPM_ID                                   = PT.PT_BINARY |    0x0018 << 16,
            PR_LATEST_DELIVERY_TIME                     = PT.PT_SYSTIME |   0x0019 << 16,
            PR_MESSAGE_CLASS                            = PT.PT_TSTRING |   0x001A << 16,
            PR_MESSAGE_CLASS_W                          = PT.PT_UNICODE |   0x001A << 16,
            PR_MESSAGE_CLASS_A                          = PT.PT_STRING8 |   0x001A << 16,
            PR_MESSAGE_DELIVERY_ID                      = PT.PT_BINARY |    0x001B << 16,

            PR_MESSAGE_SECURITY_LABEL                   = PT.PT_BINARY |    0x001E << 16,
            PR_OBSOLETED_IPMS                           = PT.PT_BINARY |    0x001F << 16,
            PR_ORIGINALLY_INTENDED_RECIPIENT_NAME       = PT.PT_BINARY |    0x0020 << 16,
            PR_ORIGINAL_EITS                            = PT.PT_BINARY |    0x0021 << 16,
            PR_ORIGINATOR_CERTIFICATE                   = PT.PT_BINARY |    0x0022 << 16,
            PR_ORIGINATOR_DELIVERY_REPORT_REQUESTED     = PT.PT_BOOLEAN |   0x0023 << 16,
            PR_ORIGINATOR_RETURN_ADDRESS                = PT.PT_BINARY |    0x0024 << 16,

            PR_PARENT_KEY                               = PT.PT_BINARY |    0x0025 << 16,
            PR_PRIORITY                                 = PT.PT_LONG |      0x0026 << 16,

            PR_ORIGIN_CHECK                             = PT.PT_BINARY |    0x0027 << 16,
            PR_PROOF_OF_SUBMISSION_REQUESTED            = PT.PT_BOOLEAN |   0x0028 << 16,
            PR_READ_RECEIPT_REQUESTED                   = PT.PT_BOOLEAN |   0x0029 << 16,
            PR_RECEIPT_TIME                             = PT.PT_SYSTIME |   0x002A << 16,
            PR_RECIPIENT_REASSIGNMENT_PROHIBITED        = PT.PT_BOOLEAN |   0x002B << 16,
            PR_REDIRECTION_HISTORY                      = PT.PT_BINARY |    0x002C << 16,
            PR_RELATED_IPMS                             = PT.PT_BINARY |    0x002D << 16,
            PR_ORIGINAL_SENSITIVITY                     = PT.PT_LONG |      0x002E << 16,
            PR_LANGUAGES                                = PT.PT_TSTRING |   0x002F << 16,
            PR_LANGUAGES_W                              = PT.PT_UNICODE |   0x002F << 16,
            PR_LANGUAGES_A                              = PT.PT_STRING8 |   0x002F << 16,
            PR_REPLY_TIME                               = PT.PT_SYSTIME |   0x0030 << 16,
            PR_REPORT_TAG                               = PT.PT_BINARY |    0x0031 << 16,
            PR_REPORT_TIME                              = PT.PT_SYSTIME |   0x0032 << 16,
            PR_RETURNED_IPM                             = PT.PT_BOOLEAN |   0x0033 << 16,
            PR_SECURITY                                 = PT.PT_LONG |      0x0034 << 16,
            PR_INCOMPLETE_COPY                          = PT.PT_BOOLEAN |   0x0035 << 16,
            PR_SENSITIVITY                              = PT.PT_LONG |      0x0036 << 16,
            PR_SUBJECT                                  = PT.PT_TSTRING |   0x0037 << 16,
            PR_SUBJECT_W                                = PT.PT_UNICODE |   0x0037 << 16,
            PR_SUBJECT_A                                = PT.PT_STRING8 |   0x0037 << 16,
            PR_SUBJECT_IPM                              = PT.PT_BINARY |    0x0038 << 16,
            PR_CLIENT_SUBMIT_TIME                       = PT.PT_SYSTIME |   0x0039 << 16,
            PR_REPORT_NAME                              = PT.PT_TSTRING |   0x003A << 16,
            PR_REPORT_NAME_W                            = PT.PT_UNICODE |   0x003A << 16,
            PR_REPORT_NAME_A                            = PT.PT_STRING8 |   0x003A << 16,
            PR_SENT_REPRESENTING_SEARCH_KEY             = PT.PT_BINARY |    0x003B << 16,
            PR_X400_CONTENT_TYPE                        = PT.PT_BINARY |    0x003C << 16,
            PR_SUBJECT_PREFIX                           = PT.PT_TSTRING |   0x003D << 16,
            PR_SUBJECT_PREFIX_W                         = PT.PT_UNICODE |   0x003D << 16,
            PR_SUBJECT_PREFIX_A                         = PT.PT_STRING8 |   0x003D << 16,
            PR_NON_RECEIPT_REASON                       = PT.PT_LONG |      0x003E << 16,
            PR_RECEIVED_BY_ENTRYID                      = PT.PT_BINARY |    0x003F << 16,
            PR_RECEIVED_BY_NAME                         = PT.PT_TSTRING |   0x0040 << 16,
            PR_RECEIVED_BY_NAME_W                       = PT.PT_UNICODE |   0x0040 << 16,
            PR_RECEIVED_BY_NAME_A                       = PT.PT_STRING8 |   0x0040 << 16,
            PR_SENT_REPRESENTING_ENTRYID                = PT.PT_BINARY |    0x0041 << 16,
            PR_SENT_REPRESENTING_NAME                   = PT.PT_TSTRING |   0x0042 << 16,
            PR_SENT_REPRESENTING_NAME_W                 = PT.PT_UNICODE |   0x0042 << 16,
            PR_SENT_REPRESENTING_NAME_A                 = PT.PT_STRING8 |   0x0042 << 16,
            PR_RCVD_REPRESENTING_ENTRYID                = PT.PT_BINARY |    0x0043 << 16,
            PR_RCVD_REPRESENTING_NAME                   = PT.PT_TSTRING |   0x0044 << 16,
            PR_RCVD_REPRESENTING_NAME_W                 = PT.PT_UNICODE |   0x0044 << 16,
            PR_RCVD_REPRESENTING_NAME_A                 = PT.PT_STRING8 |   0x0044 << 16,
            PR_REPORT_ENTRYID                           = PT.PT_BINARY |    0x0045 << 16,
            PR_READ_RECEIPT_ENTRYID                     = PT.PT_BINARY |    0x0046 << 16,
            PR_MESSAGE_SUBMISSION_ID                    = PT.PT_BINARY |    0x0047 << 16,
            PR_PROVIDER_SUBMIT_TIME                     = PT.PT_SYSTIME |   0x0048 << 16,
            PR_ORIGINAL_SUBJECT                         = PT.PT_TSTRING |   0x0049 << 16,
            PR_ORIGINAL_SUBJECT_W                       = PT.PT_UNICODE |   0x0049 << 16,
            PR_ORIGINAL_SUBJECT_A                       = PT.PT_STRING8 |   0x0049 << 16,
            PR_DISC_VAL                                 = PT.PT_BOOLEAN |   0x004A << 16,
            PR_ORIG_MESSAGE_CLASS                       = PT.PT_TSTRING |   0x004B << 16,
            PR_ORIG_MESSAGE_CLASS_W                     = PT.PT_UNICODE |   0x004B << 16,
            PR_ORIG_MESSAGE_CLASS_A                     = PT.PT_STRING8 |   0x004B << 16,
            PR_ORIGINAL_AUTHOR_ENTRYID                  = PT.PT_BINARY |    0x004C << 16,
            PR_ORIGINAL_AUTHOR_NAME                     = PT.PT_TSTRING |   0x004D << 16,
            PR_ORIGINAL_AUTHOR_NAME_W                   = PT.PT_UNICODE |   0x004D << 16,
            PR_ORIGINAL_AUTHOR_NAME_A                   = PT.PT_STRING8 |   0x004D << 16,
            PR_ORIGINAL_SUBMIT_TIME                     = PT.PT_SYSTIME |   0x004E << 16,
            PR_REPLY_RECIPIENT_ENTRIES                  = PT.PT_BINARY |    0x004F << 16,
            PR_REPLY_RECIPIENT_NAMES                    = PT.PT_TSTRING |   0x0050 << 16,
            PR_REPLY_RECIPIENT_NAMES_W                  = PT.PT_UNICODE |   0x0050 << 16,
            PR_REPLY_RECIPIENT_NAMES_A                  = PT.PT_STRING8 |   0x0050 << 16,

            PR_RECEIVED_BY_SEARCH_KEY                   = PT.PT_BINARY |    0x0051 << 16,
            PR_RCVD_REPRESENTING_SEARCH_KEY             = PT.PT_BINARY |    0x0052 << 16,
            PR_READ_RECEIPT_SEARCH_KEY                  = PT.PT_BINARY |    0x0053 << 16,
            PR_REPORT_SEARCH_KEY                        = PT.PT_BINARY |    0x0054 << 16,
            PR_ORIGINAL_DELIVERY_TIME                   = PT.PT_SYSTIME |   0x0055 << 16,
            PR_ORIGINAL_AUTHOR_SEARCH_KEY               = PT.PT_BINARY |    0x0056 << 16,

            PR_MESSAGE_TO_ME                            = PT.PT_BOOLEAN |   0x0057 << 16,
            PR_MESSAGE_CC_ME                            = PT.PT_BOOLEAN |   0x0058 << 16,
            PR_MESSAGE_RECIP_ME                         = PT.PT_BOOLEAN |   0x0059 << 16,

            PR_ORIGINAL_SENDER_NAME                     = PT.PT_TSTRING |   0x005A << 16,
            PR_ORIGINAL_SENDER_NAME_W                   = PT.PT_UNICODE |   0x005A << 16,
            PR_ORIGINAL_SENDER_NAME_A                   = PT.PT_STRING8 |   0x005A << 16,
            PR_ORIGINAL_SENDER_ENTRYID                  = PT.PT_BINARY |    0x005B << 16,
            PR_ORIGINAL_SENDER_SEARCH_KEY               = PT.PT_BINARY |    0x005C << 16,
            PR_ORIGINAL_SENT_REPRESENTING_NAME          = PT.PT_TSTRING |   0x005D << 16,
            PR_ORIGINAL_SENT_REPRESENTING_NAME_W        = PT.PT_UNICODE |   0x005D << 16,
            PR_ORIGINAL_SENT_REPRESENTING_NAME_A        = PT.PT_STRING8 |   0x005D << 16,
            PR_ORIGINAL_SENT_REPRESENTING_ENTRYID       = PT.PT_BINARY |    0x005E << 16,
            PR_ORIGINAL_SENT_REPRESENTING_SEARCH_KEY    = PT.PT_BINARY |    0x005F << 16,

            PR_START_DATE                               = PT.PT_SYSTIME |   0x0060 << 16,
            PR_END_DATE                                 = PT.PT_SYSTIME |   0x0061 << 16,
            PR_OWNER_APPT_ID                            = PT.PT_LONG |      0x0062 << 16,
            //PR_RESPONSE_REQUESTED                       = PT.PT_BOOLEAN |   0x0063 << 16,

            PR_SENT_REPRESENTING_ADDRTYPE               = PT.PT_TSTRING |   0x0064 << 16,
            PR_SENT_REPRESENTING_ADDRTYPE_W             = PT.PT_UNICODE |   0x0064 << 16,
            PR_SENT_REPRESENTING_ADDRTYPE_A             = PT.PT_STRING8 |   0x0064 << 16,
            PR_SENT_REPRESENTING_EMAIL_ADDRESS          = PT.PT_TSTRING |   0x0065 << 16,
            PR_SENT_REPRESENTING_EMAIL_ADDRESS_W        = PT.PT_UNICODE |   0x0065 << 16,
            PR_SENT_REPRESENTING_EMAIL_ADDRESS_A        = PT.PT_STRING8 |   0x0065 << 16,

            PR_ORIGINAL_SENDER_ADDRTYPE                 = PT.PT_TSTRING |   0x0066 << 16,
            PR_ORIGINAL_SENDER_ADDRTYPE_W               = PT.PT_UNICODE |   0x0066 << 16,
            PR_ORIGINAL_SENDER_ADDRTYPE_A               = PT.PT_STRING8 |   0x0066 << 16,
            PR_ORIGINAL_SENDER_EMAIL_ADDRESS            = PT.PT_TSTRING |   0x0067 << 16,
            PR_ORIGINAL_SENDER_EMAIL_ADDRESS_W          = PT.PT_UNICODE |   0x0067 << 16,
            PR_ORIGINAL_SENDER_EMAIL_ADDRESS_A          = PT.PT_STRING8 |   0x0067 << 16,

            PR_ORIGINAL_SENT_REPRESENTING_ADDRTYPE      = PT.PT_TSTRING |   0x0068 << 16,
            PR_ORIGINAL_SENT_REPRESENTING_ADDRTYPE_W    = PT.PT_UNICODE |   0x0068 << 16,
            PR_ORIGINAL_SENT_REPRESENTING_ADDRTYPE_A    = PT.PT_STRING8 |   0x0068 << 16,
            PR_ORIGINAL_SENT_REPRESENTING_EMAIL_ADDRESS = PT.PT_TSTRING |   0x0069 << 16,
            PR_ORIGINAL_SENT_REPRESENTING_EMAIL_ADDRESS_W   = PT.PT_UNICODE |   0x0069 << 16,
            PR_ORIGINAL_SENT_REPRESENTING_EMAIL_ADDRESS_A   = PT.PT_STRING8 |   0x0069 << 16,

            PR_CONVERSATION_TOPIC                       = PT.PT_TSTRING |   0x0070 << 16,
            PR_CONVERSATION_TOPIC_W                     = PT.PT_UNICODE |   0x0070 << 16,
            PR_CONVERSATION_TOPIC_A                     = PT.PT_STRING8 |   0x0070 << 16,
            PR_CONVERSATION_INDEX                       = PT.PT_BINARY |    0x0071 << 16,

            PR_ORIGINAL_DISPLAY_BCC                     = PT.PT_TSTRING |   0x0072 << 16,
            PR_ORIGINAL_DISPLAY_BCC_W                   = PT.PT_UNICODE |   0x0072 << 16,
            PR_ORIGINAL_DISPLAY_BCC_A                   = PT.PT_STRING8 |   0x0072 << 16,
            PR_ORIGINAL_DISPLAY_CC                      = PT.PT_TSTRING |   0x0073 << 16,
            PR_ORIGINAL_DISPLAY_CC_W                    = PT.PT_UNICODE |   0x0073 << 16,
            PR_ORIGINAL_DISPLAY_CC_A                    = PT.PT_STRING8 |   0x0073 << 16,
            PR_ORIGINAL_DISPLAY_TO                      = PT.PT_TSTRING |   0x0074 << 16,
            PR_ORIGINAL_DISPLAY_TO_W                    = PT.PT_UNICODE |   0x0074 << 16,
            PR_ORIGINAL_DISPLAY_TO_A                    = PT.PT_STRING8 |   0x0074 << 16,

            PR_RECEIVED_BY_ADDRTYPE                     = PT.PT_TSTRING |   0x0075 << 16,
            PR_RECEIVED_BY_ADDRTYPE_W                   = PT.PT_UNICODE |   0x0075 << 16,
            PR_RECEIVED_BY_ADDRTYPE_A                   = PT.PT_STRING8 |   0x0075 << 16,
            PR_RECEIVED_BY_EMAIL_ADDRESS                = PT.PT_TSTRING |   0x0076 << 16,
            PR_RECEIVED_BY_EMAIL_ADDRESS_W              = PT.PT_UNICODE |   0x0076 << 16,
            PR_RECEIVED_BY_EMAIL_ADDRESS_A              = PT.PT_STRING8 |   0x0076 << 16,

            PR_RCVD_REPRESENTING_ADDRTYPE               = PT.PT_TSTRING |   0x0077 << 16,
            PR_RCVD_REPRESENTING_ADDRTYPE_W             = PT.PT_UNICODE |   0x0077 << 16,
            PR_RCVD_REPRESENTING_ADDRTYPE_A             = PT.PT_STRING8 |   0x0077 << 16,
            PR_RCVD_REPRESENTING_EMAIL_ADDRESS          = PT.PT_TSTRING |   0x0078 << 16,
            PR_RCVD_REPRESENTING_EMAIL_ADDRESS_W        = PT.PT_UNICODE |   0x0078 << 16,
            PR_RCVD_REPRESENTING_EMAIL_ADDRESS_A        = PT.PT_STRING8 |   0x0078 << 16,

            PR_ORIGINAL_AUTHOR_ADDRTYPE                 = PT.PT_TSTRING |   0x0079 << 16,
            PR_ORIGINAL_AUTHOR_ADDRTYPE_W               = PT.PT_UNICODE |   0x0079 << 16,
            PR_ORIGINAL_AUTHOR_ADDRTYPE_A               = PT.PT_STRING8 |   0x0079 << 16,
            PR_ORIGINAL_AUTHOR_EMAIL_ADDRESS            = PT.PT_TSTRING |   0x007A << 16,
            PR_ORIGINAL_AUTHOR_EMAIL_ADDRESS_W          = PT.PT_UNICODE |   0x007A << 16,
            PR_ORIGINAL_AUTHOR_EMAIL_ADDRESS_A          = PT.PT_STRING8 |   0x007A << 16,

            PR_ORIGINALLY_INTENDED_RECIP_ADDRTYPE       = PT.PT_TSTRING |   0x007B << 16,
            PR_ORIGINALLY_INTENDED_RECIP_ADDRTYPE_W     = PT.PT_UNICODE |   0x007B << 16,
            PR_ORIGINALLY_INTENDED_RECIP_ADDRTYPE_A     = PT.PT_STRING8 |   0x007B << 16,
            PR_ORIGINALLY_INTENDED_RECIP_EMAIL_ADDRESS  = PT.PT_TSTRING |   0x007C << 16,
            PR_ORIGINALLY_INTENDED_RECIP_EMAIL_ADDRESS_W    = PT.PT_UNICODE |   0x007C << 16,
            PR_ORIGINALLY_INTENDED_RECIP_EMAIL_ADDRESS_A    = PT.PT_STRING8 |   0x007C << 16,

            PR_TRANSPORT_MESSAGE_HEADERS                = PT.PT_TSTRING |    0x007D << 16,
            PR_TRANSPORT_MESSAGE_HEADERS_W              = PT.PT_UNICODE |    0x007D << 16,
            PR_TRANSPORT_MESSAGE_HEADERS_A              = PT.PT_STRING8 |    0x007D << 16,

            PR_DELEGATION                               = PT.PT_BINARY |     0x007E << 16,

            PR_TNEF_CORRELATION_KEY                     = PT.PT_BINARY |     0x007F << 16,

            // Message content properties
            PR_BODY                                     = PT.PT_TSTRING |   0x1000 << 16,
            PR_BODY_W                                   = PT.PT_UNICODE |   0x1000 << 16,
            PR_BODY_A                                   = PT.PT_STRING8 |   0x1000 << 16,
            PR_REPORT_TEXT                              = PT.PT_TSTRING |   0x1001 << 16,
            PR_REPORT_TEXT_W                            = PT.PT_UNICODE |   0x1001 << 16,
            PR_REPORT_TEXT_A                            = PT.PT_STRING8 |   0x1001 << 16,
            PR_ORIGINATOR_AND_DL_EXPANSION_HISTORY      = PT.PT_BINARY |    0x1002 << 16,
            PR_REPORTING_DL_NAME                        = PT.PT_BINARY |    0x1003 << 16,
            PR_REPORTING_MTA_CERTIFICATE                = PT.PT_BINARY |    0x1004 << 16,
        };

        internal const int SizeOfSPropValue = 16;
        public const int MaxQueryRowCount = 10;

        [DllImport("cemapi.dll")]
        public static extern uint MAPIFreeBuffer(IntPtr lpBuffer);

        [DllImport("MAPILib.dll")]
        public static extern uint Release(IntPtr iUnknown);

        private static byte[] EntryIDFromPtr(IntPtr ptr)
        {
            int cbEntryID = Marshal.ReadInt32(ptr);
            byte[] entryID = new byte[cbEntryID];
            IntPtr lpEntryID = Marshal.ReadIntPtr((IntPtr)((uint)ptr + Marshal.SizeOf(typeof(int))));
            for (int i = 0; i < cbEntryID; i++)
                entryID[i] = Marshal.ReadByte(lpEntryID, i);
            return entryID;
        }

        public static IMAPISession MAPILogon() { return new MAPISession(); }
    }
}
