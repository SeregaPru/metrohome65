using System.Collections.Generic;
using System.Drawing;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Pivot;

namespace FleuxControls
{
    public class MainForm : FleuxControlPage
    {
        public MainForm() : base(false)
        {
            TheForm.Menu = null;

            var pivot = new Pivot("SETTINGS")
                            {
                                Size = new Size(480, 700),
                            };
            pivot.AddPivotItem(CreateFirstPage());
            pivot.AddPivotItem(CreateSecondPage());
            Control.AddElement(pivot);
        }

        private PivotItem CreateSecondPage()
        {
            var page = new PivotItem { Title = "first" };
            var stackPanel = new StackPanel { Size = new Size(480, 700) };
            page.Body = stackPanel;

            stackPanel.AddElement(
                new Button("Test button 1")
                    {
                        Size = new Size(300, 50),
                        AutoSizeMode = Button.AutoSizeModeOptions.None,
                    }
                );

            return page;
        }

        private PivotItem CreateFirstPage()
        {
            var page = new PivotItem { Title = "second", };
            var stackPanel = new StackPanel { Size = new Size(480, 700) };
            page.Body = stackPanel;

            stackPanel.AddElement(new ComboBox()
                    {
                        Size = new Size(300, 50),
                        Items = new List<object>() { "11111", "222222", "333333", "444444", "5", "6", "7", "8" },
                        Style = MetroTheme.PhoneTextNormalStyle,
                    }
                );
            stackPanel.AddElement(new Canvas() { Size = new Size(100, 100), });

            stackPanel.AddElement(new ColorComboBox()
                    {
                        Size = new Size(300, 50),
                        Items = new List<object>()
                                    {
                                        new ColorItem(Color.Magenta, "magenta"), 
                                        new ColorItem(Color.Purple, "purple"),  
                                        new ColorItem(Color.Teal, "teal"), 
                                        new ColorItem(Color.Lime, "lime"), 
                                        new ColorItem(Color.Brown, "brown"), 
                                        new ColorItem(Color.Pink, "pink"), 
                                        new ColorItem(Color.Orange, "orange"), 
                                        new ColorItem(Color.Blue, "blue"), 
                                        new ColorItem(Color.Red, "red"), 
                                        new ColorItem(Color.Green, "green")
                                    },
                        Style = MetroTheme.PhoneTextNormalStyle,
                    }
                );
            stackPanel.AddElement(new Canvas() { Size = new Size(100, 100), });

            stackPanel.AddElement(new ComboBox()
                    {
                        Size = new Size(300, 50),
                        Items = new List<object>() { "dark", "light" },
                        Style = MetroTheme.PhoneTextNormalStyle,
                    }
                );
            stackPanel.AddElement(new Canvas() { Size = new Size(100, 100), });

            stackPanel.AddElement(new ColorComboBox()
                    {
                        Size = new Size(300, 50),
                        Items = new List<object>()
                                    {
                                        new ColorItem(Color.Magenta, "magenta"), 
                                        new ColorItem(Color.Purple, "purple"),  
                                        new ColorItem(Color.Teal, "teal"), 
                                        new ColorItem(Color.Lime, "lime"), 
                                        new ColorItem(Color.Brown, "brown"), 
                                        new ColorItem(Color.Pink, "pink"), 
                                        new ColorItem(Color.Orange, "orange"), 
                                        new ColorItem(Color.Blue, "blue"), 
                                        new ColorItem(Color.Red, "red"), 
                                        new ColorItem(Color.Green, "green")
                                    },
                        Style = MetroTheme.PhoneTextNormalStyle,
                    }
                );
            stackPanel.AddElement(new Canvas() { Size = new Size(100, 100), });

            return page;
        }

    }
}
