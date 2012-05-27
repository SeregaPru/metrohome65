using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        public interface IMAPIFolder : IMAPIContainer
        {
            void DeleteMessages(List<byte[]> msgEntryIds);
            void DeleteMessage(byte[] msgEntryId);

            void EmptyFolder();
        }

        private class MAPIFolder : MAPIContainer, IMAPIFolder
        {
            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIFolderDeleteMessages")]
            private static extern HRESULT pIMAPIFolderDeleteMessages(IntPtr folder, IntPtr lpMsgs);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIFolderEmptyFolder")]
            private static extern HRESULT pIMAPIFolderEmptyFolder(IntPtr folder);

            public MAPIFolder(IntPtr ptr) : base(ptr) { }

            public void DeleteMessages(List<byte[]> msgEntryIds)
            {
                IntPtr pMsgs = Marshal.AllocHGlobal(8);
                int numIds = msgEntryIds.Count;
                Marshal.WriteInt32(pMsgs, numIds);
                IntPtr pArray = Marshal.AllocHGlobal(8 * numIds);
                Marshal.WriteInt32(pMsgs, 4, (int)pArray);

                for (int i = 0; i < numIds; ++i)
                {
                    byte[] id = msgEntryIds[i];
                    int idLen = id.Length;
                    IntPtr bytes = Marshal.AllocHGlobal(idLen);
                    Marshal.WriteInt32(pArray, i * 8, idLen);
                    Marshal.WriteInt32(pArray, i * 8 + 4, (int)bytes);
                    for (int j = 0; j < idLen; ++j)
                        Marshal.WriteByte(bytes, j, id[j]);
                }

                HRESULT hr = pIMAPIFolderDeleteMessages(this.ptr, pMsgs);
                if (hr != HRESULT.S_OK)
                    throw new Exception("Delete messages failed: " + hr.ToString());

                for (int i = 0; i < numIds; ++i)
                    Marshal.FreeHGlobal((IntPtr)Marshal.ReadInt32(pArray, 8 * i + 4));
                Marshal.FreeHGlobal(pArray);
                Marshal.FreeHGlobal(pMsgs);
            }

            public void DeleteMessage(byte[] msgEntryId)
            {
                int idLen = msgEntryId.Length;
                IntPtr pMsg = Marshal.AllocHGlobal(8);
                Marshal.WriteInt32(pMsg, 1);
                IntPtr pArray = Marshal.AllocHGlobal(8);
                Marshal.WriteInt32(pMsg, 4, (int)pArray);
                Marshal.WriteInt32(pArray, idLen);
                IntPtr entryBytes = Marshal.AllocHGlobal(idLen);
                Marshal.WriteInt32(pArray, 4, (int)entryBytes);
                for (int j = 0; j < idLen; ++j)
                    Marshal.WriteByte(entryBytes, j, msgEntryId[j]);

                HRESULT hr = pIMAPIFolderDeleteMessages(this.ptr, pMsg);
                if (hr != HRESULT.S_OK)
                    throw new Exception("Delete messages failed: " + hr.ToString());

                Marshal.FreeHGlobal(entryBytes);
                Marshal.FreeHGlobal(pMsg);
                Marshal.FreeHGlobal(pArray);
            }

            public void EmptyFolder()
            {
                HRESULT hr = pIMAPIFolderEmptyFolder(this.ptr);
                if (hr != HRESULT.S_OK)
                    throw new Exception("Empty folder failed: " + hr.ToString());
            }
        }


    }
}
