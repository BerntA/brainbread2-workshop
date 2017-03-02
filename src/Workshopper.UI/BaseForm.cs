//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Form base class
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Workshopper.Controls;
using Workshopper.Core;
using Workshopper.Filesystem;

namespace Workshopper.UI
{
    public partial class BaseForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        protected DynamicLayoutLoader _layoutLoader;

        public BaseForm()
        {
            InitializeComponent();

            ImageButton exitButton = new ImageButton("exit", "exit_hover");
            exitButton.Parent = this;
            exitButton.Click += new EventHandler(OnClickExit);
            exitButton.Name = "CloseButton";
            timFadeIn.Enabled = true;
        }

        virtual public DynamicLayoutLoader GetLayoutLoader() { return _layoutLoader; }
        virtual public void LoadLayout(string file)
        {
            _layoutLoader = DynamicLayoutLoader.LoadLayoutForControl(file, this);
        }

        virtual protected void OnFormActive()
        {
        }

        virtual protected void OnFormExit()
        {
            Dispose();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return;
            }

            base.OnMouseDown(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Opacity > 0)
            {
                e.Cancel = true;
                timFadeOut.Enabled = true;
                return;
            }

            OnFormExit();
            base.OnFormClosing(e);
        }

        private void OnClickExit(object sender, EventArgs e)
        {
            timFadeOut.Enabled = true;
        }

        private void timFadeOut_Tick(object sender, EventArgs e)
        {
            Opacity -= .05;
            if (Opacity <= .05)
            {
                Opacity = 0;
                timFadeOut.Enabled = false;
                Close();
            }
        }

        private void timFadeIn_Tick(object sender, EventArgs e)
        {
            Opacity += .05;
            if (Opacity >= .95)
            {
                Opacity = 1;
                timFadeIn.Enabled = false;
                OnFormActive();
            }
        }
    }
}
