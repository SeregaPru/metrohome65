using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Fleux.Animations;
using Fleux.Styles;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using Microsoft.WindowsMobile.PocketOutlook;
using MetroHome65.Settings.Controls;
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
        private const int NameRectHeight = 82;
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
                        _needAnimateTile = (_contact != null) && ((_contact.Picture != null) || (_alternateImage != null));
                    }
                    catch (Exception) { _needAnimateTile = false; }

                    NotifyPropertyChanged("ContactId");
                }
            }
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
                    _needRepaint = true;
                    UpdateAlternatePicture();
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
                    var mySession = new OutlookSession();

                    var collection = mySession.Contacts.Items;
                    foreach (var contact in collection)
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

        protected virtual void UpdateAlternatePicture()
        {
            _alternateImage = _alternatePicturePath != "" ? new AlphaImage(_alternatePicturePath) : null;
        }

        public override void PaintBuffer(Graphics g, Rectangle rect)
        {
            try
            {

                if (_contact == null)
                {
                    g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                        new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height));
                    var errorFont = new Font(MetroTheme.TileTextStyle.FontFamily, 8, FontStyle.Regular);
                    g.DrawString("Contact \n not \n found", errorFont, new SolidBrush(MetroTheme.TileTextStyle.Foreground), rect.Left + 10, rect.Top + 10);
                    return;
                }

                var captionFont = new Font(MetroTheme.TileTextStyle.FontFamily, 11, FontStyle.Regular);
                var pictureRect = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
                var nameRectHeight = NameRectHeight;
                var nameRectTop = rect.Height + 25;

                // if assigned alternate picture - use it
                if (_alternateImage != null)
                {
                    _alternateImage.PaintBackground(g, pictureRect);
                }
                else
                // use picture from contact, if present
                if (_contact.Picture != null)
                {
                    g.DrawImage(_contact.Picture,
                                new Rectangle(0, 0, rect.Width, rect.Height),
                                0, 0, _contact.Picture.Width, _contact.Picture.Height,
                                GraphicsUnit.Pixel, new System.Drawing.Imaging.ImageAttributes());
                }
                else
                {
                    g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush), pictureRect);
                    nameRectHeight = 0;
                    nameRectTop -= rect.Height;
                }

                // draw contact name - below picture
                g.FillRectangle(new SolidBrush(MetroTheme.PhoneAccentBrush),
                    new Rectangle(rect.Left, rect.Top + rect.Height, rect.Width, nameRectHeight));
                var contactName = _contact.FileAs;
                g.DrawString(contactName, captionFont, new SolidBrush(MetroTheme.TileTextStyle.Foreground), rect.Left + 10, rect.Top + nameRectTop);

            }
            catch (Exception) { }
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if ((_buffer == null) || (_needRepaint))
            {
                var newbuffer = new DoubleBuffer(new Size(Size.Width, (_needAnimateTile) ? Size.Height + NameRectHeight : Size.Height));
                PaintBuffer(newbuffer.Graphics, new Rectangle(0, 0, Size.Width, Size.Height));
                _buffer = newbuffer;
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
                To = ((_offsetY <= 0) ? NameRectHeight : 0),
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

        #endregion

        public override List<Control> EditControls
        {
            get
            {
                List<Control> controls = base.EditControls;
                var editControl = new Settings_contact {Value = ContactId};
                controls.Add(editControl);

                var imgControl = new Settings_image {Caption = "Alternate picture", Value = AlternatePicturePath};
                controls.Add(imgControl);

                var bindingManager = new BindingManager();
                bindingManager.Bind(this, "ContactId", editControl, "Value");
                bindingManager.Bind(this, "AlternatePicturePath", imgControl, "Value");

                return controls;
            }
        }


        /// <summary>
        /// on click open contact
        /// </summary>
        /// <param name="location"></param>
        public override bool OnClick(Point location)
        {
            var contactPage = new ContactPage(_contact);

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Publish(new ShowPageMessage(contactPage));

            return true;
        }

    }

}
