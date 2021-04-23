namespace VirtualPet
{
    partial class PetForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PetForm));
            this.petPanel = new System.Windows.Forms.Panel();
            this.animTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // petPanel
            // 
            this.petPanel.BackColor = System.Drawing.Color.DodgerBlue;
            this.petPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("petPanel.BackgroundImage")));
            this.petPanel.Location = new System.Drawing.Point(0, 0);
            this.petPanel.Name = "petPanel";
            this.petPanel.Size = new System.Drawing.Size(40, 40);
            this.petPanel.TabIndex = 0;
            this.petPanel.DoubleClick += new System.EventHandler(this.panel1_DoubleClick);
            this.petPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseClick);
            this.petPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.petPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            this.petPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseUp);
            // 
            // animTimer
            // 
            this.animTimer.Interval = 16;
            this.animTimer.Tick += new System.EventHandler(this.animTimer_Tick);
            // 
            // PetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Magenta;
            this.ClientSize = new System.Drawing.Size(1045, 496);
            this.Controls.Add(this.petPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PetForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PetForm";
            this.TransparencyKey = System.Drawing.Color.Magenta;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel petPanel;
        private System.Windows.Forms.Timer animTimer;
    }
}