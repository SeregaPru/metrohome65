using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Fleux.Controls.Gestures;
using Fleux.Core;
using MetroHome65.Controls;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.LockScreen;
using MetroHome65.Routines.File;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.UIControls;
using MetroHome65.Tile;
using Microsoft.WindowsMobile.Status;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Animations;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.HomeScreen.ProgramsMenu;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.HomeScreen
{
    public class HomeScreen : FleuxControlPage
    {
        private readonly Canvas _homeScreenCanvas;

        private readonly List<UIElement> _sections = new List<UIElement>();
        private int _fullScreenCount = 0;
        private int _curSection;
        private TileTheme _tileTheme;
        private UIElement _switchArrowNext;
        private UIElement _switchArrowBack;
        
        // system state for receiving notifications about system events
        private readonly SystemState _systemState = new SystemState(0);
        
        private int ArrowPosNext { get { return Size.Width + _tileTheme.ArrowNextPosX; } }
        private int ArrowPosBack { get { return Size.Width * 2 + _tileTheme.ArrowPrevPosX; } }


        public HomeScreen() : base(false)
        {
            theForm.Menu = null;
            theForm.Text = "";

            Control.EntranceDuration = 100;

            ReadThemeSettings();

            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            _tileTheme = mainSettings.GetTileTheme();
            if (mainSettings.FullScreen)
                SwitchFullScreen(true); //!! do not decrease counter when start in normal mode

            // фон окна
            var background = new ThemedBackground { Location = new Point(0, 0), };
            Control.AddElement(background);
            TinyIoCContainer.Current.Register<ScaledBackground>(background);

            // загрузчик плагинов
            TinyIoCContainer.Current.Register<IPluginManager>(new PluginManager());
            
            // холст главной страницы
            _homeScreenCanvas = new Canvas 
                                    {
                                        Size = this.Size,
                                        Location = new Point(0, 0),
                                    };

            // экран блокировки
            var lockScreen = new LockScreenManager();
            AddSection(lockScreen, 0);

            // прокрутчик холста плиток
            var tilesGrid = new TilesGrid.TilesGrid(_tileTheme)
                                {
                                    OnExit = ExitApp,
                                    OnShowMainSettings = ShowMainSettings,
                                };
            AddSection(tilesGrid, 1);

            // стрелка переключатель страниц
            _switchArrowNext = new ThemedImageButton("next")
            {
                Location = new Point(ArrowPosNext, _tileTheme.ArrowPosY),
                TapHandler = p => { CurrentSection = 2; return true; },
            };
            _homeScreenCanvas.AddElement(_switchArrowNext);

            _switchArrowBack = new ThemedImageButton("back")
            {
                Location = new Point(ArrowPosBack, _tileTheme.ArrowPosY),
                TapHandler = p => { CurrentSection = 1; return true; },
            };
            _homeScreenCanvas.AddElement(_switchArrowBack);

            // список программ
            var programsSv = new ProgramsMenuPage();
            AddSection(programsSv, 2);

            Control.AddElement(_homeScreenCanvas);

            _homeScreenCanvas.FlickHandler = Flick;

            _systemState.Changed += OnSystemStateChanged;
            TheForm.Deactivate += (s, e) => OnDeactivate();

            // deactivate all other pages but first
            CurrentSection = 1;

            // subscribe to events - show page and change main settings
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<ShowPageMessage>(msg => OnShowPage(msg.Page));
            messenger.Subscribe<SettingsChangedMessage>(OnSettingsChanged);
            messenger.Subscribe<FullScreenMessage>(OnFullScreen);
        }

        /// <summary>
        /// Shows user page with entrance animation
        /// </summary>
        /// <param name="page"></param>
        private void OnShowPage(FleuxPage page)
        {
            NavigateTo(page);
        }

        private void AddSection(UIElement section, int position)
        {
            section.Size = new Size(this.Size.Width - 2, this.Size.Height);
            section.Location = new Point(position * this.Size.Width + 1, 0);

            _homeScreenCanvas.AddElement(section);
            _sections.Insert(position, section);
        }

        private void ReadThemeSettings()
        {
            (new MainSettingsProvider()).ReadSettings();
        }

        private bool Flick(Point from, Point to, int millisecs, Point start)
        {
            if (GesturesEngine.IsHorizontal(from, to) && 
                (Math.Abs(to.X - from.X) > ScreenConsts.ScreenWidth / 10))
            {
                CurrentSection = CurrentSection + ((to.X - from.X > 0) ? -1 : 1);
                return true;
            }
            return false;
        }

        private int CurrentSection
        {
            get { return _curSection; }
            set
            {
                if ((_curSection == value) || (value < 0) || (value >= _sections.Count))
                    return;

                var prevPage = _curSection;
                _curSection = value;

                SwitchScreen(prevPage, _curSection);
            }
        }

        private void SwitchScreen(int fromSection, int toSection)
        {
            OnDeactivate();

            var oldSection = _sections[fromSection] as IVisible;
            if (oldSection != null) oldSection.Visible = false;

            //var animateArrow = (toPage + fromPage >= 2);
            //var ArrowPosFrom = (toPage == 1) ? ArrowPos2 : ArrowPos1;
            //var arrowPosTo = (toPage == 1) ? ArrowPos1 : ArrowPos2;

            var screenAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
                                       {
                                           Duration = 300,
                                           From = _sections[fromSection].Location.X,
                                           To = _sections[toSection].Location.X,
                                           OnAnimation = v =>
                                                             {
                                                                 _homeScreenCanvas.Location = new Point(-v, 0);
                                                                 _homeScreenCanvas.Update();

                                                                 /* пока слишком медленно
                                                                 if (animateArrow)
                                                                 {
                                                                     var ArrowPos = ArrowPosFrom + (ArrowPosTo - ArrowPosFrom) * (v - _pages[fromPage].Location.X) / (_pages[toPage].Location.X - _pages[fromPage].Location.X);
                                                                     _switchArrow.Location = new Point(ArrowPos, _switchArrow.Location.Y);
                                                                     _switchArrow.Update();
                                                                 }
                                                                 */
                                                             },
                                           OnAnimationStop = () =>
                                                                 {
                                                                     /*
                                                                     if (toPage == 1)
                                                                         _switchArrow.Next();
                                                                     else
                                                                         _switchArrow.Prev();
                                                                     
                                                                     _switchArrow.Location = new Point(arrowPosTo, _switchArrow.Location.Y);
                                                                     _switchArrow.Update();
                                                                     */

                                                                     var newSection = _sections[toSection] as IVisible;
                                                                     if (newSection != null) newSection.Visible = true;
                                                                     OnActivated();
                                                                 },
                                       };

            StoryBoard.BeginPlay(screenAnimation);
        }

        protected override void OnActivated()
        {
            if (_sections[_curSection] is IActive)
            {
                var active = _sections[_curSection] as IActive;
                if (active != null) 
                    active.Active = true;
            }
            base.OnActivated();
        }

        /// <summary>
        /// cancel all current animations, including scroll in scrollviews.
        /// to stop scrolling - emulate press on scrollview
        /// </summary>
        private void OnDeactivate()
        {
            foreach (var page in _sections)
            {
                if (page is IActive)
                    (page as IActive).Active = false;
            }
        }

        private void ExitApp()
        {
            OnDeactivate();
            TheForm.Close();
        }

        // handler for system state change event
        private void OnSystemStateChanged(object sender, ChangeEventArgs eventArgs)
        {
            var str = SystemState.ActiveApplication;
            if (str.Length > 6)
                str = str.Substring(str.Length - 7, 7);
            if (str.ToLower() == "desktop")
                theForm.Activate();
        }

        /// <summary>
        /// shows main settings dialog
        /// </summary>
        private static void ShowMainSettings()
        {
            try
            {
                var mainSettingsForm = new MainSettingsForm(MainSettings.Clone());
                mainSettingsForm.OnApplySettings += (sender, settings) => 
                    (new MainSettingsProvider()).WriteSettings();

                var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
                messenger.Publish(new ShowPageMessage(mainSettingsForm));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "Tile settings dialog error");
            }
        }

        protected virtual void OnSettingsChanged(SettingsChangedMessage settingsChangedMessage)
        {
            if (settingsChangedMessage.PropertyName == "TileTheme")
            {
                _tileTheme = settingsChangedMessage.Value as TileTheme;
                _switchArrowNext.Location = new Point(ArrowPosNext, _tileTheme.ArrowPosY);
                _switchArrowBack.Location = new Point(ArrowPosBack, _tileTheme.ArrowPosY);

                _homeScreenCanvas.Update();
            }
        }

        /// <summary>
        /// Handler for event - swith application to fullscreen mode and back
        /// </summary>
        /// <param name="fullScreenMessage"></param>
        private void OnFullScreen(FullScreenMessage fullScreenMessage)
        {
            Action action = () => SwitchFullScreen(fullScreenMessage.FullScreen);

            if (theForm.InvokeRequired)
                theForm.Invoke(action);
            else
                action();
        }

        private void SwitchFullScreen(bool fullScreen)
        {
            var prevState = theForm.WindowState;
            if (fullScreen)
            {
                if (_fullScreenCount == 0)
                    theForm.WindowState = FormWindowState.Maximized;
                _fullScreenCount++;
            }
            else
            {
                _fullScreenCount--;
                if (_fullScreenCount == 0)
                    theForm.WindowState = FormWindowState.Normal;
            }

            if (prevState != theForm.WindowState)
                foreach (var section in _sections)
                    section.Size = new Size(this.Size.Width - 2, this.Size.Height);
        }

    }
}
