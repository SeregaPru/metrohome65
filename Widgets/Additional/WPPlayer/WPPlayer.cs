using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;
using Microsoft.WindowsMobile.Status;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;


namespace MetroHome65.Widgets
{
    [TileInfo("WP Player Control")]
    public class WPPlayer : ShortcutWidget, IActive
    {
        #region Enum Player
        public enum _PlayerCommand : int
        {
            Stop = 0,
            Play = 1,
            Next = 3,
            Previous = 2,
            VolumeUp = 4,
            VolumeDown = 5,
            FastForward = 6,
            FastBackward = 7,
        }

        public enum _SP2Command : uint
        {
            Play = 301,
            Stop = 301,
            Next = 302,
            Previous = 303,
            VolumeUp = 403,
            VolumeDown = 404,
            FastForward = 408,
            FastBackward = 409
        }

        public enum _NitrogenCommand : uint
        {
            Play = 40001,
            Stop = 40001,
            Next = 40003,
            Previous = 40002,
            VolumeUp = 40005,
            VolumeDown = 40004,
            FastForward = 0,            // Нет данных по команде
            FastBackward = 0            //Нет данных по команде
        }

        public enum _StatusPlay : int
        {
            Unknown = -1,
            Play = 0,
            Stop = 0x01
        }

        private struct _PlayerState
        {
            public string Title;
            public string Album;
            public string Artist;
            public string Cover;
            public _StatusPlay Status;
            public bool Run;
            public int Volume;
            public string PlayerName;
        }
        #endregion
        
        #region WIN32
        [DllImport("coredll.dll")]
        private static extern IntPtr FindWindow(IntPtr className, string windowName);
        [DllImport("coredll.dll")]
        internal static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("coredll.dll", EntryPoint = "SendMessage")]
        private static extern uint SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);
        private const Int32 WM_COMMAND = 273;
        #endregion


        #region Private
        private ThreadTimer _updateTimer;
        private _PlayerState _state;
        private Brush _brush;
        private Font _fnt;
        private Pen _pen;
        private Rectangle _rRew;
        private Rectangle _rPlay;
        private Rectangle _rStop;
        private Rectangle _rNext;
        private Rectangle _rCover;

        #endregion

        #region Public
        public string _typeColorIcon = "white";                             // Опубликовать дать возможность смены

        private static readonly List<String> _fontSizes = new List<String>() { "6", "7", "8", "9", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30" };
        private readonly List<object> _fontSizesData;
        private static readonly List<String> _fontNames = new List<String>() { "Segoe WP", "Segoe WP Light", "Segoe WP SemiLight", "Segoe WP Semibold" };
        private readonly List<object> _fontNamesData;

        private int _fontSize = Int16.Parse(_fontSizes[4]);
        private Color _fontColor = MetroTheme.TileTextStyle.Foreground;
        private string _fontName = _fontNames[3];

        #endregion



        public WPPlayer() 
        {
            _fontSizesData = new List<object>(); foreach (var size in _fontSizes) { _fontSizesData.Add(size); }
            _fontNamesData = new List<object>(); foreach (var name in _fontNames) { _fontNamesData.Add(name); }

            _brush = new SolidBrush(_fontColor);
            _fnt = new Font(_fontName, _fontSize.ToLogic(), FontStyle.Regular);
            _pen = new Pen(Color.WhiteSmoke, 2);

            _state.Title = "";
            _state.Album = "";
            _state.Artist = "";
            _state.Cover = "";
            _state.Status = _StatusPlay.Unknown;
            _state.Run = false;
            _state.Volume = 0;
            _state.PlayerName = "";
            
            UpdateStatus();
            ForceUpdate();
        }

        private void GetState()
        {
            _state.Title = "";
            _state.Album = "";
            _state.Artist = "";
            _state.Cover = "";
            _state.Status = _StatusPlay.Unknown;
            _state.Run = false;
            _state.Volume = 0;

            try
            {
                if (FindWindow(IntPtr.Zero, "S2P") != IntPtr.Zero) // Нашли S2P
                {
                    RegistryKey hwKey_s2p = Registry.CurrentUser.OpenSubKey("Software\\A_C\\S2P", false);
                    if (hwKey_s2p != null)
                    {
                        _state.PlayerName = "S2P";
                        _state.Run = true;
                        if ((int)hwKey_s2p.GetValue("Status", 1) == 0) // 0 - Play 1 - Stop
                        {
                            _state.Status = _StatusPlay.Play;
                        }
                        else
                        {
                            _state.Status = _StatusPlay.Stop;
                        }
                        _state.Title = (string)hwKey_s2p.GetValue("CurrentTitle", "");
                        _state.Album = (string)hwKey_s2p.GetValue("CurrentAlbumName", "");
                        _state.Artist = (string)hwKey_s2p.GetValue("CurrentArtist", "");
                        _state.Cover = (string)hwKey_s2p.GetValue("CurrentAlbum", "");
                        _state.Volume = (int)hwKey_s2p.GetValue("MusicVolume", 0);
                        hwKey_s2p.Close();
                    }
                }
                if (FindWindow(IntPtr.Zero, "Nitrogen") != IntPtr.Zero) // Нашли Nitrogen
                {
                    RegistryKey hwKey_nit = Registry.LocalMachine.OpenSubKey("System\\State\\Nitrogen", false);
                    if (hwKey_nit != null)
                    {
                        _state.PlayerName = "Nitrogen";
                        _state.Run = true;
                        if ((int)hwKey_nit.GetValue("PlayStatus", 1) > 1)
                        {
                            _state.Status = _StatusPlay.Play;
                        }
                        else
                        {
                            _state.Status = _StatusPlay.Stop;
                        }
                        _state.Title = (string)hwKey_nit.GetValue("SongTitle", "");
                        _state.Album = "";
                        _state.Artist = (string)hwKey_nit.GetValue("SongArtist", "");
                        _state.Cover = (string)hwKey_nit.GetValue("AlbumArtFilename", "");
                        _state.Volume = 1;
                        hwKey_nit.Close();
                    }
                }
            }
            catch (Exception) { }

        }

        public bool UpdateStatus()
        {
            GetState();
            return true;
        }

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(4, 2)
            };
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            try
            {
                var _dxb = rect.Width / 4;
                var _dx = 3;
                var _dy = 5;
                var _r = new Rectangle(_dx, _dy, 60, 60);
                

                _brush = new SolidBrush(_fontColor);
                _fnt = new Font(_fontName, _fontSize.ToLogic(), FontStyle.Regular);
                _pen = new Pen(Color.WhiteSmoke, 2);
                
                var _size_box = g.MeasureString(":", _fnt);
                
                _rRew = new Rectangle( _dxb / 2 + _dxb * 0 - 24, rect.Height - 48 - _dy, 48, 48);
                (new AlphaImage("WPPlayer.Images." + _typeColorIcon + "_rew.png", this.GetType().Assembly)).PaintBackground(g, _rRew);

                _rPlay = new Rectangle(_dxb / 2 + _dxb * 1 - 24, rect.Height - 48 - _dy, 48, 48);
                (new AlphaImage("WPPlayer.Images." + _typeColorIcon + "_pause.png", this.GetType().Assembly)).PaintBackground(g, _rPlay);

                _rStop = new Rectangle(_dxb / 2 + _dxb * 2 - 24, rect.Height - 48 - _dy, 48, 48);
                (new AlphaImage("WPPlayer.Images." + _typeColorIcon + "_play.png", this.GetType().Assembly)).PaintBackground(g, _rStop);

                _rNext = new Rectangle(_dxb / 2 + _dxb * 3 - 24, rect.Height - 48 - _dy, 48, 48);
                (new AlphaImage("WPPlayer.Images." + _typeColorIcon + "_next.png", this.GetType().Assembly)).PaintBackground(g, _rNext);

                _r = new Rectangle(_dx, _dy, (rect.Width - _dx * 2), ((int)_size_box.Height));
                g.DrawString(_state.Title, _fnt, _brush, _r); _r.Y += ((int)_size_box.Height) + _dy;
                g.DrawString(_state.Artist, _fnt, _brush, _r); _r.Y += ((int)_size_box.Height) + _dy;
                g.DrawString(_state.Album, _fnt, _brush, _r);

                _rCover = new Rectangle(rect.Width - _dx - 100 - 2, _dy + 2, 100, 100);
                if ((_state.Cover == "") || (!_state.Run)) { (new AlphaImage("WPPlayer.Images.folder.png", this.GetType().Assembly)).PaintBackground(g, _rCover); }
                else
                {
                    try { new AlphaImage(_state.Cover).PaintBackground(g, _rCover); }
                    catch { (new AlphaImage("WPPlayer.Images.folder.png", this.GetType().Assembly)).PaintBackground(g, _rCover); }
                }
            }
            catch
            {
                g.DrawString("Error load images", _fnt, _brush, rect); 
            }
        }

        public override bool OnClick(Point location)
        {
            if (_rCover.Contains(location))
            {
                var _s2pIntPtr = FindWindow(IntPtr.Zero, "S2P");
                var _nitIntPtr = FindWindow(IntPtr.Zero, "Nitrogen");
                if (_s2pIntPtr != null) { this.ActivateWindow(_s2pIntPtr); }
                if (_nitIntPtr != null) { this.ActivateWindow(_nitIntPtr); }
            }

            if (_rRew.Contains(location)) { SendCommand(_PlayerCommand.Previous); }
            if (_rPlay.Contains(location)) { SendCommand(_PlayerCommand.Stop); }
            if (_rStop.Contains(location)) { SendCommand(_PlayerCommand.Play); }
            if (_rNext.Contains(location)) { SendCommand(_PlayerCommand.Next); }
            return true;
        }
        
        #region Control Player

        private void ActivateWindow(IntPtr hwnd)
        {
            uint modifiedWnd = ((uint)hwnd) | 0x01;
            SetForegroundWindow((IntPtr)modifiedWnd);
        }

        private void SendS2P(IntPtr _hwnd, _PlayerCommand _command)
        {
            switch (_command)
            {
                case _PlayerCommand.Play:
                    {
                        if (_state.Status == _StatusPlay.Stop) { SendMessage(_hwnd, WM_COMMAND, (uint)_SP2Command.Play, 0); }
                        break;
                    }
                case _PlayerCommand.Stop:
                    {
                        if (_state.Status == _StatusPlay.Play) { SendMessage(_hwnd, WM_COMMAND, (uint)_SP2Command.Stop, 0); }
                        break;
                    }
                case _PlayerCommand.Previous:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_SP2Command.Previous, 0);
                        break;
                    }
                case _PlayerCommand.Next:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_SP2Command.Next, 0);
                        break;
                    }
                case _PlayerCommand.VolumeDown:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_SP2Command.VolumeDown, 0);
                        break;
                    }
                case _PlayerCommand.VolumeUp:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_SP2Command.VolumeUp, 0);
                        break;
                    }
                default: { break; }
            }
        }

        private void SendNitrogen(IntPtr _hwnd, _PlayerCommand _command)
        {
            switch (_command)
            {
                case _PlayerCommand.Play:
                    {
                        if (_state.Status == _StatusPlay.Stop) { SendMessage(_hwnd, WM_COMMAND, (uint)_NitrogenCommand.Play, 0); }
                        break;
                    }
                case _PlayerCommand.Stop:
                    {
                        if (_state.Status == _StatusPlay.Play) { SendMessage(_hwnd, WM_COMMAND, (uint)_NitrogenCommand.Stop, 0); }
                        break;
                    }
                case _PlayerCommand.Previous:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_NitrogenCommand.Previous, 0);
                        break;
                    }
                case _PlayerCommand.Next:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_NitrogenCommand.Next, 0);
                        break;
                    }
                case _PlayerCommand.VolumeDown:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_NitrogenCommand.VolumeDown, 0);
                        break;
                    }
                case _PlayerCommand.VolumeUp:
                    {
                        SendMessage(_hwnd, WM_COMMAND, (uint)_NitrogenCommand.VolumeUp, 0);
                        break;
                    }
                default: { break; }
            }
        }

        private void SendCommand(_PlayerCommand _command)
        {

            var _s2pIntPtr = FindWindow(IntPtr.Zero, "S2P");
            var _nitIntPtr = FindWindow(IntPtr.Zero, "Nitrogen");

            if (_s2pIntPtr != IntPtr.Zero)
            {
                SendS2P(_s2pIntPtr, _command);
                _state.Run = true;
                UpdateStatus();
                return;
            }
            else
            {
                _state.Run = false;
            }

            if (_nitIntPtr != IntPtr.Zero)
            {
                SendNitrogen(_nitIntPtr, _command);
                _state.Run = true;
                UpdateStatus();
                return;
            }
            else
            {
                _state.Run = false;
            }


        }
        #endregion

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null) _updateTimer = new ThreadTimer(2000, () => 
                        {
                            UpdateStatus(); 
                            ForceUpdate();
                        });
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        protected override bool GetDoExitAnimation() { return false; }
        protected override void PaintIcon(Graphics g, Rectangle rect) { }
        protected override void PaintCaption(Graphics g, Rectangle rect) { }

        #region  Tile Parameter

        private Boolean gettypeicon()
        {
            if (_typeColorIcon == "white") { return true; } else { return false; }
        }

        [TileParameter]
        public Boolean WhiteIcon
        {
            get { return gettypeicon(); }
            set
            {
                if (value) { _typeColorIcon = "white"; } else { _typeColorIcon = "black"; }
                NotifyPropertyChanged("DateFormat");
            }
        }

        [TileParameter]
        public String FontName { get { return _fontName; } set { _fontName = value; NotifyPropertyChanged("FontName"); } }

        public int FontNameIndex
        {
            get { for (var i = 0; i < _fontNames.Count; i++) if (_fontNames[i] == FontName) return i; return 0; }
            set { if ((value >= 0) && (value < _fontNames.Count)) FontName = _fontNames[value]; }
        }
        [TileParameter]
        public int FontSize { get { return _fontSize; } set { _fontSize = value; NotifyPropertyChanged("FontSize"); } }
        public int FontSizeIndex
        {
            get { for (var i = 0; i < _fontSizes.Count; i++) if (_fontSizes[i] == Convert.ToString(FontSize)) return i; return 0; }
            set { if ((value >= 0) && (value < _fontSizes.Count)) FontSize = Convert.ToInt32(_fontSizes[value]); }
        }
        [TileParameter]
        public int FontColor { get { return _fontColor.ToArgb(); } set { _fontColor = Color.FromArgb(value); } }
        public Color FontColorIndex { get { return _fontColor; } set { _fontColor = value; NotifyPropertyChanged("FontColor"); } }

        #endregion

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var WhiteIconflagControl = new FlagSettingsControl { Caption = "Player white icon", };
            controls.Add(WhiteIconflagControl);
            bindingManager.Bind(this, "WhiteIcon", WhiteIconflagControl, "Value", true);

            var fontNameControl = new SelectSettingsControl { Caption = "Font Name", Items = _fontNamesData, };
            controls.Add(fontNameControl);
            bindingManager.Bind(this, "FontNameIndex", fontNameControl, "SelectedIndex", true);

            var fontSizeControl = new SelectSettingsControl { Caption = "Font Size", Items = _fontSizesData, };
            controls.Add(fontSizeControl);
            bindingManager.Bind(this, "FontSizeIndex", fontSizeControl, "SelectedIndex", true);

            var fontColorControl = new ColorSettingsControl(true) { Caption = "Font Color", };
            controls.Add(fontColorControl);
            bindingManager.Bind(this, "FontColorIndex", fontColorControl, "Value", true);

            
            
            
            
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }
            foreach (var control in controls)
                if (control.Name.Contains("Caption"))
                {
                    controls.Remove(control);
                    break;
                }

            return controls;
        }

    }

}
