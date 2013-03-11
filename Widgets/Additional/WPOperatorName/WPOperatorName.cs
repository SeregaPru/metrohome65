// MicronSys and Dev 2012 year
using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Win32;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    [TileInfo("WP Operator Name")]
    public class WPOperatorName : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;

        private static readonly List<String> _fontSizes = new List<String>() {"6","7","8","9","10","12","14","16","18","20","22","24","26","28","30" };
        private readonly List<object> _fontSizesData;
        private static readonly List<String> _fontNames = new List<String>() {"Segoe WP","Segoe WP Light","Segoe WP SemiLight","Segoe WP Semibold" };
        private readonly List<object> _fontNamesData;

        private int _fontSize = Int16.Parse(_fontSizes[4]);
        private Color _fontColor = MetroTheme.TileTextStyle.Foreground;
        private string _fontName = _fontNames[3];

        private string _oldOperatorName = "WP Operator Name";

        public WPOperatorName() 
        {
            _fontSizesData = new List<object>(); foreach (var size in _fontSizes){ _fontSizesData.Add(size); }
            _fontNamesData = new List<object>(); foreach (var name in _fontNames) { _fontNamesData.Add(name); }
            ApplySizeSettings();
        }

        private void ApplySizeSettings()
        {

            return;
        }

        protected override Size[] GetSizes() { return new Size[] { new Size(2, 1), new Size(2, 2), new Size(4, 1), new Size(4, 2) }; }

        protected override void GridSizeChanged() { ApplySizeSettings(); }


        [TileParameter]
        public String FontName { get { return _fontName; } set { _fontName = value; NotifyPropertyChanged("FontName"); } }

        public int FontNameIndex
        {
            get { for (var i = 0; i < _fontNames.Count; i++) if (_fontNames[i] == FontName) return i; return 0; }
            set { if ((value >= 0) && (value < _fontNames.Count)) FontName = _fontNames[value]; }
        }
        [TileParameter]
        public int FontSize { get { return _fontSize; } set { _fontSize = value; NotifyPropertyChanged("FontSize"); } }
        public int FontSizeIndex
        {
            get { for (var i = 0; i < _fontSizes.Count; i++) if (_fontSizes[i] == Convert.ToString(FontSize)) return i; return 0; }
            set { if ((value >= 0) && (value < _fontSizes.Count)) FontSize = Convert.ToInt32(_fontSizes[value]); }
        }
        [TileParameter]
        public int FontColor { get { return _fontColor.ToArgb(); } set { _fontColor = Color.FromArgb(value); } }
        public Color FontColorIndex { get { return _fontColor; } set { _fontColor = value; NotifyPropertyChanged("FontColor"); } }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            var _fnt = new Font(_fontName, _fontSize.ToLogic(), FontStyle.Regular);
            var _brh = new SolidBrush(_fontColor);
            g.DrawString(_oldOperatorName, _fnt, _brh, rect);
        }

        public bool UpdateStatus()
        {
            var _name = "";
            RegistryKey hwKey = Registry.LocalMachine.OpenSubKey("System\\State\\Phone", false);
            if (hwKey != null)
            {
                _name = (string)hwKey.GetValue("Current Operator Name", "Not");
                hwKey.Close();
            }
            else { _name = ""; }
            if (_oldOperatorName != _name) { _oldOperatorName = _name; return true; }
            return false;
        }


        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null) _updateTimer = new ThreadTimer(2000, () =>
                        {
                            if (UpdateStatus()) ForceUpdate();
                        });
                }
                else
                {
                    if (_updateTimer != null) _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        // overriding paint icon method - don't paint icon
        protected override void PaintIcon(Graphics g, Rectangle rect)
        { }

        // overriding paint caption method - don't paint caption
        protected override void PaintCaption(Graphics g, Rectangle rect)
        { }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();


            var fontNameControl = new SelectSettingsControl {  Caption = "Font Name", Items = _fontNamesData, };
            controls.Add(fontNameControl);
            bindingManager.Bind(this, "FontNameIndex", fontNameControl, "SelectedIndex", true);

            var fontSizeControl = new SelectSettingsControl {  Caption = "Font Size", Items = _fontSizesData, };
            controls.Add(fontSizeControl);
            bindingManager.Bind(this, "FontSizeIndex", fontSizeControl, "SelectedIndex", true);

            var fontColorControl = new ColorSettingsControl(true) { Caption = "Font Color", };
            controls.Add(fontColorControl);
            bindingManager.Bind(this, "FontColorIndex", fontColorControl, "Value", true);

            // hide control for icon / caption selection
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }
            foreach (var control in controls)
                if (control.Name.Contains("Caption"))
                {
                    controls.Remove(control);
                    break;
                }
           
            return controls;
        }

        
    }

}
