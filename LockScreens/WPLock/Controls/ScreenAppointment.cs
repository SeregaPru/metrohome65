using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using Microsoft.WindowsMobile.Status;
using Microsoft.WindowsMobile.PocketOutlook;

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
    
    
    
    public class ScreenAppointment : Canvas
    {
        #region Private

        private struct _StructAppointment
        {
            public string Location;
            public string Subject;
            public DateTime StartTime;
            public DateTime EndTime;
        }
        
        private int _timerStep = 0;
        private int _timerTime = 30;
        private int _timerTimeConst = 30;
        
        const int leftOffset = 20;
        const int rightOffset = 10;

        private TextElementAdvanced _textLocation;
        private TextElementAdvanced _textSubject;
        private TextElementAdvanced _textTime;
        private TextElementAdvanced _textCount;

        private OutlookSession _session = new OutlookSession();
        
        private List<_StructAppointment> _listAppointment = new List<_StructAppointment>();
        private int _indexAppointment = 0;
        #endregion

        #region Public
        public string _typeColorIcon = "white";                         // Опубликовать дать возможность смены
        private string _fontName = "Segoe WP Semibold";                  // Опубликовать дать возможность смены
        private int _fontSize = 7;                                       // Опубликовать дать возможность смены
        public Color _fontColor = MetroTheme.PhoneForegroundBrush;      // Опубликовать дать возможность смены
        public int _onAppointmentShowTime = 5;                          // Опубликовать дать возможность смены 
        public int _liveHourAppointment = 12;                           // Опубликовать дать возможность смены 
        #endregion

        public ScreenAppointment(Size _size, bool _white)
        {
            this.Size = _size;
            _listAppointment.Clear(); _indexAppointment = 0; 
            _typeColorIcon = "white";
            if (!_white) _typeColorIcon = "black";

            var _style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            var _height = FleuxApplication.DummyDrawingGraphics.Style(_style).CalculateMultilineTextHeight("0", 100);

            this.AddElement(_textLocation = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset,_height * 0),
                Size = new Size(this.Size.Width - leftOffset - rightOffset,_height),
                Style = _style,
            });

            this.AddElement(_textSubject = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset, _height * 1),
                Size = new Size(this.Size.Width - leftOffset - rightOffset, _height),
                Style = _style,
            });

            this.AddElement(_textTime = new TextElementAdvanced("")
            {
                Location = new Point(leftOffset, _height * 2),
                Size = new Size(this.Size.Width - leftOffset - rightOffset - 100, _height),
                Style = _style,
            });

            this.AddElement(_textCount = new TextElementAdvanced("")
            {
                Location = new Point(this.Size.Width - 100 + 60, _height * 2),
                Size = new Size(100 - 60, _height),
                Style = _style,
            });



            
            this.Size = new Size(_size.Width, _height * 3);
        }

        /*public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            base.Draw(drawingGraphics);
        }*/

        private void GetState()
        {
            _listAppointment.Clear(); _indexAppointment = 0; 

            DateTime _start = DateTime.Now;
            DateTime _end = DateTime.Now.AddHours(_liveHourAppointment);

            for (int i = 0; i < _session.Appointments.Items.Count; ++i)
            {
                Appointment app = _session.Appointments.Items[i];
                if ((app.End >= _start) && (app.Start <= _end))
                {
                    _StructAppointment _app = new _StructAppointment();
                    _app.Location = app.Location;
                    _app.Subject = app.Subject;
                    _app.StartTime = app.Start;
                    _app.EndTime = app.End;
                    _listAppointment.Add(_app);
                }
            }
            if (_listAppointment.Count > 0)
            {
                _timerTime = _onAppointmentShowTime;
                _timerStep = 0;
            }
            else
            {
                _timerTime = _timerTimeConst;
                _timerStep = 0;
            }
        }
        
        private void UpdateState()
        {
            if (_listAppointment.Count < 1) // Если нет встреч то выход
            {
                _textLocation.Text = "";
                _textSubject.Text = "";
                _textTime.Text = "";
                _textCount.Text = "";
                GetState();
                return;
            }

            if (_indexAppointment > _listAppointment.Count - 1) { _indexAppointment = 0; }
            _textLocation.Text = _listAppointment[_indexAppointment].Location;
            _textSubject.Text = _listAppointment[_indexAppointment].Subject;
            if (_listAppointment[_indexAppointment].StartTime.Date != DateTime.Now.Date) // Если не сегодня 
            {
                _textTime.Text = _listAppointment[_indexAppointment].StartTime.ToShortDateString() + " " +
                    _listAppointment[_indexAppointment].StartTime.ToShortTimeString() + " - " +
                    _listAppointment[_indexAppointment].EndTime.ToShortTimeString();
            }
            else
            {

                _textTime.Text = _listAppointment[_indexAppointment].StartTime.ToShortTimeString() + " - " +
                    _listAppointment[_indexAppointment].EndTime.ToShortTimeString();
            }
            _textCount.Text = (_indexAppointment + 1).ToString() + "/" + _listAppointment.Count.ToString();
            _indexAppointment++;
            if (_indexAppointment > _listAppointment.Count) // Если показали все
            {
                GetState();   
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
                UpdateState();
            }
        }

        public void UpdateTheme()
        {
            _textLocation.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            _textSubject.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            _textTime.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
            _textCount.Style = new TextStyle(_fontName, _fontSize.ToLogic(), _fontColor);
        }

    }
}
