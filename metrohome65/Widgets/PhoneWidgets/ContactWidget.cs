﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Fleux.Animations;
using Fleux.Styles;
using MetroHome65.Interfaces;
using Microsoft.WindowsMobile.PocketOutlook;
using MetroHome65.Settings.Controls;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [TileInfo("Contact")]
    public class ContactWidget : BaseWidget, IActive
    {
        private int _contactId = -1;
        private String _alternatePicturePath = "";
        private AlphaImage _alternateImage;

        // current Y offset for contact name animation
        private int _offsetY;
        private const int _nameRectHeight = 82;
        private int _animateStep = 4;
        private ThreadTimer _animateTimer;

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        [TileParameter]
        public int ContactId
        {
            get { return _contactId; }
            set {
                if (_contactId != value)
                {
                    _contactId = value;
                    _needRepaint = true;

                    // флаг анимировать ли плитку - анимируем только когда есть картинка
                    try
                    {
                        var contact = FindContact(ContactId);
                        _needAnimateTile = (contact != null) && ((contact.Picture != null) || (_alternateImage != null));
                    }
                    catch (Exception) { _needAnimateTile = false; }

                    NotifyPropertyChanged("ContactId");
                }
            }
        }

        /// <summary>
        /// relative or absolute path to alternate contact picture file.
        /// picture format must be transparent PNG
        /// </summary>
        [TileParameter]
        public String AlternatePicturePath
        {
            get { return _alternatePicturePath; }
            set
            {
                if (_alternatePicturePath != value)
                {
                    _alternatePicturePath = value;
                    _needRepaint = true;
                    UpdateAlternatePicture();
                    NotifyPropertyChanged("AlternatePicturePath");
                }
            }
        }

        Contact FindContact(int itemIdKey)
        {
            Contact findedContact = null;

            // locked access to outlook session
            lock (this)
            {
                var mySession = new OutlookSession();

                var collection = mySession.Contacts.Items;
                foreach (var contact in collection)
                {
                    if (contact.ItemId.GetHashCode().Equals(itemIdKey))
                    {
                        findedContact = contact;
                        break;
                    }
                }
            }

            return findedContact;
        }

        #region Draw

        // double buffer
        private Bitmap _doubleBuffer;
        private Graphics _graphics;
        private bool _needRepaint;

        private void ClearBuffer()
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
            if (_doubleBuffer != null)
            {
                _doubleBuffer.Dispose();
                _doubleBuffer = null;
            }
        }

        private void PrepareBuffer()
        {
            ClearBuffer();

            _doubleBuffer = new Bitmap(Bounds.Width, (_needAnimateTile) ? Bounds.Height + _nameRectHeight : Bounds.Height);
            _graphics = Graphics.FromImage(_doubleBuffer);
        }

        protected virtual void UpdateAlternatePicture()
        {
            _alternateImage = _alternatePicturePath != "" ? new AlphaImage(_alternatePicturePath) : null;
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            var captionFont = new Font("Segoe WP", 9, FontStyle.Regular);

            var contact = FindContact(ContactId);

            if (contact == null)
            {
                g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                    new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height));
                g.DrawString("Contact \n not \n found", captionFont, new SolidBrush(MetroTheme.PhoneForegroundBrush), rect.Left + 10, rect.Top + 10);
                return;
            }

            var pictureRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
            var nameRectHeight = _nameRectHeight;
            var nameRectTop = rect.Height + 25;

            // if assigned alternate picture - use it
            if (_alternateImage != null)
            {
                _alternateImage.PaintBackground(g, pictureRect);
            }
            else
            // use picture from contact, if present
            if (contact.Picture != null)
            {
                g.DrawImage(contact.Picture,
                            new Rectangle(0, 0, rect.Width, rect.Height),
                            0, 0, contact.Picture.Width, contact.Picture.Height,
                            GraphicsUnit.Pixel, new System.Drawing.Imaging.ImageAttributes());
            }
            else
            {
                g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush), pictureRect);
                nameRectHeight = 0;
                nameRectTop -= rect.Height;
            }

            // draw contact name - below picture
            g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                new Rectangle(rect.Left, rect.Top + rect.Height, rect.Width, nameRectHeight));
            var contactName = contact.FileAs;
            g.DrawString(contactName, captionFont, new SolidBrush(MetroTheme.PhoneForegroundBrush), rect.Left + 10, rect.Top + nameRectTop);
            //// - g.MeasureString(contactName, captionFont).Height
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if ((_doubleBuffer == null) || (_needRepaint))
            {
                PrepareBuffer();
                PaintBuffer(_graphics, new Rectangle(0, 0, Bounds.Width, Bounds.Height));
                _needRepaint = false;
            }

            // for faster draw - paintdirectly to graphic
            //drawingGraphics.DrawImage(_doubleBuffer, 0, - _offsetY);
            drawingGraphics.Graphics.DrawImage(_doubleBuffer, - drawingGraphics.VisibleRect.Left, - drawingGraphics.VisibleRect.Top - _offsetY);

            // border around
            drawingGraphics.Color(MetroTheme.PhoneAccentBrush);
            drawingGraphics.DrawRectangle(0, 0, Bounds.Width - 1, Bounds.Height - 1);
            drawingGraphics.DrawRectangle(1, 1, Bounds.Width - 2, Bounds.Height - 2);
        }

        public override void ForceUpdate()
        {
            _needRepaint = true;
            Update();
            Application.DoEvents();
        }

        #endregion


        #region Animation

        private IAnimation _animation;

        // флаг анимировать ли плитку - анимируем только когда есть картинка
        private bool _needAnimateTile;

        private static StoryBoard sb = new StoryBoard();

        public bool Active
        {
            get { return (_animateTimer != null); }
            set
            {
                if (!_needAnimateTile)
                    return;

                if (value)
                {
                    if (_animateTimer == null)
                        _animateTimer = new ThreadTimer(10, AnimateTile, (new Random()).Next(10000));
                }
                else
                {
                    if (_animation != null)
                        _animation.Cancel();

                    if (_animateTimer != null)
                        _animateTimer.Stop();
                    _animateTimer = null;
                }
            }
        }

        private IAnimation GetAnimation()
        {
            return new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = 1000,
                From = _offsetY,
                To = ((_offsetY <= 0) ? _nameRectHeight : 0),
                OnAnimation = v =>
                {
                    _offsetY = v;
                    Update();
                    Application.DoEvents();
                },
            };
        }

        private void AnimateTile()
        {
            _animation = GetAnimation(); 
            lock (sb)
            {
                if (!Active) return;
                sb.Clear();
                sb.AddAnimation(_animation);
                sb.AnimateSync();
            }

            if (!Active) return;
            _animateTimer.SafeSleep(5000 + (new Random()).Next(5000));
                       
            _animation = GetAnimation();
            lock (sb)
            {
                if (!Active) return;
                sb.Clear();
                sb.AddAnimation(_animation);
                sb.AnimateSync();
            }

            if (!Active) return;
            _animateTimer.SafeSleep(10000 + (new Random()).Next(10000));
        }

        #endregion

        public override List<Control> EditControls
        {
            get
            {
                List<Control> controls = base.EditControls;
                var editControl = new Settings_contact {Value = ContactId};
                controls.Add(editControl);

                var imgControl = new Settings_image {Caption = "Alternate picture", Value = AlternatePicturePath};
                controls.Add(imgControl);

                var bindingManager = new BindingManager();
                bindingManager.Bind(this, "ContactId", editControl, "Value");
                bindingManager.Bind(this, "AlternatePicturePath", imgControl, "Value");

                return controls;
            }
        }


        // launch external application - play exit animation
        public override bool AnimateExit { get { return true; } }

        /// <summary>
        /// on click open contact
        /// </summary>
        /// <param name="location"></param>
        public override bool OnClick(Point location)
        {
            return OpenContact();
        }


        /*
        private bool MakeCall()
        {
            var contact = FindContact(ContactId);
            if (contact == null)
                return false;

            var myPhone = new Microsoft.WindowsMobile.Telephony.Phone();
            myPhone.Talk(contact.MobileTelephoneNumber, false);
            return true;
        }

        private void SendSMS()
        {
            var contact = FindContact(ContactId);
            var mySession = new OutlookSession();
            var message = new SmsMessage(contact.MobileTelephoneNumber, "");
            MessagingApplication.DisplayComposeForm(message);
        }
        */

        private bool OpenContact()
        {
            var contact = FindContact(ContactId);
            if (contact == null)
                return false;

            contact.ShowDialog();
            return true;
        }

    }

}
