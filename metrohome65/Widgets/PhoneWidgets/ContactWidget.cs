using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
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
        private AlphaImage _alternateImage = null;

        // current Y offset for contact name animation
        private int _offsetY;
        private const int _nameRectHeight = 48;
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
                    _bufferedImage = null; // clear buffered image
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
                    UpdateAlternatePicture();
                    _bufferedImage = null; // clear buffered image
                    NotifyPropertyChanged("AlternatePicturePath");
                }
            }
        }

        protected virtual void UpdateAlternatePicture()
        {
            if (_alternatePicturePath != "")
                _alternateImage = new AlphaImage(_alternatePicturePath);
            else
                _alternateImage = null;
        }

        Contact FindContact(int itemIdKey)
        {
            Contact FindedContact = null;
            var mySession = new OutlookSession();
            
            var collection = mySession.Contacts.Items;
            foreach (var contact in collection)
            {
                if (contact.ItemId.GetHashCode().Equals(itemIdKey))
                {
                    FindedContact = contact;
                    break;
                }
            }

            return FindedContact;
        }

        private Image _bufferedImage = null;

        private void DrawBufferedImage(Rectangle rect)
        {
            _bufferedImage = new Bitmap(rect.Width, rect.Height + _nameRectHeight);
            var g = Graphics.FromImage(_bufferedImage);

            var captionFont = new Font("Segoe WP", 9, FontStyle.Regular);
            var captionBrush = new SolidBrush(MetroTheme.PhoneForegroundBrush);

            var contact = FindContact(ContactId);

            if (contact == null)
            {
                g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                    new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height + _nameRectHeight));
                g.DrawString("Contact \n not \n found", captionFont, captionBrush, rect.Left + 10, rect.Top + 20);
                return;
            }

            var pictureRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);

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
                    g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush), pictureRect);

            // draw contact name - below picture
            g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                new Rectangle(rect.Left, rect.Top + rect.Height, rect.Width, _nameRectHeight));
            var contactName = contact.FileAs;
            g.DrawString(contactName, captionFont, captionBrush, rect.Left + 10, rect.Top + rect.Height + 5);
            // - g.MeasureString(contactName, captionFont).Height
        }

        public override void Paint(Graphics g, Rectangle rect)
        {
            if (_bufferedImage == null)
                DrawBufferedImage(rect);
            int top = rect.Top - _offsetY;
            g.DrawImage(_bufferedImage, rect.Left, top);

            // border around
            var borderPen = new Pen(MetroTheme.PhoneAccentBrush, 1);
            g.DrawRectangle(borderPen, new Rectangle(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1));
            g.DrawRectangle(borderPen, new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2));
        }

        public bool Active
        {
            get { return (_animateTimer != null); }
            set
            {
                if (value)
                {
                    if (_animateTimer == null)
                        _animateTimer = new ThreadTimer(10, AnimateTile, (new Random()).Next(2000));
                }
                else
                {
                    if (_animateTimer != null)
                        _animateTimer.Stop();
                    _animateTimer = null;
                }
            }
        }

        private void AnimateTile()
        {
            _offsetY += _animateStep;
            ForceUpdate();

            if ((_offsetY <= 0) || (_offsetY >= _nameRectHeight))
            {
                _animateStep = -_animateStep;
                _animateTimer.SafeSleep(2000 + (new Random()).Next(2000));
            }
        }


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


        /// <summary>
        /// on click open contact
        /// </summary>
        /// <param name="location"></param>
        public override bool OnClick(Point location)
        {
            return OpenContact();
        }
        
        /// <summary>
        /// on double click make a call
        /// </summary>
        /// <param name="location"></param>
        public override bool OnDblClick(Point location)
        {
            return MakeCall();
        }


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

        private bool OpenContact()
        {
            Contact contact = FindContact(this.ContactId);
            if (contact != null)
            {
                contact.ShowDialog();
                return true;
            }
            return false;
        }

    }

}
