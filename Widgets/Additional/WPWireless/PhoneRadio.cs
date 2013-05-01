namespace MetroHome65.Widgets
{
    using System;
    using System.Runtime.InteropServices;

    public class PhoneRadio : IDisposable
    {
        private bool _disposed;
        private IntPtr _hLine;
        private IntPtr _hLineApp;

        public PhoneRadio()
        {
            uint num;
            var lineinitializeexparams = new LINEINITIALIZEEXPARAMS();
            int num3;
            LINEEXTENSIONID lineextensionid;
            this._hLine = IntPtr.Zero;
            this._hLineApp = IntPtr.Zero;
            this._disposed = false;
            uint aPIVersion = 0x20000;

            lineinitializeexparams = new LINEINITIALIZEEXPARAMS {
                dwTotalSize = Marshal.SizeOf(lineinitializeexparams),
                dwNeededSize = lineinitializeexparams.dwTotalSize,
                dwUsedSize = lineinitializeexparams.dwTotalSize,
                hEvent = IntPtr.Zero,
                dwOptions = 2
            };

            lineInitializeEx(out _hLineApp, IntPtr.Zero, IntPtr.Zero, "deltaProfile", out num, ref aPIVersion, ref lineinitializeexparams);
            lineNegotiateAPIVersion(_hLineApp, 0, 0x10004, 0x20000, out num3, out lineextensionid);
            lineOpen(_hLineApp, 0, out _hLine, num3, 0, 0, 4, 0x10, IntPtr.Zero);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;

            lineClose(_hLine);
            _hLine = IntPtr.Zero;
            lineShutdown(_hLineApp);
            _hLineApp = IntPtr.Zero;
            _disposed = true;
        }

        ~PhoneRadio()
        {
            Dispose(false);
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
            lineSetEquipmentState(_hLine, 4);
        }

        public void SwitchOn()
        {
            lineSetEquipmentState(_hLine, 5);
            lineRegister(_hLine, 1, null, 0);
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

