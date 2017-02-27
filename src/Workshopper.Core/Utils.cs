//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Logging and other handy functions
//
//=============================================================================================//

using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Workshopper.Controls;

namespace Workshopper.Core
{
    public static class Utils
    {
        private static Queue<string> _pszLogQueue = null;
        public static void LogAction(string text)
        {
            if (_pszLogQueue == null)
                return;

            _pszLogQueue.Enqueue(text);
        }

        public static void Initialize()
        {
            _pszLogQueue = new Queue<string>();
        }

        public static DialogResult ShowWarningDialog(string warning, string logMessage = null, bool bOverrideQueue = false)
        {
            NotificationForm notificationForm = new NotificationForm(warning);

            if (bOverrideQueue)
                WriteToLog((string.IsNullOrEmpty(logMessage) ? warning : logMessage));
            else
                LogAction((string.IsNullOrEmpty(logMessage) ? warning : logMessage));

            return notificationForm.ShowDialog(SteamHandler.GetMainForm());
        }

        public static void HandleLogging()
        {
            if (_pszLogQueue == null)
                return;

            if (_pszLogQueue.Count() <= 0)
                return;

            for (int i = (_pszLogQueue.Count() - 1); i >= 0; i--)
                WriteToLog(_pszLogQueue.Dequeue());
        }

        public static void WriteToLog(string text)
        {
            StreamWriter fileWriter = null;
            try
            {
                fileWriter = new StreamWriter(string.Format("{0}\\workshopper_log.txt", Globals.GetAppPath()), true);
                fileWriter.WriteLine(text);
            }
            catch
            {
                // TODO?
            }
            finally
            {
                if (fileWriter != null)
                {
                    fileWriter.Close();
                    fileWriter = null;
                }
            }
        }

        public static int GetMaxTagCategories() { return GetAvailableTagCategories.Count(); }
        public static int GetMaxTags() { return 6; }
        public static string[] GetAvailableTagCategories = { "Gamemode", "Model", "Sound", "UI" };
        public static string[][] GetAvailableTags = 
        {
            // Gamemodes:
            new string[] { "Story Mode", "Objective", "Arena", "Elimination", "Deathmatch", "Custom" },
            // Models
            new string[] { "Weapons", "NPCs", "Survivors", "Props", "Misc", "Textures" },
            // Sounds
              new string[] { "Survivor Voicesets", "NPC Voicesets", "Weapon Sounds", "Misc Sounds" },
            // UI
             new string[] { "HUD", "VGUI", "Textures" },
        };

        public static void AddCategoriesToItemList(ItemList list)
        {
            for (int i = 0; i < GetMaxTagCategories(); i++)
                list.AddItem(GetAvailableTagCategories[i]);
        }

        public static int GetCategoryIndexForTag(string tag)
        {
            for (int i = 0; i < GetMaxTagCategories(); i++)
            {
                int categorySize = GetAvailableTags[i].Count();
                for (int x = 0; x < categorySize; x++)
                {
                    if (GetAvailableTags[i][x].Equals(tag, StringComparison.CurrentCulture))
                        return i;
                }
            }

            return 0;
        }

        public static int GetCategoryIndexForCatergoryName(string category)
        {
            for (int i = 0; i < GetMaxTagCategories(); i++)
            {
                if (GetAvailableTagCategories[i].Equals(category, StringComparison.CurrentCulture))
                    return i;
            }

            return 0;
        }

        public static string[] GetContestTags = { "Christmas", "Halloween" };
        public static void AddContestItems(ComboBox box)
        {
            // Check what time of the year it is:
            DateTime timeNow = DateTime.Now;

            // Christmas:
            if (timeNow.Month == 12 && timeNow.Day >= 20 && timeNow.Day <= 27)
                box.Items.Insert(box.Items.Count, "Christmas");

            // Halloween
            if (timeNow.Month == 10 && timeNow.Day >= 24 && timeNow.Day <= 27)
                box.Items.Insert(box.Items.Count, "Halloween");
        }

        public static ulong GetSizeOfContent(string directory)
        {
            if (!Directory.Exists(directory))
                return 0;

            ulong fileSize = 0;
            foreach (string file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                FileInfo pInfo = new FileInfo(file);
                fileSize += (ulong)pInfo.Length;
            }

            return fileSize;
        }

        public static string GetDateFromTimeCreated(ulong time)
        {
            float newTime = (float)time;

            uint years = 1970, months = 1, days = 1;

            while (newTime >= 31556926.0F)
            {
                newTime -= 31556926.0F;
                years++;
            }

            while (newTime >= 2629743.83F)
            {
                newTime -= 2629743.83F;
                months++;
            }

            while (newTime >= 86400.0F)
            {
                newTime -= 86400.0F;
                days++;
            }

            string day_format, month_format;

            day_format = string.Format("{0}", days);
            if (days < 10)
                day_format = string.Format("0{0}", days);

            month_format = string.Format("{0}", months);
            if (months < 10)
                month_format = string.Format("0{0}", months);

            return string.Format("{0}.{1}.{2}", day_format, month_format, years.ToString());
        }

        public static string GetCurrentDate()
        {
            DateTime dateNow = DateTime.Now;

            try
            {
                uint days = uint.Parse(dateNow.Day.ToString());
                uint months = uint.Parse(dateNow.Month.ToString());
                uint years = uint.Parse(dateNow.Year.ToString());

                string day_format, month_format;

                day_format = string.Format("{0}", days);
                if (days < 10)
                    day_format = string.Format("0{0}", days);

                month_format = string.Format("{0}", months);
                if (months < 10)
                    month_format = string.Format("0{0}", months);

                return string.Format("{0}.{1}.{2}", day_format, month_format, years.ToString());
            }
            catch
            {
                return "N/A";
            }
        }

        public static string GetMapDataScriptTemplate()
        {
            string fileTemplate =
@"""MapData""
{
	""map"" ""%s1""
	""title"" ""%s2""
	""description"" ""%s3""
	""author"" ""%s4""
}
";

            return fileTemplate;
        }

        public static void CreateMapDataScripts(string path, string title, string description)
        {
            string fileTemplate = GetMapDataScriptTemplate();
            foreach (string file in Directory.EnumerateFiles(path, "*.bsp", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string dest = string.Format("{0}\\data\\maps\\{1}.txt", path, fileName);
                string content = fileTemplate.Replace("%s1", fileName).Replace("%s2", title).Replace("%s3", description).Replace("%s4", SteamUser.GetSteamID().m_SteamID.ToString());

                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.WriteAllText(dest, content);
            }
        }

        public static ulong GetSizeOfFile(string path)
        {
            if (!File.Exists(path))
                return 0;

            FileInfo fileData = new FileInfo(path);
            return (ulong)fileData.Length;
        }

        public static string GetGameDirectory(AppId_t appID)
        {
            if (!SteamApps.BIsAppInstalled(appID))
                return "";

            string path = "";
            SteamApps.GetAppInstallDir(appID, out path, 256);

            return path;
        }
    }
}
