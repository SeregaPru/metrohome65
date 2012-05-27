using System;
using System.Collections.Generic;
using System.Text;

namespace MAPIdotnet
{
    internal class MAPIMsgStores
    {
        /*private MAPI parent;
        private cemapi.IMAPISession session;
        private cemapi.IMAPITable msgStores = null;

        public MAPIMsgStores(MAPI parent, cemapi.IMAPISession session)
        {
            this.parent = parent; this.session = session;
            this.msgStores = this.session.GetMsgStoresTable();
        }

        //public void Dispose()

        ~MAPIMsgStores()
        {
            this.session.Dispose();
        }

        //public IMAPIMsgStore DefaultStore { get; }

        public IMAPIMsgStore[] Stores 
        {
            get
            {
                this.msgStores.SetColumns(new cemapi.PropTags[] { cemapi.PropTags.PR_DISPLAY_NAME, cemapi.PropTags.PR_ENTRYID });
                cemapi.SRow[] rows = this.msgStores.QueryRows(msgStoresRows);
                IMAPIMsgStore[] stores = new IMAPIMsgStore[rows.Length];
                for (int i = 0; i < rows.Length; i++)
                    stores[i] = new MAPIMsgStore(rows[i].propVals[0].AsString, rows[i].propVals[1].AsBinary);
                return stores;
            }
        }

        public string[] StoreNames 
        {
            get
            {
                this.msgStores.SetColumns(new cemapi.PropTags[] { cemapi.PropTags.PR_DISPLAY_NAME });
                cemapi.SRow[] rows = this.msgStores.QueryRows(9); // only check first 9, unlikely to be more!
                return cemapi.SRow.PropsToStringArray(rows, 0);
            }
        }*/

    }
}
