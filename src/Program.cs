//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Main entry point.
//
//=============================================================================================//

using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Workshopper.Core;
using Workshopper.Filesystem;
using Workshopper.UI;

namespace Workshopper
{
    static class Program
    {
        static Timer _timFrame;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DynamicLayoutLoader.LoadLayoutFiles();
            Localization.LoadLocalization("english"); // FAIL safe in case steam api doesn't load. Reload the localization if it loads, will allow multiple languages. 
            if (!SteamHandler.InitSteamAPI())
            {
                SteamHandler.Shutdown();
                Application.Exit();
                return;
            }

            Localization.LoadLocalization(SteamApps.GetCurrentGameLanguage());
            MainForm mainForm = new MainForm();
            SteamHandler.SetMainFormHandle(mainForm);
            UGCHandler.Initialize();

            _timFrame = new Timer();
            _timFrame.Enabled = true;
            _timFrame.Interval = 5;
            _timFrame.Tick += new EventHandler(OnFrame);

            Application.Run(mainForm);

            _timFrame.Enabled = false;
            _timFrame.Dispose();
            _timFrame = null;

            SteamHandler.Shutdown();
        }

        static void OnFrame(object sender, EventArgs e)
        {
            Utils.HandleLogging();
            SteamHandler.RunCallbacks();
        }
    }
}