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

        protected const int ItemHeight = 60;

        private List<object> _items;

        private int _selectedIndex;

        private ComboBoxPopup _popupList;

        private bool _droppedDown;

        #endregion


        #region Properties

        public TextStyle Style { get; set; }

        public List<object> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                _selectedIndex = 0;

                _popupList = null;
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
                if (value >= _items.Count) _selectedIndex = _items.Count;
                else
                _selectedIndex = value;

                Update();
            }
        }

        private UIElement _selectedElement;

        #endregion


        #region Methods

        public ComboBox()
        {
            _items = new List<object> { "" };
            Style = MetroTheme.PhoneTextNormalStyle;
            TapHandler = p => DropDown(); 
        }

        private ComboBoxPopup CreatePopup()
        {
            var sourceItems = new BindingList<object>();
            foreach (var item in _items)
                sourceItems.Add(item);

            return new ComboBoxPopup
                             {
                                 Size = new Size(Size.Width, sourceItems.Count * ItemHeight),
                                 DataTemplateSelector = item => BuildItem,
                                 SourceItems = sourceItems,
                             };
        }

        protected virtual UIElement BuildItem(object arg)
        {
            var item = BuildCustomItem(arg, true);
            item.TapHandler = p => SelectItem(arg);
            return item;
        }

        protected virtual UIElement BuildCustomItem(object arg, bool forList)
        {
            return new TextElement((string)arg)
            {
                Style = ((forList) && ((arg as string) == Items[SelectedIndex])) ?
                    new TextStyle(
                        MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                        MetroTheme.PhoneAccentBrush) :
                    new TextStyle(
                        MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeNormal,
                        MetroTheme.PhoneBackgroundBrush),
                Size = new Size(Size.Width - 10, ItemHeight),
            };
        }

        private Point ScreenLocation(UIElement element)
        {
            var pos = new Point(element.Location.X, element.Location.Y);
            var e = element;
            while ((e = e.Parent) != null)
                pos.Offset(e.Location.X, e.Location.Y);
            return pos;
        }

        private bool DropDown()
        {
            if (_droppedDown) return false;

            _popupList = CreatePopup();
            _popupList.Location = new Point(10, 0);

            this.Clear();
            AddElement(_popupList);
            _droppedDown = true;

            //ParentControl.AddElement(_popupList);
            Update();

            return true;
        }

        private bool CloseUp()
        {
            if ((! _droppedDown) || (_popupList == null)) return false;
            _droppedDown = false;

            //ParentControl.RemoveElement(_popupList);
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

                    _selectedElement = BuildCustomItem(_items[_selectedIndex], false);

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

            /*
            if (_droppedDown)
                _popupList.Draw(drawingGraphics.CreateChild(new Point(10, 0)));
            else
                _selectedElement.Draw(drawingGraphics.CreateChild(new Point(10, 0)));
             */ 
        }

        #endregion

    }
}
