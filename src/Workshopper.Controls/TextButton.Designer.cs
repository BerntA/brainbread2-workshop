namespace Workshopper.Controls
{
    partial class TextButton
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timColorFadeOut = new System.Windows.Forms.Timer(this.components);
            this.timColorFadeIn = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timColorFadeOut
            // 
            this.timColorFadeOut.Interval = 5;
            this.timColorFadeOut.Tick += new System.EventHandler(this.timColorFadeOut_Tick);
            // 
            // timColorFadeIn
            // 
            this.timColorFadeIn.Interval = 5;
            this.timColorFadeIn.Tick += new System.EventHandler(this.timColorFadeIn_Tick);
            // 
            // TextButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TextButton";
            this.Size = new System.Drawing.Size(100, 32);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timColorFadeOut;
        private System.Windows.Forms.Timer timColorFadeIn;
    }
}
