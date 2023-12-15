using checkFileContent.NonFormClasses;
using System;
using System.IO;
using System.Windows.Forms;

namespace checkFileContent
{
    public partial class SettingsUI : Form
    {

        private SettingsManager settingsManager = new SettingsManager();

        public delegate void ApplySettings(long newSize);
        public event ApplySettings OnApplySettings;

        private string logPath;
        private string errorPath;

        private int expireDate = 7;

        public SettingsUI(string log, string error)
        {
            this.logPath = log;
            this.errorPath = error;
            this.expireDate = settingsManager.Settings.ExpireDate;
           
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
            currentFolderSizeLabel.Text = $"{(settingsManager.Settings.UserInputSize / 1024)} GB";
            currentExpireDateLabel.Text = $"{settingsManager.Settings.ExpireDate} 일";
        }

        private void folderSizeSave_Click(object sender, EventArgs e)
        {
            long newSize = (long)folderSizeUpDown.Value * 1024;

            settingsManager.Settings.UserInputSize = (int)newSize;
            settingsManager.SaveSettings();
   
            OnApplySettings?.Invoke(newSize);
            currentFolderSizeLabel.Text = $"{(settingsManager.Settings.UserInputSize / 1024)} GB";

            MessageBox.Show("저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void logDeleteSave_Click(object sender, EventArgs e)
        {
            int newExpireDate = (int)oldFileUpDown.Value;

            settingsManager.Settings.ExpireDate = newExpireDate;
            settingsManager.SaveSettings();
            currentExpireDateLabel.Text = $"{settingsManager.Settings.ExpireDate} 일";
            DeleteOldLogs();
            MessageBox.Show("저장되었습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
