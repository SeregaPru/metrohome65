using System;
using System.Collections.Generic;
using System.Drawing;
using MetroHome65.Interfaces;
using Microsoft.WindowsMobile.Status;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Animations;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.HomeScreen.ProgramsMenu;

namespace MetroHome65.HomeScreen
{
    public class HomeScreen : FleuxControlPage
    {
        private readonly UIElement _lockScreen;
        private readonly Canvas _homeScreenCanvas;
        private readonly Arrow _switchArrow;
        private IAnimation _animation;

        private readonly List<UIElement> _pages = new List<UIElement>();
        private int _curPage;
        
        // system state for receiving notifications about system events
        private readonly SystemState _systemState = new SystemState(0);

        private const int ArrowPadding = 18;
        private const int ArrowPos1 = ScreenConsts.ScreenWidth + 410;
        private const int ArrowPos2 = ScreenConsts.ScreenWidth * 2 + ArrowPadding;
        

        public HomeScreen() : base(false)
        {
            theForm.Menu = null;
            theForm.Text = "";

            Control.EntranceDuration = 300;

            ReadThemeSettings();

            // фон окна
            var background = new HomeScreenBackground()
            {
                Location = new Point(0, 0),
            };
            Control.AddElement(background);

            
            // холст главной страницы
            _homeScreenCanvas = new Canvas 
                                    {
                                        Size = new Size(ScreenConsts.ScreenWidth * 2, Size.Height),
                                        Location = new Point(0, 0),
                                    };

            // экран блокировки
            _lockScreen = new LockScreen.LockScreen();
            AddPage(_lockScreen, 0);

            // прокрутчик холста плиток
            //!! todo - потом вместо контрола передавать холст _homeScreenCanvas
            var tilesGrid = new TilesGrid.TilesGrid(Control);
            tilesGrid.OnExit = Exit;
            AddPage(tilesGrid, 1);

            // стрелка переключатель страниц
            _switchArrow = new Arrow 
                               {
                                   Location = new Point(ArrowPos1, 10),
                                   TapHandler = p => { CurrentPage = (_curPage == 1) ? 2 : 1; return true; },
                               };
            _homeScreenCanvas.AddElement(_switchArrow);

            // список программ
            var programsSv = new ProgramsMenuPage();
            AddPage(programsSv, 2);

            Control.AddElement(_homeScreenCanvas);

            _homeScreenCanvas.FlickHandler = Flick;

            _systemState.Changed += OnSystemStateChanged;
            TheForm.Deactivate += (s, e) => OnDeactivate();

            CurrentPage = 1;
        }

        private void Exit()
        {
            OnDeactivate();
            TheForm.Close();
        }

        private void AddPage(UIElement page, int position)
        {
            page.Size = new Size(ScreenConsts.ScreenWidth, ScreenConsts.ScreenHeight);
            page.Location = new Point(position * ScreenConsts.ScreenWidth, 0);

            _homeScreenCanvas.AddElement(page);
            _pages.Insert(position, page);
        }

        private void ReadThemeSettings()
        {
            (new MainSettingsProvider()).ReadSettings();
        }

        private bool Flick(Point from, Point to, int millisecs, Point start)
        {
            if (Math.Abs(from.X - to.X) > Math.Abs(from.Y - to.Y))
            {
                CurrentPage = CurrentPage + ((to.X - from.X > 0) ? -1 : 1);
                return true;
            }
            return false;
        }

        private int CurrentPage
        {
            get { return _curPage; }
            set
            {
                if ((_curPage == value) || (value < 0) || (value >= _pages.Count))
                    return;

                var prevPage = _curPage;
                _curPage = value;

                SwitchScreen(prevPage, _curPage);
            }
        }
        private void SwitchScreen(int fromPage, int toPage)
        {
            OnDeactivate();

            //var animateArrow = (toPage + fromPage >= 2);
            //var ArrowPosFrom = (toPage == 1) ? ArrowPos2 : ArrowPos1;
            var ArrowPosTo = (toPage == 1) ? ArrowPos1 : ArrowPos2;

            var _screenAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
                                       {
                                           Duration = 300,
                                           From = _pages[fromPage].Location.X,
                                           To = _pages[toPage].Location.X,
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
                                                                     if (toPage == 1)
                                                                         _switchArrow.Next();
                                                                     else
                                                                         _switchArrow.Prev();

                                                                     _switchArrow.Location = new Point(ArrowPosTo, _switchArrow.Location.Y);
                                                                     _switchArrow.Update();

                                                                     OnActivated();
                                                                 },
                                       };

            StoryBoard.BeginPlay(_screenAnimation);
        }

        protected override void OnActivated()
        {
            if (_pages[_curPage] is IActive)
            {
                var active = _pages[_curPage] as IActive;
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
            if (_animation != null)
            {
                _animation.Cancel();
            }
            foreach (var page in _pages)
            {
                if (page is IActive)
                    (page as IActive).Active = false;
            }
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

    }
}
