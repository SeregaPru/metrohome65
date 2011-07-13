﻿using System;
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

        public override void Paint(Graphics g, Rectangle Rect)
        {
            Pen BorderPen = new System.Drawing.Pen(Color.Gray, 2);
            g.DrawRectangle(BorderPen, Rect.Left, Rect.Top, Rect.Width - 2, Rect.Height - 2);

            Font captionFont = new System.Drawing.Font("Helvetica", 9, FontStyle.Regular);
            Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);

            Contact contact = FindContact(this.ContactId);

            if (contact == null)
            {
                g.FillRectangle(new System.Drawing.SolidBrush(Color.DarkBlue),
                    new Rectangle(Rect.Left + 1, Rect.Top + 1, Rect.Width - 3, Rect.Height - 3));
                g.DrawString("Contact \n not \n found", captionFont, captionBrush,
                    Rect.Left + 10, Rect.Top + 10);
                return;
            }

            if (contact.Picture != null)
                g.DrawImage(contact.Picture,
                    new Rectangle(Rect.Left + 1, Rect.Top + 1, Rect.Width - 3, Rect.Height - 3),
                    0, 0, contact.Picture.Width, contact.Picture.Height, GraphicsUnit.Pixel,
                    new System.Drawing.Imaging.ImageAttributes());
            else
                g.FillRectangle(new System.Drawing.SolidBrush(Color.DarkBlue), 
                    new Rectangle(Rect.Left + 1, Rect.Top + 1, Rect.Width - 3, Rect.Height - 3));

            String ContactName = contact.FileAs;
            g.DrawString(ContactName, captionFont, captionBrush,
                Rect.Left + 10, Rect.Bottom - 5 - g.MeasureString(ContactName, captionFont).Height);
        }


        public override void OnMenuItemClick(String ItemName)
        {
            if (ItemName == "Call")
                MakeCall();
            else
            if (ItemName == "Send SMS")
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

                BindingManager BindingManager = new BindingManager();
                BindingManager.Bind(this, "ContactId", EditControl, "Value");

                return Controls;
            }
        }


        /// <summary>
        /// on click open contact
        /// </summary>
        /// <param name="Location"></param>
        public override void OnClick(Point Location)
        {
            OpenContact();
        }
        
        /// <summary>
        /// on double click make a call
        /// </summary>
        /// <param name="Location"></param>
        public override void OnDblClick(Point Location)
        {
            MakeCall();
        }


        private void MakeCall()
        {
            Contact contact = FindContact(this.ContactId);
            if (contact == null)
                return;

            Microsoft.WindowsMobile.Telephony.Phone myPhone = new Microsoft.WindowsMobile.Telephony.Phone();
            myPhone.Talk(contact.MobileTelephoneNumber, false);
        }

        private void SendSMS()
        {
            Contact contact = FindContact(ContactId);
            OutlookSession mySession = new OutlookSession();
            SmsMessage message = new Microsoft.WindowsMobile.PocketOutlook.SmsMessage(contact.MobileTelephoneNumber, "");
            Microsoft.WindowsMobile.PocketOutlook.MessagingApplication.DisplayComposeForm(message);
        }

        private void OpenContact()
        {
            Contact contact = FindContact(this.ContactId);
            if (contact != null)
                contact.ShowDialog();
        }

    }

}
