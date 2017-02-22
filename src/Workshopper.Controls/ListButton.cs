//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: List Button - Linked to the ItemList.
//
//=============================================================================================//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Workshopper.Core;

namespace Workshopper.Controls
{
    public partial class ListButton : UserControl
    {
        [Browsable(true)]
        [Description("Label Text"), Category("Appearance")]
        public string LabelTxt
        {
            get { return szText; }
            set { szText = value; Invalidate(); }
        }

        private string szText;
        private static Image m_pListImage = Globals.GetTexture("List");
        public ListButton()
        {
            InitializeComponent();

            this.SetStyle(
System.Windows.Forms.ControlStyles.UserPaint |
System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
true);

            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle newBounds = new Rectangle(0, 0, Width, Height);
            e.Graphics.DrawImage(m_pListImage, newBounds);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;

            Font eFont = new System.Drawing.Font("Calibri", 10, FontStyle.Regular);
            e.Graphics.DrawString(LabelTxt, eFont, new SolidBrush(Color.White), newBounds, stringFormat);
        }
    }
}
