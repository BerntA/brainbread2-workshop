//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: A more appealing checkbox control
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
using Workshopper.Core;
using Workshopper.UI;

namespace Workshopper.Controls
{
    public partial class CheckBoxItem : UserControl
    {
        enum CheckBoxStates
        {
            STATE_DEF = 0,
            STATE_HOVER = 1,
            STATE_ACTIVATED = 2,
        }

        private static Image m_pImgChecked = Globals.GetTexture("CBox_Check");
        private static Image m_pImgUnChecked = Globals.GetTexture("CBox_UnCheck");

        public bool IsItemChecked() { return (m_iItemState >= (int)CheckBoxStates.STATE_ACTIVATED); }
        public void ActiviateItem(bool value)
        {
            if (value)
                m_iItemState = (int)CheckBoxStates.STATE_ACTIVATED;
            else
                m_iItemState = (int)CheckBoxStates.STATE_DEF;

            Invalidate();
        }

        public string GetText() { return pszText; }
        public void SetText(string text) { pszText = text; Invalidate(); }

        private int m_iItemState;
        private string pszText;

        public CheckBoxItem(string text)
        {
            InitializeComponent();
            pszText = text;
            m_iItemState = (int)CheckBoxStates.STATE_DEF;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED)
                return;

            base.OnMouseEnter(e);
            m_iItemState = (int)CheckBoxStates.STATE_HOVER;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED)
                return;

            base.OnMouseLeave(e);
            m_iItemState = (int)CheckBoxStates.STATE_DEF;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            if (m_iItemState != (int)CheckBoxStates.STATE_ACTIVATED)
                m_iItemState = (int)CheckBoxStates.STATE_ACTIVATED;
            else
                m_iItemState = (int)CheckBoxStates.STATE_HOVER;

            Invalidate();

            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(((m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED) ? m_pImgChecked : m_pImgUnChecked), new Rectangle(0, 0, Height, Height));

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Near;

            e.Graphics.DrawString(pszText, Font, (m_iItemState == ((int)CheckBoxStates.STATE_DEF) ? Brushes.White : Brushes.DarkRed), new Rectangle(Height, 0, Width - Height, Height), format);
        }
    }
}
