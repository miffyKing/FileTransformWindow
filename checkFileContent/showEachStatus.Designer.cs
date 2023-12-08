namespace checkFileContent
{
    partial class showEachStatus
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
            this.FailureDataGridView1 = new System.Windows.Forms.DataGridView();
            this.SuccessDataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FailureDataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SuccessDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // FailureDataGridView1
            // 
            this.FailureDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.FailureDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.FailureDataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.FailureDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FailureDataGridView1.Location = new System.Drawing.Point(110, 400);
            this.FailureDataGridView1.Name = "FailureDataGridView1";
            this.FailureDataGridView1.RowHeadersWidth = 62;
            this.FailureDataGridView1.RowTemplate.Height = 30;
            this.FailureDataGridView1.Size = new System.Drawing.Size(688, 286);
            this.FailureDataGridView1.TabIndex = 1;
            // 
            // SuccessDataGridView1
            // 
            this.SuccessDataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.SuccessDataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.SuccessDataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.SuccessDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SuccessDataGridView1.Location = new System.Drawing.Point(110, 65);
            this.SuccessDataGridView1.Name = "SuccessDataGridView1";
            this.SuccessDataGridView1.RowHeadersWidth = 62;
            this.SuccessDataGridView1.RowTemplate.Height = 30;
            this.SuccessDataGridView1.Size = new System.Drawing.Size(688, 285);
            this.SuccessDataGridView1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.LightCyan;
            this.label1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(107, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "성공 내역";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightCyan;
            this.label2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(107, 364);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "실패 내역";
            // 
            // showEachStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 720);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SuccessDataGridView1);
            this.Controls.Add(this.FailureDataGridView1);
            this.Name = "showEachStatus";
            this.Text = "showEachStatus";
            this.Load += new System.EventHandler(this.showEachStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FailureDataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SuccessDataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView FailureDataGridView1;
        private System.Windows.Forms.DataGridView SuccessDataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}