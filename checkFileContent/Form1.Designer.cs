﻿namespace checkFileContent
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.errorLogCheck = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // errorLogCheck
            // 
            this.errorLogCheck.Location = new System.Drawing.Point(12, 251);
            this.errorLogCheck.Name = "errorLogCheck";
            this.errorLogCheck.Size = new System.Drawing.Size(104, 63);
            this.errorLogCheck.TabIndex = 1;
            this.errorLogCheck.Text = "오류 로그 확인";
            this.errorLogCheck.UseVisualStyleBackColor = true;
            this.errorLogCheck.Click += new System.EventHandler(this.errorLogCheck_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(476, 35);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(54, 18);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "label1";
            this.labelStatus.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 550);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.errorLogCheck);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button errorLogCheck;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label labelStatus;
    }
}

