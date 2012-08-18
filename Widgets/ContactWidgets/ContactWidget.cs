using System;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines.Screen;
using Metrohome65.Settings.Controls;
using Microsoft.WindowsMobile.PocketOutlook;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.Widgets
{
    [TileInfo("Contact")]
    public class ContactWidget : BaseWidget, IActive, IPause
    {
        private int _contactId = -1;
        private Contact _contact;
        private String _alternatePicturePath = "";
        private AlphaImage _alternateImage;

        // current Y offset for contact name animation
        private int _offsetY;

        private ThreadTimer _animateTimer;

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
        }

        // launch external application - play exit animation
        protected override bool GetDoExitAnimation() { return true; }


        private static OutlookSession _session = new OutlookSession();

        [TileParameter]
        public int ContactId
        {
            get { return _contactId; }
            set {
                if (_contactId != value)
                {
                    _contactId = value;
                    _needRepaint = true;

                    // флаг анимировать ли плитку - анимируем только когда есть картинка
                    try
                    {
                        _contact = FindContact(ContactId);
                        CalcNeedAnimate();
                    }
                    catch (Exception) { _needAnimateTile = false; }

                    NotifyPropertyChanged("ContactId");
                }
            }
        }

        private void CalcNeedAnimate()
        {
            _needAnimateTile = (_contact != null) && ((_contact.Picture != null) || (_alternateImage != null));
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
                    _alternateImage = _alternatePicturePath != "" ? new AlphaImage(_alternatePicturePath) : null;
                    CalcNeedAnimate();
                    _needRepaint = true;
                    NotifyPropertyChanged("AlternatePicturePath");
                }
            }
        }

        Contact FindContact(int itemIdKey)
        {
            Contact findedContact = null;

            // locked access to outlook session
            lock (this)
            {
                try
                {
                    foreach (var contact in (new OutlookSession()).Contacts.Items)
                    {
                        if (contact.ItemId.GetHashCode().Equals(itemIdKey))
                        {
                            findedContact = contact;
                            break;
                        }
                    }

                }
                catch (Exception) { }
            }

            return findedContact;
        }

        #region Draw

        // double buffer
        private DoubleBuffer _buffer;
        private bool _needRepaint;

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            try
            {
                if (_contact == null)
                {
                    g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                        new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height));
                    var errorFont = new Font(
                        MetroTheme.TileTextStyle.FontFamily, MetroTheme.TileTextStyle.FontSize.ToLogic(), FontStyle.Regular);
                    g.DrawString("Contact \n not \n found", errorFont, new SolidBrush(MetroTheme.TileTextStyle.Foreground), rect.Left + 10, rect.Top + 10);
                    return;
                }

                var captionFont = new Font(
                    MetroTheme.TileTextStyle.FontFamily, 11.ToLogic(), FontStyle.Regular);

                // draw contact name - above picture
                var nameRectHeight = NameRectHeight();
                g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                    new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height));
                var contactName = _contact.FileAs.Replace(" ", "\n").Replace(",", "");
                var measure = g.MeasureString(contactName, captionFont);
                int textOffsetY = (nameRectHeight - ((int)measure.Height).ToLogic()) / 2;

                g.DrawString(contactName, captionFont, new SolidBrush(MetroTheme.TileTextStyle.Foreground),
                    rect.Left + 10, rect.Top + textOffsetY);

                // if assigned alternate picture - use it
                if (_alternateImage != null)
                {
                    _alternateImage.PaintBackground(g, 
                        new Rectangle(rect.Left, rect.Top + nameRectHeight, rect.Width, rect.Height));
                }
                else
                // use picture from contact, if present
                if (_contact.Picture != null)
                {
                    g.DrawImage(_contact.Picture,
                                new Rectangle(0, nameRectHeight, rect.Width, rect.Height),
                                0, 0, _contact.Picture.Width, _contact.Picture.Height,
                                GraphicsUnit.Pixel, new System.Drawing.Imaging.ImageAttributes());
                }

            }
            catch (Exception) { }
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if ((_buffer == null) || (_needRepaint))
            {
                var newbuffer = new DoubleBuffer(new Size(Size.Width, (_needAnimateTile) ? Size.Height + NameRectHeight() : Size.Height));
                PaintBuffer(newbuffer.Graphics, new Rectangle(0, 0, Size.Width, Size.Height));
                _buffer = newbuffer;
                _offsetY = 0;
                _needRepaint = false;
            }

            drawingGraphics.DrawImage(_buffer.Image,
                                      new Rectangle(0, 0, Size.Width, Size.Height),
                                      new Rectangle(0, _offsetY, Size.Width, Size.Height));
        }

        public override void ForceUpdate()
        {
            _needRepaint = true;
            Update();
        }

        #endregion


        #region Animation

        private IAnimation _animation;

        // флаг анимировать ли плитку - анимируем только когда есть картинка
        private bool _needAnimateTile;

        private static readonly StoryBoard _sb = new StoryBoard();

        private bool _pause;

        public bool Pause
        {
            get { return _pause; }
            set
            {
                _pause = value;
                if (_animation != null)
                    _animation.Cancel();
            }
        }

        private bool _active;

        public bool Active
        {
            get { return _active; }
            set
            {
                if (!_needAnimateTile)
                    return;

                _active = value;

                if (value)
                {
                    if (_animateTimer == null)
                        _animateTimer = new ThreadTimer(10, AnimateTile, (new Random()).Next(10000));
                }
                else
                {
                    if (_animation != null)
                        _animation.Cancel();

                    if (_animateTimer != null)
                        _animateTimer.Stop();
                    _animateTimer = null;
                }
            }
        }

        private IAnimation GetAnimation()
        {
            return new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = 500,
                From = _offsetY,
                To = ((_offsetY <= 0) ? NameRectHeight() : 0),
                OnAnimation = v =>
                {
                    if (Pause || !Active) return;

                    _offsetY = v;
                    Update();
                },
            };
        }

        private void AnimateTile()
        {
            _animation = GetAnimation(); 
            lock (_sb)
            {
                if (Pause || !Active) return;
                _sb.Clear();
                _sb.AddAnimation(_animation);
                _sb.AnimateSync();
            }

            if (Pause || !Active) return;
            _animateTimer.SafeSleep(5000 + (new Random()).Next(5000));
            if (Pause || !Active) return;
                       
            _animation = GetAnimation();
            lock (_sb)
            {
                if (Pause || !Active) return;
                _sb.Clear();
                _sb.AddAnimation(_animation);
                _sb.AnimateSync();
            }

            if (Pause || !Active) return;
            _animateTimer.SafeSleep(10000 + (new Random()).Next(10000));
        }

        private int NameRectHeight()
        {
            return (GridSize.Height == 1) ? Bounds.Height : Bounds.Height / 2;
        }

        #endregion

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var contactControl = new ContactSettingsControl
                                 {
                                     Caption = "Contact", 
                                     Value = ContactId,
                                 };
            controls.Add(contactControl);
            bindingManager.Bind(this, "ContactId", contactControl, "Value");

            var imgControl = new ImageSettingsControl()
            {
                Caption = "Alternate image",
                Value = AlternatePicturePath,
            };
            controls.Add(imgControl);
            bindingManager.Bind(this, "AlternatePicturePath", imgControl, "Value");

            return controls;
        }

        private ContactPage _contactPage;

        /// <summary>
        /// on click open contact
        /// </summary>
        /// <param name="location"></param>
        public override bool OnClick(Point location)
        {
            if (_contactPage == null)
            {
                _contactPage = new ContactPage();
                _contactPage.ContactChanged += (s, e) => OnContactChanged();
            }
            _contactPage.Contact = _contact;

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Publish(new ShowPageMessage(_contactPage));

            return true;
        }

        // update contact properties on any contact change
        private void OnContactChanged()
        {
            var newContact = FindContact(_contactId);
            _contact = newContact;

            CalcNeedAnimate();
            ForceUpdate();
        }

    }

}
