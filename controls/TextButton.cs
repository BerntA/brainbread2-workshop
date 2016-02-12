//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: Text based button, the text color blends & animates into the desired hover color
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
    public partial class TextButton : UserControl
    {
        private string pszText;
        private Color colDefault;
        private Color colHover;
        private Color colRender;
        public TextButton(string text, Color defColor, Color targetColor)
        {
            pszText = text;
            colRender = colDefault = defColor;
            colHover = targetColor;

            InitializeComponent();
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            timColorFadeOut.Enabled = false;
            timColorFadeIn.Enabled = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            timColorFadeOut.Enabled = true;
            timColorFadeIn.Enabled = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawString(pszText, Font, new SolidBrush(colRender), new Rectangle(0, 0, Width, Height));
        }

        private void timColorFadeIn_Tick(object sender, EventArgs e)
        {
            colRender = globals.GetColorFraction(colRender, colHover, 3);
            if (colRender == colHover)
                timColorFadeIn.Enabled = false;

            Invalidate();
        }

        private void timColorFadeOut_Tick(object sender, EventArgs e)
        {
            colRender = globals.GetColorFraction(colRender, colDefault, 3);
            if (colRender == colDefault)
                timColorFadeOut.Enabled = false;

            Invalidate();
        }
    }
}
