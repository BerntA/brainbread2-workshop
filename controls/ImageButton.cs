//=========       Copyright © Reperio Studios 2013-2016 @ Bernt Andreas Eide!       ============//
//
// Purpose: Customized image based button
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
    public partial class ImageButton : UserControl
    {
        private bool m_bRollover;
        private string pszTextureDef;
        private string pszTextureOver;
        public ImageButton(string texture, string textureRollover)
        {
            m_bRollover = false;
            pszTextureDef = texture;
            pszTextureOver = textureRollover;

            InitializeComponent();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            m_bRollover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            m_bRollover = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Image renderImage = m_bRollover ? globals.GetTexture(pszTextureOver) : globals.GetTexture(pszTextureDef);
            e.Graphics.DrawImage(renderImage, 0, 0, Width, Height);
        }
    }
}
