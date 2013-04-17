using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
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

    
    [TileInfo("SMS Extra")]
    public class WPSMSExt : ShortcutWidget, IActive
    {
        private readonly MAPI _mapi;
        private readonly IMAPIMsgStore _storeSMS;
        private readonly IMAPIFolderID _receiveFolder;
        private readonly List<MessagingItems> _smsList;
        private int _smsIndex = -1;
        
        private ThreadTimer _updateTimer;
        private int _unreadCount;

        private Size _iconSize = new Size(46, 44);

        public WPSMSExt()
        {
            _smsList = new List<MessagingItems>();
            _mapi = new MAPI();
            var stores = _mapi.MessageStores;
            for (int i = 0, length = stores.Length; i < length; i++)
            {
                var store = stores[i];
                if (store.ToString() == "SMS")
                    _storeSMS = store;
            }

            _receiveFolder = _storeSMS.ReceiveFolder;

            Caption = "Messages".Localize();
        }

        private void LoadMessage()
        {
            _smsList.Clear();

            var folder = _receiveFolder.OpenFolder();
            folder.SortMessagesByDeliveryTime(TableSortOrder.TABLE_SORT_DESCEND);
            var messages = folder.GetNextMessages(folder.NumSubItems);

            foreach (var msg in messages)
            {
                msg.PopulateProperties(EMessageProperties.DeliveryTime | EMessageProperties.Sender | EMessageProperties.Subject);

                if (msg.Flags == EMessageFlags.MSGFLAG_UNREAD)
                {
                    _smsList.Add(new MessagingItems()
                    {
                        Name = (msg.Sender != null) ? msg.Sender.Name : "",
                        FullAddress = (msg.Sender != null) ? msg.Sender.FullAddress : "",
                        Messages = msg.Subject,
                        Unread = true,
                        Date = msg.LocalDeliveryTime,
                        Folder = FolderType.Receive,
                        Imessage = msg,
                    });
                }
            }
        }


        protected override Size[] GetSizes()
        {
            return new[] { 
                new Size(4, 1),
                new Size(4, 2) 
            };
        }
        
        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            base.PaintBuffer(g, rect);
            PaintCount(g, rect);
        }

        protected override void PaintIcon(Graphics g, Rectangle rect)
        {
        }

        protected override void PaintCaption(Graphics g, Rectangle rect)
        {
            if ((GridSize.Height == 1) && (_unreadCount != 0)) return;
            base.PaintCaption(g, rect);
        }

        private void PaintCount(Graphics g, Rectangle rect)
        {
            // если нет новых сообщений то просто рисуем иконку
            if (_unreadCount == 0)
            {
                (new AlphaImage("WPSMSExt.Images.message.png", this.GetType().Assembly)).
                    PaintIcon(g,
                        (rect.Width - _iconSize.Width) / 2,
                        (rect.Height - _iconSize.Height) / 2);
                return;
            }

            // если есть новые сообщения то рисуем иконку и кол-во сообщений
            var countFont = new Font(MetroTheme.PhoneFontFamilySemiBold, 22.ToLogic(), FontStyle.Bold);
            Brush countBrush = new SolidBrush(CaptionFont.Foreground);

            var missedCountStr = _unreadCount.ToString(CultureInfo.InvariantCulture);
            var cntBox = g.MeasureString(missedCountStr, countFont);
            var x = (_smsList.Any()) ?
                rect.Width - _iconSize.Width - (int)cntBox.Width - 2 * CaptionLeftOffset :
                (rect.Width - _iconSize.Width) / 2;

            (new AlphaImage("WPSMSExt.Images.message.png", this.GetType().Assembly)).
                PaintIcon(g, x, (rect.Height - _iconSize.Height) / 2 + 5);

            g.DrawString(missedCountStr, countFont, countBrush, 
                x + _iconSize.Width + CaptionLeftOffset, (rect.Height - cntBox.Height) / 2);


            // если есть сообщения то выводим первое из них
            if (_smsList.Any())
            {
                if (_smsIndex < _smsList.Count())
                {
                    var captionFont = new Font(CaptionFont.FontFamily, CaptionFont.FontSize, FontStyle.Regular);

                    g.DrawString(_smsList[_smsIndex].Name + "\n" + _smsList[_smsIndex].Messages, 
                        captionFont, countBrush,
                        new Rectangle(CaptionLeftOffset, CaptionBottomOffset, 
                            rect.Width - _iconSize.Width - (int)cntBox.Width - 3 * CaptionLeftOffset, 
                            rect.Height - CaptionBottomOffset));
                }
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
            if (currentMissedCount != _unreadCount)
            {
                _unreadCount = currentMissedCount;
                _smsIndex = 0;
                try
                {
                    LoadMessage();
                } 
                catch (Exception)
                {
                    _smsIndex = 0; 
                    _smsList.Clear();
                }

                //ForceUpdate();
            }

            //if (_smsList.Any()) 
            //    ForceUpdate();

            ForceUpdate();

        }

        protected virtual int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            
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
