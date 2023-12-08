using MetroFramework.Forms;     //이쁘게 보이게 하고싶습니다.

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace checkFileContent
{

    public partial class Form1 : MetroForm
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
        
        private FileProcessCount[] fileCounts = new FileProcessCount[3];
        private ConcurrentQueue<FailureInfo> failedFiles = new ConcurrentQueue<FailureInfo>();         //실패한 파일 모을 배열 -> 이걸 어떻게 표로 나타낼 수 있다면 UI 완성임.
        private ConcurrentQueue<SuccessInfo> successedFiles = new ConcurrentQueue<SuccessInfo>();

        private string ORIGINALPATH = "..\\DATAS\\original\\";
        private string TRANSFORMEDPATH = "..\\DATAS\\transformed\\";
        private string INPUTROUTE = "..\\DATAS\\inputRoute\\";
        private static string LOGPATH = "..\\DATAS\\log\\";
        //private string ERRORPATH = "..\\DATAS\\log\\errorLog";
        private static string ERRORPATH = Path.Combine(LOGPATH, "errorLog");

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                fileCounts[i] = new FileProcessCount { SuccessCount = 0, FailureCount = 0 };
            }

            RunGenerateFolder();                //폴더 생성
            InitializeFileSystemWatcher();      //fsw 생성 - 감시 시작
            InitializeThreadsAndLabels();       //UI 표시용 invoke
            DeleteOldLogs();                    //오래된 로그파일 지우기
            UpdateStatus("파일 변환 전");
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void InitializeFileSystemWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetFullPath(@INPUTROUTE);
            watcher.Filter = "*.*";
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            fileList.Enqueue(e.FullPath);
            UpdateStatus("파일 변환 중");
        }

        private void InitializeThreadsAndLabels()
        {
            for (int i = 0; i < 3; i++)
            {
                threadLabels[i] = new Label();
                threadLabels[i].Location = new System.Drawing.Point(150, 100 + (i * 70));
                threadLabels[i].Size = new System.Drawing.Size(200, 15);
                this.Controls.Add(threadLabels[i]);
                threadLabels[i].BringToFront();
                threadLabels[i].Text = "파일 입력 대기";

                fileCountLabels[i] = new Label();
                fileCountLabels[i].Location = new System.Drawing.Point(350, 100 + (i * 70));
                fileCountLabels[i].Size = new System.Drawing.Size(200, 15);
                fileCountLabels[i].Text = "Count: 0";
                fileCountLabels[i].BringToFront();
                this.Controls.Add(fileCountLabels[i]);

                fileCounts[i].SuccessCount = 0;
                fileCounts[i].FailureCount = 0;

                int threadIndex = i;
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
                    UpdateFileCountLabel(threadIndex);
                    string originalPath = ORIGINALPATH;
                    string prevName = filePath;
                    filePath = checkDupFileName(filePath, originalPath);

                    Thread.Sleep(500);
                    UpdateThreadLabel(threadIndex, "변환 시작");
                    Thread.Sleep(1000);

                    try
                    {
                        string originalFilePath = Path.Combine(ORIGINALPATH, Path.GetFileName(filePath));              
                        string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(filePath) + ".txt");
                        WriteLog(logFilePath, "Thread index :" + threadIndex + " Starts transrform");
                        if (prevName != filePath)
                        {
                            WriteLog(logFilePath, "Duplicate name found in original path, File Name changed.");
                        }
                        if (checkTransformFunction(filePath, threadIndex) == true)
                        {
                            fileCounts[threadIndex].SuccessCount++;
                            UpdateFileCountLabel(threadIndex);  // 이거 변환프로세스 따라 값 바꿔야함
                            transformFile(filePath, threadIndex);
                            successedFiles.Enqueue(new SuccessInfo(filePath, threadIndex));
                        }
                        else
                        {
                            string errorMessage = "File failed to transform, move to Error log folder";
                            WriteLog(logFilePath, errorMessage);
                            string errorLogPath = Path.Combine(ERRORPATH + "\\", "ERROR_" + Path.GetFileName(logFilePath));
                            File.Move(logFilePath, errorLogPath);
                            UpdateThreadLabel(threadIndex, "변환 실패");
                            Thread.Sleep(1000);
                        }
                        File.Move(filePath, originalFilePath);
                    }
                    catch (Exception ex)
                    {
                        UpdateThreadLabel(threadIndex, "변환 실패");
                        Thread.Sleep(1000);
                    }
                    UpdateThreadLabel(threadIndex, "변환 종료.");
                    Thread.Sleep(1000);
                    SleepTenSecond(threadIndex);
                    //Thread.Sleep(10000);
                    UpdateThreadLabel(threadIndex, $"변환 파일 입력 대기");
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

        private void SleepTenSecond(int threadIndex)
        {

                for (int i = 1; i <= 9; i++)
                {
                    string lineToUpdate = "휴식 중 ";
                    string whatToAdd = "";
                    for (int j = 0; j < i % 4; j++)
                    {
                        whatToAdd += ".";
                    }

                    UpdateThreadLabel(threadIndex, lineToUpdate + whatToAdd);
                    Thread.Sleep(1000);

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

                // "[TargetFileName] "을 제거한 파일명
                string trimmedFileName = Path.GetFileNameWithoutExtension(file).Replace("[TargetFileName] ", "");
                string transformedFileName = "";
                byte[] fileData = File.ReadAllBytes(file);

                try
                {
                    if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                    {
                        transformedFileName = Path.Combine(TRANSFORMEDPATH, trimmedFileName + ".atxt");
                        if (!File.Exists(transformedFileName))
                        {
                            Console.Write("no duplicate file name in transform area\n");
                            File.WriteAllText(transformedFileName, Encoding.UTF8.GetString(fileData), Encoding.UTF8);
                        }
                    }
                    else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                    {
                        transformedFileName = Path.Combine(TRANSFORMEDPATH, trimmedFileName + ".abin");
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
                    return;
                }

            
        }

        bool checkTransformFunction(string file, int threadIndex)
        {
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
            string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(file) + ".txt");
            string extension = Path.GetExtension(file);
            string originalFilePath = Path.Combine(ORIGINALPATH, Path.GetFileName(file));

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
            string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(file) + ".txt");
            string fileName = Path.GetFileName(file);
            string originalFilePath = Path.Combine(ORIGINALPATH, fileName);

            //여기에서, [TargetFileName] 헤더 정확하게 확인하고, 띄어쓰기는 하나만, 반드시 하나만 있는지 확인하기.
            //또 [TargetFileName] .txt  이런거는 걸러내야하니까 유의하자.
            if (!fileName.StartsWith("[TargetFileName] "))
            {
                fileCounts[threadIndex].FailureCount++;
                WriteLog(logFilePath, "File name error - Invalid Header, transformation failed: " + fileName);
                File.Move(file, originalFilePath); //이거도 나중에 없애고 변환한 후에로 바꿔야함.
                UpdateFileCountLabel(threadIndex);
                return false;
            }

            //로직 추가 - TargetFileName 으로 시작해도, 띄어쓰기가 뒤에 몇개 있는지 확인해야함.
            string trimmedFileName = fileName.Replace("[TargetFileName] ", "");
            //string pattern = @"\(\d+\)\.(atxt|abin)$";  //(2).abin 처리
            if (string.IsNullOrWhiteSpace(trimmedFileName) || trimmedFileName.StartsWith(" ") || trimmedFileName.Equals(".abin") || trimmedFileName.Equals(".atxt"))
            {
                fileCounts[threadIndex].FailureCount++;
                WriteLog(logFilePath, "File name error - valid Header, but other issues: " + fileName);
                File.Move(file, originalFilePath);
                UpdateFileCountLabel(threadIndex);
                return false;
            }

            //나중에 수정 제대로 해야함. 같은 파일이 10개 넘게 들어오면 이 로직을 통과하게 되어요.
            if (trimmedFileName[0] == '(' && trimmedFileName[2] == ')' && trimmedFileName.Length == 8)
            {
                fileCounts[threadIndex].FailureCount++;
                WriteLog(logFilePath, "File name error - valid Header, but other issues: " + fileName);
                File.Move(file, originalFilePath);
                UpdateFileCountLabel(threadIndex);
                return false;
            }

            WriteLog(logFilePath, "File Name check PASSED for " + fileName);
            return true;
        }

        bool checkFileSize(string file, int threadIndex)
        {
            string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(file) + ".txt");
            string fileName = Path.GetFileName(file);
            string originalFilePath = Path.Combine(ORIGINALPATH, fileName);
            FileInfo fileInfo = new FileInfo(file);

            if (fileInfo.Length < 18)
            {
                fileCounts[threadIndex].FailureCount++;
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
            string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(file) + ".txt");
            string extension = Path.GetExtension(file);
            string originalFilePath = Path.Combine(ORIGINALPATH , Path.GetFileName(file));
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
            try
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
            catch (InvalidAsynchronousStateException)
            {
                // 예외 처리 로직, 예를 들어 로깅을 수행하거나 무시할 수 있습니다.
            }
        }

        private void UpdateFileCountLabel(int threadIndex)
        {
            try
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
            catch (InvalidAsynchronousStateException)
            {
                // 예외 처리 로직, 예를 들어 로깅을 수행하거나 무시할 수 있습니다.
                // 예외가 발생했을 때 수행할 작업을 여기에 추가합니다.
            }
        }

        private void UpdateStatus(string status)
        {
            try
            {
                if (labelStatus.InvokeRequired)
                {
                    labelStatus.Invoke(new Action(() => labelStatus.Text = status));
                }
                else
                {
                    labelStatus.Text = status;
                }
            }
            catch (InvalidAsynchronousStateException)
            {
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
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DeleteOldLogs()
        {
            string logPath = LOGPATH;
            string errorLogPath =ERRORPATH;

            DeleteFileByDate(logPath, 1);
            DeleteFileByDate(errorLogPath, 1);
        }

        private void DeleteFileByDate(string path, int days)
        {
            try
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < DateTime.Now.AddDays(-1*days))
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                isRunning = false;

                foreach (Thread thread in conversionThreads)
                {
                    if (thread != null && thread.IsAlive)
                    {
                        thread.Join(1000); // 기다릴 시간을 1000 밀리초(1초)로 설정
                    }
                }
            }
            catch (InvalidAsynchronousStateException ex)
            {
                MessageBox.Show("애플리케이션이 안전하게 종료되지 못했습니다. 잠시 후 다시 시도.\n" + ex.Message, "종료 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            showErrorList newForm2 = new showErrorList(failedFiles);
            newForm2.ShowDialog();
        }
        private void firstThreadButton_Click(object sender, EventArgs e)
        {
            showEachStatus newForm2 = new showEachStatus(failedFiles, successedFiles, 0);
            newForm2.ShowDialog();
        }
        private void secondThreadButton_Click(object sender, EventArgs e)
        {
            showEachStatus newForm2 = new showEachStatus(failedFiles, successedFiles, 1);
            newForm2.ShowDialog();
        }
        private void thirdThreadButton_Click(object sender, EventArgs e)
        {
            showEachStatus newForm2 = new showEachStatus(failedFiles, successedFiles, 2);
            newForm2.ShowDialog();
        }
        private void label1_Click_1(object sender, EventArgs e)
        {
        }
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
        }

        private void DirectUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file to upload";
            openFileDialog.Filter = "All files (*.*)|*.*"; // 필요한 파일 형식에 맞게 필터 조정
            openFileDialog.Multiselect = false; // 단일 파일 선택

            // 파일 탐색기를 열고 사용자가 파일을 선택하면
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                string destinationPath = Path.Combine(INPUTROUTE, Path.GetFileName(selectedFilePath));
                try
                {
                    File.Copy(selectedFilePath, destinationPath, false);
                    MessageBox.Show("File uploaded successfully.");
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show("An error occurred: " + ioEx.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the Original Folder";

                // 기본 경로 설정 (옵션)
                folderBrowserDialog.SelectedPath = ORIGINALPATH;

                // 폴더 대화 상자 표시
                DialogResult result = folderBrowserDialog.ShowDialog();

                // 사용자가 OK를 눌렀는지 확인
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    // originalPath 변수에 선택된 경로 할당
                    ORIGINALPATH = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the transformed Folder";

                // 기본 경로 설정 (옵션)
                folderBrowserDialog.SelectedPath = TRANSFORMEDPATH;

                // 폴더 대화 상자 표시
                DialogResult result = folderBrowserDialog.ShowDialog();

                // 사용자가 OK를 눌렀는지 확인
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    // originalPath 변수에 선택된 경로 할당
                    TRANSFORMEDPATH = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the input Folder";

                // 기본 경로 설정 (옵션)
                folderBrowserDialog.SelectedPath =  INPUTROUTE;

                // 폴더 대화 상자 표시
                DialogResult result = folderBrowserDialog.ShowDialog();

                // 사용자가 OK를 눌렀는지 확인
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    // originalPath 변수에 선택된 경로 할당
                    INPUTROUTE = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select the log Folder";

                // 기본 경로 설정 (옵션)
                folderBrowserDialog.SelectedPath = LOGPATH;

                // 폴더 대화 상자 표시
                DialogResult result = folderBrowserDialog.ShowDialog();

                // 사용자가 OK를 눌렀는지 확인
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    // originalPath 변수에 선택된 경로 할당
                    LOGPATH = folderBrowserDialog.SelectedPath;
                }
            }
        }
    }
}
