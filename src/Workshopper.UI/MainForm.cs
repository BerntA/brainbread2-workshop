//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
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
using Workshopper.Controls;
using Workshopper.Core;
using Steamworks;
using Workshopper.Filesystem;

namespace Workshopper.UI
{
    public partial class MainForm : BaseForm
    {
        public AddonList _addonList;
        public MainForm()
        {
            InitializeComponent();
            Opacity = 0;

            LoadLayout("main");

            TextButton btnCreate = new TextButton(Localization.GetTextForToken("MAIN_CREATE"), GetLayoutLoader().GetResItemBgColor("ButtonColors"), GetLayoutLoader().GetResItemFgColor("ButtonColors"));
            btnCreate.Parent = this;
            btnCreate.Click += new EventHandler(OnClickCreate);
            btnCreate.Name = "CreateButton";

            TextButton btnHelp = new TextButton(Localization.GetTextForToken("MAIN_HELP"), GetLayoutLoader().GetResItemBgColor("ButtonColors"), GetLayoutLoader().GetResItemFgColor("ButtonColors"));
            btnHelp.Parent = this;
            btnHelp.Click += new EventHandler(OnClickHelp);
            btnHelp.Name = "HelpButton";

            TextButton btnRefresh = new TextButton(Localization.GetTextForToken("MAIN_REFRESH"), GetLayoutLoader().GetResItemBgColor("ButtonColors"), GetLayoutLoader().GetResItemFgColor("ButtonColors"));
            btnRefresh.Parent = this;
            btnRefresh.Click += new EventHandler(OnClickRefresh);
            btnRefresh.Name = "RefreshButton";

            _addonList = new AddonList();
            _addonList.Parent = this;
            _addonList.Name = "AddonList";

            LoadLayout("main"); // reload.
        }

        private void OnClickRefresh(object sender, EventArgs e)
        {
            if (_addonList.IsUploadingAddon())
                return;

            UGCHandler.RetrievePublishedItems();
        }

        private void OnClickHelp(object sender, EventArgs e)
        {
            Process.Start("http://steamcommunity.com/sharedfiles/filedetails/?id=381191681");
        }

        private void OnClickCreate(object sender, EventArgs e)
        {
            if (_addonList.IsUploadingAddon())
                return;

            CreationPanel panel = new CreationPanel();
            panel.ShowDialog(this);
            panel = null;
        }

        protected override void OnFormExit()
        {
            _addonList.SaveAddonItemDataToCloud();

            base.OnFormExit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(new SolidBrush(GetLayoutLoader().GetResItemFgColor("NavBar")), GetLayoutLoader().GetResItemBounds("NavBar"));
            e.Graphics.DrawRectangle(Pens.DimGray, GetLayoutLoader().GetResItemBounds("Frame"));
        }
    }
}
