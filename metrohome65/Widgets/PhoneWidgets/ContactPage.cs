using System.Drawing;
using Fleux.Animations;
using Fleux.Core;
using Fleux.Styles;
using Fleux.Templates;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines;
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

        public ContactPage(Contact contact) : base("", "profile", false)
        {
            _contact = contact;
            this.Control.ShadowedAnimationMode = Fleux.Controls.FleuxControl.ShadowedAnimationOptions.FromRight;

            this.theForm.Menu = null;

            var appBar = new ApplicationBar()
                             {
                                 Size = new Size(ScreenConsts.ScreenWidth, 48 + 2 * 10),
                                 Location = new Point(0, ScreenConsts.ScreenHeight - ScreenConsts.TopBarSize - Content.Location.Y - 48 - 2 * 10)
                             };
            appBar.ButtonTap += OnAppBarButtonTap;
            Content.AddElement(appBar.AnimateHorizontalEntrance(true));

            appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("PhoneWidgets.Images.edit.bmp"));
            appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("PhoneWidgets.Images.cancel.bmp"));

            if (_contact == null) return;

            // write contact's name as title
            title1.Text = string.Format("{0} {1}", contact.FirstName, contact.LastName).ToUpper();

            var stackPanel = new StackPanel()
                                 {
                                     Size = new Size(ScreenConsts.ScreenWidth - PaddingHor * 2, 10),
                                 };
            Content.AddElement(stackPanel.AnimateHorizontalEntrance(true));

            var scroller = new ScrollViewer()
                               {
                                   Content = stackPanel,
                                   Location = new Point(PaddingHor, title2.Bounds.Bottom),
                                   Size = new Size(
                                       ScreenConsts.ScreenWidth,
                                       ScreenConsts.ScreenHeight - title2.Bounds.Bottom - appBar.Size.Height),
                                   HorizontalScroll = false,
                                   ShowScrollbars = true,
                               };
            Content.AddElement(scroller.AnimateHorizontalEntrance(true));

            // contact's picture
            if (contact.Picture != null)
            {
                var img = new ImageElement(contact.Picture)
                              {
                                  Size = new Size(ScreenConsts.ScreenWidth/5*2, ScreenConsts.ScreenWidth/5*2),
                              };
                stackPanel.AddElement(img);
                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, PaddingVer), });
            }

            var titleStyle = new TextStyle(
                    MetroTheme.PhoneFontFamilySemiLight,
                    MetroTheme.PhoneFontSizeMediumLarge,
                    MetroTheme.PhoneForegroundBrush);

            var subTitleStyle = new TextStyle(
                MetroTheme.PhoneFontFamilySemiLight,
                MetroTheme.PhoneFontSizeSmall,
                MetroTheme.PhoneAccentBrush);

            // call mobile
            if (!string.IsNullOrEmpty(_contact.MobileTelephoneNumber))
            {
                stackPanel.AddElement(new TextElement("call mobile")
                {
                    Style = titleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = (p) => MakeCall(_contact.MobileTelephoneNumber),
                });
                stackPanel.AddElement(new TextElement(_contact.MobileTelephoneNumber)
                {
                    Style = subTitleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = (p) => MakeCall(_contact.MobileTelephoneNumber),
                });
                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, PaddingVer), });
            }

            // call mobile
            if (!string.IsNullOrEmpty(_contact.HomeTelephoneNumber))
            {
                stackPanel.AddElement(new TextElement("call home")
                {
                    Style = titleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = (p) => MakeCall(_contact.HomeTelephoneNumber),
                });
                stackPanel.AddElement(new TextElement(_contact.HomeTelephoneNumber)
                {
                    Style = subTitleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = (p) => MakeCall(_contact.HomeTelephoneNumber),
                });
                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, PaddingVer), });
            }

            // send sms to mobile
            if (!string.IsNullOrEmpty(_contact.MobileTelephoneNumber))
            {
                stackPanel.AddElement(new TextElement("text")
                {
                    Style = titleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = (p) => SendSMS(_contact.MobileTelephoneNumber),
                });
                stackPanel.AddElement(new TextElement("SMS")
                {
                    Style = subTitleStyle,
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                    TapHandler = (p) => SendSMS(_contact.MobileTelephoneNumber),
                });
                stackPanel.AddElement(new DelegateUIElement() { Size = new Size(10, PaddingVer), });
            }

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

            //var mySession = new OutlookSession();
            var message = new SmsMessage(number, "");
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
