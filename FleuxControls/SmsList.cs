using System;
using System.Drawing;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Pivot;
using MAPIdotnet;

namespace FleuxControls
{
    public class SmsList : FleuxControlPage
    {
        private StackPanel _stackPanel;


        public SmsList() : base(false)
        {
            TheForm.Menu = null;

            var pivot = new Pivot("SMS")
            {
                Size = new Size(480, 700),
            };
            pivot.AddPivotItem(CreateFirstPage());
            pivot.AddPivotItem(CreateFontPage());
            Control.AddElement(pivot);

            FillStores();
        }

        private PivotItem CreateFontPage()
        {
            var page = new PivotItem { Title = "incoming", };
            var stackPanel = new StackPanel { Size = new Size(480, 700) };

            var scroller = new ScrollViewer
            {
                Content = stackPanel,
                Location = new Point(0, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };

            for (var fontSize = 8; fontSize < 24; fontSize += 2)
            {
                stackPanel.AddElement(
                    new TextElement("Font " + fontSize.ToString())
                        {
                            Size = new Size(400, 50),
                            Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, fontSize, Color.White),
                        }
                    );
            }

            page.Body = scroller;
            return page;
        }

        private PivotItem CreateFirstPage()
        {
            var page = new PivotItem { Title = "incoming", };
            _stackPanel = new StackPanel { Size = new Size(480, 700) };

            var scroller = new ScrollViewer
            {
                Content = _stackPanel,
                Location = new Point(0, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };

            page.Body = scroller;
            return page;
        }

        private MAPI _mapi;
        private IMAPIMsgStore[] _stores;

        private void FillStores()
        {
            this._mapi = new MAPI();
            this._stores = this._mapi.MessageStores;

            for (int i = 0, length = this._stores.Length; i < length; i++)
            {
                IMAPIMsgStore store = this._stores[i];

                _stackPanel.AddElement(
                    new TextElement("STORE:" + store.ToString() + " ")
                        {
                            Size = new Size(400, 50),
                        }
                    );

                PopulateFolders(store.RootFolder.OpenFolder());
            }
        }

        private void PopulateFolders(IMAPIFolder folder)
        {
            IMAPIFolderID[] subFolders = folder.GetSubFolders((int)folder.NumSubFolders);
            foreach (IMAPIFolderID fId in subFolders)
            {
                IMAPIFolder f = fId.OpenFolder();

                _stackPanel.AddElement(
                    new TextElement(" FOLDER:" + f.ToString() + " (" + f.NumSubItems.ToString() + " sub items)")
                        {
                            AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                            Size = new Size(450, 70),
                        }
                    );
                
                PopulateFolders(f);
                FillFolder(f);
            }
        }


        private void FillFolder(IMAPIFolder folder)
        {
            // May as well show latest messages first:
            folder.SortMessagesByDeliveryTime(TableSortOrder.TABLE_SORT_DESCEND);
            // Get all the messages
            IMAPIMessage[] messages = folder.GetNextMessages(folder.NumSubItems);

            for (int i = 0, length = messages.Length; i < length; i++)
            {
                IMAPIMessage msg = messages[i];
                try
                {
                    _stackPanel.AddElement(new SmsBox(msg));
                }
                catch (Exception ex)
                {
                }
            }

        }

    }



    public class SmsBox : StackPanel
    {
        public SmsBox(IMAPIMessage msg)
        {
            msg.PopulateProperties(EMessageProperties.DeliveryTime | EMessageProperties.Sender |
                                   EMessageProperties.Subject);
            IMAPIContact sender = msg.Sender;
            DateTime delivery = msg.LocalDeliveryTime;


            // Subject:
            AddElement(
                new TextElement(msg.Subject)
                {
                    Size = new Size(450, 50),
                    AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                    Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, 10, Color.White),
                }
                );

            var add = "";
            // Delivery Date:
            if (delivery != null)
            {
                add += delivery.ToString(" d/MM/yy H:mm   ");
            }

            // Sender:
            if (sender != null)
            {
                add += sender.FullAddress;
            }

            if (( (uint)msg.Status & (uint)EMessageStatus.MSGSTATUS_RECTYPE_SMTP) == 0)
                add += " (SMS) ";
            
            add += " " + msg.Flags.ToString() + " " + msg.Status.ToString();

            if (! string.IsNullOrEmpty(add))
                AddElement( new TextElement(add)
                    {
                        Size = new Size(450, 30),
                        Style = new TextStyle(MetroTheme.PhoneFontFamilyLight, 7, Color.LightSteelBlue),
                    }
                );

            // Recipients:
            /*
            IMAPIContact[] recipients = msg.Recipients;
            if (recipients.Length > 0)
            {
                TreeNode recipNode = new TreeNode("Recipients");
                foreach (IMAPIContact recipient in recipients)
                {
                    n = new TreeNode(recipient.Name + " (" + recipient.FullAddress + ')');
                    n.Tag = recipient;
                    recipNode.Nodes.Add(n);
                }
                node.Nodes.Add(recipNode);
            }
            nodes[i] = node;
            */

            AddElement( new Canvas() { Size = new Size(10, 10), } );
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(Color.Gray);
            drawingGraphics.FillRectangle(0, 0, Size.Width, Size.Height - 10);

            base.Draw(drawingGraphics);
        }

    }
}
