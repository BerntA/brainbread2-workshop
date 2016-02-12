//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
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
using workshopper.controls;
using workshopper.core;

namespace workshopper.gui
{
    public partial class BaseForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

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

        public BaseForm()
        {
            InitializeComponent();
            OnFormCreate(0.075F, 0.075F);
        }

        private void OnClickExit(object sender, EventArgs e)
        {
            timFadeOut.Enabled = true;
        }

        virtual protected void OnFormCreate(float percentW, float percentH)
        {
            float btnSizeW, btnSizeH;
            btnSizeW = ((float)Width * percentW);
            btnSizeH = ((float)Height * percentH);

            ImageButton exitButton = new ImageButton("exit", "exit_hover");
            exitButton.Parent = this;
            exitButton.Bounds = new Rectangle((Width - (int)btnSizeW), 0, (int)btnSizeW, (int)btnSizeH);
            exitButton.Click += new EventHandler(OnClickExit);

            timFadeIn.Enabled = true;
        }

        virtual protected void OnFormActive()
        {
            timFrame.Enabled = true;
        }

        virtual protected void OnFormExit()
        {
            Dispose();
        }

        virtual protected void OnFormRunFrame()
        {

        }

        private void timFadeOut_Tick(object sender, EventArgs e)
        {
            Opacity -= .05;
            if (Opacity <= .05)
            {
                Opacity = 0;
                timFadeOut.Enabled = false;
                timFrame.Enabled = false;
                OnFormExit();
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

        private void BaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            timFadeOut.Enabled = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void timFrame_Tick(object sender, EventArgs e)
        {
            OnFormRunFrame();
        }
    }
}
