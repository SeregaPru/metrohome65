using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;

namespace Fleux.UIElements
{

    public class ComboBoxPopup : ListElement
    {
        public ComboBoxPopup()
        {
            ThreadedFill = false;
        }
    }


    public class ComboBox: Canvas
    {
        #region Fields

        protected const int PopupItemHeight = 70;
        protected const int EditItemHeight = 50;
        protected const int Padding = 10;

        private List<object> _items;

        private int _selectedIndex;

        private UIElement _selectedElement;

        private ComboBoxPopup _popupList;

        private bool _droppedDown;

        private Size _prevSize;

        #endregion


        #region Properties

        public TextStyle Style { get; set; }

        public List<object> Items
        {
            get { return _items; }
            set
            {
                _items = value;

                _selectedIndex = -1;
                SelectedIndex = 0;
            }
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex == value) return;

                if (value <= 0) _selectedIndex = 0;
                else
                if (value >= _items.Count) _selectedIndex = _items.Count - 1;
                else
                    _selectedIndex = value;

                _selectedElement = BuildCustomItem(_items[_selectedIndex], false);
                _selectedElement.Location = new Point(Padding, 0);
                _selectedElement.Size = new Size(Size.Width - Padding, Math.Min(this.Size.Height, EditItemHeight));
                _selectedElement.TapHandler = p => DropDown();

                if (!_droppedDown)
                {
                    this.Clear();
                    AddElement(_selectedElement);
                    Update();
                }

                if (SelectedIndexChanged != null)
                    SelectedIndexChanged(this, new EventArgs());
            }
        }

        #endregion


        #region Methods

        public ComboBox()
        {
            Style = MetroTheme.PhoneTextNormalStyle;
            _items = new List<object> { "" };
            _selectedIndex = 0;
            TapHandler = p => DropDown();
        }

        private ComboBoxPopup CreatePopup()
        {
            var sourceItems = new BindingList<object>();
            foreach (var item in _items)
                sourceItems.Add(item);

            return new ComboBoxPopup
                             {
                                 Size = new Size(Size.Width - Padding, sourceItems.Count * PopupItemHeight),
                                 DataTemplateSelector = item => BuildItem,
                                 SourceItems = sourceItems,
                             };
        }

        protected virtual UIElement BuildItem(object arg)
        {
            var item = BuildCustomItem(arg, true);
            item.TapHandler += p => SelectItem(arg);
            return item;
        }

        protected virtual UIElement BuildCustomItem(object arg, bool forList)
        {
            return new TextElement((string)arg)
            {
                Style = ((forList) && ((arg as string) == (string) Items[SelectedIndex])) ?
                    new TextStyle(
                        MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                        MetroTheme.PhoneAccentBrush) :
                    new TextStyle(
                        MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                        MetroTheme.PhoneTextBoxFontBrush),
                Size = new Size(Size.Width - Padding, PopupItemHeight),
            };
        }

        private bool DropDown()
        {
            if (_droppedDown) return false;

            _popupList = CreatePopup();
            _popupList.Location = new Point(Padding, 0);

            _prevSize = this.Size;
            this.Clear();
            AddElement(_popupList);
            _droppedDown = true;

            Update();

            return true;
        }

        private bool CloseUp()
        {
            if (! _droppedDown) return false;

            this.Clear();
            AddElement(_selectedElement);
            this.Size = _prevSize;

            _droppedDown = false;
            _popupList = null;

            Update();
            return true;
        }

        /// <summary>
        /// mark current item as selected
        /// </summary>
        /// <returns></returns>
        protected bool SelectItem(object arg)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == arg)
                {
                    SelectedIndex = i;

                    if (CloseUp())
                        return true;
                }
            }
            return false;
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(MetroTheme.PhoneTextBoxBrush);
            drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height);

            drawingGraphics.Color(MetroTheme.PhoneTextBoxBorderBrush);
            drawingGraphics.PenWidth(MetroTheme.PhoneBorderThickness.BorderThickness.Pixels);
            drawingGraphics.DrawRectangle(0, 0, Size.Width, Size.Height);

            base.Draw(drawingGraphics);
        }

        // event triggered when selected item changed
        public event EventHandler SelectedIndexChanged;

        #endregion

    }
}
