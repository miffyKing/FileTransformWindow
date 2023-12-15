using System;
using System.Collections.Concurrent;
using System.IO;
using System.Windows.Forms;

namespace checkFileContent
{

    public partial class ShowErrorList : Form
    {

        private ConcurrentQueue<FailureInfo> failureQueue;
        private string newErrorPath;
        public ShowErrorList(ConcurrentQueue<FailureInfo> failures, string newPath)
        {
            InitializeComponent();
            failureQueue = failures;
            newErrorPath = newPath;

            InitializeDataGridView();
            PopulateDataGridView(); 
            PopulateDataGridView2(); 
            PopulateDataGridView3();
        }

        private void InitializeDataGridView()
        {
            failureDataGridView1.Columns.Clear();
            failureDataGridView1.Columns.Add("FileName", "File Name");
            failureDataGridView1.Columns.Add("ThreadIndex", "Thread Index");
            failureDataGridView1.Columns.Add("Reason", "Reason");
            failureDataGridView2.Columns.Clear();
            failureDataGridView2.Columns.Add("FileName", "File Name");
            failureDataGridView2.Columns.Add("ThreadIndex", "Thread Index");
            failureDataGridView2.Columns.Add("Reason", "Reason");
            failureDataGridView3.Columns.Clear();
            failureDataGridView3.Columns.Add("FileName", "File Name");
            failureDataGridView3.Columns.Add("ThreadIndex", "Thread Index");
            failureDataGridView3.Columns.Add("Reason", "Reason");
            failureDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            failureDataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            failureDataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void PopulateDataGridView()
        {
            foreach (var failure in failureQueue)
            {
                string ActualFileName = Path.GetFileName(failure.FileName);
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (failure.ThreadIndex == 0)
                        {
                            failureDataGridView1.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                        }
                    });
                }
                else
                {
                    if (failure.ThreadIndex == 0)
                    {
                        failureDataGridView1.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                    }
                }
            }
        }
        private void PopulateDataGridView2()
        {
            foreach (var failure in failureQueue)
            {
                string ActualFileName = Path.GetFileName(failure.FileName);
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (failure.ThreadIndex == 1)
                        {
                            failureDataGridView2.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                        }
                    });
                }
                else
                {
                    if (failure.ThreadIndex == 1)
                    {
                        failureDataGridView2.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                    }
                }
            }
        }

        private void PopulateDataGridView3()
        {
            foreach (var failure in failureQueue)
            {
                string ActualFileName = Path.GetFileName(failure.FileName);
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (failure.ThreadIndex == 2)
                        {
                            failureDataGridView3.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                        }
                    });
                }
                else
                {
                    if (failure.ThreadIndex == 2)
                    {
                        failureDataGridView3.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                    }
                }
            }
        }


        private void showErrorList_Load(object sender, EventArgs e)
        {
        }


        private void errorLogCheck_Click(object sender, EventArgs e)
        {
            try
            {
                string folderPath = newErrorPath;
                if (Directory.Exists(folderPath))
                {
                    System.Diagnostics.Process.Start(folderPath);
                }
                else
                {
                    MessageBox.Show("Error log folder does not exist.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening the folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
