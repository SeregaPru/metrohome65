using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsMobile.Status;

using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;

using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;
using MetroHome65.Routines.Settings;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

using MetroHome65.WPLock.Controls;

namespace MetroHome65.WPLock.Controls
{
    public class ScreenStatus : Canvas
    {
        private struct _StructureStatus
        {
            public int _countPhone;
            public int _countSms;
            public int _countEmail;
            public int _countMms;
        }
        #region Private
        private int _timerStep = 0;
        private int _timerTime = 10;
        const int leftOffset = 20;
        private _StructureStatus _status;
        private TransparentImage _imagePhone;
        private TransparentImage _imageSms;
        private TransparentImage _imageEmail;
        private TransparentImage _imageMms;
        private TextElementAdvanced _textPhone;
        private TextElementAdvanced _textSms;
        private TextElementAdvanced _textEmail;
        private TextElementAdvanced _textMms;
        #endregion

        #region Public
        public string _typeColorIcon = "white";                        // Опубликовать дать возможность смены
        private Size _imageSize = new Size(32, 32);                     // Опубликовать дать возможность смены
        private string _fontName = "Segoe WP Semibold";                 // Опубликовать дать возможность смены
        private int _fontSize = 14;                                     // Опубликовать дать возможность смены
        public Color _fontColor = MetroTheme.PhoneForegroundBrush;     // Опубликовать дать возможность смены
        #endregion

        public ScreenStatus(Size _size, bool _white)
        {
            _status._countPhone = 0;
            _status._countSms = 0;
            _status._countEmail = 0;
            _status._countMms = 0;
            
            this.Size = _size;

            _typeColorIcon = "white";
            if (!_white) _typeColorIcon = "black";
            var _dx = _size.Width / 4;
           
            this.AddElement(_imagePhone = new TransparentImage(_typeColorIcon + "_phone.png")
            {
                Location = new Point(leftOffset + _dx * 0 - (_imageSize.Width / 2), _size.Height - _imageSize.Height),
                Size = _imageSize,
            });

            this.AddElement(_imageSms = new TransparentImage(_typeColorIcon + "_sms.png")
            {
                Location = new Point(leftOffset + _dx * 1 - (_imageSize.Width / 2), _size.Height - _imageSize.Height),
                Size = _imageSize,
            });

            this.AddElement(_imageEmail = new TransparentImage(_typeColorIcon + "_email.png")
            {
                Location = new Point(leftOffset + _dx * 2 - (_imageSize.Width / 2), _size.Height - _imageSize.Height),
                Size = _imageSize,
            });

            this.AddElement(_imageMms = new TransparentImage(_typeColorIcon + "_mms.png")
            {
                Location = new Point(leftOffset + _dx * 3 - (_imageSize.Width / 2), _size.Height - _imageSize.Height),
                Size = _imageSize,
            });
            
            // 
            var _style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            var _height = FleuxApplication.DummyDrawingGraphics.Style(_style).CalculateMultilineTextHeight("0", 100);
            this.AddElement(_textPhone = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset + _dx * 0 + (_imageSize.Width / 2), _size.Height - _height),
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Left,
                Style = _style,
            });

            this.AddElement(_textSms = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset + _dx * 1 + (_imageSize.Width / 2), _size.Height - _height),
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Left,
                Style = _style,
            });

            this.AddElement(_textEmail = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset + _dx * 2 + (_imageSize.Width / 2), _size.Height - _height),
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Left,
                Style = _style,
            });

            this.AddElement(_textMms = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset + _dx * 3 + (_imageSize.Width / 2), _size.Height - _height),
                AutoSizeMode = TextElementAdvanced.AutoSizeModeOptions.Left,
                Style = _style,
            });
        }

        private void GetState()
        {
            _status._countPhone = Microsoft.WindowsMobile.Status.SystemState.PhoneMissedCalls;
            _status._countSms = Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
            _status._countEmail = Microsoft.WindowsMobile.Status.SystemState.MessagingTotalEmailUnread;
            _status._countMms = Microsoft.WindowsMobile.Status.SystemState.MessagingMmsUnread;
        }
        private void UpdateState()
        {
            // Count Phone Missed Calls
            if (_status._countPhone == 0)
            {
                _imagePhone.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource("clear.png", this.GetType().Assembly);
                _textPhone.Text = "";
            }
            else
            {
                _imagePhone.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_phone.png", this.GetType().Assembly);
                _textPhone.Text = _status._countPhone.ToString();
            }
            // Count Messaging Sms Unread 
            if (_status._countSms == 0)
            {
                _imageSms.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource("clear.png", this.GetType().Assembly);
                _textSms.Text = "";
            }
            else
            {
                _imageSms.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_sms.png", this.GetType().Assembly);
                _textSms.Text = _status._countSms.ToString();
            }
            // Count Messaging Total Email Unread
            if (_status._countEmail == 0)
            {
                _imageEmail.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource("clear.png", this.GetType().Assembly);
                _textEmail.Text = "";
            }
            else
            {
                _imageEmail.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_email.png", this.GetType().Assembly);
                _textEmail.Text = _status._countEmail.ToString();
            }
            // Count Messaging Mms Unread
            if (_status._countMms == 0)
            {
                _imageMms.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource("clear.png", this.GetType().Assembly);
                _textMms.Text = "";
            }
            else
            {
                _imageMms.Image = ResourceManager.Instance.GetIImageFromEmbeddedResource(_typeColorIcon + "_mms.png", this.GetType().Assembly);
                _textMms.Text = _status._countMms.ToString();
            }
        }

        public void Active()
        {
            GetState();
            UpdateState();
            this.Update();
        }

        public void Timer()
        {
            _timerStep++;
            if (_timerStep > _timerTime)
            {
                _timerStep = 0;
                Active();
            }
        }

        public void UpdateTheme()
        {
            _textPhone.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            _textSms.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            _textEmail.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            _textMms.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            UpdateState();
        }
        

    }
}
