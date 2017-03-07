//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
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
using Workshopper.UI;

namespace Workshopper.Core
{
    public partial class NotificationForm : BaseForm
    {
        private string _textNotification;
        public NotificationForm(string text)
        {
            InitializeComponent();
            Opacity = 0;
            _textNotification = text;
            Text = text;
            LoadLayout("warningdialog");

            SizeF textSize = TextRenderer.MeasureText(_textNotification, Font);
            if (textSize.Width > Width)
                Width = (int)textSize.Width + 1;

            exitButton.Location = new Point(Width - exitButton.Size.Width, 0);
        }

        protected override void OnFormExit()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            base.OnFormExit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle exitBtnBounds = exitButton.Bounds;
            Rectangle textBounds = GetLayoutLoader().GetResItemBounds("WarningText");

            e.Graphics.DrawString(_textNotification, Font, Brushes.White, new Rectangle(textBounds.X, textBounds.Y, Width - exitBtnBounds.Width, textBounds.Height));
            e.Graphics.DrawRectangle(new Pen(GetLayoutLoader().GetResItemFgColor("Frame")), new Rectangle(0, 0, Width - exitBtnBounds.Width - 1, Height - 1));
        }
    }
}
