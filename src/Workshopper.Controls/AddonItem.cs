//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: UGC Addon item
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
using System.Diagnostics;

namespace Workshopper.Controls
{
    public partial class AddonItem : UserControl
    {
        public PublishedFileId_t GetItemFileID() { return ulFileID; }

        private DynamicLayoutLoader _layout;
        private Color colOverlay;
        private bool m_bWhitelisted;
        private PublishedFileId_t ulFileID;
        private string pszName;
        private string pszDescription;
        private string pszTags;
        private int m_iVisibility;
        private string pszDate;
        private ImageButton btnUpdate;
        private Image m_pImagePreview;
        private CustomProgressBar progressBar;

        // Upload Definitions:
        private bool m_bUploading;
        private UGCUpdateHandle_t updateHandle;

        public AddonItem(string title, string description, string tags, int visibility, PublishedFileId_t fileID, string lastChangeDate)
        {
            m_pImagePreview = Globals.GetTexture(fileID.ToString(), "jpg");
            InitializeComponent();
            progressBar = new CustomProgressBar(fileID);
            progressBar.Parent = this;
            progressBar.Name = "ProgressBar";
            progressBar.Visible = false;
            progressBar.MouseEnter += new EventHandler(OnMouseEnterProgressBar);
            progressBar.MouseLeave += new EventHandler(OnMouseLeaveProgressBar);
            _layout = DynamicLayoutLoader.LoadLayoutForControl("addonitem", this, true);
            pszName = title;
            pszDescription = description;
            pszTags = tags;
            m_iVisibility = visibility;
            ulFileID = fileID;
            pszDate = lastChangeDate;
            colOverlay = _layout.GetResItemBgColor("Overlay");
            m_bUploading = false;
            btnUpdate = null;
            m_bWhitelisted = pszTags.Contains("Whitelisted");
        }

        public void UpdateItem(string title, string description, string tags, int visibility, string lastChangeDate)
        {
            pszName = title;
            pszDescription = description;
            pszTags = tags;
            m_iVisibility = visibility;
            pszDate = lastChangeDate;
            m_bUploading = false;
            m_bWhitelisted = pszTags.Contains("Whitelisted");
            Invalidate();
        }

        public void StartUploading(UGCUpdateHandle_t updHandle)
        {
            updateHandle = updHandle;
            m_bUploading = true;

            if (btnUpdate != null)
                btnUpdate.Visible = btnUpdate.Enabled = false;

            progressBar.ShowProgress(updateHandle);
            Invalidate();
        }

        public void StopUploading()
        {
            m_bUploading = false;

            if (btnUpdate != null)
                btnUpdate.Visible = btnUpdate.Enabled = true;

            progressBar.HideProgress();
            Invalidate();
        }

        public void SetupItem()
        {
            btnUpdate = new ImageButton("update", "update_hover");
            btnUpdate.Parent = this;
            btnUpdate.Bounds = _layout.GetResItemBounds("UpdateButton");
            btnUpdate.Click += new EventHandler(OnClickUpdate);
            btnUpdate.MouseEnter += new EventHandler(OnEnterUpdateButton);
            btnUpdate.MouseLeave += new EventHandler(OnLeaveUpdateButton);

            if (m_bUploading)
                btnUpdate.Visible = false;
        }

        public void UpdateItemLayout()
        {
            m_pImagePreview = Globals.GetTexture(ulFileID.ToString(), "jpg");
            Invalidate();
        }

        public void Cleanup()
        {
            if (m_pImagePreview != null && m_pImagePreview != Properties.Resources.unknown)
            {
                m_pImagePreview.Dispose();
                m_pImagePreview = Properties.Resources.unknown;
            }
        }

        private void OnClickUpdate(object sender, EventArgs e)
        {
            if (m_bUploading)
                return;

            CreationPanel panel = new CreationPanel(pszName, pszDescription, pszTags, m_iVisibility, ulFileID);
            panel.ShowDialog(this);
            panel = null;
        }

        private void OnEnterUpdateButton(object sender, EventArgs e)
        {
            DoRollover(true);
        }

        private void OnLeaveUpdateButton(object sender, EventArgs e)
        {
            DoRollover();
        }

        private void OnMouseEnterProgressBar(object sender, EventArgs e)
        {
            DoRollover(true);
        }

        private void OnMouseLeaveProgressBar(object sender, EventArgs e)
        {
            DoRollover();
        }

        private void DoRollover(bool bOver = false)
        {
            if (!bOver)
                colOverlay = _layout.GetResItemBgColor("Overlay");
            else
                colOverlay = _layout.GetResItemFgColor("Overlay");

            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            DoRollover(true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            DoRollover();
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (!m_bUploading)
                Process.Start(string.Format("http://steamcommunity.com/sharedfiles/filedetails/?id={0}", ulFileID));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(m_pImagePreview, _layout.GetResItemBounds("ImagePreview"));
            e.Graphics.FillRectangle(new SolidBrush(colOverlay), new Rectangle(0, 0, Width, Height));
            e.Graphics.DrawRectangle(Pens.Black, _layout.GetResItemBounds("ImagePreview"));

            base.OnPaint(e);

            if (progressBar.Visible)
                return;

            StringFormat formatter = new StringFormat();
            formatter.LineAlignment = StringAlignment.Center;
            formatter.Alignment = StringAlignment.Near;

            e.Graphics.DrawString(pszName, new Font("Georgia", 14, FontStyle.Bold), new SolidBrush(Color.White), _layout.GetResItemBounds("TitleLabel"), formatter);
            if (m_bWhitelisted)
            {
                e.Graphics.DrawImage(Properties.Resources.verified, _layout.GetResItemBounds("WhitelistedIcon"));
                e.Graphics.DrawString("Your map is whitelisted!", new Font("Times New Roman", 10, FontStyle.Regular), new SolidBrush(Color.Green), _layout.GetResItemBounds("WhitelistLabel"), formatter);
            }

            formatter.LineAlignment = StringAlignment.Center;
            formatter.Alignment = StringAlignment.Far;

            e.Graphics.DrawString(pszDate, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.White), _layout.GetResItemBounds("DateLabel"), formatter);
        }
    }
}
