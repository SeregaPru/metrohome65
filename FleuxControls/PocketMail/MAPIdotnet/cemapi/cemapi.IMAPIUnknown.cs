using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MAPIdotnet
{
    internal static partial class cemapi
    {
        public interface IMAPIUnknown : IDisposable
        {
            void Release();

            IntPtr Ptr { get; }
        }

        private abstract class MAPIUnknown : IMAPIUnknown
        {
            [DllImport("MAPILib.dll", EntryPoint = "Release")]
            public static extern uint pRelease(IntPtr iUnknown);

            protected IntPtr ptr = IntPtr.Zero;

            public void Release()
            {
                if (this.ptr != IntPtr.Zero)
                {
                    pRelease(this.ptr);
                    this.ptr = IntPtr.Zero;
                }
            }

            public IntPtr Ptr { get { return this.ptr; } }

            public virtual void Dispose() { Release(); }

            ~MAPIUnknown() { Release(); }
        }
    }
}
