using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MAPIdotnet;
using Microsoft.WindowsMobile.PocketOutlook;

namespace PocketMail
{
    public partial class MainForm : Form
    {
        private MAPI mapi;
        private IMAPIMsgStore[] stores;

        public MainForm()
        {
            InitializeComponent();
            this.mapi = new MAPI();
            this.stores = this.mapi.MessageStores;
            PopulateFolderTree();
        }

        private void PopulateFolderTree()
        {
            this.treeViewMain.Nodes.Clear();
            // Populate message stores
            for (int i = 0, length = this.stores.Length; i < length; i++)
            {
                IMAPIMsgStore store = this.stores[i];
                TreeNode n = new TreeNode(store.ToString());
                this.treeViewMain.Nodes.Add(n);
                // Recursively populate store folders
                PopulateFolders(store.RootFolder.OpenFolder(), n);
                // For this we'll register for every event
                store.MessageEvent += new MessageEventHandler(store_MessageEvent);
                store.NewMessage += new NewMessageEventHandler(store_NewMessage);
                store.FolderEvent += new FolderEventHandler(store_FolderEvent);
                
#warning Uncomment below line to receive events
                //store.EventNotifyMask = EEventMask.fnevNewMail | EEventMask.fnevObjectCreated | EEventMask.fnevObjectModified | EEventMask.fnevObjectMoved | EEventMask.fnevObjectCopied;
                this.treeViewMain.Focus();
            }
        }

        private void store_FolderEvent(IMAPIFolderID newFolderID, IMAPIFolderID oldFolderID, EEventMask flags)
        {
            IMAPIFolder f = newFolderID.OpenFolder();
            MessageBox.Show(f.DisplayName, "Folder event: " + flags.ToString());
        }

        private void store_MessageEvent(IMAPIMessageID newMessageID, IMAPIMessageID oldMessageID, EEventMask messageFlags)
        {
            IMAPIMessage msg = newMessageID.OpenMessage();
            string m = msg.Subject + " at " + msg.LocalDeliveryTime.ToString();
            MessageBox.Show(m + ", in " + newMessageID.ParentFolder.OpenFolder().DisplayName, "Message event: " + messageFlags.ToString());
        }

        private void store_NewMessage(IMAPIMessageID newMessageID, EMessageFlags flags)
        {
            IMAPIMessage msg = newMessageID.OpenMessage();
            string m = msg.Subject + ", from " + msg.Sender;
            MessageBox.Show(m, "New message: " + flags.ToString());
        }

        private void PopulateFolders(IMAPIFolder folder, TreeNode node)
        {
            IMAPIFolderID[] subFolders = folder.GetSubFolders((int)folder.NumSubFolders);
            foreach (IMAPIFolderID fId in subFolders)
            {
                IMAPIFolder f = fId.OpenFolder();
                TreeNode newNode = new TreeNode(f.ToString() + " (" + f.NumSubItems.ToString() + " sub items)");
                newNode.Tag = fId;
                node.Nodes.Add(newNode);
                PopulateFolders(f, newNode);
            }
        }

        private void treeViewFolders_AfterSelect(object o, EventArgs e)
        {
            bool wasShowingFolders = this.treeViewMain.Visible;
            
            // If was showing folders, hide folder tree and show message tree (and vice versa)
            if (wasShowingFolders)
            {
                TreeNode selectedNode = this.treeViewMain.SelectedNode;
                if (selectedNode == null || selectedNode.Tag == null)
                    return;
                this.treeViewMessages.Nodes.Clear();
                this.treeViewMain.Visible = !wasShowingFolders;
                this.treeViewMessages.Visible = wasShowingFolders;
                this.treeViewMessages.Nodes.Add("Populating nodes now...");
                this.Refresh();

                IMAPIFolderID folderId = selectedNode.Tag as IMAPIFolderID;
                // Clone so that we're getting a fresh copy to read messages from each time
                IMAPIFolder folder = folderId.OpenFolder();
                this.treeViewMessages.Tag = folder;
                // May as well show latest messages first:
                folder.SortMessagesByDeliveryTime(TableSortOrder.TABLE_SORT_DESCEND);
                // Get all the messages
                IMAPIMessage[] messages = folder.GetNextMessages(folder.NumSubItems);

                TreeNode[] nodes = new TreeNode[messages.Length];
                for (int i = 0, length = messages.Length; i < length; i++)
                {
                    IMAPIMessage msg = messages[i];
                    TreeNode node = new TreeNode(i.ToString());
                    node.Tag = msg;
                    msg.PopulateProperties(EMessageProperties.DeliveryTime | EMessageProperties.Sender | EMessageProperties.Subject);

                    // Sender:
                    IMAPIContact sender = msg.Sender;
                    TreeNode n;
                    if (sender != null)
                    {
                        n = new TreeNode("Sender: " + sender.FullAddress);
                        n.Tag = sender;
                        node.Nodes.Add(n);
                    }

                    // Subject:
                    node.Nodes.Add("Subject: " + msg.Subject);

                    // Delivery Date:
                    DateTime delivery = msg.LocalDeliveryTime;
                    if (delivery != null)
                    {
                        n = new TreeNode("Delivery date: " + delivery.ToString("H:mm d/MM/yy"));
                        n.Tag = delivery;
                        node.Nodes.Add(n);
                    }

                    // Recipients:
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
                }
                this.treeViewMessages.Nodes.Add("Done populating, adding now...");
                this.Refresh();
                SuspendLayout();
                this.treeViewMessages.Nodes.Clear();
                this.treeViewMessages.Nodes.AddRange(nodes);
                ResumeLayout();

                if (this.treeViewMessages.Nodes.Count > 0)
                    this.treeViewMessages.SelectedNode = this.treeViewMessages.Nodes[0];
                this.menuItemSelect.Text = "Folders";
                this.treeViewMessages.Focus();
            }
            else
            {
                object t = this.treeViewMessages.SelectedNode.Tag;
                if (typeof(IMAPIContact).IsAssignableFrom(t.GetType()))
                {
                    IMAPIContact c = t as IMAPIContact;
                    MessagingApplication.DisplayComposeForm("SMS", c.AsPOOM().ToString());
                }

                this.menuItemSelect.Text = "Select";
                this.treeViewMain.Visible = !wasShowingFolders;
                this.treeViewMessages.Visible = wasShowingFolders;
                PopulateFolderTree();
            }

        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItemDeleteMsg_Click(object sender, EventArgs e)
        {
            if (!this.treeViewMessages.Visible)
                return;
            TreeNode n = this.treeViewMessages.SelectedNode;
            while (n.Parent != null)
                n = n.Parent;
            IMAPIMessage msg = n.Tag as IMAPIMessage;
            IMAPIFolder folder = (IMAPIFolder)this.treeViewMessages.Tag;
            folder.DeleteMessage(msg.MessageID);
            //folder.DeleteMessages(new IMAPIMessageID[] { msg.MessageID });
            treeViewFolders_AfterSelect(null, null);
        }

        private void menuItemEmptyFolder_Click(object sender, EventArgs e)
        {
            if (!this.treeViewMain.Visible || this.treeViewMain.SelectedNode == null)
                return;
            IMAPIFolder folder = (this.treeViewMain.SelectedNode.Tag as IMAPIFolderID).OpenFolder();
            folder.EmptyFolder();
        }
    }
}