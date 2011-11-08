using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.WindowsMobile.Status;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Animations;
using Fleux.UIElements.Panorama;
using WindowsPhone7Sample.Elements;

namespace MetroHome65.HomeScreen
{
    public class HomeScreen : FleuxControlPage
    {
        private Canvas homeScreenCanvas;
        private Arrow switchArrow;
        private TilesGrid _TilesGrid = null;
        bool _showingTiles = true;
        // system state for receiving notifications about system events
        private SystemState _SystemState = new SystemState(0);

        private int _ArrowPos1 = 410;
        private int _ArrowPos2 = 498;
        private int _ScreenWidth = 480;

        public HomeScreen()
            : base(false)
        {
            this.theForm.Menu = null;

            this.Control.EntranceDuration = 500;
            
            // холст страницы с плитками
            homeScreenCanvas = new Canvas { 
                Size = new Size(875, this.Size.Height),
                Location = new Point(0, 0)
            };


            // прокрутчик холста плиток
            _TilesGrid = new TilesGrid(this.Control)
            {
                Size = new Size(_ScreenWidth, this.Size.Height),
                Location = new Point(0, 0),
            };
            this.homeScreenCanvas.AddElement(_TilesGrid);
            

            // стрелка переключатель страниц
            this.switchArrow = new Arrow()
            {
                Location = new Point(_ArrowPos1, 10),
                TapHandler = this.TapOnArrow,
            };
            this.homeScreenCanvas.AddElement(this.switchArrow);


            // список программ
            var programsSv = new ProgramsMenu()
            {
                Size = new Size(320, this.Size.Height - 5),
                Location = new Point(565, 5),
            };
            this.homeScreenCanvas.AddElement(programsSv);

            this.Control.AddElement(this.homeScreenCanvas);

            this.homeScreenCanvas.FlickHandler = this.Flick;

            _SystemState.Changed += new ChangeEventHandler(OnSystemStateChanged);

        }


        private void GoToTiles()
        {
            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 500,
                From = this.homeScreenCanvas.Location.X,
                To = 0,
                OnAnimation = v => { 
                    this.homeScreenCanvas.Location = new Point(v, 0); 
                    this.homeScreenCanvas.Update(); 
                }
            });

            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 500,
                From = this.switchArrow.Location.X,
                To = _ArrowPos1,
                OnAnimation = v =>
                {
                    this.switchArrow.Location = new Point(v, this.switchArrow.Location.Y);
                    this.switchArrow.Update();
                }
            });
            this.switchArrow.Next();
            this._showingTiles = true;
        }

        private void GoToPrograms()
        {
            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 500,
                From = this.homeScreenCanvas.Location.X,
                To = -480,
                OnAnimation = v => { 
                    this.homeScreenCanvas.Location = new Point(v, 0); 
                    this.homeScreenCanvas.Update(); 
                }
            });

            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 500,
                From = this.switchArrow.Location.X,
                To = _ArrowPos2,
                OnAnimation = v => {
                    this.switchArrow.Location = new Point(v, this.switchArrow.Location.Y);
                    this.switchArrow.Update();
                }
            });
            this.switchArrow.Prev();
            this._showingTiles = false;
        }

        public bool TapOnArrow(Point p)
        {
            if (this._showingTiles)
            {
                this.GoToPrograms();
            }
            else
            {
                this.GoToTiles();
            }
            return true;
        }

        private bool Flick(Point from, Point to, int millisecs, Point start)
        {
            if (to.X - from.X > 0)
            {
                this.GoToTiles();
            }
            else
            {
                this.GoToPrograms();
            }
            return true;
        }

        protected override void OnActivated()
        {
            _TilesGrid.Active = true;
            base.OnActivated();
        }

        // handler for system state change event
        private void OnSystemStateChanged(object sender, ChangeEventArgs EventArgs)
        {
            string str = SystemState.ActiveApplication;
            if (str.Length > 6)
                str = str.Substring(str.Length - 7, 7);
            if (str.ToLower() == "desktop")
                this.theForm.Activate();
        }

    }
}
