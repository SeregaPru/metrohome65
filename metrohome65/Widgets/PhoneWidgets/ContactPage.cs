using System.Drawing;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.Templates;
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
    public sealed class ContactPage : WindowsPhone7Page
    {
        private const int FormWidth = 400;
        private const int PaddingVer = 30;
        private const int PaddingHor = 30;
        private const int ButtonWidth = FormWidth - 2 * PaddingHor;
        private const int ButtonHeight = 50;

        private readonly Contact _contact;

        public ContactPage(Contact contact) : base("CONTACT", "NAME SURNAME")
        {
            this.Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

            _contact = contact;
            if (contact == null) return;

            var textName = new TextElement(contact.FileAs)
                               {
                                   Style = MetroTheme.PhoneTextLargeStyle,
                                   Location = new Point(PaddingHor, PaddingVer),
                                   Size = new Size(ButtonWidth, 100),
                                   AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                               };
            Content.AddElement(textName);

            var buttonCall = new Button("Call")
                                 {
                                     Size = new Size(ButtonWidth, ButtonHeight),
                                     Location = new Point(PaddingHor, textName.Bounds.Bottom + PaddingVer),
                                     AutoSizeMode = Button.AutoSizeModeOptions.OneLineAutoHeight,
                                     TapHandler = (p) => MakeCall(),
                                 };
            Content.AddElement(buttonCall);

            var buttonSms = new Button("Send SMS")
                                {
                                    Size = new Size(ButtonWidth, ButtonHeight),
                                    Location = new Point(PaddingHor, buttonCall.Bounds.Bottom + PaddingVer),
                                    AutoSizeMode = Button.AutoSizeModeOptions.OneLineAutoHeight,
                                    TapHandler = (p) => SendSMS(),
                                };
            Content.AddElement(buttonSms);

            var buttonContact = new Button("Contact info")
                                    {
                                        Size = new Size(ButtonWidth, ButtonHeight),
                                        Location = new Point(PaddingHor, buttonSms.Bounds.Bottom + PaddingVer),
                                        AutoSizeMode = Button.AutoSizeModeOptions.OneLineAutoHeight,
                                        TapHandler = (p) => OpenContact(),
                                    };
            Content.AddElement(buttonContact);


            var buttonClose = new Button("x")
            {
                Size = new Size(ButtonWidth, ButtonHeight),
                Location = new Point(PaddingHor, buttonContact.Bounds.Bottom + PaddingVer),
                AutoSizeMode = Button.AutoSizeModeOptions.OneLineAutoHeight,
                TapHandler = (p) => { this.Close(); return true; },
            };
            Content.AddElement(buttonClose);
        }


        private bool MakeCall()
        {
            if (string.IsNullOrEmpty(_contact.MobileTelephoneNumber))
                return false;

            var myPhone = new Microsoft.WindowsMobile.Telephony.Phone();
            myPhone.Talk(_contact.MobileTelephoneNumber, false);
            return true;
        }

        private bool SendSMS()
        {
            if (string.IsNullOrEmpty(_contact.MobileTelephoneNumber))
                return false;

            var mySession = new OutlookSession();
            var message = new SmsMessage(_contact.MobileTelephoneNumber, "");
            MessagingApplication.DisplayComposeForm(message);
            return true;
        }

        private bool OpenContact()
        {
            _contact.ShowDialog();
            return true;
        }


    }
}
