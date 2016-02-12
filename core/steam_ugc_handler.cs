//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: Steam UGC handler, handles uploading, downloading, updating and so forth
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using System.Net;
using System.IO;
using System.Threading;

namespace workshopper.core
{
    public static class UGCHandler
    {
        // EVENTS
        public delegate void ItemCreatedHandler(object sender, UGCCreationEventArg e);
        public static event ItemCreatedHandler OnCreateWorkshopItem;

        public struct ImagePreviewItem_t
        {
            public string url;
            public string fileDestination;
        }

        public static List<ImagePreviewItem_t> GetImagePreviewQueue() { return pszImagePreviewURLs; }

        private static List<ImagePreviewItem_t> pszImagePreviewURLs;
        private static UGCQueryHandle_t _ugcHandle;
        private static UGCUpdateHandle_t _ugcUpdateHandle;
        private static CallResult<SteamUGCQueryCompleted_t> m_QueryUGCCompleted;
        private static CallResult<CreateItemResult_t> m_CreateItemResult;
        private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;
        private static bool m_bInit = false;

        public static void Initialize()
        {
            if (m_bInit)
                return;

            m_bInit = true;

            m_QueryUGCCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnUGCQueryComplete);
            m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItem);
            m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItem);
            pszImagePreviewURLs = new List<ImagePreviewItem_t>();

            RetrievePublishedItems();
        }

        public static void RetrievePublishedItems()
        {
            if (!factory_list_init.m_bHasInitialized)
                return;

            _ugcHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderAsc, SteamUtils.GetAppID(), (AppId_t)346330, 1);
            SteamUGC.SetReturnKeyValueTags(_ugcHandle, true);
            SteamAPICall_t registerCallback = SteamUGC.SendQueryUGCRequest(_ugcHandle);
            m_QueryUGCCompleted.Set(registerCallback);
        }

        public static void CreateItem()
        {
            SteamAPICall_t registerCallback = SteamUGC.CreateItem((AppId_t)346330, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            m_CreateItemResult.Set(registerCallback);
        }

        public static void SubmitItem(
            PublishedFileId_t itemID,
            string title,
            string description,
            int visibility,
            string contentPath,
            string previewFilePath,
            List<string> tagList,
            string changeLog,
            bool bIsUpdate
            )
        {
            _ugcUpdateHandle = SteamUGC.StartItemUpdate((AppId_t)346330, itemID);
            if (_ugcUpdateHandle == null)
                return;

            if (tagList == null)
                return;

            if (tagList.Count() <= 0)
                return;

            if (!string.IsNullOrEmpty(contentPath))
            {
                if (bIsUpdate)
                {
                    SteamUGC.RemoveItemKeyValueTags(_ugcUpdateHandle, "map_name");
                    SteamUGC.RemoveItemKeyValueTags(_ugcUpdateHandle, "map_size");
                }

                PerformMapItemCheck(_ugcUpdateHandle, contentPath);
                SteamUGC.SetItemContent(_ugcUpdateHandle, contentPath);
            }

            if (!string.IsNullOrEmpty(previewFilePath))
                SteamUGC.SetItemPreview(_ugcUpdateHandle, previewFilePath);

            SteamUGC.SetItemTitle(_ugcUpdateHandle, title);
            SteamUGC.SetItemDescription(_ugcUpdateHandle, description);
            SteamUGC.SetItemVisibility(_ugcUpdateHandle, (ERemoteStoragePublishedFileVisibility)visibility);
            SteamUGC.SetItemTags(_ugcUpdateHandle, tagList);

            AddNewItemToList(bIsUpdate, title, description, visibility, tagList, itemID, _ugcUpdateHandle);

            SteamAPICall_t registerCallback = SteamUGC.SubmitItemUpdate(_ugcUpdateHandle, changeLog);
            m_SubmitItemUpdate.Set(registerCallback);
        }

        private static void PerformMapItemCheck(UGCUpdateHandle_t handle, string contentPath)
        {
            if (handle == null || string.IsNullOrEmpty(contentPath))
                return;

            foreach (string file in Directory.EnumerateFiles(contentPath, "*.bsp", SearchOption.AllDirectories))
            {
                FileInfo info = new FileInfo(file);
                SteamUGC.AddItemKeyValueTag(handle, "map_name", Path.GetFileNameWithoutExtension(file));
                SteamUGC.AddItemKeyValueTag(handle, "map_size", info.Length.ToString());
            }
        }

        private static void AddNewItemToList(bool bUpdate, string title, string description, int visibility, List<string> tagList, PublishedFileId_t fileID, UGCUpdateHandle_t handle)
        {
            // Generate a list of the tags:
            string tags = null;
            if (tagList != null)
            {
                for (int i = 0; i < tagList.Count(); i++)
                {
                    tags += string.Format("{0},", tagList[i]);
                }
            }

            if (bUpdate)
                factory_list_init._mainFormAccessor._addonList.UpdateItem(fileID, title, description, tags, visibility, utils.GetCurrentDate());
            else
                factory_list_init._mainFormAccessor._addonList.AddItem(title, description, tags, visibility, fileID, utils.GetCurrentDate());

            factory_list_init._mainFormAccessor._addonList.StartUploading(fileID, handle);
        }

        private static void OnCreateItem(CreateItemResult_t param, bool bIOFailure)
        {
            if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement || bIOFailure || (param.m_eResult != EResult.k_EResultOK))
            {
                utils.ShowWarningDialog("Unable to create item!", null, true);
                return;
            }

            if (OnCreateWorkshopItem == null)
                return;

            UGCCreationEventArg args = new UGCCreationEventArg(param.m_nPublishedFileId);
            OnCreateWorkshopItem(null, args);
        }

        private static void OnSubmitItem(SubmitItemUpdateResult_t param, bool bIOFailure)
        {
            if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement || bIOFailure || (param.m_eResult != EResult.k_EResultOK))
            {
                RetrievePublishedItems();
                utils.ShowWarningDialog("Unable to submit item!", null, true);
                return;
            }

            factory_list_init._mainFormAccessor._addonList.StopUploading();
            utils.ShowWarningDialog("Submitted item successfully!", null, true);
        }

        private static void OnUGCQueryComplete(SteamUGCQueryCompleted_t param, bool bIOFailure)
        {
            factory_list_init._mainFormAccessor._addonList.RemoveItems();
            pszImagePreviewURLs.Clear();

            if (bIOFailure || (param.m_eResult != EResult.k_EResultOK))
            {
                utils.ShowWarningDialog("Unable to fetch workshop items!", null, true);
                SteamUGC.ReleaseQueryUGCRequest(_ugcHandle);
                return;
            }

            for (uint i = 0; i < param.m_unNumResultsReturned; i++)
            {
                SteamUGCDetails_t pDetails;
                if (SteamUGC.GetQueryUGCResult(param.m_handle, i, out pDetails))
                {
                    string url = null;
                    if (SteamUGC.GetQueryUGCPreviewURL(param.m_handle, i, out url, 1024))
                        AddImagePreviewItemToQueue(url, pDetails.m_nPublishedFileId.m_PublishedFileId.ToString());

                    string tags = pDetails.m_rgchTags + ",";   
                    factory_list_init._mainFormAccessor._addonList.AddItem(pDetails.m_rgchTitle, pDetails.m_rgchDescription, tags, (int)pDetails.m_eVisibility, pDetails.m_nPublishedFileId, utils.GetDateFromTimeCreated((ulong)pDetails.m_rtimeUpdated));
                }
            }

            SteamUGC.ReleaseQueryUGCRequest(_ugcHandle);

            StartDownloadingPreviewImages();
        }

        private static void AddImagePreviewItemToQueue(string url, string fileID)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileID))
                return;

            string fileDest = string.Format("{0}\\assets\\workshopper\\addons\\{1}.jpg", globals.GetAppPath(), fileID);
            if (File.Exists(fileDest))
                return;

            ImagePreviewItem_t item;
            item.url = url;
            item.fileDestination = fileDest;
            pszImagePreviewURLs.Add(item);
        }

        private static void StartDownloadingPreviewImages()
        {
            if (pszImagePreviewURLs.Count() <= 0)
                return;

            ImagePreviewDownloadThread downloadThread = new ImagePreviewDownloadThread();
            Thread dlThread = new Thread(new ThreadStart(downloadThread.DownloadFiles));
            dlThread.Start();
        }
    }

    public class ImagePreviewDownloadThread
    {
        public void DownloadFiles()
        {
            for (int i = (UGCHandler.GetImagePreviewQueue().Count() - 1); i >= 0; i--)
            {
                WebClient client = null;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(UGCHandler.GetImagePreviewQueue()[i].fileDestination));
                    client = new WebClient();
                    client.DownloadFile(UGCHandler.GetImagePreviewQueue()[i].url, UGCHandler.GetImagePreviewQueue()[i].fileDestination);
                }
                catch (Exception ex)
                {
                    utils.LogAction(ex.Message);
                }
                finally
                {
                    if (client != null)
                    {
                        client.Dispose();
                        client = null;
                    }

                    UGCHandler.GetImagePreviewQueue().RemoveAt(i);
                }
            }

            UGCHandler.GetImagePreviewQueue().Clear();
            factory_list_init._mainFormAccessor._addonList.RefreshItems();
        }
    }
}
