
namespace VirtualPet.YoutubePlugin
{
    partial class YoutubeVideoItem
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
            this.previewPb = new System.Windows.Forms.PictureBox();
            this.nameLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.previewPb)).BeginInit();
            this.SuspendLayout();
            // 
            // previewPb
            // 
            this.previewPb.Location = new System.Drawing.Point(5, 5);
            this.previewPb.Name = "previewPb";
            this.previewPb.Size = new System.Drawing.Size(50, 50);
            this.previewPb.TabIndex = 0;
            this.previewPb.TabStop = false;
            // 
            // nameLbl
            // 
            this.nameLbl.AutoSize = true;
            this.nameLbl.Location = new System.Drawing.Point(61, 5);
            this.nameLbl.Name = "nameLbl";
            this.nameLbl.Size = new System.Drawing.Size(0, 15);
            this.nameLbl.TabIndex = 1;
            // 
            // YoutubeVideoItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.nameLbl);
            this.Controls.Add(this.previewPb);
            this.Name = "YoutubeVideoItem";
            this.Size = new System.Drawing.Size(250, 60);
            ((System.ComponentModel.ISupportInitialize)(this.previewPb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox previewPb;
        private System.Windows.Forms.Label nameLbl;
    }
}
