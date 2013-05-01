namespace MetroHome65.Widgets
{
    using System;
    using System.Runtime.InteropServices;

    public class PhoneRadio : IDisposable
    {
        private bool disposed;
        private IntPtr hLine;
        private IntPtr hLineApp;

        public PhoneRadio()
        {
            uint num;
            LINEINITIALIZEEXPARAMS lineinitializeexparams;
            int num3;
            LINEEXTENSIONID lineextensionid;
            this.hLine = IntPtr.Zero;
            this.hLineApp = IntPtr.Zero;
            this.disposed = false;
            uint aPIVersion = 0x20000;
            lineinitializeexparams = new LINEINITIALIZEEXPARAMS {
                hEvent = IntPtr.Zero,
                dwTotalSize = Marshal.SizeOf(lineinitializeexparams),
                dwNeededSize = lineinitializeexparams.dwTotalSize,
                dwUsedSize = lineinitializeexparams.dwTotalSize,
                hEvent = IntPtr.Zero,
                dwOptions = 2
            };
            LineErrReturn return2 = lineInitializeEx(out this.hLineApp, IntPtr.Zero, IntPtr.Zero, "deltaProfile", out num, ref aPIVersion, ref lineinitializeexparams);
            int num4 = lineNegotiateAPIVersion(this.hLineApp, 0, 0x10004, 0x20000, out num3, out lineextensionid);
            int num5 = lineOpen(this.hLineApp, 0, out this.hLine, num3, 0, 0, 4, 0x10, IntPtr.Zero);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
                lineClose(this.hLine);
                this.hLine = IntPtr.Zero;
                lineShutdown(this.hLineApp);
                this.hLineApp = IntPtr.Zero;
                this.disposed = true;
            }
        }

        ~PhoneRadio()
        {
            this.Dispose(false);
        }

        [DllImport("coredll.dll")]
        private static extern int lineClose(IntPtr hLine);
        [DllImport("coredll.dll", SetLastError=true)]
        private static extern LineErrReturn lineInitializeEx(out IntPtr hLineApp, IntPtr hAppHandle, IntPtr lCallBack, string FriendlyAppName, out uint NumDevices, ref uint APIVersion, ref LINEINITIALIZEEXPARAMS lineExInitParams);
        [DllImport("coredll.dll")]
        private static extern int lineNegotiateAPIVersion(IntPtr lphLineApp, int dwDeviceID, int dwAPILowVersion, int dwAPIHighVersion, out int lpdwAPIVersion, out LINEEXTENSIONID lpExtensionID);
        [DllImport("coredll.dll")]
        private static extern int lineOpen(IntPtr hLineApp, int dwDeviceID, out IntPtr hLine, int dwAPIVersion, int dwExtVersion, int dwCallbackInstance, int dwPrivileges, int dwMediaModes, IntPtr lpCallParams);
        [DllImport("cellcore.dll")]
        private static extern int lineRegister(IntPtr hLine, int dwRegisterMode, string lpszOperator, int dwOperatorFormat);
        [DllImport("cellcore.dll")]
        private static extern int lineSetEquipmentState(IntPtr hLine, int dwState);
        [DllImport("coredll.dll")]
        private static extern int lineShutdown(IntPtr hLine);
        public void SwitchOff()
        {
            int num = lineSetEquipmentState(this.hLine, 4);
        }

        public void SwitchOn()
        {
            int num = lineSetEquipmentState(this.hLine, 5);
            int num2 = lineRegister(this.hLine, 1, null, 0);
        }

        private enum LINEEQUIPSTATE
        {
            LINEEQUIPSTATE_FULL = 5,
            LINEEQUIPSTATE_MINIMUM = 1,
            LINEEQUIPSTATE_NOTXRX = 4,
            LINEEQUIPSTATE_RXONLY = 2,
            LINEEQUIPSTATE_TXONLY = 3
        }

        private enum LineErrReturn : uint
        {
            LINE_OK = 0,
            LINEERR_INIFILECORRUPT = 0x8000000e,
            LINEERR_INVALAPPNAME = 0x80000015,
            LINEERR_INVALPARAM = 0x80000032,
            LINEERR_INVALPOINTER = 0x80000035,
            LINEERR_NOMEM = 0x80000044,
            LINEERR_OPERATIONFAILED = 0x80000048,
            LINEERR_REINIT = 0x80000052
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LINEEXTENSIONID
        {
            public IntPtr dwExtensionID0;
            public IntPtr dwExtensionID1;
            public IntPtr dwExtensionID2;
            public IntPtr dwExtensionID3;
        }

        private enum LINEINITIALIZEEXOPTION
        {
            LINEINITIALIZEEXOPTION_USECOMPLETIONPORT = 3,
            LINEINITIALIZEEXOPTION_USEEVENT = 2,
            LINEINITIALIZEEXOPTION_USEHIDDENWINDOW = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LINEINITIALIZEEXPARAMS
        {
            public int dwTotalSize;
            public int dwNeededSize;
            public int dwUsedSize;
            public int dwOptions;
            public IntPtr hEvent;
            public int dwCompletionKey;
        }

        private enum LINEOPFORMAT
        {
            LINEOPFORMAT_ALPHALONG = 2,
            LINEOPFORMAT_ALPHASHORT = 1,
            LINEOPFORMAT_NONE = 0,
            LINEOPFORMAT_NUMERIC = 4
        }

        private enum LINEREGMODE
        {
            LINEREGMODE_AUTOMATIC = 1,
            LINEREGMODE_MANAUTO = 3,
            LINEREGMODE_MANUAL = 2
        }
    }
}

