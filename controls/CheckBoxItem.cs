//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
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
using workshopper.core;

namespace workshopper.controls
{
    public partial class CheckBoxItem : UserControl
    {
        enum CheckBoxStates
        {
            STATE_DEF = 0,
            STATE_HOVER = 1,
            STATE_ACTIVATED = 2,
        }

        public bool IsItemChecked() { return (m_iItemState >= 2); }
        public void ActiviateItem(bool value)
        {
            if (value)
                m_iItemState = (int)CheckBoxStates.STATE_ACTIVATED;
            else
                m_iItemState = (int)CheckBoxStates.STATE_DEF;

            Invalidate();
        }
        public string GetText() { return pszText; }

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

            if (m_iItemState == (int)CheckBoxStates.STATE_ACTIVATED)
                e.Graphics.DrawImage(globals.GetTexture("CBox_Check"), new Rectangle(0, 0, Height, Height));
            else
                e.Graphics.DrawImage(globals.GetTexture("CBox_UnCheck"), new Rectangle(0, 0, Height, Height));

            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Near;

            e.Graphics.DrawString(pszText, Font, (m_iItemState == 0 ? Brushes.White : Brushes.DarkRed), new Rectangle(Height, 0, Width - Height, Height), format);
        }
    }
}
