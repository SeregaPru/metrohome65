using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Fleux.Controls.Gestures;
using MetroHome65.HomeScreen.Tile;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
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
        private readonly UIElement _lockScreen;
        private readonly Canvas _homeScreenCanvas;
        private readonly Arrow _switchArrow;

        private readonly List<UIElement> _sections = new List<UIElement>();
        private int _curPage;
        
        // system state for receiving notifications about system events
        private readonly SystemState _systemState = new SystemState(0);

        private const int ArrowPos1 = ScreenConsts.ScreenWidth + TileConsts.ArrowPosX;
        private const int ArrowPos2 = ScreenConsts.ScreenWidth * 2 + TileConsts.ArrowPadding;
        

        public HomeScreen() : base(false)
        {
            TinyIoCContainer.Current.Register<FleuxControlPage>(this);

            theForm.Menu = null;
            theForm.Text = "";

            Control.EntranceDuration = 100;

            ReadThemeSettings();

            // фон окна
            var background = new HomeScreenBackground() { Location = new Point(0, 0), };
            Control.AddElement(background);

            
            // холст главной страницы
            _homeScreenCanvas = new Canvas 
                                    {
                                        Size = new Size(ScreenConsts.ScreenWidth, Size.Height),
                                        Location = new Point(0, 0),
                                    };

            // экран блокировки
            _lockScreen = new LockScreen.LockScreen();
            AddSection(_lockScreen, 0);

            // прокрутчик холста плиток
            //!! todo - потом вместо контрола передавать холст _homeScreenCanvas
            var tilesGrid = new TilesGrid.TilesGrid();
            tilesGrid.OnExit = Exit;
            AddSection(tilesGrid, 1);

            // стрелка переключатель страниц
            _switchArrow = new Arrow 
                               {
                                   Location = new Point(ArrowPos1, TileConsts.TilesPaddingTop),
                                   TapHandler = p => { CurrentPage = (_curPage == 1) ? 2 : 1; return true; },
                               };
            _homeScreenCanvas.AddElement(_switchArrow);

            // список программ
            var programsSv = new ProgramsMenuPage();
            AddSection(programsSv, 2);

            Control.AddElement(_homeScreenCanvas);

            _homeScreenCanvas.FlickHandler = Flick;

            _systemState.Changed += OnSystemStateChanged;
            TheForm.Deactivate += (s, e) => OnDeactivate();

            // subscribe to event - show page
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Subscribe<ShowPageMessage>(msg => OnShowPage(msg.Page));
        }

        /// <summary>
        /// Shows user page with entrance animation
        /// </summary>
        /// <param name="page"></param>
        private void OnShowPage(FleuxControlPage page)
        {
            NavigateTo(page);
        }

        private void AddSection(UIElement section, int position)
        {
            section.Size = new Size(ScreenConsts.ScreenWidth - 2, ScreenConsts.ScreenHeight);
            section.Location = new Point(position * ScreenConsts.ScreenWidth + 1, 0);

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
                if ((_curPage == value) || (value < 0) || (value >= _sections.Count))
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
            var arrowPosTo = (toPage == 1) ? ArrowPos1 : ArrowPos2;

            var screenAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
                                       {
                                           Duration = 300,
                                           From = _sections[fromPage].Location.X,
                                           To = _sections[toPage].Location.X,
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

                                                                     _switchArrow.Location = new Point(arrowPosTo, _switchArrow.Location.Y);
                                                                     _switchArrow.Update();

                                                                     OnActivated();
                                                                 },
                                       };

            StoryBoard.BeginPlay(screenAnimation);
        }

        protected override void OnActivated()
        {
            if (_sections[_curPage] is IActive)
            {
                var active = _sections[_curPage] as IActive;
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

        private void Exit()
        {
            OnDeactivate();
            _sections.RemoveAll(element => true);

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

    }
}
