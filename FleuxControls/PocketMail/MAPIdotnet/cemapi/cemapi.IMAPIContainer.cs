using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        public interface IMAPIContainer : IMAPIProp
        {
            IMAPITable GetContentsTable();

            IMAPITable GetHierarchyTable();

            IMAPIFolder OpenEntryAsFolder(byte[] entryId);
            IMessage OpenEntryAsMessage(byte[] entryId);
        }


        private abstract class MAPIContainer : MAPIProp, IMAPIContainer
        {
            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIContainerGetContentsTable")]
            private static extern HRESULT pIMAPIContainerGetContentsTable(IntPtr container, ref IntPtr lppTable/*IMAPITable*/);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIContainerGetHierarchyTable")]
            private static extern HRESULT pIMAPIContainerGetHierarchyTable(IntPtr container, ref IntPtr lppTable/*IMAPITable*/);

            [DllImport("MAPIlib.dll", EntryPoint = "IMAPIContainerOpenEntry")]
            private static extern HRESULT pIMAPIContainerOpenEntry(IntPtr container, ref SBinary entryId, ObjectType type, ref IntPtr entry);


            public MAPIContainer(IntPtr containerPtr) : base(containerPtr)
            { }

            public IMAPITable GetContentsTable()
            {
                IntPtr table = IntPtr.Zero;
                HRESULT hr = pIMAPIContainerGetContentsTable(this.ptr, ref table);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPIContainerGetContentsTable failed: " + hr.ToString());
                return new MAPITable(table);
            }


            public IMAPITable GetHierarchyTable()
            {
                IntPtr table = IntPtr.Zero;
                HRESULT hr = pIMAPIContainerGetHierarchyTable(this.ptr, ref table);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPIContainerGetHierarchyTable failed: " + hr.ToString());
                return new MAPITable(table);
            }


            public IMAPIFolder OpenEntryAsFolder(byte[] entryId)
            {
                IntPtr entryPtr = IntPtr.Zero;
                SBinary b = SBinaryCreate(entryId);
                HRESULT hr = pIMAPIContainerOpenEntry(this.ptr, ref b, ObjectType.MAPI_FOLDER, ref entryPtr);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPIContainerOpenEntry failed: " + hr.ToString());
                SBinaryRelease(ref b);
                IMAPIFolder folder = new MAPIFolder(entryPtr);
                return folder;
            }

            public IMessage OpenEntryAsMessage(byte[] entryId)
            {
                IntPtr entryPtr = IntPtr.Zero;
                SBinary b = SBinaryCreate(entryId);
                HRESULT hr = pIMAPIContainerOpenEntry(this.ptr, ref b, ObjectType.MAPI_MESSAGE, ref entryPtr);
                if (hr != HRESULT.S_OK)
                    throw new Exception("pIMAPIContainerOpenEntry failed: " + hr.ToString());
                SBinaryRelease(ref b);
                IMessage msg = new Message(entryPtr);
                return msg;
            }
        }
    }
}
