using System.Drawing;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Routines;
using Microsoft.WindowsMobile.PocketOutlook;
using TinyIoC;

namespace PhoneWidgets
{
    /// <summary>
    /// Popup form for contact tile.
    /// Shows on tile click.
    /// Contain buttons for call, send SMS etc.
    /// </summary>
    public sealed class ContactPage : Canvas
    {
        private const int FormWidth = 400;
        private const int ButtonWidth = FormWidth - 2 * 25;
        private const int ButtonHeight = 25;
        private const int PaddingVer = 10;
        private const int PaddingHor = 25;

        private readonly Contact _contact;

        public ContactPage(Contact contact)
        {
            Size = new Size(FormWidth, FormWidth);

            _contact = contact;
            if (contact == null) return;

            var buttonClose = new Button("x")
            {
                Size = new Size(20, 20),
                Location = new Point(FormWidth - 40, 20),
                TapHandler = (p) => { Close(); return true; },
            };
            AddElement(buttonClose);

            var textName = new TextElement(contact.FileAs)
                               {
                                   Style = MetroTheme.PhoneTextLargeStyle,
                                   Location = new Point(PaddingHor, PaddingHor),
                                   Size = new Size(ButtonWidth, 100),
                                   AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                               };
            AddElement(textName);

            var buttonCall = new Button("Call")
                                 {
                                     Size = new Size(ButtonWidth, ButtonHeight),
                                     Location = new Point(PaddingHor, textName.Bounds.Bottom + PaddingVer * 2),
                                     TapHandler = (p) => { MakeCall(); return true; },
                                 };
            AddElement(buttonCall);

            var buttonSms = new Button("Send SMS")
            {
                Size = new Size(ButtonWidth, ButtonHeight),
                Location = new Point(PaddingHor, buttonCall.Bounds.Bottom + PaddingVer),
                TapHandler = (p) => { SendSMS(); return true; },
            };
            AddElement(buttonSms);

            var buttonContact = new Button("Contact info")
            {
                Size = new Size(ButtonWidth, ButtonHeight),
                Location = new Point(PaddingHor, buttonSms.Bounds.Bottom + PaddingVer),
                TapHandler = (p) => { OpenContact(); return true; },
            };
            AddElement(buttonContact);
        }

        // draw solid background and frame around form
        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            var rect = new Rectangle(0, 0, Bounds.Width, Bounds.Height);

            drawingGraphics.Color(MetroTheme.PhoneBackgroundBrush);
            drawingGraphics.FillRectangle(rect);

            drawingGraphics.Color(MetroTheme.PhoneForegroundBrush);
            drawingGraphics.PenWidth(2);
            drawingGraphics.DrawRectangle(rect);

            base.Draw(drawingGraphics);
        }

        private bool MakeCall()
        {
            if (_contact == null)
                return false;

            var myPhone = new Microsoft.WindowsMobile.Telephony.Phone();
            myPhone.Talk(_contact.MobileTelephoneNumber, false);
            return true;
        }

        private void SendSMS()
        {
            var mySession = new OutlookSession();
            var message = new SmsMessage(_contact.MobileTelephoneNumber, "");
            MessagingApplication.DisplayComposeForm(message);
        }

        private bool OpenContact()
        {
            _contact.ShowDialog();
            return true;
        }

        private void Close()
        {
            var control = TinyIoCContainer.Current.Resolve<FleuxControl>();
            control.RemoveElement(this);
        }


    }
}
