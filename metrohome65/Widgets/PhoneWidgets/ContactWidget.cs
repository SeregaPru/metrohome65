using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.WindowsMobile.PocketOutlook;
using MetroHome65.Settings.Controls;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [WidgetInfo("Contact")]
    public class ContactWidget : BaseWidget
    {
        private int _ContactId = -1;
        private String _AlternatePicturePath = "";
        private AlphaImage _ContactImage = null;

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        protected override String[] GetMenuItems()
        {
            String[] Items = { "Call", "Send SMS" };
            return Items;
        }

        [WidgetParameter]
        public int ContactId
        {
            get { return _ContactId; }
            set {
                if (_ContactId != value)
                {
                    _ContactId = value;
                    NotifyPropertyChanged("ContactId");
                }
            }
        }

        /// <summary>
        /// relative or absolute path to alternate contact picture file.
        /// picture format must be transparent PNG
        /// </summary>
        [WidgetParameter]
        public String AlternatePicturePath
        {
            get { return _AlternatePicturePath; }
            set
            {
                if (_AlternatePicturePath != value)
                {
                    _AlternatePicturePath = value;
                    UpdateAlternatePicture();
                    NotifyPropertyChanged("AlternatePicturePath");
                }
            }
        }

        protected virtual void UpdateAlternatePicture()
        {
            if (_AlternatePicturePath != "")
                _ContactImage = new AlphaImage(_AlternatePicturePath);
            else
                _ContactImage = null;
        }

        Contact FindContact(int ItemIdKey)
        {
            Contact FindedContact = null;
            OutlookSession mySession = new OutlookSession();
            
            ContactCollection collection = mySession.Contacts.Items;
            foreach (Contact contact in collection)
            {
                if (contact.ItemId.GetHashCode().Equals(ItemIdKey))
                {
                    FindedContact = contact;
                    break;
                }
            }

            return FindedContact;
        }

        public override void Paint(Graphics g, Rectangle rect)
        {
            Pen BorderPen = new System.Drawing.Pen(Color.Gray, 2);
            g.DrawRectangle(BorderPen, rect.Left, rect.Top, rect.Width - 2, rect.Height - 2);

            Font captionFont = new System.Drawing.Font("Helvetica", 9, FontStyle.Regular);
            Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            Contact contact = FindContact(this.ContactId);

            if (contact == null)
            {
                g.FillRectangle(new System.Drawing.SolidBrush(Color.DarkBlue),
                    new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 3, rect.Height - 3));
                g.DrawString("Contact \n not \n found", captionFont, captionBrush,
                    rect.Left + 10, rect.Top + 10);
                return;
            }

            // if assigned alternate picture - use it
            if (_ContactImage != null)
            {
                _ContactImage.PaintBackground(g, rect);
            }
            else

            // use picture from contact, if present
            if (contact.Picture != null)
                g.DrawImage(contact.Picture,
                    new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 3, rect.Height - 3),
                    0, 0, contact.Picture.Width, contact.Picture.Height, GraphicsUnit.Pixel,
                    new System.Drawing.Imaging.ImageAttributes());
            else
                g.FillRectangle(new System.Drawing.SolidBrush(Color.DarkBlue), 
                    new Rectangle(rect.Left + 1, rect.Top + 1, rect.Width - 3, rect.Height - 3));

            String ContactName = contact.FileAs;
            g.DrawString(ContactName, captionFont, captionBrush,
                rect.Left + 10, rect.Bottom - 5 - g.MeasureString(ContactName, captionFont).Height);
        }


        public override void OnMenuItemClick(String itemName)
        {
            if (itemName == "Call")
                MakeCall();
            else
            if (itemName == "Send SMS")
                SendSMS();
        }


        public override List<Control> EditControls
        {
            get
            {
                List<Control> Controls = base.EditControls;
                Settings_contact EditControl = new Settings_contact();
                EditControl.Value = ContactId;
                Controls.Add(EditControl);

                Settings_image ImgControl = new Settings_image();
                ImgControl.Caption = "Alternate picture";
                ImgControl.Value = AlternatePicturePath;
                Controls.Add(ImgControl);

                BindingManager BindingManager = new BindingManager();
                BindingManager.Bind(this, "ContactId", EditControl, "Value");
                BindingManager.Bind(this, "AlternatePicturePath", ImgControl, "Value");

                return Controls;
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
            Contact contact = FindContact(this.ContactId);
            if (contact == null)
                return false;

            Microsoft.WindowsMobile.Telephony.Phone myPhone = new Microsoft.WindowsMobile.Telephony.Phone();
            myPhone.Talk(contact.MobileTelephoneNumber, false);
            return true;
        }

        private void SendSMS()
        {
            Contact contact = FindContact(ContactId);
            OutlookSession mySession = new OutlookSession();
            SmsMessage message = new Microsoft.WindowsMobile.PocketOutlook.SmsMessage(contact.MobileTelephoneNumber, "");
            Microsoft.WindowsMobile.PocketOutlook.MessagingApplication.DisplayComposeForm(message);
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
