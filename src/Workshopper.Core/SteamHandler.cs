//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: SteamAPI entry point
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using System.IO;
using Workshopper.UI;
using Workshopper.Filesystem;
using Workshopper.Controls;

namespace Workshopper.Core
{
    public static class SteamHandler
    {
        private static bool m_bHasInitialized = false;
        private static MainForm _mainFormAccessor;

        public static MainForm GetMainForm() { return _mainFormAccessor; }
        public static AddonList GetAddonList() { return _mainFormAccessor._addonList; }
        public static bool HasInitializedSteam() { return m_bHasInitialized; }
        public static void SetMainFormHandle(MainForm handle) { _mainFormAccessor = handle; }

        public static bool IsUsingCorrectAppID()
        {
            string steamAppIDPath = string.Format("{0}\\steam_appid.txt", Globals.GetAppPath());
            bool bResult = false;
            if (!File.Exists(steamAppIDPath))
                return false;

            StreamReader fileStream = null;
            try
            {
                fileStream = new StreamReader(steamAppIDPath);
                while (!fileStream.EndOfStream)
                {
                    string line = fileStream.ReadLine();
                    if (line.StartsWith("382990"))
                        bResult = true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream = null;
                }
            }

            return bResult;
        }

        public static bool InitSteamAPI()
        {
            Directory.CreateDirectory(string.Format("{0}\\assets\\workshopper\\addons", Globals.GetAppPath()));
            Directory.CreateDirectory(string.Format("{0}\\itemdata", Globals.GetAppPath()));
            Utils.Initialize();

            if (!IsUsingCorrectAppID())
            {
                Utils.ShowWarningDialog(Localization.GetTextForToken("APP_INIT_FAIL_1"), null, true);
                return false;
            }

            if (SteamAPI.Init())
                m_bHasInitialized = true;
            else
                Utils.ShowWarningDialog(Localization.GetTextForToken("APP_INIT_FAIL_2"), null, true);

            return m_bHasInitialized;
        }

        public static void RunCallbacks()
        {
            if (!m_bHasInitialized)
                return;

            SteamAPI.RunCallbacks();
        }

        public static void Shutdown()
        {
            if (m_bHasInitialized)
                SteamAPI.Shutdown();

            m_bHasInitialized = false;
        }
    }
}
