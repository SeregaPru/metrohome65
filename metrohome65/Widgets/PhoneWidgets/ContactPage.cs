using System.Drawing;
using Fleux.Animations;
using Fleux.Core;
using Fleux.Styles;
using Fleux.Templates;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using Microsoft.WindowsMobile.PocketOutlook;

namespace PhoneWidgets
{
    /// <summary>
    /// Popup form for contact tile.
    /// Shows on tile click.
    /// Contain buttons for call, send SMS etc.
    /// </summary>
    public sealed class ContactPage : WindowsPhone7Page
    {
        private const int PaddingVer = 10;
        private const int PaddingHor = 30;

        private readonly Contact _contact;

        private readonly TextStyle _titleStyle = new TextStyle(
                MetroTheme.PhoneFontFamilySemiLight,
                MetroTheme.PhoneFontSizeMediumLarge,
                MetroTheme.PhoneForegroundBrush);

        private readonly TextStyle _subTitleStyle = new TextStyle(
            MetroTheme.PhoneFontFamilySemiLight,
            MetroTheme.PhoneFontSizeSmall,
            MetroTheme.PhoneAccentBrush);

        public ContactPage(Contact contact) : base("", "profile", false)
        {
            _contact = contact;
            Control.ShadowedAnimationMode = Fleux.Controls.FleuxControl.ShadowedAnimationOptions.FromRight;

            theForm.Menu = null;
            Content.Size = new Size(Size.Width, Size.Height - 150); 

            var appBar = new ApplicationBar
                             {
                                 Size = new Size(Content.Size.Width, 48 + 2 * 10),
                                 Location = new Point(0, Content.Size.Height - 48 - 2 * 10)
                             };
            appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("PhoneWidgets.Images.edit.bmp"));
            appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("PhoneWidgets.Images.cancel.bmp"));
            appBar.ButtonTap += OnAppBarButtonTap;
            Content.AddElement(appBar.AnimateHorizontalEntrance(true));

            if (_contact == null) return;

            // write contact's name as title
            title1.Text = string.Format("{0} {1}", contact.FirstName, contact.LastName).ToUpper();

            var stackPanel = new StackPanel { Size = new Size(Content.Size.Width - PaddingHor * 2, 1), };

            var scroller = new ScrollViewer
                               {
                                   Content = stackPanel,
                                   Location = new Point(PaddingHor, 0),
                                   Size = new Size(Content.Size.Width - PaddingHor, Content.Size.Height - appBar.Size.Height),
                                   ShowScrollbars = true,
                                   HorizontalScroll = false,
                                   VerticalScroll = true,
                               };
            Content.AddElement(scroller.AnimateHorizontalEntrance(false));

            // contact's picture
            if (contact.Picture != null)
            {
                var img = new ImageElement(contact.Picture) { Size = new Size(180, 180), };
                stackPanel.AddElement(img);
                stackPanel.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
            }

            // call mobile
            AddCallPanel(stackPanel, "mobile", _contact.MobileTelephoneNumber);

            // call home
            AddCallPanel(stackPanel, "home", _contact.HomeTelephoneNumber);

            // call work
            AddCallPanel(stackPanel, "work", _contact.BusinessTelephoneNumber);

            // send sms to mobile
            if (!string.IsNullOrEmpty(_contact.MobileTelephoneNumber))
            {
                stackPanel.AddElement(new TextElement("text")
                {
                    Style = _titleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = p => SendSMS(_contact.MobileTelephoneNumber),
                });
                stackPanel.AddElement(new TextElement("SMS")
                {
                    Style = _subTitleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = p => SendSMS(_contact.MobileTelephoneNumber),
                });
                stackPanel.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
            }

            // send email to mobile
            if (!string.IsNullOrEmpty(_contact.Email1Address))
            {
                stackPanel.AddElement(new TextElement("send email")
                {
                    Style = _titleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = p => SendEmail(_contact.Email1Address),
                });
                stackPanel.AddElement(new TextElement(_contact.Email1Address)
                {
                    Style = _subTitleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = p => SendEmail(_contact.Email1Address),
                });
                stackPanel.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
            }

        }

        private void AddCallPanel(StackPanel parent, string phoneName, string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return;

            parent.AddElement(new TextElement("call " + phoneName)
                                  {
                                      Style = _titleStyle,
                                      AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                                      TapHandler = p => MakeCall(phoneNumber),
                                  });
            parent.AddElement(new TextElement(phoneNumber)
                                  {
                                      Style = _subTitleStyle,
                                      AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                                      TapHandler = p => MakeCall(phoneNumber),
                                  });
            parent.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
        }

        /// <summary>
        /// Handler for app bar buttons.
        /// 0 is edit contact
        /// 1 is close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // edit
                OpenContact();
            else
                Close();
        }

        private bool MakeCall(string number)
        {
            if (string.IsNullOrEmpty(number))
                return false;

            var myPhone = new Microsoft.WindowsMobile.Telephony.Phone();
            myPhone.Talk(number, false);

            Close();
            return true;
        }

        private bool SendSMS(string number)
        {
            if (string.IsNullOrEmpty(number))
                return false;

            var message = new SmsMessage(number, "");
            MessagingApplication.DisplayComposeForm(message);

            Close();
            return true;
        }

        private bool SendEmail(string emailTo)
        {
            if (string.IsNullOrEmpty(emailTo))
                return false;

            var message = new EmailMessage();
            message.To.Add(new Recipient(emailTo));
            MessagingApplication.DisplayComposeForm(message);

            Close();
            return true;
        }

        private bool OpenContact()
        {
            if (_contact == null) return false;
            _contact.ShowDialog();

            Close();
            return true;
        }


    }
}
