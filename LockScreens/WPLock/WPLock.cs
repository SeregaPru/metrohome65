using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Fleux.Animations;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.UIControls;
using TinyIoC;
using TinyMessenger;
using MetroHome65.WPLock.Controls;

namespace MetroHome65.WPLock
{
    /// <summary>
    /// Lock screen with background and clock with configured date format
    /// </summary>
    [LockScreenInfo("WP lock screen")]
    public class WPLock : Canvas, IActive, ILockScreen
    {
        #region AllKeys
        [DllImport("coredll.dll", SetLastError = true)]
        static extern bool AllKeys(bool bAllKeys);
        private HookKeys hook;
        private HookEventArgs _globalArgs = new HookEventArgs();
        private KeyBoardInfo _globalkey = new KeyBoardInfo();
        #endregion
       
        const int topbarHeight = 32;
        const int bottomOffset = 10;
        const int leftOffset = 20;
        const int rightOffset = 10;

        private Boolean _enabledScreenAnimation = true;

        private ScreenTopBar _topBar;
        private ScreenMedia _mediaPlayer;
        private ScreenStatus _statusPhone;
        private ScreenAppointment _appointment;
        
        private TextElement _lblClock;

        private ScaledBackground _background;

        private ThreadTimer _updateTimer;
        

        private readonly TextStyle _style = new TextStyle(
            MetroTheme.PhoneFontFamilyNormal,
            22.ToLogic(),
            MetroTheme.PhoneForegroundBrush);


        [LockScreenSettings]
        public WPLockSettings Settings { get; set; }


        public WPLock()
        {
            CreateSettings();
            CreateVisual();

            hook = new HookKeys();
            hook.HookEvent += new HookKeys.HookEventHandler(HookEvent);
            hook.Start(); // Проверка 
            AllKeys(true);
        }


        public void HookEvent(HookEventArgs e, KeyBoardInfo keyBoardInfo)
        {
            _globalkey = keyBoardInfo;  //  Отсалось для проверки
            _globalArgs = e;            //  Отсалось для проверки
            _mediaPlayer.HardwareKeys(keyBoardInfo);
        }



        private void CreateSettings()
        {
            Settings = new WPLockSettings();
            
        }

        private void CreateVisual()
        {
            _background = new ScaledBackground(Settings.Background);
            AddElement(_background);

            this.AddElement(_topBar = new ScreenTopBar(new Size(ScreenConsts.ScreenWidth.ToLogic(), topbarHeight), true)
            {
                Location = new Point(0,0),
            });

            this.AddElement(_mediaPlayer = new ScreenMedia(new Size(ScreenConsts.ScreenWidth.ToLogic(), 150), true)
            {
                Location = new Point(0,topbarHeight),
            });

            this.AddElement(_statusPhone = new ScreenStatus(new Size(ScreenConsts.ScreenWidth.ToLogic(), 80), true)
            {
                Location = new Point(0, this.Size.Height - 80 - bottomOffset),
            });


            this.AddElement(_appointment = new ScreenAppointment(new Size(ScreenConsts.ScreenWidth.ToLogic(), 10), true)
            {
                Location = new Point(0, this._statusPhone.Location.Y),
            });
            _appointment.Location = new Point(0, this._statusPhone.Location.Y - _appointment.Size.Height); 
            
            var lineHeight = FleuxApplication.DummyDrawingGraphics.Style(_style).CalculateMultilineTextHeight("0", 100);

            this.AddElement(_lblClock = new TextElement(GetText())
            {
                Style = _style,
                AutoSizeMode = TextElement.AutoSizeModeOptions.None,
                Size = new Size(ScreenConsts.ScreenWidth.ToLogic() - leftOffset - rightOffset, lineHeight * 3),
                Location = new Point(leftOffset, _appointment.Location.Y - bottomOffset - lineHeight * 3),
            });               
            

            this.TapHandler = OnTap;
            _topBar.Active();
            _mediaPlayer.Active();
            UpdateTime();
            _appointment.Active();
            _statusPhone.Active();

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<FullScreenMessage>(OnFullScreen);
                        

        }

        private void UpdateTime()
        {
            _lblClock.Text = GetText(); // do not call update because change text property calls update inside
        }

        private string GetText()
        {
            return DateTime.Now.ToString(Settings.TimeFormat + "\n" + Settings.DateFormat.Replace(", ", "\n"));
        }

        private void UpdateScreen()
        {
            _enabledScreenAnimation = !_enabledScreenAnimation;
            _topBar.Timer();
            _mediaPlayer.Timer();
            UpdateTime();
            _appointment.Timer();
            _statusPhone.Timer();

        }
        
        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(1000, UpdateScreen);
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }


        /// <summary>
        /// when tap to lockscreen, plays animation
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool OnTap(Point arg)
        {
            if (_mediaPlayer.Visible)
            {
                if (arg.Y < (_mediaPlayer.Location.Y + _mediaPlayer.Size.Height + 50) && (arg.Y > _mediaPlayer.Location.Y))
                {
                    return true;
                }
            }
            if (!_enabledScreenAnimation) { return true; }

            var screenAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = 1000,
                From = 100,
                To = 0,
                OnAnimation = v =>
                {
                    Location = new Point(-(int)(Math.Round(Math.Abs(Math.Sin(v / 10.0) * v))),0);
                    Update();
                },
                OnAnimationStop = () => { Location = new Point(0, 0); },
            };
            StoryBoard.BeginPlay(screenAnimation);

            return true;
        }


        public void ApplySettings(ILockScreenSettings settings)
        {
            Settings = settings as WPLockSettings;
            _background.Image = Settings.Background;

            // Top Bar
            _topBar._fontColor = Color.FromArgb(Settings.TopBarFontColor);
            _topBar._batteryColor = Settings.TopBarColorBattery;
            if (Settings.TopBarWhiteIcon) { _topBar._typeColorIcon = "white"; } else { _topBar._typeColorIcon = "black"; } 
            
            // Player
            _mediaPlayer._fontColor = Color.FromArgb(Settings.PlayerFontColor);
            if (Settings.PlayerWhiteIcon) { _mediaPlayer._typeColorIcon = "white"; } else { _mediaPlayer._typeColorIcon = "black"; } 

            // Date Time
            _style.Foreground = Color.FromArgb(Settings.DateTimeFontColor);
            _lblClock.Style = _style;
            _lblClock.Update();


            // Appointment
            _appointment._fontColor = Color.FromArgb(Settings.AppointmentFontColor);
            if (Settings.AppointmentWhiteIcon) { _appointment._typeColorIcon = "white"; } else { _appointment._typeColorIcon = "black"; }             
            //_appointment._liveHourAppointment
            //_appointment._onAppointmentShowTime

            // Status
            _statusPhone._fontColor = Color.FromArgb(Settings.StatusFontColor);
            if (Settings.StatusWhiteIcon) { _statusPhone._typeColorIcon = "white"; } else { _statusPhone._typeColorIcon = "black"; }

            _topBar.UpdateTheme();
            _mediaPlayer.UpdateTheme();
            _appointment.UpdateTheme();
            _statusPhone.UpdateTheme();
            UpdateScreen();
        }

        private void OnFullScreen(FullScreenMessage fullScreenMessage)
        {
            if (fullScreenMessage.FullScreen)
            {
                _mediaPlayer.Active();
                UpdateTime();
                _appointment.Active();
                _statusPhone.Active();
            }
            else
            {
            }
        }
    }
}