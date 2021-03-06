﻿using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.Core.GraphicsHelpers;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.File;
using MetroHome65.Routines.Settings;

namespace MetroHome65.Tile
{
  
    /// <summary>
    /// Container for tile view, middle layer between tiles grid and tile plugin.
    /// </summary>
    public class TileWrapper : UIElement
    {
        #region Fields

        private ITile _tile;
        private TileTheme _tileTheme;
        private Point _gridPosition = new Point(0, 0);
        private Size _gridSize = new Size(1, 1);

        private readonly List<PropertyInfo> _propertyInfos = new List<PropertyInfo>();

        private ThreadTimer _movingTimer;
        private bool _pause;

        #endregion


        #region Properties

        /// <summary>
        /// Tile location in grid
        /// </summary>
        public Point GridPosition { 
            get { return _gridPosition; } 
            set {
                if (_gridPosition == value) return;

                _gridPosition = value;
                CalcScreenPosition();
            }
        }
        
        /// <summary>
        /// Tile size in cells
        /// </summary>
        public Size GridSize { get { return _gridSize; } set { SetGridSize(value); } }

        public ITile Tile { get { return _tile; } }

        /// <summary>
        /// Property for serialize tile class name.
        /// When deserialized Tile will be created
        /// </summary>
        public String TileClass
        {
            // return Tile's class name
            get { return (_tile != null) ? _tile.GetType().ToString() : ""; }

            // create new tile instance by class name
            set
            {
                if (TileClass == value) return;

                // remove old tile
                if (_tile != null)
                {
                    (_tile as UIElement).Parent = null;
                    (_tile as UIElement).Updated = null;
                }

                // create and insert new tile
                var pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();
                _tile = pluginManager.CreateTile(value);
                FillTileProperties();

                // insert new tile
                (_tile as UIElement).Parent = this;
                (_tile as UIElement).Updated = this.OnUpdated;

                Active = true;
            }
        }


        /// <summary>
        /// Swithes off tile activity when application goes to background
        /// </summary>
        public Boolean Active
        {
            set
            {
                if (_tile is IActive)
                    (_tile as IActive).Active = value;
            }
        }

        public Boolean Pause
        {
            get { return _pause; }
            set
            {
                _pause = value;
                if (_tile is IPause)
                    (_tile as IPause).Pause = value;
            }
        }

        public TileTheme TileTheme
        {
            get { return _tileTheme; }
            set
            {
                if (_tileTheme == value) return;
                _tileTheme = value;
                CalcScreenPosition();
            }
        }

        #endregion


        #region Methods

        // empty constructor for deserialize
        public TileWrapper() { }

        /// <summary>
        /// Sets tile size in grid cells.
        /// Checks if specified size contain in tile's possible sizes.
        /// If not, size will be first available tile size.
        /// </summary>
        /// <param name="value"> </param>
        /// <returns></returns>
        private void SetGridSize(Size value)
        {
            if (_tile == null)
                return;

            var sizeOk = false;
            if ((_tile.Sizes != null) && (_tile.Sizes.Length > 0))
            {
                foreach (var possibleSize in _tile.Sizes)
                {
                    if (possibleSize.Equals(value))
                    {
                        _gridSize = value;
                        sizeOk = true;
                        break;
                    }
                }
            }

            if (!sizeOk) 
                if ((_tile.Sizes != null) && (_tile.Sizes.Length > 0))
                {
                    _gridSize = _tile.Sizes[0];
                }
                else
                {
                    _gridSize = new Size(2, 2);
                }

            _tile.GridSize = _gridSize;

            CalcScreenPosition();
        }

        private void CalcScreenPosition()
        {
            var screenRect = GetScreenRect();
            Location = screenRect.Location;
            Size = screenRect.Size;

            UIElement tile;
            if ((tile = (_tile as UIElement)) != null)
            {
                tile.Location = new Point(0, 0);
                tile.Size = this.Size;
            }
        }

        public Rectangle GetScreenRect()
        {
            return new Rectangle(
                _gridPosition.X * (TileTheme.TileSize + TileTheme.TileSpacing) + TileTheme.TilesPaddingLeft,
                _gridPosition.Y * (TileTheme.TileSize + TileTheme.TileSpacing) + TileTheme.TilesPaddingTop,
                _gridSize.Width * (TileTheme.TileSize + TileTheme.TileSpacing) - TileTheme.TileSpacing,
                _gridSize.Height * (TileTheme.TileSize + TileTheme.TileSpacing) - TileTheme.TileSpacing);
        }

        private void FillTileProperties()
        {
            _propertyInfos.Clear();

            if (_tile == null) return;

            // get tile properties
            foreach (var propertyInfo in (_tile).GetType().GetProperties())
                if (propertyInfo.GetCustomAttributes(typeof(TileParameterAttribute), true).Length > 0)
                {
                    _propertyInfos.Add(propertyInfo);
                }
        }


        /// <summary>
        /// prepare struct for serialization and store settings
        /// </summary>
        /// <returns></returns>
        public TileWrapperSettings SerializeSettings()
        {
            var settings = new TileWrapperSettings
                               {
                                   Size = GridSize,
                                   Location = GridPosition,
                                   TileClassName = TileClass,
                                   Parameters = new List<StoredParameter>()
                               };

            foreach (var prop in _propertyInfos)
                settings.Parameters.Add(new StoredParameter(prop.Name, prop.GetValue(Tile, null).ToString()));

            return settings;
        }

        /// <summary>
        /// parse settings struct read from settings file and apply these settings to widget
        /// </summary>
        /// <param name="settings"></param>
        public void DeserializeSettings(TileWrapperSettings settings)
        {
            try
            {
                TileClass = settings.TileClassName;
                GridSize = settings.Size;
                GridPosition = settings.Location;

                foreach (var param in settings.Parameters)
                    SetParameter(param.Name, param.Value);
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "DeserializeSettings error");
            }
        }

        public TileWrapper SetParameter(String name, object value)
        {
            foreach (var propertyInfo in _propertyInfos.Where(propertyInfo => propertyInfo.Name == name))
            {
                object propObject;
                var type = propertyInfo.PropertyType;

                MethodInfo methodInfo = type.GetMethod("FromString");
                if (methodInfo != null)
                {
                    propObject = methodInfo.Invoke(null, new[] { value });
                }
                else
                    propObject = Convert.ChangeType(value, propertyInfo.PropertyType, null);

                propertyInfo.SetValue(Tile, propObject, null);

                break;
            }
            return this;
        }


        public bool OnClick(Point clickLocation)
        {
            try
            {
                return (Tile != null) && Tile.OnClick(clickLocation);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /*
        public bool OnDblClick(Point clickLocation)
        {
            try
            {
                return (Tile != null) && Tile.OnClick(clickLocation);
            }
            catch (Exception)
            {
                return false;
            }
        }
        */


        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (drawingGraphics == null)
                return;

            {
                var tileGraphic = _moving
                                      ? drawingGraphics.CreateChild(new Point(0, 0), (100.0 + _movingScale) / 100.0, new Point(Bounds.Width / 2, Bounds.Height / 2))
                                      : drawingGraphics;

                if (Tile != null)
                {
                    (Tile as UIElement).Draw(tileGraphic);
                }
                else
                {
                    // if tile is wrong - draw stub tile with warning text
                    var drawRect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

                    tileGraphic.Color(MetroTheme.PhoneAccentBrush);
                    tileGraphic.FillRectangle(drawRect);

                    tileGraphic.Color(MetroTheme.PhoneForegroundBrush);
                    tileGraphic.DrawRectangle(drawRect);

                    tileGraphic.Style(MetroTheme.PhoneTextSmallStyle);
                    tileGraphic.DrawText("Tile\nnot\nfound");
                }
            }
        }

        public void ForceUpdate()
        {
            if ((Tile != null) && (! Pause))
                Tile.ForceUpdate();
        }

        #endregion


        #region Moving

        private bool _moving;

        /// <summary>
        /// Flag when tile is in moving mode
        /// </summary>
        public bool Moving
        {
            get { return _moving; }
            set
            {
                if (_moving != value)
                {
                    _moving = value;

                    if (value)
                    {
                        if (_movingTimer == null)
                            _movingTimer = new ThreadTimer(100, RepaintMovingTile);
                    }
                    else
                    {
                        if (_movingTimer != null)
                            _movingTimer.Stop();
                        _movingTimer = null;
                        Update();
                    }
                }
            }
        }

        private int _movingScale = 0;
        private int _movingScaleStep = 3;

        private void RepaintMovingTile()
        {
            if (!_moving)
                return;

            if ((_movingScale >= 0) || (_movingScale <= -15))
                _movingScaleStep = -_movingScaleStep;
            _movingScale += _movingScaleStep;

            // paint tile
            Update();
        }

        #endregion

    }
}
