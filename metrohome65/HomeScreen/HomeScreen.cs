using System.Drawing;
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
        private readonly TilesGrid _tilesGrid;
        bool _showingTiles = true;
        // system state for receiving notifications about system events
        private readonly SystemState _systemState = new SystemState(0);

        private const int ScreenWidth = 480;
        private const int ArrowPadding = 18;
        private const int ArrowPos1 = 410;
        private const int ArrowPos2 = ScreenWidth + ArrowPadding;
        private const int GridWidth = 480;

        public HomeScreen() : base(false)
        {
            theForm.Menu = null;

            Control.EntranceDuration = 500;
            
            // холст страницы с плитками
            _homeScreenCanvas = new Canvas 
            { 
                Size = new Size(875, Size.Height),
                Location = new Point(0, 0)
            };

            // прокрутчик холста плиток
            _tilesGrid = new TilesGrid(Control)
            {
                Size = new Size(GridWidth, Size.Height),
                Location = new Point(0, 0),
            };
            _homeScreenCanvas.AddElement(_tilesGrid);

            // стрелка переключатель страниц
            _switchArrow = new Arrow {
                Location = new Point(ArrowPos1, 10),
                TapHandler = TapOnArrow,
            };
            _homeScreenCanvas.AddElement(_switchArrow);


            // список программ
            var programsPosX = ArrowPos2 + _switchArrow.Size.Width + ArrowPadding;
            var programsSv = new ProgramsMenu {
                Location = new Point(programsPosX, 5),
                Size = new Size(2 * ScreenWidth - programsPosX, Size.Height - 5),
            };
            _homeScreenCanvas.AddElement(programsSv);

            Control.AddElement(_homeScreenCanvas);

            _homeScreenCanvas.FlickHandler = Flick;

            _systemState.Changed += OnSystemStateChanged;
            TheForm.Deactivate += (s, e) => OnDeactivate();

        }

        public bool TapOnArrow(Point p)
        {
            SwitchScreen(!_showingTiles);
            return true;
        }

        private bool Flick(Point from, Point to, int millisecs, Point start)
        {
            SwitchScreen(to.X - from.X > 0);
            return true;
        }

        private void SwitchScreen(bool showTiles)
        {
            OnDeactivate();

            var homeScreenPosFrom = _homeScreenCanvas.Location.X;
            var homeScreenPosTo = showTiles ? 0 : -ScreenWidth;
            var arrowPosTo = showTiles ? ArrowPos1 : ArrowPos2;

            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = 500,
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
            });

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

        protected override void OnActivated()
        {
            _tilesGrid.Active = true;
            base.OnActivated();
        }

        private void OnDeactivate()
        {
            _tilesGrid.Active = false;
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
