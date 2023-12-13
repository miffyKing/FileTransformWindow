using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public SettingsUI()
        {
            InitializeComponent();
        }

        private void SettingsUI_Load(object sender, EventArgs e)
        {

        }

        private void folderSizeSave_Click(object sender, EventArgs e)
        {
            long newSize = (long)folderSizeUpDown.Value * 1024; // 선택된 값
            OnApplySettings?.Invoke(newSize); // Form1에 변경 사항 적용 요청
            this.Close();
        }
    }
}
