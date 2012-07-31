using System;
using System.Drawing;
using System.Globalization;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines.File;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.UIControls;
using MetroHome65.Tile;
using TinyIoC;
using TinyMessenger;

namespace FolderWidget
{
    public class HubPage : FleuxControlPage
    {
        #region Fields

        private readonly string _folderGuid;
        private readonly TextElement _title;
        private readonly HubPageTileGrid _tileGrid;
        private readonly ApplicationBar _appBar;
        private readonly ScaledBackground _background;

        private const int AppBarHeight = 48 + 2 * 10;

        #endregion


        #region Properties

        public Canvas Content { get; set; }

        #endregion


        public HubPage(string folderGuid) : base(false)
        {
            _folderGuid = folderGuid;

            ScreenRoutines.CursorWait();
            try
            {
                theForm.Menu = null;

                Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

                _background = new ScaledBackground("") { Size = this.Size.ToPixels() };
                Control.AddElement(_background);

                Content = new Canvas
                {
                    Size = new Size(this.Size.Width, this.Size.Height),
                    Location = new Point(0, 0)
                };
                Control.AddElement(Content);

                _appBar = new ApplicationBar
                {
                    Size = new Size(Content.Size.Width, AppBarHeight),
                    Location = new Point(0, Content.Size.Height - AppBarHeight)
                };
                _appBar.ButtonTap += OnAppBarButtonTap;
                _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("FolderWidget.Images.back.bmp"));
                Content.AddElement(_appBar.AnimateHorizontalEntrance(false));

                _title = new TextElement("Folder hub")
                {
                    Style = MetroTheme.PhoneTextTitle1Style,
                    Location = new Point(24 - 3, 5), // -3 is a correction for Segoe fonts
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                };
                _title.ResizeForWidth(Content.Size.Width);
                Content.AddElement(_title);

                _tileGrid = new HubPageTileGrid(new TileThemeWP7() { TilesPaddingTop = 0 }, 
                    _background, "", 4, 100)
                                {
                                    OnReadSettings = ReadSettings,
                                    OnWriteSettings = WriteSettings,
                                    OnShowMainSettings = ShowHubSettings,
                                };
                SetTilesLocation(_title.Bounds.Bottom + 50);
                Content.AddElement(_tileGrid);
                
                ReadSettings();

            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        public override void Close()
        {
            // clear grid manually to prevent reference cycling and provide normal GC work
            _tileGrid.Clear();
            base.Close();
        }

        protected override void OnActivated()
        {
            _tileGrid.Active = true;
            base.OnActivated();
        }


        /// <summary>
        /// Places tiles grid according to user defined offset, and change its size to screen bottom
        /// </summary>
        /// <param name="offset"></param>
        private void SetTilesLocation(int offset)
        {
            _tileGrid.Location = new Point(0, offset);
            _tileGrid.Size = new Size(Content.Size.Width, Content.Size.Height - _tileGrid.Location.Y - _appBar.Size.Height);
        }

        private string GetSettingsFileName()
        {
            return  FileRoutines.CoreDir + @"\settings\hub_" + _folderGuid + ".xml";
        }

        private void ReadSettings()
        {
            var hubSettings = XmlHelper<HubSettings>.Read(GetSettingsFileName());
            ApplyHubSettings(hubSettings);
        }

        private void WriteSettings()
        {
            XmlHelper<HubSettings>.Write(HubSettings, GetSettingsFileName());
        }

        public HubSettings HubSettings
        {
            get {
                return new HubSettings
                {
                    Background = _background.Image,
                    Title = _title.Text,
                    Offset = _tileGrid.Location.Y.ToString(CultureInfo.InvariantCulture),
                    TileSettings = _tileGrid.TileSettings,
                };
            }
        }

        /// <summary>
        /// shows page with hub settings - tile, background etc.
        /// </summary>
        private void ShowHubSettings()
        {
            try
            {
                var hubSettingsPage = new HubSettingsPage(HubSettings);
                hubSettingsPage.OnApplySettings += (sender, settings) =>
                                           {
                                               ApplyHubSettings(settings);
                                               WriteSettings();
                                           };

                var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
                messenger.Publish(new ShowPageMessage(hubSettingsPage));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, "Hub settings dialog error");
            }
        }

        private void ApplyHubSettings(HubSettings hubSettings)
        {
            if (hubSettings == null) return;

            try
            {
                _background.Image = hubSettings.Background;
                _title.Text = hubSettings.Title;
                _tileGrid.TileSettings = hubSettings.TileSettings;

                int offset = 0;
                try
                {
                    offset = int.Parse(hubSettings.Offset);
                }
                catch (Exception) { }
                SetTilesLocation(offset);
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Apply hub settings error");
            }
        }

        /// <summary>
        /// handler for bottom soft buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            switch (e.ButtonID)
            {
                // exit
                case 0:
                    {
                        Close();
                        break;
                    }
            }
        }

    }
}
