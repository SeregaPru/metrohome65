namespace Fleux.Core.NativeHelpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class Led
    {
        private static uint ledCountInfoId = 0;
        private static uint ledSettingsInfoId = 2;

        private int ledCount;

        public Led()
        {
            this.ledCount = this.GetLedCount();
        }

        public enum Status
        {
            OFF = 0,
            ON,
            BLINK
        }

        public void SetLedStatus(Status status)
        {
            LedSettingsInfo nsi = new LedSettingsInfo();
            nsi.OffOnBlink = (uint)status;
            for (int i = 0; i < this.ledCount; i++)
            {
                nsi.LedNum = (uint)i;
                NLedSetDevice(ledSettingsInfoId, nsi);
            }
        }

        public void Vibrate(int millisecondsTimeout)
        {
            new Thread(() =>
                {
                    this.SetLedStatus(Status.ON);
                    Thread.Sleep(millisecondsTimeout);
                    this.SetLedStatus(Status.OFF);
                })
                {
                    Priority = ThreadPriority.Highest
                }
                .Start();
        }

        public void Vibrate()
        {
            this.Vibrate(FleuxSettings.HapticFeedbackMillisecs);
        }

        [DllImport("coredll.dll")]
        private static extern bool NLedGetDeviceInfo(uint nID, LedCountInfo output);

        [DllImport("coredll.dll")]
        private static extern bool NLedSetDevice(uint nID, LedSettingsInfo output);

        private int GetLedCount()
        {
            int count = 0;
            LedCountInfo nci = new LedCountInfo();
            if (NLedGetDeviceInfo(ledCountInfoId, nci))
            {
                count = nci.LedsCount;
            }
            return count;
        }

        private class LedSettingsInfo
        {
            public uint LedNum = 0;
            public uint OffOnBlink = 0;
            public int TotalCycleTime = 0;
            public int OnTime = 0;
            public int OffTime = 0;
            public int MetaCycleOn = 0;
            public int MetaCycleOff = 0;
        }

        private class LedCountInfo
        {
            public int LedsCount = 0 ;
        }
    }
}
