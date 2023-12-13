namespace checkFileContent
{
    partial class SettingsUI
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
            this.folderSizeUpDown = new System.Windows.Forms.NumericUpDown();
            this.folderSizeSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.folderSizeUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // folderSizeUpDown
            // 
            this.folderSizeUpDown.Location = new System.Drawing.Point(193, 124);
            this.folderSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.folderSizeUpDown.Name = "folderSizeUpDown";
            this.folderSizeUpDown.Size = new System.Drawing.Size(120, 28);
            this.folderSizeUpDown.TabIndex = 0;
            this.folderSizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // folderSizeSave
            // 
            this.folderSizeSave.Location = new System.Drawing.Point(337, 124);
            this.folderSizeSave.Name = "folderSizeSave";
            this.folderSizeSave.Size = new System.Drawing.Size(75, 28);
            this.folderSizeSave.TabIndex = 1;
            this.folderSizeSave.Text = "저장";
            this.folderSizeSave.UseVisualStyleBackColor = true;
            this.folderSizeSave.Click += new System.EventHandler(this.folderSizeSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "폴더 저장 공간";
            // 
            // SettingsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.folderSizeSave);
            this.Controls.Add(this.folderSizeUpDown);
            this.Name = "SettingsUI";
            this.Text = "SettingsUI";
            this.Load += new System.EventHandler(this.SettingsUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.folderSizeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown folderSizeUpDown;
        private System.Windows.Forms.Button folderSizeSave;
        private System.Windows.Forms.Label label1;
    }
}