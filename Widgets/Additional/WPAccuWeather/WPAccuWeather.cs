using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;
using Microsoft.WindowsMobile.Status;
using Microsoft.Win32;
using Fleux.Styles;
using Fleux.Core.Scaling;
using System;

namespace MetroHome65.Widgets
{
    [TileInfo("WP AccuWeather")]
    public class WPAccuWeather : ShortcutWidget
    {
        private ThreadTimer _updateTimer;
        
        private  Font _fnt;
        private  Brush _brush;

        public string _app_patch = "";
        public string _wp7_patch = "\\Program Files\\wp7weather";
        public string _wp7_name = "WP7Weather.exe";

        public string _RegState = "HKEY_LOCAL_MACHINE\\Software\\MicronSys\\WP7Weather";
        public string _RegRW = "\\Software\\MicronSys\\WP7Weather";
        public string _RegKey_cyti_name = "cyti_name";
        public string _RegKey_cyti_id = "cyti_id";
        public string _RegKey_time_valid = "time_valid";
        public string _RegKey_time_update = "time_update";
        public string _RegKey_s2u2_icon = "s2u2_icon";
        public string _RegKey_s2u2_wallpaper = "s2u2_wallpaper";
        public string _RegKey_widget_icon = "widget_icon";
        public string _RegKey_widget_temp_c = "widget_temp_c";
        public string _RegKey_widget_temp_h = "widget_temp_h";
        public string _RegKey_widget_temp_l = "widget_temp_l";


        public string _accu_old_state = "";
        public string _accu_icon;
        public string _accu_temp_c;
        public string _accu_temp_h;
        public string _accu_temp_l;
        public string _accu_city;




        
        #region WorkRegistry Accu Weather
        public bool IsRegistryKey(string _reg)
        {
            RegistryKey hwKey = Registry.LocalMachine.OpenSubKey(_reg, false);
            if (hwKey != null)
            {
                hwKey.Close();
                return true;
            }
            else
            {
                return false;
            }

        }
        public string ReadKey(string _reg, string _key)
        {
            var _ret = "~";
            RegistryKey hwKey = Registry.LocalMachine.OpenSubKey(_reg, false);
            if (hwKey != null)
            {
                _ret = (string)hwKey.GetValue(_key, "~");
                hwKey.Close();
                return _ret;
            }
            else
            {
                return _ret;
            }

        }
        public bool CreateRegistryKey(string _reg, string _key, string _val)
        {
            RegistryKey hwKey = Registry.LocalMachine.CreateSubKey(_reg);
            if (hwKey != null)
            {
                hwKey.SetValue(_key, _val, RegistryValueKind.String);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion 

        public string AppPatch()
        {
            string str;
            str = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase.ToString();
            return str.Substring(0, str.LastIndexOf("\\"));
        }
        public WPAccuWeather() 
        {
            _brush = new SolidBrush(MetroTheme.TileTextStyle.Foreground);
            _fnt = new Font(MetroTheme.PhoneFontFamilySemiBold, 10.ToLogic(), FontStyle.Regular);

            _app_patch = AppPatch();

            UpdateStatus();
            ForceUpdate();
        }
        
        public bool UpdateStatus()
        {
            _accu_city = ReadKey(_RegRW, _RegKey_cyti_name);            
            var _text_c = ReadKey(_RegRW, _RegKey_widget_temp_c);
            var _text_h = ReadKey(_RegRW, _RegKey_widget_temp_h);
            var _text_l = ReadKey(_RegRW, _RegKey_widget_temp_l);
            var _icon = ReadKey(_RegRW, _RegKey_widget_icon);
            var _state = _text_c + _text_h + _text_l + _icon;

            if (_accu_old_state != _state)
            {
                _accu_temp_c = _text_c;
                _accu_temp_h = _text_h;
                _accu_temp_l = _text_l;
                _accu_icon = _icon;
                _accu_old_state = _state;
                return true;
            }
            return false;
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            var _image_path = _wp7_patch + "\\Images\\Weather\\HH_WEATHER_" + _accu_icon + ".png";
            var _r = new Rectangle(0, 0, 0, 0);
            var _image_w = 171;
            var _image_h = 110;
            base.PaintBuffer(g, rect);

            _fnt = new Font(MetroTheme.PhoneFontFamilySemiLight, 8.ToLogic(), FontStyle.Regular);
            var _box0 = g.MeasureString(_accu_city, _fnt);
            var _y = -5;
            var _x = (170 - (int)_box0.Width) / 2;
            if (_x > 170) { _x = 170; }
            g.DrawString(_accu_city, _fnt, _brush, _x, _y);
            
            try
            {
                _r = new Rectangle(1, (int)_box0.Height - 10 , _image_w, _image_h);
                new AlphaImage(_image_path).PaintBackground(g, _r);
            }
            catch (Exception)
            {
            
            }

            _fnt = new Font(MetroTheme.PhoneFontFamilySemiBold, 22.ToLogic(), FontStyle.Regular);
            
            var _box1 = g.MeasureString(_accu_temp_c, _fnt);
            g.DrawString(_accu_temp_c, _fnt, _brush, 10,185 - (int)_box1.Height);



            _fnt = new Font(MetroTheme.PhoneFontFamilySemiLight, 8.ToLogic(), FontStyle.Regular);
            var _box2 = g.MeasureString(_accu_temp_l, _fnt);
            g.DrawString(_accu_temp_h, _fnt, _brush, 120, 120);

            var _box3 = g.MeasureString(_accu_temp_h, _fnt);
            g.DrawString(_accu_temp_l, _fnt, _brush, 120, 145);


        
        }

        protected override void PaintIcon(Graphics g, Rectangle rect) { }
        protected override void PaintCaption(Graphics g, Rectangle rect) { }


        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null) _updateTimer = new ThreadTimer(10000, () =>
                        {
                            UpdateStatus(); 
                            ForceUpdate();
                        });
                }
                else
                {
                    if (_updateTimer != null) _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        
        
        
        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(2, 2)
            };
        }

        
        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            
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
