namespace checkFileContent
{
    partial class showErrorList
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
            this.failureDataGridView1 = new System.Windows.Forms.DataGridView();
            this.errorLogCheck = new System.Windows.Forms.Button();
            this.failureDataGridView2 = new System.Windows.Forms.DataGridView();
            this.failureDataGridView3 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView3)).BeginInit();
            this.SuspendLayout();
            // 
            // failureDataGridView1
            // 
            this.failureDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.failureDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.failureDataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.failureDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.failureDataGridView1.Location = new System.Drawing.Point(12, 102);
            this.failureDataGridView1.Name = "failureDataGridView1";
            this.failureDataGridView1.RowHeadersWidth = 62;
            this.failureDataGridView1.RowTemplate.Height = 30;
            this.failureDataGridView1.Size = new System.Drawing.Size(517, 444);
            this.failureDataGridView1.TabIndex = 0;
            // 
            // errorLogCheck
            // 
            this.errorLogCheck.BackColor = System.Drawing.Color.LightCyan;
            this.errorLogCheck.Font = new System.Drawing.Font("굴림", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.errorLogCheck.Location = new System.Drawing.Point(39, 606);
            this.errorLogCheck.Name = "errorLogCheck";
            this.errorLogCheck.Size = new System.Drawing.Size(174, 83);
            this.errorLogCheck.TabIndex = 1;
            this.errorLogCheck.Text = "로그 폴더 열기";
            this.errorLogCheck.UseVisualStyleBackColor = false;
            this.errorLogCheck.Click += new System.EventHandler(this.errorLogCheck_Click);
            // 
            // failureDataGridView2
            // 
            this.failureDataGridView2.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.failureDataGridView2.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.failureDataGridView2.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.failureDataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.failureDataGridView2.Location = new System.Drawing.Point(576, 102);
            this.failureDataGridView2.Name = "failureDataGridView2";
            this.failureDataGridView2.RowHeadersWidth = 62;
            this.failureDataGridView2.RowTemplate.Height = 30;
            this.failureDataGridView2.Size = new System.Drawing.Size(517, 444);
            this.failureDataGridView2.TabIndex = 2;
            // 
            // failureDataGridView3
            // 
            this.failureDataGridView3.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.failureDataGridView3.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.failureDataGridView3.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.failureDataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.failureDataGridView3.Location = new System.Drawing.Point(1129, 102);
            this.failureDataGridView3.Name = "failureDataGridView3";
            this.failureDataGridView3.RowHeadersWidth = 62;
            this.failureDataGridView3.RowTemplate.Height = 30;
            this.failureDataGridView3.Size = new System.Drawing.Size(517, 444);
            this.failureDataGridView3.TabIndex = 3;
            // 
            // showErrorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1677, 722);
            this.Controls.Add(this.failureDataGridView3);
            this.Controls.Add(this.failureDataGridView2);
            this.Controls.Add(this.errorLogCheck);
            this.Controls.Add(this.failureDataGridView1);
            this.Name = "showErrorList";
            this.Text = "showErrorList";
            this.Load += new System.EventHandler(this.showErrorList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView failureDataGridView1;
        private System.Windows.Forms.Button errorLogCheck;
        private System.Windows.Forms.DataGridView failureDataGridView2;
        private System.Windows.Forms.DataGridView failureDataGridView3;
    }
}