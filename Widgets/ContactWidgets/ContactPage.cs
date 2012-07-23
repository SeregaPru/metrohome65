using System;
using System.Drawing;
using Fleux.Animations;
using Fleux.Core;
using Fleux.Styles;
using Fleux.Templates;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines.Screen;
using Microsoft.WindowsMobile.PocketOutlook;

namespace MetroHome65.Widgets
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

        private Contact _contact;

        private readonly StackPanel _stackPanel;

        private readonly TextStyle _titleStyle = new TextStyle(
                MetroTheme.PhoneFontFamilySemiLight,
                MetroTheme.PhoneFontSizeMediumLarge,
                MetroTheme.PhoneForegroundBrush);

        private readonly TextStyle _subTitleStyle = new TextStyle(
            MetroTheme.PhoneFontFamilySemiLight,
            MetroTheme.PhoneFontSizeSmall,
            MetroTheme.PhoneAccentBrush);


        public Contact Contact
        {
            get { return _contact; }
            set { SetContact(value); }
        }

        public ContactPage() : base("", "profile", false)
        {
            ScreenRoutines.CursorWait();
            try
            {
                Control.ShadowedAnimationMode = Fleux.Controls.FleuxControl.ShadowedAnimationOptions.FromRight;

                theForm.Menu = null;
                Content.Size = new Size(Size.Width, Size.Height - 150); 

                var appBar = new ApplicationBar
                                 {
                                     Size = new Size(Content.Size.Width, 48 + 2 * 10),
                                     Location = new Point(0, Content.Size.Height - 48 - 2 * 10)
                                 };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("ContactWidgets.Images.back.bmp"));
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("ContactWidgets.Images.edit.bmp"));
                appBar.ButtonTap += OnAppBarButtonTap;
                Content.AddElement(appBar.AnimateHorizontalEntrance(false));

                _stackPanel = new StackPanel { Size = new Size(Content.Size.Width - PaddingHor * 2, 1), };

                var scroller = new ScrollViewer
                {
                    Content = _stackPanel,
                    Location = new Point(PaddingHor, 0),
                    Size = new Size(Content.Size.Width - PaddingHor, Content.Size.Height - appBar.Size.Height),
                    ShowScrollbars = true,
                    HorizontalScroll = false,
                    VerticalScroll = true,
                };
                Content.AddElement(scroller.AnimateHorizontalEntrance(false));

            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        private void SetContact(Contact value)
        {
            if (value == null) return;

            ScreenRoutines.CursorWait();
            try
            {
                _contact = value;

                // write contact's name as title
                title1.Text = string.Format("{0} {1}", _contact.FirstName, _contact.LastName).ToUpper();

                _stackPanel.Clear();

                // contact's picture
                if (_contact.Picture != null)
                {
                    var img = new ImageElement(_contact.Picture) { Size = new Size(180, 180), };
                    _stackPanel.AddElement(img);
                    _stackPanel.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
                }

                // call mobile
                AddCallPanel(_stackPanel, "mobile", _contact.MobileTelephoneNumber);

                // call home
                AddCallPanel(_stackPanel, "home", _contact.HomeTelephoneNumber);

                // call work
                AddCallPanel(_stackPanel, "work", _contact.BusinessTelephoneNumber);

                // send sms to mobile
                if (!string.IsNullOrEmpty(_contact.MobileTelephoneNumber))
                {
                    _stackPanel.AddElement(new TextElement("text")
                    {
                        Style = _titleStyle,
                        AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                        TapHandler = p => SendSMS(_contact.MobileTelephoneNumber),
                    });
                    _stackPanel.AddElement(new TextElement("SMS")
                    {
                        Style = _subTitleStyle,
                        AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                        TapHandler = p => SendSMS(_contact.MobileTelephoneNumber),
                    });
                    _stackPanel.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
                }

                // send email to mobile
                if (!string.IsNullOrEmpty(_contact.Email1Address))
                {
                    _stackPanel.AddElement(new TextElement("send email")
                    {
                        Style = _titleStyle,
                        AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                        TapHandler = p => SendEmail(_contact.Email1Address),
                    });
                    _stackPanel.AddElement(new TextElement(_contact.Email1Address)
                    {
                        Style = _subTitleStyle,
                        AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                        TapHandler = p => SendEmail(_contact.Email1Address),
                    });
                    _stackPanel.AddElement(new DelegateUIElement { Size = new Size(10, PaddingVer), });
                }
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }


        public event EventHandler ContactChanged;

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
            if (e.ButtonID == 1) // edit
                OpenContact();
            else
                Close();
        }

        public override void Close()
        {
            _contact = null;
            this.TheForm.Hide();
            //base.Close();
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

            if (ContactChanged != null)
                ContactChanged(this, new EventArgs());

            Close();
            return true;
        }


    }
}
