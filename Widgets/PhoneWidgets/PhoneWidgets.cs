using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    [TileInfo("Phone")]
    public class PhoneWidget : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;
        private int _missedCount;

        private readonly List<String> _fontSizes = new List<String>() { "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30", "32", "34", "36", "38", "40" };
        private readonly List<object> _fontSizesData;
        private readonly List<String> _fontNames = new List<String>() { "Segoe WP", "Segoe WP Light", "Segoe WP SemiLight", "Segoe WP Semibold" };
        private readonly List<object> _fontNamesData;

        private int _fontSize;
        private Color _fontColor;
        private string _fontName;

        private const int PaddingRightCnt = 55; //todo comment
        private const int PaddingRightIco = 160; //todo comment

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(1, 1),
                new Size(2, 1),
                new Size(2, 2),
                new Size(4, 2) 
            };
        }

        [TileParameter]
        public String FontName
        {
            get { return _fontName; }
            set
            {
                _fontName = value;
                NotifyPropertyChanged("FontName");
            }
        }

        public int FontNameIndex
        {
            get
            {
                for (var i = 0; i < _fontNames.Count; i++) 
                    if (_fontNames[i] == FontName) 
                        return i;
                return 0;
            }
            set
            {
                if ((value >= 0) && (value < _fontNames.Count)) 
                    FontName = _fontNames[value];
            }
        }

        [TileParameter]
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                NotifyPropertyChanged("FontSize");
            }
        }

        public int FontSizeIndex
        {
            get
            {
                for (var i = 0; i < _fontSizes.Count; i++)
                    if (_fontSizes[i] == Convert.ToString(FontSize))
                        return i;
                return 0;
            }
            set
            {
                if ((value >= 0) && (value < _fontSizes.Count)) 
                    FontSize = Convert.ToInt32(_fontSizes[value]);
            }
        }

        [TileParameter]
        public int FontColor
        {
            get { return _fontColor.ToArgb(); } 
            set { _fontColor = Color.FromArgb(value); }
        }

        public Color FontColorIndex
        {
            get { return _fontColor; } 
            set
            {
                _fontColor = value; 
                NotifyPropertyChanged("FontColor");
            }
        }


        public PhoneWidget()
        {
            _fontSizesData = new List<object>(); 
            foreach (var size in _fontSizes) 
                _fontSizesData.Add(size); 

            _fontNamesData = new List<object>(); 
            foreach (var name in _fontNames) 
                _fontNamesData.Add(name);

            _fontSize = Int16.Parse(_fontSizes[7]);
            _fontColor = MetroTheme.TileTextStyle.Foreground;
            _fontName = _fontNames[3];
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            PaintCount(g, rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
            var dx = (this.GridSize.Width == 1) ? 2 : 1;

            base.PaintIcon(g, new Rectangle(
                rect.Right - (PaddingRightIco / dx) - 5, 
                rect.Top, 
                (PaddingRightIco / dx) - (PaddingRightCnt / dx), 
                rect.Height));
        }

        private void PaintCount(Graphics g, Rectangle rect)
        {
            if (_missedCount == 0) return; // don't display zero count

            var missedCountStr = _missedCount.ToString(CultureInfo.InvariantCulture);

            var captionHeight = (Caption == "") ? 0 : (CaptionSize);

            var captionFont = new Font(_fontName, _fontSize.ToLogic(), FontStyle.Regular);
            //var captionFont = new Font(MetroTheme.TileTextStyle.FontFamily, 24.ToLogic(), FontStyle.Regular);

            Brush captionBrush = new SolidBrush(_fontColor);
            //Brush captionBrush = new SolidBrush(MetroTheme.TileTextStyle.Foreground);

            var dx = (this.GridSize.Width == 1) ? 2 : 1;
            g.DrawString(missedCountStr, captionFont, captionBrush,
                rect.Right - PaddingRightCnt / dx - 5,
                rect.Top + (rect.Height - g.MeasureString("0", captionFont).Height - captionHeight) / 2);
            //g.DrawString(missedCountStr, captionFont, captionBrush,
            //    rect.Right - PaddingRightCnt,
            //    rect.Top + (rect.Height - g.MeasureString("0", captionFont).Height - captionHeight) / 2);
        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(2000, UpdateStatus);
                }
                else
                {
                    if (_updateTimer != null)
                        _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        private void UpdateStatus()
        {
            var currentMissedCount = GetMissedCount();
            if (currentMissedCount != _missedCount)
            {
                _missedCount = currentMissedCount;
                ForceUpdate();
            }
        }

        protected virtual int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.PhoneMissedCalls;
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var fontNameControl = new SelectSettingsControl { Caption = "Font Name", Items = _fontNamesData, };
            controls.Add(fontNameControl);
            bindingManager.Bind(this, "FontNameIndex", fontNameControl, "SelectedIndex", true);

            var fontSizeControl = new SelectSettingsControl { Caption = "Font Size", Items = _fontSizesData, };
            controls.Add(fontSizeControl);
            bindingManager.Bind(this, "FontSizeIndex", fontSizeControl, "SelectedIndex", true);

            var fontColorControl = new ColorSettingsControl(true) { Caption = "Font Color", };
            controls.Add(fontColorControl);
            bindingManager.Bind(this, "FontColorIndex", fontColorControl, "Value", true);

            return controls;
        }
    }
}
