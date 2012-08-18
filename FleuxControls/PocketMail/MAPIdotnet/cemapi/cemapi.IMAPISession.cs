using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        public interface IMAPISession : IMAPIUnknown, IDisposable
        {
            IMAPITable GetMsgStoresTable();

            IMsgStore OpenMsgStore(byte[] msgStoreEntryId);

            bool CompareEntryIDs(byte[] entryID1, byte[] entryID2);
        }

        private class MAPISession : MAPIUnknown, IMAPISession
        {
            [DllImport("cemapi.dll")]
            private static extern HRESULT MAPIInitialize(IntPtr lpMapiInit);

            [DllImport("cemapi.dll")]
            private static extern void MAPIUninitialize();

            [DllImport("MAPIlib.dll", EntryPoint = "MAPIlibLogon")]
            private static extern HRESULT pMAPIlibLogon(ref IntPtr pSession);

            [DllImport("MAPIlib.dll")]
            private static extern HRESULT IMAPISessionLogoff(IntPtr pSession);

            [DllImport("MAPIlib.dll")]
            private static extern HRESULT IMAPISessionOpenMsgStore(IntPtr pSession, ref SBinary entryId, ref IntPtr pMsgStore);

            /// <summary>
            /// Must be Released
            /// </summary>
            /// <param name="pSession"></param>
            /// <param name="table"></param>
            /// <returns></returns>
            [DllImport("MAPIlib.dll", EntryPoint = "IMAPISessionGetMsgStoresTable")]
            private static extern HRESULT pIMAPISessionGetMsgStoresTable(IntPtr pSession, ref IntPtr table);

            [DllImport("MAPIlib.dll")]
            private static extern HRESULT IMAPISessionCompareEntryIDs(IntPtr pSession, ref SBinary entryID1, ref SBinary entryID2, ref uint lpulResult);

            private IMAPITable msgStoresTable = null;

            public MAPISession()
            {
                HRESULT hr = MAPIInitialize(IntPtr.Zero);
                if (hr != HRESULT.S_OK)
                    throw new Exception("MAPIInitiailize failed: " + hr.ToString());

                hr = pMAPIlibLogon(ref this.ptr);
                if (hr != HRESULT.S_OK)
                    throw new Exception("MAPIlibLogon failed: " + hr.ToString());
            }

            public override void Dispose()
            {
                this.msgStoresTable = null;
            }

            ~MAPISession()
            {
                Dispose();
                IMAPISessionLogoff(this.ptr);
                MAPIUninitialize();
            }

            public IMAPITable GetMsgStoresTable()
            {
                if (this.msgStoresTable != null)
                    return this.msgStoresTable;

                IntPtr pMsgTable = IntPtr.Zero;
                HRESULT hr = pIMAPISessionGetMsgStoresTable(this.ptr, ref pMsgTable);
                if (hr != HRESULT.S_OK)
                    throw new Exception("GetMsgStoresTable failed: " + hr.ToString());
                this.msgStoresTable = new MAPITable(pMsgTable);
                return this.msgStoresTable;
            }

            public IMsgStore OpenMsgStore(byte[] msgStoreEntryId)
            {
                IntPtr pMsgStore = IntPtr.Zero;
                SBinary b = SBinaryCreate(msgStoreEntryId);
                HRESULT hr = IMAPISessionOpenMsgStore(this.ptr, ref b, ref pMsgStore);
                if (hr != HRESULT.S_OK)
                    throw new Exception("OpenMsgStore failed: " + hr.ToString());
                SBinaryRelease(ref b);
                return new MsgStore(pMsgStore, this);
            }

            public bool CompareEntryIDs(byte[] entryID1, byte[] entryID2)
            {
                SBinary bID1 = SBinaryCreate(entryID1);
                SBinary bID2 = SBinaryCreate(entryID2);
                uint isEqual = 0;
                HRESULT hr = IMAPISessionCompareEntryIDs(this.ptr, ref bID1, ref bID2, ref isEqual);
                if (hr != HRESULT.S_OK)
                    throw new Exception("IMAPISessionCompareEntryIDs failed: " + hr.ToString());
                SBinaryRelease(ref bID1);
                SBinaryRelease(ref bID2);
                return isEqual != 0;
            }
        }
    }
}
