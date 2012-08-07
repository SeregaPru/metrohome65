using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Media;
using System.IO;

using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Win32;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using Metrohome65.Settings.Controls;

using MAPIdotnet;

namespace MetroHome65.Widgets
{
    public enum FolderType
    {
        Unknown,
        Receive,
        Sent,
        Trash,
        Outbox
    }

    public class MessagingItems
    {
        public MessagingItems()
        {
            Name = "";
            FullAddress = "";
            Messages = "";
            Unread = false;
            Date = DateTime.Now;
            Folder = FolderType.Unknown;
            Select = false;
        }
        public string Name { get; set; }
        public string FullAddress { get; set; }
        public string Messages { get; set; }
        public bool Unread { get; set; }
        public DateTime Date { get; set; }
        public FolderType Folder { get; set; }
        public IMAPIMessage Imessage { get; set; }
        public bool Select { get; set; }

    }
    
    [TileInfo("WP SMS Ext")]
    public class WPSMSExt : ShortcutWidget, IActive
    {
        
        private MAPI _Mapi;// = new MAPI();
        private IMAPIMsgStore _StoreSMS;
        private IMAPIFolderID _ReceiveFolder;
        private List<MessagingItems> _smsList = new List<MessagingItems>();
        private int _smsIndex = -1;
        
        private ThreadTimer _updateTimer;
        private int _missedCount;

        private static readonly List<String> _fontSizes = new List<String>() { "5", "6", "7", "8", "9", "10", "11", "12", "14","16" };
        private readonly List<object> _fontSizesData;
        private static readonly List<String> _fontNames = new List<String>() { "Segoe WP", "Segoe WP Light", "Segoe WP SemiLight", "Segoe WP Semibold" };
        private readonly List<object> _fontNamesData;

        private int _fontSize = Int16.Parse(_fontSizes[4]);
        private Color _fontColor = MetroTheme.TileTextStyle.Foreground;
        private string _fontName = _fontNames[3];

        public WPSMSExt()
        {
            _Mapi = new MAPI();
            IMAPIMsgStore[] _stores = _Mapi.MessageStores;
            for (int i = 0, length = _stores.Length; i < length; i++)
            {
                IMAPIMsgStore _store = _stores[i];
                if (_store.ToString() == "SMS")
                {
                    _StoreSMS = _store;
                }
            }

            _ReceiveFolder = _StoreSMS.ReceiveFolder;

            _fontSizesData = new List<object>(); foreach (var size in _fontSizes) { _fontSizesData.Add(size); }
            _fontNamesData = new List<object>(); foreach (var name in _fontNames) { _fontNamesData.Add(name); }
        }

        private void LoadMessage()
        {
            _smsList.Clear();
            IMAPIFolder _folder = _ReceiveFolder.OpenFolder();
            _folder.SortMessagesByDeliveryTime(TableSortOrder.TABLE_SORT_DESCEND);
            IMAPIMessage[] messages = _folder.GetNextMessages(_folder.NumSubItems);
            for (int ii = 0; ii < messages.Count() - 1; ii++)
            {
                IMAPIMessage msg = messages[ii];
                msg.PopulateProperties(EMessageProperties.DeliveryTime | EMessageProperties.Sender | EMessageProperties.Subject);
                IMAPIContact sender = msg.Sender;
                DateTime delivery = msg.LocalDeliveryTime;
                DateTime _m_date = DateTime.Today;
                string _m_name = "";
                string _m_full_address = "";
                bool _m_flag = false;
                if (delivery != null) { _m_date = delivery; }
                if (sender != null) { _m_name = sender.Name; }
                if (sender != null) { _m_full_address = sender.FullAddress; }
                string _m_message = msg.Subject.ToString();
                if (msg.Flags == EMessageFlags.MSGFLAG_UNREAD)
                {
                    _m_flag = true;
                    _smsList.Add(new MessagingItems()
                    {
                        Name = _m_name,
                        FullAddress = _m_full_address,
                        Messages = _m_message,
                        Unread = _m_flag,
                        Date = _m_date,
                        Folder = FolderType.Receive,
                        Imessage = msg,
                    });
                }

            }
        }


        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(4, 1),
                new Size(4, 2) 
            };
        }
        
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
            PaintCount(g, rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
            return;            
        }

        private void PaintCount(Graphics g, Rectangle rect)
        {
            var _x = 0; var _y = 0; var _w = 0; var _h = 0;
            if (_missedCount == 0)
            {
                _x = this.Size.Width - 46 - 10;
                _y = this.Size.Height - 45 - 10;
                (new AlphaImage("message.png", this.GetType().Assembly)).PaintBackground(g, new Rectangle(_x, _y, 46, 45));
                return;
            }

            var missedCountStr = _missedCount.ToString(CultureInfo.InvariantCulture);
            var captionHeight = (Caption == "") ? 0 : (CaptionSize);
            var captionFont = new Font(_fontName, 22.ToLogic(), FontStyle.Regular);
            Brush captionBrush = new SolidBrush(_fontColor);

            var _box = g.MeasureString(missedCountStr, captionFont);
            _x = this.Size.Width - (int)_box.Width - 10;
            _y = this.Size.Height - (int)_box.Height ;
            g.DrawString(missedCountStr, captionFont, captionBrush, _x, _y);
            _x -= (10 + 46);
            _y = this.Size.Height - 45 - 10;
            (new AlphaImage("message.png", this.GetType().Assembly)).PaintBackground(g, new Rectangle(_x, _y, 46, 45));

            captionFont = new Font(_fontName, _fontSize.ToLogic() + 3, FontStyle.Regular);
            if (_smsList.Count() > 0)
            {
                if (_smsIndex < _smsList.Count())
                {
                    g.DrawString(_smsList[_smsIndex].Name, captionFont, captionBrush, 10, -5);
                }
                var _box1 = g.MeasureString(_smsList[_smsIndex].Name, captionFont);

                _x = 10;
                _y = (int)_box1.Height - 5 ;

                if (this.GridSize.Height == 1)
                {
                    _w = this.Size.Width - 20 - (int)_box.Width - 50;
                    _h = this.Size.Height - (int)_box1.Height ;

                }
                else
                {
                    _w = this.Size.Width - 20;
                    _h = this.Size.Height - (int)_box1.Height; 
                }
                var _r = new Rectangle(_x, _y, _w, _h);
                captionFont = new Font(_fontName, _fontSize.ToLogic(), FontStyle.Regular);
                g.DrawString(_smsList[_smsIndex].Messages, captionFont, captionBrush, _r);
            }

        }

        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null)
                        _updateTimer = new ThreadTimer(4000, UpdateStatus);
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
            _smsIndex++;
            if (_smsIndex >= _smsList.Count()) { _smsIndex = 0; }
            var currentMissedCount = GetMissedCount();
            if (currentMissedCount != _missedCount)
            {
                _missedCount = currentMissedCount;
                _smsIndex = 0;
                try { LoadMessage(); } catch (Exception) { _smsIndex = 0; _smsList.Clear(); }
                ForceUpdate();
            }
            if (_smsList.Count() > 0) { ForceUpdate(); }
            ForceUpdate();

        }

        protected virtual int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
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
            
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }

            return controls;
        }

    }
}
