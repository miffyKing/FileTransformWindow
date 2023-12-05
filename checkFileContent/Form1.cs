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

        //실패 파일 저장 구조체
        public class FailureInfo
        {
            public string FileName { get; set; }
            public int ThreadIndex { get; set; }
            public string Reason { get; set; }

            public FailureInfo(string fileName, int threadIndex, string reason)
            {
                FileName = fileName;
                ThreadIndex = threadIndex;
                Reason = reason;
            }
        }


        private FileSystemWatcher watcher;
        private ConcurrentQueue<string> fileList = new ConcurrentQueue<string>();
        private Thread[] conversionThreads = new Thread[3];


        private Label[] threadLabels = new Label[3];
        private Label[] fileCountLabels = new Label[3];


        private bool isRunning = true; // 스레드 실행 제어를 위한 플래그
        private FileProcessCount[] fileCounts = new FileProcessCount[3];

        //실패한 파일 모을 배열 -> 이걸 어떻게 표로 나타낼 수 있다면 UI 완성임.
        private ConcurrentQueue<FailureInfo> failedFiles = new ConcurrentQueue<FailureInfo>();


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
            watcher.Filter = "*.*";
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            fileList.Enqueue(e.FullPath);
        }

        private void InitializeThreadsAndLabels()
        {
            for (int i = 0; i < 3; i++)
            {
                threadLabels[i] = new Label();
                threadLabels[i].Location = new System.Drawing.Point(100, 50 + (i * 20));
                threadLabels[i].Size = new System.Drawing.Size(200, 15);
                this.Controls.Add(threadLabels[i]);

                fileCountLabels[i] = new Label();
                fileCountLabels[i].Location = new System.Drawing.Point(350, 50 + (i * 20));
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
                    UpdateThreadLabel(threadIndex, $"Processing {Path.GetFileName(filePath)}");
                    UpdateFileCountLabel(threadIndex);

                    string originalPath = "..\\DATAS\\original\\";
                    filePath = checkDupFileName(filePath, originalPath);
                    Thread.Sleep(5000);
                    Console.WriteLine($"Thread {threadIndex} is deleting file: {Path.GetFileName(filePath)}");
                    Thread.Sleep(1000);
                    try
                    {
                        string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(filePath));

                        //originalpath 에 같은 파일ㅇ명 있으면 _ 추가하는 로직 작성
                        
                        Console.Write("Changed file name is    || " + filePath);
                        // 파일명 바뀌었음을 로그파일에 적고싶은데 어떻게 해야할까 - 그냥 flag 달자.

                        string logFilePath = Path.Combine("..\\DATAS\\log\\", "log_" + Path.GetFileName(filePath) + ".txt");
                        WriteLog(logFilePath, "Thread index :" + threadIndex + " Starts transrform");
                                                
                        if (checkTransformFunction(filePath, threadIndex) == true)
                        {
                            fileCounts[threadIndex].SuccessCount++;
                            UpdateFileCountLabel(threadIndex);  // 이거 변환프로세스 따라 값 바꿔야함
                            transformFile(filePath, threadIndex);
                        }
                        else
                        {
                            string errorMessage = "File failed to transform, move to Error log folder";
                            WriteLog(logFilePath, errorMessage);
                            string errorLogPath = Path.Combine("..\\DATAS\\log\\errorLog\\", "ERROR_" + Path.GetFileName(logFilePath));
                            
                            File.Move(logFilePath, errorLogPath);
                        }

                        Console.Write("before move file to originla folder" + filePath);
                        File.Move(filePath, originalFilePath);
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
                    Thread.Sleep(1000);
                }
            }
        }

        private string checkDupFileName(string filePath, string originalDir)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);
            string newFilePath = filePath;
            int count = 2;

            while (File.Exists(Path.Combine(originalDir, Path.GetFileName(newFilePath))))
            {
                newFilePath = Path.Combine(Path.GetDirectoryName(filePath), $"{fileNameWithoutExt}({count++}){extension}");
            }

            File.Move(filePath, newFilePath);

            return newFilePath;
        }


        void transformFile(string file, int threadIndex)
        {
            Console.Write("Processing File: " + file + "\n");
            string extension = Path.GetExtension(file);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string transformedFileName = "";
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));
            byte[] fileData = File.ReadAllBytes(file);
            try
            {
                if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                {
                    transformedFileName = Path.Combine("..\\DATAS\\transformed\\", fileNameWithoutExt + ".atxt");
                    if (!File.Exists(transformedFileName))
                    {
                        Console.Write("no duplicate file name in transform area\n");
                        File.WriteAllText(transformedFileName, Encoding.UTF8.GetString(fileData), Encoding.UTF8);
                    }
                }
                else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                {
                    transformedFileName = Path.Combine("..\\DATAS\\transformed\\", fileNameWithoutExt + ".abin");
                    if (!File.Exists(transformedFileName))
                    {
                        Console.Write("no duplicate file name in transform area\n");
                        File.WriteAllBytes(transformedFileName, fileData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in transformFile: " + ex.Message);
                // 오류 발생 시 함수를 종료하여 후속 단계로 진행하지 않음
                return;
            }
        }


        bool checkTransformFunction(string file, int threadIndex)
        {
            // File.Delete(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));
            string[] errorReasons = { "Extension check Failed", "Name check Failed", "Size check Failed", "Header check Failed" };


            if (checkExtension(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file, threadIndex, errorReasons[0]));
                return false;
            }
                
            if (checkFileName(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file, threadIndex, errorReasons[1]));
                return false;
            }
                
            if (checkFileSize(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file, threadIndex, errorReasons[2]));
                return false;
            }
            
            if (checkFileHeader(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file, threadIndex, errorReasons[3]));
                return false;
            }
            return true;
        }

        bool checkExtension(string file, int threadIndex)
        {
            string logFilePath = Path.Combine("..\\DATAS\\log\\", "log_" + Path.GetFileName(file) + ".txt");

            string extension = Path.GetExtension(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));

            if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
            {
                WriteLog(logFilePath, "Extension check passed for .abin");
            }
            else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
            {
                WriteLog(logFilePath, "Extension check passed for .abin");
            }
            else
            {
                fileCounts[threadIndex].FailureCount++;
                WriteLog(logFilePath, "Extension check Failed for filename" + file);
                UpdateFileCountLabel(threadIndex);
                File.Move(file, originalFilePath);
                return false;
            }
            return true;
        }

        bool checkFileName(string file, int threadIndex)
        {
            string logFilePath = Path.Combine("..\\DATAS\\log\\", "log_" + Path.GetFileName(file) + ".txt");
            string fileName = Path.GetFileName(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", fileName);

            if (!fileName.StartsWith("'[TargetFileName]'"))
            {
                fileCounts[threadIndex].FailureCount++;
                Console.WriteLine($"File name error, transformation failed: {fileName}");
                WriteLog(logFilePath, "File name error, transformation failed: " + fileName);
                File.Move(file, originalFilePath); //이거도 나중에 없애고 변환한 후에로 바꿔야함.
                UpdateFileCountLabel(threadIndex);
                return false;
            }
            WriteLog(logFilePath, "File Name check PASSED for " + fileName);

            return true;
        }

        bool checkFileSize(string file, int threadIndex)
        {
            string logFilePath = Path.Combine("..\\DATAS\\log\\", "log_" + Path.GetFileName(file) + ".txt");
            string fileName = Path.GetFileName(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", fileName);

            FileInfo fileInfo = new FileInfo(file);

            //length로 파일 크기 바이트 단위 확인 가능
            if (fileInfo.Length < 18)
            {
                fileCounts[threadIndex].FailureCount++;
                Console.WriteLine($"File Size error, small then 18 byte, transformation failed: {fileName}");
                WriteLog(logFilePath, "File Size error, small then 18 byte, transformation failed" + fileName);
                File.Move(file, originalFilePath); //이거도 나중에 없애고 변환한 후에로 바꿔야함.
                UpdateFileCountLabel(threadIndex);
                return false;
            }

            WriteLog(logFilePath, "File Size check PASSED for " + fileName);
            return true;
        }

        bool checkFileHeader(string file, int threadIndex)
        {
            string logFilePath = Path.Combine("..\\DATAS\\log\\", "log_" + Path.GetFileName(file) + ".txt");
            string extension = Path.GetExtension(file);
            string originalFilePath = Path.Combine("..\\DATAS\\original\\", Path.GetFileName(file));
            const string headerToCheck = "[ATRANS] ";

            try
            {
                byte[] fileData = File.ReadAllBytes(file);

                if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase) || extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                {
                    string fileContent = Encoding.UTF8.GetString(fileData);
                    string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    if (lines.Length > 0 && lines[0].StartsWith(headerToCheck))
                    {
                        WriteLog(logFilePath, "File Header correct, it starts with [ATRANS]  " + Path.GetFileName(file));
                        return true;
                    }
                    else
                    {
                        WriteLog(logFilePath, "File Header error: Should start with [ATRANS] : " + Path.GetFileName(file));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(logFilePath, "Error in checkFileHeader: " + ex.Message);
            }

            // 예외 발생 시 또는 조건 불만족 시 false 반환
            fileCounts[threadIndex].FailureCount++;
            File.Move(file, originalFilePath); //이거도 나중에 없애고 변환한 후에로 바꿔야함.
            UpdateFileCountLabel(threadIndex);
            return false;
        }


        private void WriteLog(string filePath, string message)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(DateTime.Now + ": " + message);
            }
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
                    "..\\DATAS\\log\\",
                    "..\\DATAS\\log\\errorLog"
                };

                foreach (string folder in folders)
                {
                    // 이미 폴더가 존재한다면 삭제
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder, true); // true 폴더 내 모든 파일 및 하위 폴더 삭제
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

        private void errorLogCheck_Click(object sender, EventArgs e)
        {
            try
            {
                // 지정된 경로에 대한 전체 경로를 계산
                string folderPath = Path.GetFullPath("..\\DATAS\\log\\errorLog");

                // 폴더가 실제로 존재하는지 확인
                if (Directory.Exists(folderPath))
                {
                    // 폴더 열기
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
