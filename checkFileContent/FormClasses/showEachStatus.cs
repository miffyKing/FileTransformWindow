using System;
using System.Collections.Concurrent;
using System.IO;
using System.Windows.Forms;

namespace checkFileContent
{
    public partial class ShowEachStatus : Form
    {
        private ConcurrentQueue<FailureInfo> failureQueue;
        private ConcurrentQueue<SuccessInfo> successQueue;
        private int threadIndex;
        public ShowEachStatus(ConcurrentQueue<FailureInfo> failures, ConcurrentQueue<SuccessInfo> successes, int threadIdx)
        {
            InitializeComponent();
            failureQueue = failures;
            successQueue = successes;
            threadIndex = threadIdx;

            InitializeDataGridView();
            PopulateDataGridViewFailure();
            PopulateDataGridViewSuccess();
        }
        private void InitializeDataGridView()
        {
            FailureDataGridView1.Columns.Clear();
            FailureDataGridView1.Columns.Add("FileName", "File Name");
            FailureDataGridView1.Columns.Add("ThreadIndex", "Thread Index");
            FailureDataGridView1.Columns.Add("Reason", "Reason");

            SuccessDataGridView1.Columns.Clear();
            SuccessDataGridView1.Columns.Add("FileName", "File Name");
            SuccessDataGridView1.Columns.Add("ThreadIndex", "Thread Index");

            FailureDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            SuccessDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void PopulateDataGridViewFailure()
        {
            foreach (var failure in failureQueue)
            {
                if (failure.ThreadIndex == threadIndex)
                {
                    string ActualFileName = Path.GetFileName(failure.FileName);
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (failure.ThreadIndex == threadIndex)
                            {
                                FailureDataGridView1.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                            }
                        });
                    }
                    else
                    {
                        if (failure.ThreadIndex == threadIndex)
                        {
                            FailureDataGridView1.Rows.Add(ActualFileName, failure.ThreadIndex, failure.Reason);
                        }
                    }
                }
                
            }
        }

        private void PopulateDataGridViewSuccess()
        {
            foreach (var success in successQueue)
            {
                if (success.ThreadIndex == threadIndex)
                {
                    string ActualFileName = Path.GetFileName(success.FileName);
                    if (this.InvokeRequired)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (success.ThreadIndex == threadIndex)
                            {
                                SuccessDataGridView1.Rows.Add(ActualFileName, success.ThreadIndex);
                            }
                        });
                    }
                    else
                    {
                        if (success.ThreadIndex == threadIndex)
                        {
                            SuccessDataGridView1.Rows.Add(ActualFileName, success.ThreadIndex);
                        }
                    }
                }

            }
        }

        private void showEachStatus_Load(object sender, EventArgs e)
        {

        }
    }
}
