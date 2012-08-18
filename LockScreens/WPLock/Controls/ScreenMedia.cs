using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;

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
using MetroHome65.WPLock;

namespace MetroHome65.WPLock.Controls
{
    
    
    
    public class ScreenMedia : Canvas
    {
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
        private int _timerStep = 0;
        private int _timerTime = 10;
        private bool _visible = true;
        private _PlayerState _state;
        private TransparentButton _buttonPrev;
        private TransparentButton _buttonPause;
        private TransparentButton _buttonPlay;
        private TransparentButton _buttonNext;
        private TextElementAdvanced _textTitle;
        private TextElementAdvanced _textAlbum;
        private TextElementAdvanced _textArtist;
        #endregion

        #region Public
        public string _typeColorIcon = "white";                         // Опубликовать дать возможность смены
        private string _fontName = "Segoe WP";                           // Опубликовать дать возможность смены
        private int _fontSize = 8;                                       // Опубликовать дать возможность смены
        public Color _fontColor = MetroTheme.PhoneForegroundBrush;      // Опубликовать дать возможность смены
        public bool Visible { get { return _visible; } }
        #endregion


        public ScreenMedia(Size _size, bool _white)
        {
            this.Size = _size;

            _state.Title = "";
            _state.Album = "";
            _state.Artist = "";
            _state.Cover = "";
            _state.Status = _StatusPlay.Unknown;
            _state.Run = false;
            _state.Volume = 0;
            _state.PlayerName = "";

            _typeColorIcon = "white";
            if (!_white) _typeColorIcon = "black";

            var _dx = _size.Width / 4;

            this.AddElement(_buttonPrev = new TransparentButton(_typeColorIcon + "_rew.png")
            {
                Location = new Point(_dx / 2 + _dx * 0 - 24, _size.Height - 48),
                Size = new Size(48,48),
                TapHandler = p => OnButtonTap(0),
            });

            this.AddElement(_buttonPause = new TransparentButton(_typeColorIcon + "_pause.png")
            {
                Location = new Point(_dx / 2 + _dx * 1 - 24, _size.Height - 48),
                Size = new Size(48, 48),
                TapHandler = p => OnButtonTap(1),
            });

            this.AddElement(_buttonPlay = new TransparentButton(_typeColorIcon + "_play.png")
            {
                Location = new Point(_dx / 2 + _dx * 2 - 24, _size.Height - 48),
                Size = new Size(48, 48),
                TapHandler = p => OnButtonTap(2),
            });

            this.AddElement(_buttonNext = new TransparentButton(_typeColorIcon + "_next.png")
            {
                Location = new Point(_dx / 2 + _dx * 3 - 24, _size.Height - 48),
                Size = new Size(48, 48),
                TapHandler = p => OnButtonTap(3),
            });

            var _style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            var _height = FleuxApplication.DummyDrawingGraphics.Style(_style).CalculateMultilineTextHeight("0", 100);

            this.AddElement(_textTitle = new TextElementAdvanced("")
            {
                Location = new Point(10, _buttonNext.Location.Y - _height * 3),
                Size = new Size(_size.Width - 20, _height),
                Style = _style,
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Center,
            });

            this.AddElement(_textAlbum = new TextElementAdvanced("")
            {
                Location = new Point(10, _buttonNext.Location.Y - _height * 1),
                Size = new Size(_size.Width - 20, _height),
                Style = _style,
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Center,
            });

            this.AddElement(_textArtist = new TextElementAdvanced("")
            {
                Location = new Point(10, _buttonNext.Location.Y - _height * 2),
                Size = new Size(_size.Width - 20, _height),
                Style = _style,
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Center,
            });
            this.TapHandler = OnTap;
            this.HoldHandler = OnHold;
        
        }
        private bool OnTap(Point p)
        {
            if (!_visible) return true;

            return true;
        }
        private bool OnHold(Point p)
        {
            if (!_visible) return true;
            var _s2pIntPtr = FindWindow(IntPtr.Zero, "S2P");
            var _nitIntPtr = FindWindow(IntPtr.Zero, "Nitrogen");
            if (_s2pIntPtr != null) { this.ActivateWindow(_s2pIntPtr); }
            if (_nitIntPtr != null) { this.ActivateWindow(_nitIntPtr); }
            return true;

        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if(this._visible) base.Draw(drawingGraphics);
        }

        private bool OnButtonTap(int _indexButton)
        {
            if (!_visible) return true;
            switch (_indexButton)
            {
                case 0: // Button Previous
                    {
                        SendCommand(_PlayerCommand.Previous);
                        break;
                    }
                case 1: // Button Pause
                    {
                        SendCommand(_PlayerCommand.Stop);
                        break;
                    }
                case 2: // Button Play
                    {
                        SendCommand(_PlayerCommand.Play);
                        break;
                    }
                case 3: // Button Next
                    {
                        SendCommand(_PlayerCommand.Next);
                        break;
                    }
                case 4: // Button Volume Down
                    {
                        SendCommand(_PlayerCommand.VolumeDown);
                        break;
                    }
                case 5: // Button Volume Up
                    {
                        SendCommand(_PlayerCommand.VolumeUp);
                        break;
                    }
            }
            return true;
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
                _timerStep = _timerTime - 2;
                Active();
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
                _timerStep = _timerTime - 2;
                Active();
                return;
            }
            else
            {
                _state.Run = false;
            }


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
        private void UpdateState()
        {
            _textTitle.Text = _state.Title;
            _textAlbum.Text = _state.Album;
            _textArtist.Text = _state.Artist;
        }
        
        public void Active()
        {
            GetState();
            if (_state.Run)
            {
                _visible = true;
            }
            else
            {
                _visible = false;
            }
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

        public void HardwareKeys(KeyBoardInfo _kbInfo)
        {
            switch (_kbInfo.vkCode)
            {
                case 117:
                    {
                        SendCommand(_PlayerCommand.VolumeUp);
                        Active();
                        break;
                    }
                case 118:
                    {
                        SendCommand(_PlayerCommand.VolumeDown);
                        Active();
                        break;
                    }
                default:
                    {
                        Active();
                        break;
                    }
            }
        }

        public void UpdateTheme()
        {
            _buttonPrev.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_rew.png", this.GetType().Assembly);
            _buttonPause.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_pause.png", this.GetType().Assembly);
            _buttonPlay.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_play.png", this.GetType().Assembly);
            _buttonNext.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_next.png", this.GetType().Assembly);

            _textTitle.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor); _textTitle.Update();
            _textAlbum.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);  _textAlbum.Update();
            _textArtist.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor); _textArtist.Update();

        }
        private void ActivateWindow(IntPtr hwnd)
        {
            uint modifiedWnd = ((uint)hwnd) | 0x01;
            SetForegroundWindow((IntPtr)modifiedWnd);
        }

    }
}
