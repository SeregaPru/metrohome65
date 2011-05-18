using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MetroHome65.Widgets
{

    public class ContactWidget : BaseWidget
    {
        protected override String[] GetMenuItems()
        {
            String[] Items = { "Call", "Send SMS" };
            return Items;
        }

        public override void OnMenuItemClick(String ItemName)
        {
            MessageBox.Show(ItemName);
        }

        public override void Paint(Graphics g, Rectangle Rect)
        {
            Pen BorderPen = new System.Drawing.Pen(Color.Gray, 2);
            g.DrawRectangle(BorderPen, Rect.Left, Rect.Top, Rect.Width - 2, Rect.Height - 2);

            String ContactName = "";

            OutlookSession mySession = new OutlookSession();
            
            ContactCollection collection = mySession.Contacts.Items;
            foreach (Contact contact in collection)
            {
                ContactName = contact.FileAs;
                //contact.ItemId 
                if (contact.Picture != null)
                    g.DrawImage(contact.Picture,
                        new Rectangle(Rect.Left + 1, Rect.Top + 1, Rect.Width - 3, Rect.Height - 3),
                        0, 0, contact.Picture.Width, contact.Picture.Height, GraphicsUnit.Pixel,
                        new System.Drawing.Imaging.ImageAttributes());

                Font captionFont = new System.Drawing.Font("Helvetica", 9, FontStyle.Regular);
                Brush captionBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                g.DrawString(ContactName, captionFont, captionBrush,
                    Rect.Left + 10, Rect.Bottom - 5 - g.MeasureString(ContactName, captionFont).Height);
            }
        }
    }

}
