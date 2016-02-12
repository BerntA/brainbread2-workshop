//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: Base form, displays your addons and their upload/update progress and other options
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using workshopper.controls;
using workshopper.core;
using workshopper.gui;
using Steamworks;

namespace workshopper
{
    public partial class MainForm : BaseForm
    {
        float flStandardHeight;
        public AddonList _addonList;
        protected override void OnFormCreate(float percentW, float percentH)
        {
            InitializeComponent();
            Opacity = 0;
            flStandardHeight = ((float)Height * 0.075F);

            TextButton btnCreate = new TextButton("Create", Color.FromArgb(255, 255, 255), Color.FromArgb(235, 20, 20));
            btnCreate.Parent = this;
            btnCreate.Bounds = new Rectangle(4, 2, 50, (int)flStandardHeight);
            btnCreate.Click += new EventHandler(OnClickCreate);

            TextButton btnHelp = new TextButton("Help", Color.FromArgb(255, 255, 255), Color.FromArgb(235, 20, 20));
            btnHelp.Parent = this;
            btnHelp.Bounds = new Rectangle(60, 2, 40, (int)flStandardHeight);
            btnHelp.Click += new EventHandler(OnClickHelp);

            TextButton btnRefresh = new TextButton("Refresh", Color.FromArgb(255, 255, 255), Color.FromArgb(235, 20, 20));
            btnRefresh.Parent = this;
            btnRefresh.Bounds = new Rectangle(104, 2, 60, (int)flStandardHeight);
            btnRefresh.Click += new EventHandler(OnClickRefresh);

            _addonList = new AddonList();
            _addonList.Parent = this;
            _addonList.Bounds = new Rectangle(0, (int)flStandardHeight, Width - 1, (Height - (int)flStandardHeight - 2));

            base.OnFormCreate(0.075F, 0.075F);
        }

        private void OnClickRefresh(object sender, EventArgs e)
        {
            UGCHandler.RetrievePublishedItems();
        }

        private void OnClickHelp(object sender, EventArgs e)
        {
            Process.Start("http://steamcommunity.com/sharedfiles/filedetails/?id=381191681");
        }

        private void OnClickCreate(object sender, EventArgs e)
        {
            CreationPanel panel = new CreationPanel();
            panel.ShowDialog(this);
        }

        protected override void OnFormRunFrame()
        {
            base.OnFormRunFrame();

            utils.HandleLogging();

            if (!factory_list_init.m_bHasInitialized)
                return;

            SteamAPI.RunCallbacks();
        }

        protected override void OnFormActive()
        {
            if (factory_list_init.InitSteamAPI(this))
            {
                UGCHandler.Initialize();
            }
            else
            {
                OnFormExit();
                return;
            }

            base.OnFormActive();
        }

        protected override void OnFormExit()
        {
            factory_list_init.Shutdown();
            base.OnFormExit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(12, 12, 10)), new Rectangle(0, 0, Width, (int)flStandardHeight));
            e.Graphics.DrawRectangle(Pens.DimGray, new Rectangle(0, (int)flStandardHeight, Width - 1, (Height - (int)flStandardHeight - 2)));
        }
    }
}
