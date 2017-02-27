//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: UGC Creation/Updating Form
//
//=============================================================================================//

using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Workshopper.Controls;
using Workshopper.Core;
using Workshopper.Filesystem;

namespace Workshopper.UI
{
    public partial class CreationPanel : BaseForm
    {
        private TextBox m_pTitle;
        private RichTextBox m_pDescription;
        private RichTextBox m_pPatchNotes;
        private TextBox[] m_pLabelFields;
        private CheckBoxItem[] m_pCheckBox;
        private RadioButton[] m_pVisibilityChoices;
        private ComboBox m_pContestTags;
        private ListButton m_pItemCategoryButton;
        private ItemList m_pItemCategoryList;
        private List<string> pszTagList;
        private string pszImagePath;
        private string pszContentPath;
        private PublishedFileId_t itemUniqueID;
        private OpenFileDialog _fileDialog;
        private FolderBrowserDialog _folderDialog;
        private bool m_bShouldUpdateItem;
        private bool m_bHasChangedImagePath;
        private Image m_pImgPreview;

        public CreationPanel()
        {
            InitializeComponent();
            Opacity = 0;

            pszTagList = new List<string>();
            pszImagePath = null;
            pszContentPath = null;
            m_bShouldUpdateItem = false;
            m_bHasChangedImagePath = false;

            _fileDialog = new OpenFileDialog();
            _fileDialog.DefaultExt = ".jpg";
            _fileDialog.CheckFileExists = true;
            _fileDialog.CheckPathExists = true;
            _fileDialog.Title = "Select an image";
            _fileDialog.AddExtension = true;
            _fileDialog.Multiselect = false;
            _fileDialog.Filter = "JPG files|*.jpg";
            _fileDialog.FileOk += new CancelEventHandler(OnSelectImage);
            _fileDialog.InitialDirectory = Utils.GetGameDirectory((AppId_t)346330);

            _folderDialog = new FolderBrowserDialog();
            _folderDialog.SelectedPath = Utils.GetGameDirectory((AppId_t)346330);

            TextButton btnImg = new TextButton("Select Image:", Color.White, Color.Red);
            btnImg.Parent = this;
            btnImg.Click += new EventHandler(OnOpenImageSelection);
            btnImg.Name = "ImagePathButton";

            TextButton btnFile = new TextButton("Select File Dir:", Color.White, Color.Red);
            btnFile.Parent = this;
            btnFile.Click += new EventHandler(OnOpenFolderDialog);
            btnFile.Name = "FilePathButton";

            m_pLabelFields = new TextBox[2];
            for (int i = 0; i < 2; i++)
            {
                m_pLabelFields[i] = new TextBox();
                m_pLabelFields[i].Parent = this;
                m_pLabelFields[i].ForeColor = Color.White;
                m_pLabelFields[i].BackColor = Color.FromArgb(45, 45, 45);
                m_pLabelFields[i].AutoSize = false;
                m_pLabelFields[i].ReadOnly = true;
                m_pLabelFields[i].BorderStyle = BorderStyle.None;
                m_pLabelFields[i].Multiline = false;
                m_pLabelFields[i].TextAlign = HorizontalAlignment.Left;
                m_pLabelFields[i].Font = new Font("Arial", 10, FontStyle.Regular);
            }

            m_pLabelFields[0].Name = "ImagePathLabel";
            m_pLabelFields[1].Name = "FilePathLabel";

            m_pCheckBox = new CheckBoxItem[Utils.GetMaxTags()];
            for (int i = 0; i < Utils.GetMaxTags(); i++)
            {
                m_pCheckBox[i] = new CheckBoxItem("");
                m_pCheckBox[i].Parent = this;
                m_pCheckBox[i].Click += new EventHandler(OnTagClicked);
                m_pCheckBox[i].Name = string.Format("CheckBox{0}", (i + 1));
                m_pCheckBox[i].Visible = false;
            }

            m_pContestTags = new ComboBox();
            m_pContestTags.Parent = this;
            m_pContestTags.DropDownStyle = ComboBoxStyle.DropDownList;
            m_pContestTags.Cursor = Cursors.Hand;
            m_pContestTags.Items.Insert(0, "None");
            Utils.AddContestItems(m_pContestTags);
            m_pContestTags.SelectedIndex = 0;
            m_pContestTags.Name = "ContestTags";

            m_pTitle = new TextBox();
            m_pTitle.Parent = this;
            m_pTitle.Multiline = false;
            m_pTitle.Font = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            m_pTitle.ForeColor = Color.White;
            m_pTitle.BackColor = BackColor;
            m_pTitle.BorderStyle = BorderStyle.FixedSingle;
            m_pTitle.Name = "TitleField";

            m_pDescription = new RichTextBox();
            m_pDescription.Parent = this;
            m_pDescription.ForeColor = Color.White;
            m_pDescription.BackColor = BackColor;
            m_pDescription.BorderStyle = BorderStyle.FixedSingle;
            m_pDescription.Font = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            m_pDescription.Name = "DescriptionField";

            m_pPatchNotes = new RichTextBox();
            m_pPatchNotes.Parent = this;
            m_pPatchNotes.ForeColor = Color.White;
            m_pPatchNotes.BackColor = BackColor;
            m_pPatchNotes.BorderStyle = BorderStyle.FixedSingle;
            m_pPatchNotes.Font = new System.Drawing.Font("Arial", 8, FontStyle.Regular);
            m_pPatchNotes.Visible = m_pPatchNotes.Enabled = false;
            m_pPatchNotes.Name = "PatchNotes";

            ImageButton btnUpload = new ImageButton("upload", "upload_hover");
            btnUpload.Parent = this;
            btnUpload.Click += new EventHandler(OnUploadAddon);
            btnUpload.Name = "CreateButton";

            m_pVisibilityChoices = new RadioButton[3];
            for (int i = 0; i < 3; i++)
            {
                m_pVisibilityChoices[i] = new RadioButton();
                m_pVisibilityChoices[i].Parent = this;
                m_pVisibilityChoices[i].Font = new Font("Arial", 9, FontStyle.Regular);
                m_pVisibilityChoices[i].ForeColor = Color.White;
                m_pVisibilityChoices[i].BackColor = Color.Transparent;
                m_pVisibilityChoices[i].AutoSize = false;
                m_pVisibilityChoices[i].Cursor = Cursors.Hand;
                m_pVisibilityChoices[i].Name = string.Format("VisibilityButton{0}", (i + 1));
            }

            m_pVisibilityChoices[0].Text = "Public";
            m_pVisibilityChoices[1].Text = "Private";
            m_pVisibilityChoices[2].Text = "Hidden";
            m_pVisibilityChoices[0].Select();

            m_pItemCategoryButton = new ListButton();
            m_pItemCategoryButton.Parent = this;
            m_pItemCategoryButton.Name = "CategoryButton";
            m_pItemCategoryButton.LabelTxt = Localization.GetTextForToken("CREATION_WORKSHOP_CATEORY");
            m_pItemCategoryButton.Click += new EventHandler(OnCategoryButtonClick);

            m_pItemCategoryList = new ItemList();
            m_pItemCategoryList.Parent = this;
            m_pItemCategoryList.Name = "CategoryList";
            m_pItemCategoryList.bUseFixedWidth = false;
            m_pItemCategoryList.Visible = false;
            m_pItemCategoryList.OnItemClick += new EventHandler(OnCategoryListClick);
            Utils.AddCategoriesToItemList(m_pItemCategoryList);

            LoadLayout("creationmenu");

            UGCHandler.OnCreateWorkshopItem += new UGCHandler.ItemCreatedHandler(OnCreateItem);
        }

        public CreationPanel(string title, string description, string tags, int visibility, PublishedFileId_t fileID)
            : this()
        {
            m_bShouldUpdateItem = true;
            m_bHasChangedImagePath = false;
            Text = "Update Addon";

            // Set the stuff:
            pszContentPath = null;
            SetImagePreview(string.Format("{0}\\workshopper\\addons\\{1}.jpg", Globals.GetTexturePath(), fileID.ToString()));
            m_pTitle.Text = title;
            m_pDescription.Text = description;
            itemUniqueID = fileID;

            if (visibility == 0)
                m_pVisibilityChoices[0].Select();
            else if (visibility == 1)
                m_pVisibilityChoices[1].Select();
            else if (visibility == 2)
                m_pVisibilityChoices[2].Select();

            GetTagsFromString(tags);

            int tagCategory = Utils.GetCategoryIndexForTag(pszTagList[0]); // Figure out which category this item used from the first tag itself, assuming that all tags have diff. text.
            SetupTagList(Utils.GetAvailableTagCategories[tagCategory]);

            for (int i = 0; i < pszTagList.Count(); i++)
            {
                for (int x = 0; x < Utils.GetMaxTags(); x++)
                {
                    if (pszTagList[i].Equals(m_pCheckBox[x].GetText(), StringComparison.CurrentCulture))
                        m_pCheckBox[x].ActiviateItem(true);
                }
            }

            m_pPatchNotes.Visible = m_pPatchNotes.Enabled = true;

            Invalidate();
        }

        private void SubmitWorkshopItem(PublishedFileId_t fileID, string changelog)
        {
            int iVisibility = 0;
            if (m_pVisibilityChoices[1].Checked)
                iVisibility = 1;
            else if (m_pVisibilityChoices[2].Checked)
                iVisibility = 2;

            try
            {
                File.Copy(pszImagePath, string.Format("{0}\\workshopper\\addons\\{1}.jpg", Globals.GetTexturePath(), fileID.m_PublishedFileId.ToString()), true);
            }
            catch (Exception ex)
            {
                Utils.LogAction(ex.Message);
            }
            finally
            {
                UGCHandler.SubmitItem(fileID, m_pTitle.Text, m_pDescription.Text, iVisibility, pszContentPath, pszImagePath, pszTagList, changelog, m_bShouldUpdateItem);
                Close();
            }
        }

        private void OnCreateItem(object sender, UGCCreationEventArg e)
        {
            SubmitWorkshopItem(e.FileID, "Initial Release");
        }

        private void OnTagClicked(object sender, EventArgs e)
        {
            CheckBoxItem pItem = ((CheckBoxItem)sender);
            if (pItem != null)
            {
                if (pItem.IsItemChecked())
                    AddTag(pItem.GetText());
                else
                    RemoveTag(pItem.GetText());
            }
        }

        private void OnCategoryButtonClick(object sender, EventArgs e)
        {
            m_pItemCategoryList.Visible = !m_pItemCategoryList.Visible;
            if (m_pItemCategoryList.Visible)
                m_pItemCategoryList.BringToFront();
        }

        private void OnCategoryListClick(object sender, EventArgs e)
        {
            string szItem = ((Label)sender).Text;
            m_pItemCategoryButton.LabelTxt = szItem;

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is ItemList)
                    Controls[i].Visible = false;
            }

            SetupTagList(szItem);
        }

        private void ResetTagCheckboxes()
        {
            for (int i = 0; i < Utils.GetMaxTags(); i++)
            {
                m_pCheckBox[i].SetText("");
                m_pCheckBox[i].ActiviateItem(false);
                m_pCheckBox[i].Visible = false;
            }
        }

        private void SetupTagList(string category)
        {
            ResetTagCheckboxes();

            int iCategory = Utils.GetCategoryIndexForCatergoryName(category);
            for (int i = 0; i < Utils.GetAvailableTags[iCategory].Count(); i++)
            {
                m_pCheckBox[i].Visible = true;
                m_pCheckBox[i].SetText(Utils.GetAvailableTags[iCategory][i]);
            }
        }

        private bool AddTag(string tag)
        {
            for (int i = 0; i < pszTagList.Count(); i++)
            {
                if (pszTagList[i].Equals(tag, StringComparison.CurrentCulture))
                    return false;
            }

            pszTagList.Add(tag);
            return true;
        }

        private bool RemoveTag(string tag)
        {
            for (int i = (pszTagList.Count() - 1); i >= 0; i--)
            {
                if (pszTagList[i].Equals(tag, StringComparison.CurrentCulture))
                {
                    pszTagList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void GetTagsFromString(string tags)
        {
            string tagsCopy = tags;
            while (tagsCopy.Contains(','))
            {
                int index = tagsCopy.IndexOf(',', 0);
                string tagStripped = tagsCopy.Substring(0, index);
                tagsCopy = tagsCopy.Replace(tagStripped + ",", "");
                pszTagList.Add(tagStripped);
            }
        }

        private bool IsStringValid(string inputStr)
        {
            if (string.IsNullOrEmpty(inputStr) || string.IsNullOrWhiteSpace(inputStr))
                return false;

            Regex r = new Regex(@"^[^~`^<>""'\\/]+$");
            return r.IsMatch(inputStr);
        }

        private void OnUploadAddon(object sender, EventArgs e)
        {
            string imagePreview = (m_bHasChangedImagePath ? pszImagePath : null);

            if (string.IsNullOrEmpty(m_pTitle.Text) || string.IsNullOrEmpty(m_pDescription.Text))
            {
                Utils.ShowWarningDialog("Please fill out the fields first!", null, true);
                return;
            }

            if (pszTagList.Count() <= 0)
            {
                Utils.ShowWarningDialog("You have to select at least one tag!", null, true);
                return;
            }

            if (string.IsNullOrEmpty(imagePreview) && !m_bShouldUpdateItem)
            {
                Utils.ShowWarningDialog("You have to select an image!", null, true);
                return;
            }

            if (string.IsNullOrEmpty(pszContentPath) && !m_bShouldUpdateItem)
            {
                Utils.ShowWarningDialog("You have to select the content directory!", null, true);
                return;
            }

            if (!IsStringValid(m_pTitle.Text) ||
                (m_bShouldUpdateItem && !IsStringValid(m_pPatchNotes.Text) && !string.IsNullOrEmpty(m_pPatchNotes.Text)))
            {
                Utils.ShowWarningDialog("Invalid characters detected!", null, true);
                return;
            }

            if (m_pDescription.Text.Length > 1000)
            {
                Utils.ShowWarningDialog("The description is more than 1000 characters long!", null, true);
                return;
            }

            if (!string.IsNullOrEmpty(pszContentPath))
            {
                ulong fileSize = Utils.GetSizeOfContent(pszContentPath);
                if (fileSize <= 0)
                {
                    Utils.ShowWarningDialog("No files selected!", null, true);
                    return;
                }

                if (fileSize > 104857600)
                {
                    Utils.ShowWarningDialog("Content size is too big, 100MB is max!", null, true);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(imagePreview))
            {
                ulong fileSize = Utils.GetSizeOfFile(pszImagePath);
                if (fileSize <= 0)
                {
                    Utils.ShowWarningDialog("No image selected!", null, true);
                    return;
                }

                if (fileSize > 1048576)
                {
                    Utils.ShowWarningDialog("Image size is too big, 1MB is max!", null, true);
                    return;
                }
            }

            if (m_bShouldUpdateItem)
                SubmitWorkshopItem(itemUniqueID, m_pPatchNotes.Text);
            else
                UGCHandler.CreateItem();
        }

        private void OnOpenFolderDialog(object sender, EventArgs e)
        {
            DialogResult result = _folderDialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
                pszContentPath = _folderDialog.SelectedPath;
        }

        private void OnOpenImageSelection(object sender, EventArgs e)
        {
            DialogResult result = _fileDialog.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
                m_bHasChangedImagePath = true;
        }

        private void OnSelectImage(object sender, CancelEventArgs e)
        {
            SetImagePreview(_fileDialog.FileName);
            Invalidate();
        }

        private void SetImagePreview(string path)
        {
            pszImagePath = path;
            if (pszImagePath != null && File.Exists(pszImagePath))
                m_pImgPreview = Image.FromFile(pszImagePath);
        }

        protected override void OnFormExit()
        {
            if ((m_pImgPreview != null) && (m_pImgPreview != Properties.Resources.unknown))
            {
                m_pImgPreview.Dispose();
                m_pImgPreview = Properties.Resources.unknown;
            }

            base.OnFormExit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Image render = Properties.Resources.unknown;
            if (m_pImgPreview != null)
                render = m_pImgPreview;

            e.Graphics.FillRectangle(new SolidBrush(GetLayoutLoader().GetResItemBgColor("Header")), GetLayoutLoader().GetResItemBounds("Header"));
            e.Graphics.DrawImage(render, GetLayoutLoader().GetResItemBounds("ImagePreview"));
            e.Graphics.DrawRectangle(Pens.Black, GetLayoutLoader().GetResItemBounds("ImagePreviewFrame"));

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Near;

            e.Graphics.DrawRectangle(Pens.Black, GetLayoutLoader().GetResItemBounds("TagsFrame"));

            e.Graphics.DrawString("Contest:", Font, Brushes.White, GetLayoutLoader().GetResItemBounds("ContestLabel"));
            e.Graphics.DrawString(Text, Font, Brushes.White, GetLayoutLoader().GetResItemBounds("HeaderText"));
            e.Graphics.DrawString("Title:", Font, Brushes.White, GetLayoutLoader().GetResItemBounds("TitleLabel"));
            e.Graphics.DrawString("Description:", Font, Brushes.White, GetLayoutLoader().GetResItemBounds("DescriptionLabel"));

            if (m_bShouldUpdateItem)
                e.Graphics.DrawString("Changelog:", Font, Brushes.White, GetLayoutLoader().GetResItemBounds("PatchLogNoteLabel"));

            string imagePreview = (m_bHasChangedImagePath ? pszImagePath : null);
            m_pLabelFields[0].Text = imagePreview;
            m_pLabelFields[1].Text = pszContentPath;
        }
    }
}
