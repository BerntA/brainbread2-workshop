//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: A list of UGC Addon items
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Steamworks;
using Workshopper.Core;
using Workshopper.UI;
using Workshopper.Filesystem;

namespace Workshopper.Controls
{
    public partial class AddonList : UserControl
    {
        public AddonList()
        {
            InitializeComponent();

            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        public void AddItem(string title, string description, string tags, int visibility, PublishedFileId_t fileID, string lastChangeDate, bool bRefreshLayout = false)
        {
            VerticalScroll.Value = 0;
            int iAddonItems = GetItemNumber();
            AddonItem item = new AddonItem(title, description, tags, visibility, fileID, lastChangeDate);
            item.Bounds = new Rectangle(0, (60 * iAddonItems), (Width - SystemInformation.VerticalScrollBarWidth - 2), 60);
            this.Controls.Add(item);

            if (bRefreshLayout)
                RefreshLayout();
        }

        public AddonItem GetAddonItemForFileID(PublishedFileId_t fileID)
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    AddonItem pItem = ((AddonItem)control);
                    if (pItem.GetItemFileID() == fileID)
                        return pItem;
                }
            }

            return null;
        }

        public void UpdateItem(PublishedFileId_t fileID, string title, string description, string tags, int visibility, string lastChangeDate)
        {
            AddonItem pItem = GetAddonItemForFileID(fileID);
            if (pItem != null)
                pItem.UpdateItem(title, description, tags, visibility, lastChangeDate);
        }

        public void StartUploading(PublishedFileId_t fileID, UGCUpdateHandle_t handle)
        {
            AddonItem pItem = GetAddonItemForFileID(fileID);
            if (pItem != null)
                pItem.StartUploading(handle);
        }

        public void StopUploading()
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                    ((AddonItem)control).StopUploading();
            }
        }

        public bool IsUploadingAddon()
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    AddonItem item = ((AddonItem)control);
                    if (item.IsUploading())
                        return true;
                }
            }

            return false;
        }

        public void RefreshItems()
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    AddonItem item = ((AddonItem)control);
                    item.UpdateItemLayout();
                }
            }

            RefreshLayout();
        }

        public void RemoveItems()
        {
            for (int i = (Controls.Count - 1); i >= 0; i--)
            {
                if (Controls[i] is AddonItem)
                {
                    AddonItem item = ((AddonItem)Controls[i]);
                    item.Cleanup();
                    Controls.Remove(Controls[i]);
                }
            }

            RefreshLayout();
        }

        public void SaveAddonItemDataToCloud()
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    AddonItem item = ((AddonItem)control);
                    SteamCloudHandler.SaveFileToCloud(item.GetItemFileID().ToString());
                }
            }
        }

        public void RefreshLayout()
        {
            Invalidate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private int GetItemNumber()
        {
            int iResult = 0;
            foreach (Control item in Controls)
            {
                if (item is AddonItem)
                    iResult++;
            }

            return iResult;
        }
    }
}
