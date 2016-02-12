//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: Notification Pop-Up form
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using workshopper.gui;

namespace workshopper.core
{
    public partial class NotificationForm : BaseForm
    {
        private string _textNotification;
        public NotificationForm(string text)
        {
            _textNotification = text;
            Text = text;
        }

        protected override void OnFormCreate(float percentW, float percentH)
        {
            InitializeComponent();
            Opacity = 0;

            base.OnFormCreate(0.13F, 1F);
        }

        protected override void OnFormExit()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            base.OnFormExit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawString(_textNotification, Font, Brushes.White, new Rectangle(0, 7, Width, Height));
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(42, 38, 40)), new Rectangle(0, 0, Width - 1, Height - 1));
        }
    }
}
