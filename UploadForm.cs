using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using workshopper.core;
using workshopper.gui;

namespace workshopper
{
    public partial class UploadForm : BaseForm
    {
        private ulong bytesProcessed;
        private ulong bytesTotal;
        private UGCUpdateHandle_t _uploadHandle;
        public UploadForm(UGCUpdateHandle_t uploadHandle, ulong sizeMax)
        {
            _uploadHandle = uploadHandle;
            bytesProcessed = 0;
            bytesTotal = sizeMax;
        }

        protected override void OnFormCreate(float percentW, float percentH)
        {
            InitializeComponent();
            Opacity = 0;

            base.OnFormCreate(0.07F, 0.4F);
        }

        protected override void OnFormRunFrame()
        {
            base.OnFormRunFrame();

            if (bytesProcessed >= bytesTotal)
                return;

            if (_uploadHandle == null)
                return;

            ulong punBytesProcessed = 0, punBytesTotal = 0;
            if (SteamUGC.GetItemUpdateProgress(_uploadHandle, out punBytesProcessed, out punBytesTotal) == EItemUpdateStatus.k_EItemUpdateStatusUploadingContent) ;
            {
                if (punBytesProcessed != 0 && (punBytesProcessed > bytesProcessed))
                {
                    bytesProcessed = punBytesProcessed;
                    Invalidate();
                }

                BringToFront();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawImage(globals.GetTexture("bar"), new Rectangle(4, 20, Width - 8, 16));

            double flPercent = ((double)bytesProcessed) / ((double)bytesTotal);
            double flWide = (double)(Width - 8) * flPercent;
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(145, 200, 15, 15)), new Rectangle(4, 20, (int)flWide, 16));
            e.Graphics.DrawRectangle(Pens.Black, new Rectangle(4, 20, Width - 8, 16));

            e.Graphics.DrawString(string.Format("Uploaded {0} / {1} bytes!", bytesProcessed, bytesTotal), Font, Brushes.White, new Rectangle(0, 0, Width, 18));
        }
    }
}
