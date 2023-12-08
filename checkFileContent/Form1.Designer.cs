namespace checkFileContent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.errorLogCheck = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.firstThreadStatus = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.DirectUpload = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorLogCheck
            // 
            this.errorLogCheck.BackColor = System.Drawing.Color.LightSkyBlue;
            this.errorLogCheck.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.errorLogCheck.FlatAppearance.BorderSize = 5;
            this.errorLogCheck.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.errorLogCheck.Location = new System.Drawing.Point(23, 191);
            this.errorLogCheck.Name = "errorLogCheck";
            this.errorLogCheck.Size = new System.Drawing.Size(104, 63);
            this.errorLogCheck.TabIndex = 1;
            this.errorLogCheck.Text = "오류 로그 확인";
            this.errorLogCheck.UseVisualStyleBackColor = false;
            this.errorLogCheck.Click += new System.EventHandler(this.errorLogCheck_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.BackColor = System.Drawing.Color.LightSkyBlue;
            this.labelStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelStatus.Font = new System.Drawing.Font("현대하모니 L", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStatus.Location = new System.Drawing.Point(374, 32);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(174, 32);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "상태 표시 창";
            this.labelStatus.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // firstThreadStatus
            // 
            this.firstThreadStatus.BackColor = System.Drawing.Color.LightSkyBlue;
            this.firstThreadStatus.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.firstThreadStatus.Location = new System.Drawing.Point(755, 130);
            this.firstThreadStatus.Name = "firstThreadStatus";
            this.firstThreadStatus.Size = new System.Drawing.Size(148, 53);
            this.firstThreadStatus.TabIndex = 6;
            this.firstThreadStatus.Text = "1번 스레드 현황";
            this.firstThreadStatus.UseVisualStyleBackColor = false;
            this.firstThreadStatus.Click += new System.EventHandler(this.firstThreadButton_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.LightSkyBlue;
            this.button1.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button1.Location = new System.Drawing.Point(755, 241);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 53);
            this.button1.TabIndex = 7;
            this.button1.Text = "2번 스레드 현황";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.secondThreadButton_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.LightSkyBlue;
            this.button2.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.button2.Location = new System.Drawing.Point(755, 346);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(148, 53);
            this.button2.TabIndex = 8;
            this.button2.Text = "3번 스레드 현황";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.thirdThreadButton_Click);
            // 
            // DirectUpload
            // 
            this.DirectUpload.BackColor = System.Drawing.Color.LightSkyBlue;
            this.DirectUpload.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("DirectUpload.BackgroundImage")));
            this.DirectUpload.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.DirectUpload.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DirectUpload.Location = new System.Drawing.Point(23, 301);
            this.DirectUpload.Name = "DirectUpload";
            this.DirectUpload.Size = new System.Drawing.Size(104, 72);
            this.DirectUpload.TabIndex = 9;
            this.DirectUpload.Text = "파일 직접 업로드";
            this.DirectUpload.UseVisualStyleBackColor = false;
            this.DirectUpload.Click += new System.EventHandler(this.DirectUpload_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel3.Location = new System.Drawing.Point(173, 32);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(3, 420);
            this.panel3.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel2.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(3, 420);
            this.panel2.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel1.Location = new System.Drawing.Point(733, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(3, 420);
            this.panel1.TabIndex = 12;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.DarkSlateGray;
            this.panel4.Font = new System.Drawing.Font("현대하모니 L", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(3, 420);
            this.panel4.TabIndex = 11;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(32, 496);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(144, 48);
            this.button3.TabIndex = 13;
            this.button3.Text = "원본 파일 저장 경로 변경";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(32, 572);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(144, 48);
            this.button4.TabIndex = 14;
            this.button4.Text = "변환 파일 저장 경로 변경";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(32, 647);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(144, 48);
            this.button5.TabIndex = 15;
            this.button5.Text = "파일 입력 경로 변경";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(32, 722);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(144, 48);
            this.button6.TabIndex = 16;
            this.button6.Text = "로그 파일 저장 경로 변경";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 793);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.DirectUpload);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.firstThreadStatus);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.errorLogCheck);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button errorLogCheck;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button firstThreadStatus;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button DirectUpload;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}

