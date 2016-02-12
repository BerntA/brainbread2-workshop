//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
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
using workshopper.core;

namespace workshopper.controls
{
    public partial class AddonList : UserControl
    {
        public AddonList()
        {
            InitializeComponent();
        }

        public void AddItem(string title, string description, string tags, int visibility, PublishedFileId_t fileID, string lastChangeDate)
        {
            AddonItem item = new AddonItem(title, description, tags, visibility, fileID, lastChangeDate);
            item.Parent = this;
            item.Bounds = new Rectangle(2, 2 + (60 * GetItemNumber()), Width - 2, 60);
            item.SetupItem();
        }

        public void UpdateItem(PublishedFileId_t fileID, string title, string description, string tags, int visibility, string lastChangeDate)
        {
            AddonItem pItem = null;
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    if (((AddonItem)control).GetItemFileID() == fileID)
                    {
                        pItem = ((AddonItem)control);
                        break;
                    }
                }
            }

            if (pItem != null)
                pItem.UpdateItem(title, description, tags, visibility, lastChangeDate);
        }

        public void StartUploading(PublishedFileId_t fileID, UGCUpdateHandle_t handle)
        {
            AddonItem pItem = null;
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    if (((AddonItem)control).GetItemFileID() == fileID)
                    {
                        pItem = ((AddonItem)control);
                        break;
                    }
                }
            }

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

        public void RefreshItems()
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                    control.Invalidate();
            }
        }

        public void ActivateItem(AddonItem item, bool value)
        {
            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    AddonItem controlItem = ((AddonItem)control);
                    if (controlItem.IsActive())
                        controlItem.Activate(false);
                }
            }

            foreach (Control control in Controls)
            {
                if (control is AddonItem)
                {
                    AddonItem controlItem = ((AddonItem)control);
                    if (control == item)
                        controlItem.Activate(value);
                }
            }
        }

        public void RemoveItems()
        {
            for (int i = (Controls.Count - 1); i >= 0; i--)
            {
                if (Controls[i] is AddonItem)
                    Controls.Remove(Controls[i]);
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

            return --iResult;
        }
    }
}
