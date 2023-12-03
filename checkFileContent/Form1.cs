using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace checkFileContent
{

    public partial class Form1 : Form
    {
        public class FileProcessCount
        {
            public int SuccessCount { get; set; }
            public int FailureCount { get; set; }
        }

        private FileSystemWatcher watcher;
        private ConcurrentQueue<string> fileList = new ConcurrentQueue<string>();
        private Thread[] conversionThreads = new Thread[3];


        private Label[] threadLabels = new Label[3];
        private Label[] fileCountLabels = new Label[3];


        private bool isRunning = true; // 스레드 실행 제어를 위한 플래그
        //private int[] fileCounts = new int[3]; // 각 스레드의 파일 처리 개수를 저장할 배열
        private FileProcessCount[] fileCounts = new FileProcessCount[3];



        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                fileCounts[i] = new FileProcessCount { SuccessCount = 0, FailureCount = 0 };
            }
            RunGenerateFolder();

            InitializeFileSystemWatcher();

            InitializeThreadsAndLabels();

            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing); // FormClosing 이벤트 핸들러 추가

        }


        private void InitializeFileSystemWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetFullPath(@"..\\DATAS\\inputRoute");
            watcher.Filter = "*.*"; // Set the file types you want to monitor
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            fileList.Enqueue(e.FullPath);
            ProcessFileList();
        }

        private void InitializeThreadsAndLabels()
        {
            for (int i = 0; i < 3; i++)
            {
                threadLabels[i] = new Label();
                threadLabels[i].Location = new System.Drawing.Point(10, 10 + (i * 20));
                threadLabels[i].Size = new System.Drawing.Size(200, 15);
                this.Controls.Add(threadLabels[i]);

                fileCountLabels[i] = new Label();
                fileCountLabels[i].Location = new System.Drawing.Point(220, 10 + (i * 20));
                fileCountLabels[i].Size = new System.Drawing.Size(200, 15);
                fileCountLabels[i].Text = "Count: 0";

                this.Controls.Add(fileCountLabels[i]);

                fileCounts[i].SuccessCount = 0;
                fileCounts[i].FailureCount = 0;

                int threadIndex = i; // 각 스레드의 고유 인덱스를 생성
                conversionThreads[i] = new Thread(() => ProcessFiles(threadIndex));
                conversionThreads[i].Start();

                UpdateFileCountLabel(threadIndex);

            }
        }


        private void ProcessFiles(int threadIndex)
        {
            while (isRunning)
            {
                if (fileList.TryDequeue(out string filePath))
                {
                    //fileCounts[threadIndex]++;

                    UpdateThreadLabel(threadIndex, $"Processing {Path.GetFileName(filePath)}");
                    UpdateFileCountLabel(threadIndex);
                    // Add file processing logic here, e.g., move file to another folder

                    Thread.Sleep(5000); // Sleep for 5 seconds after
                    Console.WriteLine($"Thread {threadIndex} is deleting file: {Path.GetFileName(filePath)}");
                    Thread.Sleep(1000);
                    // After sleeping, delete the file (if that's your requirement)
                    try
                    {

                        if (transformFunction(filePath, threadIndex) == true)
                        {
                            fileCounts[threadIndex].SuccessCount++;
                            UpdateFileCountLabel(threadIndex);  // 이거 변환프로세스 따라 값 바꿔야함

                            //실제 파일 변환하는 로직 checkExtension 에 주석처리 되어 있는거 여기서 구현해야함. transformFile();
                        }
                        else
                        {
                            fileCounts[threadIndex].FailureCount++;
                            UpdateFileCountLabel(threadIndex);
                        }

                        //fileCounts[threadIndex].SuccessCount++;


                        UpdateThreadLabel(threadIndex, $"Thread {threadIndex} deleted {Path.GetFileName(filePath)}");
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        UpdateThreadLabel(threadIndex, $"Error deleting file: {ex.Message}");
                    }
                    UpdateThreadLabel(threadIndex, $"Waiting for files");
                    // 중단 신호를 다시 확인
                    if (!isRunning)
                        break;
                }
                else
                { // 대기 중에 중단 신호를 확인
                    if (!isRunning)
                        break;
                    Thread.Sleep(1000); // Sleep for a short time before checking the queue again
                }
            }
        }

        bool transformFunction(string file, int threadIndex)
        {
            // File.Delete(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));
            bool isSatisfy = false;

            if (checkExtension(file, threadIndex) == true && checkFileName(file, threadIndex) == true && checkContent(file, threadIndex) == true)
            {
                isSatisfy = true;
            }
                
            /*checkExtension(file, threadIndex);
            checkFileName(file, threadIndex);
            checkContent(file, threadIndex);
*/

            File.Move(file, originalFilePath);
        
            return isSatisfy;
        }

        bool checkExtension(string file, int threadIndex)
        {
            string extension = Path.GetExtension(file);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string transformedFileName = "";
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));

            if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
            {
               /* transformedFileName = Path.Combine("..\\DATAS\\transformed\\", fileNameWithoutExt + ".atxt");
                if (!File.Exists(transformedFileName)) // 파일이 이미 존재하지 않는 경우에만 복사
                {
                    File.Copy(file, transformedFileName);
                    //fileCounts[threadIndex].SuccessCount++;
                }*/
            }
            else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
            {
               /* transformedFileName = Path.Combine("..\\DATAS\\transformed\\", fileNameWithoutExt + ".abin");
                if (!File.Exists(transformedFileName)) // 파일이 이미 존재하지 않는 경우에만 복사
                {
                    File.Copy(file, transformedFileName);
                    //fileCounts[threadIndex].SuccessCount++;
                }*/
            }
            else
            {
                //fileCounts[threadIndex].FailureCount++;
                return false;
            }
            return true;
        }

        bool checkFileName(string file, int threadIndex)
        {
            string fileName = Path.GetFileName(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", fileName);

            if (!fileName.StartsWith("'[TargetFileName]'"))
            {
                //fileCounts[threadIndex].FailureCount++;
                Console.WriteLine($"File name error, transformation failed: {fileName}");

                File.Move(file, originalFilePath); //이거도 나중에 없애고 변환한 후에로 바꿔야함.

                //UpdateFileCountLabel(threadIndex);
                return false;
            }
            return true;
        }


        bool checkContent(string file, int threadIndex)
        {
            string extension = Path.GetExtension(file);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string transformedFileName = "";
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));

            if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
            {
            }
            else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
            {
                
            }
            return true;
        }

        private void UpdateThreadLabel(int threadIndex, string text)
        {
            if (IsHandleCreated && !IsDisposed && !Disposing)
            {
                if (threadLabels[threadIndex].InvokeRequired)
                {
                    threadLabels[threadIndex].Invoke(new Action(() => threadLabels[threadIndex].Text = text));
                }
                else
                {
                    threadLabels[threadIndex].Text = text;
                }
            }
        }

        private void UpdateFileCountLabel(int threadIndex)
        {
            string text = $"Success: {fileCounts[threadIndex].SuccessCount}, Failure: {fileCounts[threadIndex].FailureCount}";
            // 폼의 핸들이 생성되었으며 폼이 Dispose되지 않았는지 확인
            if (IsHandleCreated && !IsDisposed && !Disposing)
            {
                if (fileCountLabels[threadIndex].InvokeRequired)
                {
                    fileCountLabels[threadIndex].Invoke(new Action(() => fileCountLabels[threadIndex].Text = text));
                }
                else
                {
                    fileCountLabels[threadIndex].Text = text;
                }
            }
        }

        private void ProcessFileList()
        {
        }

        private static void RunGenerateFolder()
        {
            try
            {
                string[] folders = new string[]
                {
                    "..\\DATAS\\",
                    "..\\DATAS\\original\\",
                    "..\\DATAS\\transformed\\",
                    "..\\DATAS\\inputRoute\\",
                    "..\\DATAS\\log\\"
                };

                foreach (string folder in folders)
                {
                    // 이미 폴더가 존재한다면 삭제
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true); // true는 폴더 내 모든 파일 및 하위 폴더를 삭제합니다.
                    }
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;

            foreach (Thread thread in conversionThreads)
            {
                if (thread != null && thread.IsAlive)
                {
                    thread.Join(1000);
                }
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
