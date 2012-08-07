using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsMobile.Status;

using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;

using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.Settings;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

using MetroHome65.WPLock.Controls;

namespace MetroHome65.WPLock.Controls
{
    public class ScreenTopBar : Canvas
    {

        #region GetSystemPowerStatusEx2

        [DllImport("coredll")]
        static public extern uint GetSystemPowerStatusEx2(ref SYSTEM_POWER_STATUS_EX2 lpSystemPowerStatus, uint dwLen, bool fUpdate);

        public enum ACLineStatus : byte
        {
            Offline = 0x00,
            Online = 0x01,
            BackupPower = 0x02,
            Unknown = 0xFF
        }


        public enum BatteryFlag : byte
        {
            High = 0x01,
            Low = 0x02,
            Critical = 0x04,
            Charging = 0x08,
            NoBattery = 0x80,
            Unknown = 0xFF
        }
        public enum BatteryChemistry : byte
        {
            Alkaline = 0x01,
            NiCd = 0x02,
            NiMh = 0x03,
            LiON = 0x04,
            LiPoly = 0x05,
            ZincAir = 0x06,
            Unknown = 0xFF
        }

        public struct SYSTEM_POWER_STATUS_EX2
        {
            public ACLineStatus ACLineStatus;
            public BatteryFlag BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public int BatteryLifeTime;
            public int BatteryFullLifeTime;
            public byte Reserved2;
            public BatteryFlag BackupBatteryFlag;
            public byte BackupBatteryLifePercent;
            public byte Reserved3;
            public int BackupBatteryLifeTime;
            public int BackupBatteryFullLifeTime;
            public int BatteryVoltage;
            public int BatteryCurrent;
            public int BatteryAverageCurrent;
            public int BatteryAverageInterval;
            public int BatterymAHourConsumed;
            public int BatteryTemperature;
            public int BackupBatteryVoltage;
            public BatteryChemistry BatteryChemistry;
        }


        #endregion
        
        #region Private
        private List<string> _status = new List<string>
        {
            "", // Phone
            "", // Data
            "", // Call
            "", // Roaming
            "", // WiFi
            "", // Bluetooth
            "", // Profile
            "", // Keyboard
            ""  // Battery
        };
        const int _clock_size = 60;
        private int _timerStep = 0;
        private int _timerTime = 10;
        private int _batteryPower = 0;
        private Size _imageSize = new Size(28, 28);
        #endregion

        #region Public

        private string _fontName = "Segoe WP Semibold";                  // Опубликовать дать возможность смены
        private int _fontSize = 7;                                       // Опубликовать дать возможность смены
        private Color _transparentKeyColor = Color.FromArgb(0, 0, 0);    // Опубликовать дать возможность смены
        private string _timeString = "HH:mm";                            // Опубликовать дать возможность смены

        public Color _fontColor = MetroTheme.PhoneForegroundBrush;      // Опубликовать дать возможность смены
        public string _typeColorIcon = "white";                         // Опубликовать дать возможность смены
        public Boolean _batteryColor = false;                           // Опубликовать дать возможность смены
        #endregion

        public ScreenTopBar(Size _size, bool _white)
        {
            this.Size = _size;
            _typeColorIcon = "white";
            if (!_white) _typeColorIcon = "black";
            
            // Для проверки отображения батареи
            this.TapHandler = OnTap;
            this.HoldHandler = OnHold;
        }

        public bool OnTap(Point p)
        {
            _batteryPower += 1;
            if (_batteryPower > 100) _batteryPower = 0;
            this.Update();
            return true;
        }

        public bool OnHold(Point p)
        {
            _batteryPower += 10;
            if (_batteryPower > 100) _batteryPower = 0;
            this.Update();
            return true;
        }


        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            base.Draw(drawingGraphics);

            try
            {
                for (int i = 0; i < _status.Count; i++)
                {
                    if (_status[i] != "")
                    {
                        drawingGraphics.DrawImage(ResourceManager.Instance.GetBitmapFromEmbeddedResource(_status[i], this.GetType().Assembly),
                        48 * i, (this.Size.Height - _imageSize.Height) / 2, _imageSize.Width, _imageSize.Height, _transparentKeyColor);

                        if (_status[i] == "battery-state.png") // Рисуем батарею
                        {
                            var x1 = 48 * i + 3; var x2 = 48 * i + 22; 
                            var y1 = (this.Size.Height - _imageSize.Height) / 2 + 10 + 1;
                            var y2 = (this.Size.Height - _imageSize.Height) / 2 + 18 ;
                            var l = (int)(_batteryPower / 5);
                            if (_typeColorIcon == "white") { drawingGraphics.Color(Color.White); } else { drawingGraphics.Color(Color.Black); }
                            if (_batteryColor)
                            {
                                if (_batteryPower <= 20) { drawingGraphics.Color(Color.Red); }
                                if ((_batteryPower > 20) && (_batteryPower <= 40)) { drawingGraphics.Color(Color.Orange); }
                                if ((_batteryPower > 40) && (_batteryPower <= 60)) { drawingGraphics.Color(Color.Yellow); }
                                if ((_batteryPower > 60) && (_batteryPower <= 80)) { drawingGraphics.Color(Color.Green); }
                            }
                            drawingGraphics.FillRectangle(x1, y1, x1 + l, y2);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

            drawingGraphics.Style(new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor));
            var _text = DateTime.Now.ToString(_timeString);
            var _height = drawingGraphics.CalculateMultilineTextHeight("0", _clock_size);
            drawingGraphics.MoveTo(this.Size.Width - _clock_size, (this.Size.Height - _height) / 2).DrawCenterText(_text, _clock_size);
        }

        private void GetState()
        {
            for (int i = 0; i < _status.Count; i++) { _status[i] = ""; }
            //***********************************************************************************************
            //*********************************** 1 Cellular signal strength ********************************
            if (SystemState.PhoneRadioOff)
            {
                _status[0] = "airplane-mode.png";
            }
            else
            {
                var _si = SystemState.PhoneSignalStrength;
                if ((_si >= 0) && (_si <= 10)) { _status[0] = "no-signal.png"; }
                if ((_si >= 10) && (_si <= 20)) { _status[0] = "signal-very-low.png"; }
                if ((_si >= 20) && (_si <= 40)) { _status[0] = "signal-low.png"; }
                if ((_si >= 40) && (_si <= 60)) { _status[0] = "signal-medium.png"; }
                if ((_si >= 60) && (_si <= 80)) { _status[0] = "signal-high.png"; }
                if ((_si >= 80) && (_si <= 100)) { _status[0] = "signal-full.png"; }
                if (SystemState.PhoneBlockedSim) { _status[0] = "sim-locked.png"; }
                if (SystemState.PhoneNoService) { _status[0] = "no-data.png"; }
                if (SystemState.PhoneNoSim) { _status[0] = "sim-missing.png"; }
            }
            //***********************************************************************************************
            //*********************************** 2 Cellular data connection ********************************
            if (SystemState.CellularSystemConnected1xrtt) { _status[1] = "1xrt.png"; }
            if (SystemState.CellularSystemConnectedEdge) { _status[1] = "edge.png"; }
            if (SystemState.CellularSystemConnectedEvdo) { _status[1] = "1xevdo.png"; }
            if (SystemState.CellularSystemConnectedEvdv) { _status[1] = "evdv.png"; }
            if (SystemState.CellularSystemConnectedGprs) { _status[1] = "gprs.png"; }
            if (SystemState.CellularSystemConnectedHsdpa) { _status[1] = "hsdpa.png"; }
            if (SystemState.CellularSystemConnectedUmts) { _status[1] = "umts.png"; }
            //*********************************** 3 Call forwarding *****************************************
            //***********************************************************************************************
            if ((SystemState.PhoneCallForwardingOnLine1) || (SystemState.PhoneCallForwardingOnLine2)) { _status[2] = "call-forward.png"; }
            //*********************************** 4 Roaming *************************************************
            //***********************************************************************************************
            if (SystemState.PhoneRoaming) { _status[3] = "roaming.png"; } else { _status[3] = ""; }
            //*********************************** 5 Wi-Fi connection ****************************************
            //***********************************************************************************************
            if (SystemState.WiFiStatePowerOn)
            {
                _status[4] = "wifi-not-connected.png";
                if (SystemState.WiFiStateConnecting) { _status[4] = "wifi-connected-full.png"; }
                if (SystemState.WiFiStateConnected) { _status[4] = "internet-sharing.png"; }
            }
            //*********************************** 6 Bluetooth device ****************************************
            //***********************************************************************************************
            if (SystemState.BluetoothStatePowerOn) { _status[5] = "bluetooth.png"; } else { _status[5] = ""; }
            //*********************************** 7 Phone profile *******************************************
            //***********************************************************************************************
            if (SystemState.PhoneProfile == "Silent") { _status[6] = "silent-mode.png"; } 
            if (SystemState.PhoneRingerOff) { _status[6] = "vibrate.png"; }
            //*********************************** 8 Keyboard and location ***********************************
            //***********************************************************************************************

            //*********************************** 9 Battery *************************************************
            //***********************************************************************************************
            if (SystemState.PowerBatteryState == BatteryState.Charging)
            { 
                _status[8] = "battery-charging.png"; 
            }
            else
            {
                SYSTEM_POWER_STATUS_EX2 Status = new SYSTEM_POWER_STATUS_EX2();
                uint retBytesCount = GetSystemPowerStatusEx2(ref Status, (uint)Marshal.SizeOf(Status), false);
                if ((Status.BatteryLifePercent < 0) || (Status.BatteryLifePercent > 100))
                {
                    _status[8] = "battery-unknown-state.png";
                    _batteryPower = 0;
                }
                else
                {
                    _status[8] = "battery-state.png";
                    _batteryPower = Status.BatteryLifePercent; 
                }
            }
        }
        private void UpdateState()
        {
            return;
        }
        
        public void Active()
        {
            GetState();
            UpdateState();
            this.Update();
        }
        
        public void Timer()
        {
            _timerStep++;
            if (_timerStep > _timerTime)
            {
                _timerStep = 0;
                Active();
            }
        }
        public void UpdateTheme()
        {
            this.Update();
        }

    }
}
