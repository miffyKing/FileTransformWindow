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
            this.failureDataGridView = new System.Windows.Forms.DataGridView();
            this.errorLogCheck = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // failureDataGridView
            // 
            this.failureDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.failureDataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.failureDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.failureDataGridView.Location = new System.Drawing.Point(76, 78);
            this.failureDataGridView.Name = "failureDataGridView";
            this.failureDataGridView.RowHeadersWidth = 62;
            this.failureDataGridView.RowTemplate.Height = 30;
            this.failureDataGridView.Size = new System.Drawing.Size(570, 347);
            this.failureDataGridView.TabIndex = 0;
            this.failureDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // errorLogCheck
            // 
            this.errorLogCheck.Location = new System.Drawing.Point(76, 490);
            this.errorLogCheck.Name = "errorLogCheck";
            this.errorLogCheck.Size = new System.Drawing.Size(151, 83);
            this.errorLogCheck.TabIndex = 1;
            this.errorLogCheck.Text = "button1";
            this.errorLogCheck.UseVisualStyleBackColor = true;
            this.errorLogCheck.Click += new System.EventHandler(this.errorLogCheck_Click);
            // 
            // showErrorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 623);
            this.Controls.Add(this.errorLogCheck);
            this.Controls.Add(this.failureDataGridView);
            this.Name = "showErrorList";
            this.Text = "showErrorList";
            this.Load += new System.EventHandler(this.showErrorList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.failureDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView failureDataGridView;
        private System.Windows.Forms.Button errorLogCheck;
    }
}