//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
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
using Workshopper.UI;
using Workshopper.Filesystem;

namespace Workshopper.Core
{
    public static class UGCHandler
    {
        public struct ImagePreviewItem_t
        {
            public string url;
            public string fileDestination;
        }

        public static CreationPanel creationHandle { get; set; }
        public static List<ImagePreviewItem_t> GetImagePreviewQueue() { return pszImagePreviewURLs; }

        private static List<ImagePreviewItem_t> pszImagePreviewURLs;
        private static UGCQueryHandle_t _ugcHandle;
        private static UGCUpdateHandle_t _ugcUpdateHandle;
        private static CallResult<SteamUGCQueryCompleted_t> m_QueryUGCCompleted;
        private static CallResult<CreateItemResult_t> m_CreateItemResult;
        private static CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdate;

        public static void Initialize()
        {
            m_QueryUGCCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnUGCQueryComplete);
            m_CreateItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItem);
            m_SubmitItemUpdate = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItem);
            pszImagePreviewURLs = new List<ImagePreviewItem_t>();
            creationHandle = null;

            RetrievePublishedItems();
        }

        public static void RetrievePublishedItems()
        {
            if (!SteamHandler.HasInitializedSteam())
                return;

            _ugcHandle = SteamUGC.CreateQueryUserUGCRequest(
                SteamUser.GetSteamID().GetAccountID(),
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderAsc,
                SteamUtils.GetAppID(), (AppId_t)346330, 1);
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

            string imagePath = string.IsNullOrEmpty(previewFilePath) ? null : Path.GetDirectoryName(previewFilePath);
            Utils.CreateItemDataFile(itemID, imagePath, contentPath);
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
                    tags += string.Format("{0},", tagList[i]);
            }

            if (bUpdate)
                SteamHandler.GetAddonList().UpdateItem(fileID, title, description, tags, visibility, Utils.GetCurrentDate());
            else
                SteamHandler.GetAddonList().AddItem(title, description, tags, visibility, fileID, Utils.GetCurrentDate(), true);

            SteamHandler.GetAddonList().StartUploading(fileID, handle);
        }

        private static void OnCreateItem(CreateItemResult_t param, bool bIOFailure)
        {
            if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement || bIOFailure || (param.m_eResult != EResult.k_EResultOK))
            {
                Utils.ShowWarningDialog(Localization.GetTextForToken("CREATION_FAILED1"), null, true);
                return;
            }

            if (creationHandle == null)
                return;

            creationHandle.SubmitWorkshopItem(param.m_nPublishedFileId, "Initial Release");
        }

        private static void OnSubmitItem(SubmitItemUpdateResult_t param, bool bIOFailure)
        {
            if (param.m_bUserNeedsToAcceptWorkshopLegalAgreement || bIOFailure || (param.m_eResult != EResult.k_EResultOK))
            {
                RetrievePublishedItems();
                Utils.ShowWarningDialog(Localization.GetTextForToken("CREATION_FAILED2"), null, true);
                return;
            }

            SteamHandler.GetAddonList().StopUploading();
            Utils.ShowWarningDialog(Localization.GetTextForToken("CREATION_SUCCESS1"), null, true);
        }

        private static void OnUGCQueryComplete(SteamUGCQueryCompleted_t param, bool bIOFailure)
        {
            SteamHandler.GetAddonList().RemoveItems();
            pszImagePreviewURLs.Clear();

            if (bIOFailure || (param.m_eResult != EResult.k_EResultOK))
            {
                Utils.ShowWarningDialog(Localization.GetTextForToken("ADDONLIST_FETCH_FAIL1"), null, true);
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
                    SteamHandler.GetAddonList().AddItem(pDetails.m_rgchTitle, pDetails.m_rgchDescription, tags, (int)pDetails.m_eVisibility, pDetails.m_nPublishedFileId, Utils.GetDateFromTimeCreated((ulong)pDetails.m_rtimeUpdated));
                }
            }

            SteamHandler.GetAddonList().RefreshLayout();
            SteamUGC.ReleaseQueryUGCRequest(_ugcHandle);

            StartDownloadingPreviewImages();
        }

        private static void AddImagePreviewItemToQueue(string url, string fileID)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(fileID))
                return;

            string fileDest = string.Format("{0}\\assets\\workshopper\\addons\\{1}.jpg", Globals.GetAppPath(), fileID);
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
                    Utils.LogAction(ex.Message);
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

            if (SteamHandler.GetMainForm() != null)
                SteamHandler.GetAddonList().RefreshItems();
        }
    }
}
