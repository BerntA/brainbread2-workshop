//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
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

namespace workshopper.core
{
    public static class factory_list_init
    {
        public static bool m_bHasInitialized = false;
        public static MainForm _mainFormAccessor;
        public static bool IsUsingCorrectAppID()
        {
            string steamAppIDPath = string.Format("{0}\\steam_appid.txt", globals.GetAppPath());
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

        public static bool InitSteamAPI(MainForm parent)
        {
            Directory.CreateDirectory(string.Format("{0}\\assets\\workshopper\\addons", globals.GetAppPath()));

            _mainFormAccessor = parent;
            utils.Initialize();

            if (!IsUsingCorrectAppID())
            {
                utils.ShowWarningDialog("Invalid or non existant SteamAppid.txt!", null, true);
                return false;
            }

            if (SteamAPI.Init())
                m_bHasInitialized = true;
            else
                utils.ShowWarningDialog("Unable to initialize SteamAPI!", null, true);

            return m_bHasInitialized;
        }

        public static void Shutdown()
        {
            if (m_bHasInitialized)
                SteamAPI.Shutdown();

            m_bHasInitialized = false;
        }
    }
}
