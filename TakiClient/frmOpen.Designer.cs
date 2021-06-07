namespace TakiClient
{
    partial class FrmOpen
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
            this.InstructionBtn = new System.Windows.Forms.Button();
            this.playBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InstructionBtn
            // 
            this.InstructionBtn.BackColor = System.Drawing.Color.Transparent;
            this.InstructionBtn.FlatAppearance.BorderSize = 0;
            this.InstructionBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.InstructionBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.InstructionBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InstructionBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructionBtn.Location = new System.Drawing.Point(61, 381);
            this.InstructionBtn.Name = "InstructionBtn";
            this.InstructionBtn.Size = new System.Drawing.Size(246, 78);
            this.InstructionBtn.TabIndex = 2;
            this.InstructionBtn.UseVisualStyleBackColor = false;
            this.InstructionBtn.Click += new System.EventHandler(this.InstructionBtn_Click);
            // 
            // playBtn
            // 
            this.playBtn.BackColor = System.Drawing.Color.Transparent;
            this.playBtn.FlatAppearance.BorderSize = 0;
            this.playBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.playBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.playBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playBtn.Location = new System.Drawing.Point(61, 266);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(246, 83);
            this.playBtn.TabIndex = 3;
            this.playBtn.UseVisualStyleBackColor = false;
            this.playBtn.Click += new System.EventHandler(this.playBtn_Click);
            // 
            // FrmOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TakiClient.Properties.Resources.IMG_3435;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(914, 535);
            this.Controls.Add(this.playBtn);
            this.Controls.Add(this.InstructionBtn);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "FrmOpen";
            this.Text = "frmOpen";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button InstructionBtn;
        private System.Windows.Forms.Button playBtn;
    }
}