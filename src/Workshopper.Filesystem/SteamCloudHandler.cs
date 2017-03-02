//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Handle reading from and writing to Steam Cloud.
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshopper.Core;
using Steamworks;
using System.IO;

namespace Workshopper.Filesystem
{
    public static class SteamCloudHandler
    {
        public static string GetAddonItemPath(string file) { return string.Format("{0}\\itemdata\\{1}.txt", Globals.GetAppPath(), file); }
        public static bool IsCloudAvailable(bool bWrite = false)
        {
            if (!SteamHandler.HasInitializedSteam())
                return false;

            if (!SteamRemoteStorage.IsCloudEnabledForAccount() || !SteamRemoteStorage.IsCloudEnabledForApp())
                return false;

            int totalBytes, availableBytes;
            if (!SteamRemoteStorage.GetQuota(out totalBytes, out availableBytes))
                return false;

            if (bWrite)
            {
                if (availableBytes <= 0)
                    return false;
            }
            else
            {
                if (totalBytes <= 0)
                    return false;
            }

            return true;
        }

        public static bool SaveFileToCloud(string file)
        {
            if (!IsCloudAvailable(true))
                return false;

            string path = GetAddonItemPath(file);
            if (!File.Exists(path))
                return false;

            FileInfo fileInfo = new FileInfo(path);
            long fileSize = fileInfo.Length;

            try
            {
                byte[] data = File.ReadAllBytes(path);
                if (!SteamRemoteStorage.FileWrite(file, data, ((int)fileSize)))
                    return false;
            }
            catch (Exception ex)
            {
                Utils.LogAction(string.Format("Unable to save file {0} to the cloud!\nError: {1}", file, ex.Message));
                return false;
            }

            return true;
        }

        public static bool ReadFileFromCloud(string file)
        {
            if (!IsCloudAvailable())
                return false;

            if (!SteamRemoteStorage.FileExists(file))
                return false;

            try
            {
                int fileSize = SteamRemoteStorage.GetFileSize(file);
                byte[] data = new byte[fileSize];
                int sizeRead = SteamRemoteStorage.FileRead(file, data, fileSize);
                if (sizeRead != fileSize)
                    return false;

                File.WriteAllBytes(GetAddonItemPath(file), data);
            }
            catch (Exception ex)
            {
                Utils.LogAction(string.Format("Unable to read file {0} from the cloud!\nError: {1}", file, ex.Message));
                return false;
            }

            return true;
        }
    }
}
