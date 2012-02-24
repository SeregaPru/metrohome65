using System;
using System.Drawing;
using MetroHome65.Widgets;
using Microsoft.WindowsMobile.Status;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Animations;

namespace MetroHome65.HomeScreen
{
    public class HomeScreen : FleuxControlPage
    {
        private readonly Canvas _homeScreenCanvas;
        private readonly Arrow _switchArrow;
        private readonly UIElement _tilesGrid;
        private readonly UIElement _programsSv;
        private IAnimation _animation;
        bool _showingTiles = true;
        // system state for receiving notifications about system events
        private readonly SystemState _systemState = new SystemState(0);

        private const int ScreenWidth = 480;
        private const int ArrowPadding = 18;
        private const int ArrowPos1 = 410;
        private const int ArrowPos2 = ScreenWidth + ArrowPadding;
        private const int GridWidth = ScreenWidth;

        public HomeScreen() : base(false)
        {
            theForm.Menu = null;
            theForm.Text = "";

            Control.EntranceDuration = 500;

            var mainSettings = ReadMainSettings();

            // фон окна
            var background = new HomeScreenBackground(mainSettings)
            {
                Location = new Point(0, 0),
            };
            Control.AddElement(background);

            
            // холст страницы с плитками
            _homeScreenCanvas = new Canvas 
            {
                Size = new Size(ScreenWidth * 2, Size.Height),
                Location = new Point(0, 0)
            };

            // прокрутчик холста плиток
            _tilesGrid = new TilesGrid(Control)
            {
                Location = new Point(0, 0),
                Size = new Size(GridWidth, Size.Height),
            };
            _homeScreenCanvas.AddElement(_tilesGrid);

            // стрелка переключатель страниц
            _switchArrow = new Arrow {
                Location = new Point(ArrowPos1, 10),
                TapHandler = (p) => { SwitchScreen(!_showingTiles); return true; },
            };
            _homeScreenCanvas.AddElement(_switchArrow);

            // список программ
            var programsPosX = ArrowPos2 + _switchArrow.Size.Width + ArrowPadding;
            _programsSv = new ProgramsMenu(mainSettings) {
                Location = new Point(programsPosX, 5),
                Size = new Size(2 * ScreenWidth - programsPosX, Size.Height - 5),
            };
            _homeScreenCanvas.AddElement(_programsSv);

            Control.AddElement(_homeScreenCanvas);

            _homeScreenCanvas.FlickHandler = Flick;

            _systemState.Changed += OnSystemStateChanged;
            TheForm.Deactivate += (s, e) => OnDeactivate();

        }

        private MainSettings ReadMainSettings()
        {
            return (new MainSettingsProvider()).Settings;
        }

        private bool Flick(Point from, Point to, int millisecs, Point start)
        {
            if (Math.Abs(from.X - to.X) > Math.Abs(from.Y - to.Y))
            {
                SwitchScreen(to.X - from.X > 0);
                return true;
            }
            else
                return false;
        }

        private void SwitchScreen(bool showTiles)
        {
            OnDeactivate();

            var homeScreenPosFrom = _homeScreenCanvas.Location.X;
            var homeScreenPosTo = showTiles ? 0 : -ScreenWidth;
            var arrowPosTo = showTiles ? ArrowPos1 : ArrowPos2;

            _animation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = 300,
                From = homeScreenPosFrom,
                To = homeScreenPosTo,
                OnAnimation = v =>
                {
                    _homeScreenCanvas.Location = new Point(v, 0);
                    _homeScreenCanvas.Update();
                },
                OnAnimationStop = () =>
                                      {
                                          if (showTiles)
                                          {
                                              _switchArrow.Next();
                                          }
                                          else
                                          {
                                              _switchArrow.Prev();
                                          }
                                          _switchArrow.Location = new Point(arrowPosTo, _switchArrow.Location.Y);
                                          _switchArrow.Update();

                                          _showingTiles = showTiles;

                                          if (showTiles)
                                              OnActivated();
                                      }
            };
            StoryBoard.BeginPlay(_animation);

            /*
            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 250,
                From = _switchArrow.Location.X,
                To = arrowPosTo,
                OnAnimation = v =>
                {
                    _switchArrow.Location = new Point(v, _switchArrow.Location.Y);
                    _switchArrow.Update();
                }
            });
            */

        }

        /// <summary>
        /// cancel all current animations, including scroll in scrollviews.
        /// to stop scrolling - emulate press on scrollview
        /// </summary>
        private void CancelAnimation()
        {
            if (_animation != null)
            {
                _animation.Cancel();
            }
            (_tilesGrid as IActive).Active = false;
            (_programsSv as IActive).Active = false;
        }

        protected override void OnActivated()
        {
            if (_showingTiles)
                (_tilesGrid as IActive).Active = true;
            base.OnActivated();
        }

        private void OnDeactivate()
        {
            CancelAnimation();
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
