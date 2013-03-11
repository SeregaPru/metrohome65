using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
	internal static partial class cemapi
	{
        public interface IMessage : IMAPIProp
        {
            IMAPITable GetRecipientTable();
        }

        private class Message : MAPIProp, IMessage
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg"></param>
            /// <param name="recipTable"></param>
            /// <returns>MAPI_E_NO_RECIPIENTS if no recipients</returns>
            [DllImport("MAPIlib.dll", EntryPoint = "IMessageGetRecipientTable")]
            private static extern HRESULT pIMessageGetRecipientTable(IntPtr msg, ref IntPtr recipTable);


            public Message(IntPtr ptr) : base(ptr) { }

            ~Message()
            { }

            public IMAPITable GetRecipientTable()
            {
                IntPtr tablePtr = IntPtr.Zero;
                HRESULT hr = pIMessageGetRecipientTable(this.ptr, ref tablePtr);
                if (hr == HRESULT.MAPI_E_NO_RECIPIENTS)
                    return null;
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMessageGetRecipientTable failed: " + hr.ToString());
                return new MAPITable(tablePtr);
            }

        }
	}
}
