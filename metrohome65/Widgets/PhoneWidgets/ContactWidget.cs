using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MetroHome65.Widgets
{
    [WidgetInfo("Contact")]
    public class ContactWidget : BaseWidget
    {
        [WidgetParameter]
        public int ContactId = -1;

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
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
                g.DrawString("Contact \n not \n found", captionFont, captionBrush,
                    Rect.Left + 10, Rect.Top + 10);
                return;
            }

            String ContactName = contact.FileAs;
            if (contact.Picture != null)
                g.DrawImage(contact.Picture,
                    new Rectangle(Rect.Left + 1, Rect.Top + 1, Rect.Width - 3, Rect.Height - 3),
                    0, 0, contact.Picture.Width, contact.Picture.Height, GraphicsUnit.Pixel,
                    new System.Drawing.Imaging.ImageAttributes());

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


        public override Control[] EditControls
        {
            get
            {
                Control[] Controls = new Control[1];
                Settings_contact EditControl = new Settings_contact();
                EditControl.Value = ContactId;
                EditControl.OnValueChanged += new Settings_contact.ValueChangedHandler(EditControl_OnValueChanged);
                Controls[0] = EditControl;

                return Controls;
            }
        }

        void EditControl_OnValueChanged(int Value)
        {
            ContactId = Value;
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

        }

        private void SendSMS()
        {
            Contact contact = FindContact(ContactId);
            OutlookSession mySession = new OutlookSession();
            SmsMessage message = new Microsoft.WindowsMobile.PocketOutlook.SmsMessage(contact.ItemId);
        }

        private void OpenContact()
        {
        }

    }

}
