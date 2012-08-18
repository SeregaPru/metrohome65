// MAPIlib.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
//#include <commctrl.h>
#include "MAPIlib.h"

BOOL APIENTRY DllMain( HANDLE a, DWORD b, LPVOID c)
{
    return TRUE;
}

/* IMAPIAdviseSink */
MAPIAdviseSink::MAPIAdviseSink(IMsgStore *store, ULONG eventMask, AdviseSinkCallback callback)
	: refs(0), callback(callback), msgNum(0), store(store)
{
	this->prevResult = this->store->Advise(0,
		NULL,
		eventMask,
		this,
		&(this->conNum));
}

ULONG MAPIAdviseSink::OnNotify(ULONG cNotif, LPNOTIFICATION lpNotifications)
{
	this->callback(this->store, lpNotifications);
	return 0;
}

ULONG MAPIAdviseSink::Release()
{
	this->store->Unadvise(this->conNum);
	this->conNum = 0;
	return 0;
}

HRESULT MAPIlibAdvise(IMsgStore *store, ULONG eventMask, AdviseSinkCallback callback, IMAPIAdviseSink **adviser)
{
	MAPIAdviseSink *a = new MAPIAdviseSink(store, eventMask, callback);
	*adviser = a;
	return a->prevResult;
}

HRESULT MAPIlibUnadvise(MAPIAdviseSink *adviser)
{
	return adviser->Release();
	delete adviser;
}


/* MAPI Functions */
HRESULT MAPIlibLogon(IMAPISession **pSession)
{
	return MAPILogonEx(0, NULL, NULL, 0, pSession);
}

/* IUnknown */
HRESULT Release(IUnknown *i)
{
	return i->Release();
}

/* IMAPISession */
HRESULT IMAPISessionLogoff(IMAPISession *pSession)
{
	return pSession->Logoff(0, 0, 0);
}

HRESULT IMAPISessionGetMsgStoresTable(IMAPISession *pSession, IMAPITable **pMsgStoresTable)
{
	return pSession->GetMsgStoresTable(MAPI_UNICODE, pMsgStoresTable);
}

HRESULT IMAPISessionOpenMsgStore(IMAPISession *pSession, SBinary *entryId, IMsgStore **msgStore)
{
	return pSession->OpenMsgStore(0, entryId->cb, (ENTRYID*)entryId->lpb, NULL, 0, msgStore);
}

HRESULT IMAPISessionCompareEntryIDs(IMAPISession *pSession, SBinary *entryID1, SBinary *entryID2, ULONG * lpulResult)
{
	return pSession->CompareEntryIDs(entryID1->cb, (LPENTRYID)entryID1->lpb, entryID2->cb, (LPENTRYID)entryID2->lpb, 0, lpulResult);
}

/* IMAPITable */
HRESULT IMAPITableGetRowCount(IMAPITable *table, ULONG *lpulCount)
{
	return table->GetRowCount(0, lpulCount);
}

HRESULT IMAPITableSetColumns(IMAPITable *table, LPSPropTagArray columns)
{
	return table->SetColumns(columns, 0);
}

HRESULT IMAPITableQueryRows(IMAPITable *table, LONG lRowCount, LPSRowSet *pRows)
{
	return table->QueryRows(lRowCount, 0, pRows);
}

HRESULT IMAPITableSortTable(IMAPITable *table, LPSSortOrderSet lpSortCriteria)
{
	return table->SortTable(lpSortCriteria, 0);
}

HRESULT IMAPITableSeekRow(IMAPITable *table, LONG lRowCount)
{
	return table->SeekRow(BOOKMARK_BEGINNING, lRowCount, NULL);
}

/* IMAPIProp */
HRESULT IMAPIPropGetProps(IMAPIProp *prop, LPSPropTagArray tags, ULONG *count/*out*/, LPSPropValue *lppPropArray)
{
	return prop->GetProps(tags, 0, count, lppPropArray);
}

HRESULT IMAPIPropSetProps(IMAPIProp *prop, ULONG cValues, LPSPropValue lpPropArray)
{
	return prop->SetProps(cValues, lpPropArray, NULL);
}

/* IMsgStore */
HRESULT IMsgStoreOpenEntry(IMsgStore *store, SBinary *entryID, ULONG ulObjType, IUnknown **lppUnk)
{
	return store->OpenEntry(entryID->cb, (LPENTRYID)entryID->lpb, NULL, 0, &ulObjType, lppUnk);
}

HRESULT IMsgStoreGetReceiveFolder(IMsgStore *store, ULONG *lpcbEntryID, LPENTRYID *lppEntryID)
{
	return store->GetReceiveFolder(NULL, 0, lpcbEntryID, lppEntryID, NULL);
}

/* IMAPIContainer */
HRESULT IMAPIContainerGetContentsTable(IMAPIContainer *container, IMAPITable **table)
{
	return container->GetContentsTable(0, table);
}

HRESULT IMAPIContainerGetHierarchyTable(IMAPIContainer *container, IMAPITable **table)
{
	return container->GetHierarchyTable(0, table);
}

HRESULT IMAPIContainerOpenEntry(IMAPIContainer *container, SBinary *entryID, ULONG ulObjType, IUnknown **lppUnk)
{
	return container->OpenEntry(entryID->cb, (LPENTRYID)entryID->lpb, NULL, 0, &ulObjType, lppUnk);
}

/* IMAPIFolder */
HRESULT IMAPIFolderDeleteMessages(IMAPIFolder *folder, LPENTRYLIST lpMsgList)
{
	return folder->DeleteMessages(lpMsgList, 0, NULL, 0);
}

HRESULT IMAPIFolderEmptyFolder(IMAPIFolder *folder)
{
	return folder->EmptyFolder(0, NULL, 0);
}

/* IMessage */
HRESULT IMessageGetRecipientTable(IMessage *msg, IMAPITable **table)
{
	return msg->GetRecipientTable(0, table);
}