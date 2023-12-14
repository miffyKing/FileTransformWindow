using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkFileContent
{
    public partial class SettingsUI : Form
    {
        public delegate void ApplySettings(long newSize);
        public event ApplySettings OnApplySettings;

        private string logPath;
        private string errorPath;

        private int expireDate = 7;

        public SettingsUI(string log, string error)
        {
            this.logPath = log;
            this.errorPath = error;

            this.expireDate = Properties.Settings.Default.ExpireDate;

            InitializeComponent();
        }

        public void DeleteOldLogs()
        {
            DeleteFileByDate(this.logPath, this.expireDate);
            DeleteFileByDate(this.errorPath, this.expireDate);
        }

        private void DeleteFileByDate(string path, int days)
        {
            try
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < DateTime.Now.AddDays(-1 * days))
                    {
                        fileInfo.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting old log files: {ex.Message}");
            }
        }


        private void SettingsUI_Load(object sender, EventArgs e)
        {
            currentFolderSizeLabel.Text = $"{(Properties.Settings.Default.UserInputSize / 1024)} GB";
            currentExpireDateLabel.Text = $"{(Properties.Settings.Default.ExpireDate)} 일";


        }

        private void folderSizeSave_Click(object sender, EventArgs e)
        {
            long newSize = (long)folderSizeUpDown.Value * 1024; // 선택된 값

            Properties.Settings.Default.UserInputSize = newSize;
            Properties.Settings.Default.Save();

            OnApplySettings?.Invoke(newSize); // Form1에 변경 사항 적용 요청

            MessageBox.Show("저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //this.Hide();
        }

        private void oldFileUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        private void folderSizeUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        private void logDeleteSave_Click(object sender, EventArgs e)
        {
            int newExpireDate = (int)oldFileUpDown.Value; // 선택된 값
            //OnApplySettings?.Invoke(newSize); 
            Properties.Settings.Default.ExpireDate = newExpireDate;
            Properties.Settings.Default.Save();
            this.expireDate = newExpireDate;
            DeleteOldLogs();
            MessageBox.Show("저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void currentExpireDateLabel_Click(object sender, EventArgs e)
        {

        }

        /*        protected override void OnFormClosing(FormClosingEventArgs e)
                {
                    base.OnFormClosing(e);

                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true; // 폼 닫힘 취소
                        this.Hide(); // 폼 숨기기
                    }
                }*/
    }
}
