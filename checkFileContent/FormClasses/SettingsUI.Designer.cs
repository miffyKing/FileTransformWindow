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
            this.label2 = new System.Windows.Forms.Label();
            this.logDeleteSave = new System.Windows.Forms.Button();
            this.oldFileUpDown = new System.Windows.Forms.NumericUpDown();
            this.currentFolderSizeLabel = new System.Windows.Forms.Label();
            this.currentExpireDateLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.folderSizeUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oldFileUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // folderSizeUpDown
            // 
            this.folderSizeUpDown.BackColor = System.Drawing.Color.White;
            this.folderSizeUpDown.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.folderSizeUpDown.Location = new System.Drawing.Point(193, 124);
            this.folderSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.folderSizeUpDown.Name = "folderSizeUpDown";
            this.folderSizeUpDown.Size = new System.Drawing.Size(120, 25);
            this.folderSizeUpDown.TabIndex = 0;
            this.folderSizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.folderSizeUpDown.ValueChanged += new System.EventHandler(this.folderSizeUpDown_ValueChanged);
            // 
            // folderSizeSave
            // 
            this.folderSizeSave.BackColor = System.Drawing.Color.LightSkyBlue;
            this.folderSizeSave.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.folderSizeSave.Location = new System.Drawing.Point(334, 124);
            this.folderSizeSave.Name = "folderSizeSave";
            this.folderSizeSave.Size = new System.Drawing.Size(75, 28);
            this.folderSizeSave.TabIndex = 1;
            this.folderSizeSave.Text = "저장";
            this.folderSizeSave.UseVisualStyleBackColor = false;
            this.folderSizeSave.Click += new System.EventHandler(this.folderSizeSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.label1.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(45, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "폴더 저장 공간";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.label2.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(45, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "오래된 로그 삭제";
            // 
            // logDeleteSave
            // 
            this.logDeleteSave.BackColor = System.Drawing.Color.LightSkyBlue;
            this.logDeleteSave.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.logDeleteSave.Location = new System.Drawing.Point(334, 212);
            this.logDeleteSave.Name = "logDeleteSave";
            this.logDeleteSave.Size = new System.Drawing.Size(75, 28);
            this.logDeleteSave.TabIndex = 4;
            this.logDeleteSave.Text = "저장";
            this.logDeleteSave.UseVisualStyleBackColor = false;
            this.logDeleteSave.Click += new System.EventHandler(this.logDeleteSave_Click);
            // 
            // oldFileUpDown
            // 
            this.oldFileUpDown.BackColor = System.Drawing.Color.White;
            this.oldFileUpDown.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.oldFileUpDown.Location = new System.Drawing.Point(193, 212);
            this.oldFileUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.oldFileUpDown.Name = "oldFileUpDown";
            this.oldFileUpDown.Size = new System.Drawing.Size(120, 25);
            this.oldFileUpDown.TabIndex = 3;
            this.oldFileUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.oldFileUpDown.ValueChanged += new System.EventHandler(this.oldFileUpDown_ValueChanged);
            // 
            // currentFolderSizeLabel
            // 
            this.currentFolderSizeLabel.AutoSize = true;
            this.currentFolderSizeLabel.BackColor = System.Drawing.Color.Azure;
            this.currentFolderSizeLabel.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.currentFolderSizeLabel.Location = new System.Drawing.Point(211, 74);
            this.currentFolderSizeLabel.Name = "currentFolderSizeLabel";
            this.currentFolderSizeLabel.Size = new System.Drawing.Size(58, 18);
            this.currentFolderSizeLabel.TabIndex = 6;
            this.currentFolderSizeLabel.Text = "label3";
            this.currentFolderSizeLabel.Click += new System.EventHandler(this.label3_Click);
            // 
            // currentExpireDateLabel
            // 
            this.currentExpireDateLabel.AutoSize = true;
            this.currentExpireDateLabel.BackColor = System.Drawing.Color.Azure;
            this.currentExpireDateLabel.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.currentExpireDateLabel.Location = new System.Drawing.Point(211, 172);
            this.currentExpireDateLabel.Name = "currentExpireDateLabel";
            this.currentExpireDateLabel.Size = new System.Drawing.Size(58, 18);
            this.currentExpireDateLabel.TabIndex = 7;
            this.currentExpireDateLabel.Text = "label3";
            this.currentExpireDateLabel.Click += new System.EventHandler(this.currentExpireDateLabel_Click);
            // 
            // SettingsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.currentExpireDateLabel);
            this.Controls.Add(this.currentFolderSizeLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.logDeleteSave);
            this.Controls.Add(this.oldFileUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.folderSizeSave);
            this.Controls.Add(this.folderSizeUpDown);
            this.Name = "SettingsUI";
            this.Text = "SettingsUI";
            this.Load += new System.EventHandler(this.SettingsUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.folderSizeUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oldFileUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown folderSizeUpDown;
        private System.Windows.Forms.Button folderSizeSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button logDeleteSave;
        private System.Windows.Forms.NumericUpDown oldFileUpDown;
        private System.Windows.Forms.Label currentFolderSizeLabel;
        private System.Windows.Forms.Label currentExpireDateLabel;
    }
}