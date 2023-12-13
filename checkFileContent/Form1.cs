using MetroFramework.Forms;     //이쁘게 보이게 하고싶습니다.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
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
        private System.Windows.Forms.Timer fileListUpdateTimer;

        private string ORIGINALPATH = "..\\DATAS\\original\\";
        private string TRANSFORMEDPATH = "..\\DATAS\\transformed\\";
        private string INPUTROUTE = "..\\DATAS\\inputRoute\\";
        private static string LOGPATH = "..\\DATAS\\log\\";
        private static string ERRORPATH = Path.Combine(LOGPATH, "errorLog");
        private static long userInputSize = 1024;
        /*        private FileSystemWatcher[] watchers;           //파일시스템와처를 클래스레벨에서 유지하고 있어야함.*/

        private bool isSpaceLimitWarningShown = false; // 클래스 레벨 변수 추가

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 3; i++)
            {
                fileCounts[i] = new FileProcessCount { SuccessCount = 0, FailureCount = 0 };
            }

            RunGenerateFolder();                //폴더 생성
            InitializeFileSystemWatcher();      //fsw 생성 - 감시 시작
            //InitializeFolderDeletionWatcher();

            InitializeThreadsAndLabels();       //UI 표시용 invoke

            InitializeFileListUpdateTimer();

            DeleteOldLogs();                    //오래된 로그파일 지우기
            UpdateStatus("파일 변환 전");
            UpdatePathLabel();
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void InitializeFileSystemWatcher()
        {

            if (watcher != null)
            {
                watcher.Dispose();
            }   //폴더 변경했을때, 이전 폴더는 해제해주는 로직임. -> 되는지 확인해 봐야해요


            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetFullPath(@INPUTROUTE);
            watcher.Filter = "*.*";
            watcher.Created += OnCreated;
            watcher.EnableRaisingEvents = true;
        }

        
        private void UpdatePathLabel()
        {
            originalPathlabel.Text = ORIGINALPATH;
            transformedPathLabel.Text = TRANSFORMEDPATH;
            logPathLabel.Text = LOGPATH;
            inputPathLabel.Text = INPUTROUTE;
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
                threadLabels[i].Location = new System.Drawing.Point(330, 100 + (i * 70));
                threadLabels[i].Size = new System.Drawing.Size(100, 15);
                this.Controls.Add(threadLabels[i]);
                threadLabels[i].BringToFront();
                threadLabels[i].Text = "파일 입력 대기";

                fileCountLabels[i] = new Label();
                fileCountLabels[i].Location = new System.Drawing.Point(430, 100 + (i * 70));
                fileCountLabels[i].Size = new System.Drawing.Size(200, 15);
                fileCountLabels[i].Text = "Success: 0  Failure: 0";
                fileCountLabels[i].BringToFront();
                this.Controls.Add(fileCountLabels[i]);

                fileCounts[i].SuccessCount = 0;
                fileCounts[i].FailureCount = 0;

                int threadIndex = i;
                conversionThreads[i] = new Thread(() => ProcessFiles(threadIndex));
                conversionThreads[i].Start();
                UpdateFileCountLabel(threadIndex);
            }

            //폴더 저장공간 표시.
            progressBarOriginal.Maximum = (int)userInputSize; // 1GB, MB 단위로 표시
            progressBarTransformed.Maximum = (int)userInputSize;
            progressBarInput.Maximum = (int)userInputSize;
            progressBarLog.Maximum = (int)userInputSize;
        }

        private void InitializeFileListUpdateTimer()
        {
            fileListUpdateTimer = new System.Windows.Forms.Timer();
            fileListUpdateTimer.Interval = 1000; // 1초 간격
            fileListUpdateTimer.Tick += new EventHandler(OnFileListUpdateTimerTick);
            fileListUpdateTimer.Start();
        }

        private void OnFileListUpdateTimerTick(object sender, EventArgs e)
        {
            UpdateFileListStatus();
            UpdateFileListBox();
            UpdateProgressBars();
        }


        private void ProcessFiles(int threadIndex)
        {
            while (isRunning)
            {
                if (fileList.TryDequeue(out string filePath))
                {
                    UpdateFileCountLabel(threadIndex);
                    string prevName = filePath;
                    filePath = CheckDupFileName(filePath, ORIGINALPATH);

                    Thread.Sleep(500);
                    UpdateThreadLabel(threadIndex, "변환 시작");
                    Thread.Sleep(1000);

                    try
                    {
                        string originalFilePath = Path.Combine(ORIGINALPATH, Path.GetFileName(filePath));              
                        string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(filePath) + ".txt");
                        bool isDuplicated = false;
                        WriteLog(logFilePath, "Thread index :" + threadIndex + " Starts transrform");
                        if (prevName != filePath)
                        {
                            isDuplicated = true;
                            WriteLog(logFilePath, "Duplicate name found in original path, File Name changed.");
                        }

                        if (checkTransformFunction(filePath, threadIndex, isDuplicated) == true)
                        {
                            fileCounts[threadIndex].SuccessCount++;
                            UpdateFileCountLabel(threadIndex);  // 이거 변환프로세스 따라 값 바꿔야함
                            TransformFile(filePath, threadIndex);
                            WriteLog(logFilePath, "Thread index :" + threadIndex + ": File Transform SUCCESS :" + Path.GetFileName(filePath));
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
                    string pointToAdd = "";
                    for (int j = 0; j < i % 4; j++)
                    {
                        pointToAdd += ".";
                    }
                    UpdateThreadLabel(threadIndex, lineToUpdate + pointToAdd);
                    Thread.Sleep(1000);

                }
        }
        private string CheckDupFileName(string filePath, string originalDir)
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

        private string ExtractFileName(string filePath)
        {
            //중요!! checkDupName 을 transform 폴더에 대해서 돌려서, 중복되는 경우에 인덱스 추가해서 돌려주어야 한다.
            try
            {
                string firstLine = "";
                string extension = Path.GetExtension(filePath);
                if (extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                {
                    // 이진 파일 처리
                    byte[] fileData = File.ReadAllBytes(filePath);
                    firstLine = Encoding.Unicode.GetString(fileData).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                }
                else
                {
                    // 텍스트 파일 처리
                    firstLine = File.ReadLines(filePath, Encoding.Unicode).First();
                }

                // '[ATRANS]' 문자열 제거
                if (firstLine.StartsWith("[ATRANS]"))
                {
                    return firstLine.Replace("[ATRANS]", "").Trim();
                }

                return firstLine;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        private string GenerateUniqueFileName(string basePath, string fileName)
        {
            string fullFilePath = Path.Combine(basePath, fileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            int count = 2;

            while (File.Exists(fullFilePath))
            {
                fullFilePath = Path.Combine(basePath, $"{fileNameWithoutExt}({count++}){extension}");
            }
            return fullFilePath;
        }

        /*private void TransformFile(string file, int threadIndex)
        {
            Console.Write("Processing File: " + file + "\n");
            string extension = Path.GetExtension(file);

            // "[TargetFileName] "을 제거한 파일명
            string trimmedFileName = Path.GetFileNameWithoutExtension(file).Replace("[TargetFileName] ", "");
            string transformedFileName = "";
            byte[] fileData = File.ReadAllBytes(file);
            string afterATRANSName = ExtractFileName(file);

            try
            {
                if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                {
                    //여기에 ExtractFileName 에서 리턴 받은 [atrans] 뒤에 오는 내용을 저장하고, 그거에 변환파일 내용이랑 확장자를 갖다 붙이자.
                    //transformedFileName = Path.Combine(TRANSFORMEDPATH, trimmedFileName + ".atxt");
                    transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".atxt");
                     if (!File.Exists(transformedFileName))
                    {
                        Console.Write("no duplicate file name in transform area\n");
                        File.WriteAllText(transformedFileName, Encoding.UTF8.GetString(fileData), Encoding.UTF8);
                    }
                }
                else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                {
                    //transformedFileName = Path.Combine(TRANSFORMEDPATH, trimmedFileName + ".abin");
                    transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".abin");
                    
                    if (!File.Exists(transformedFileName))
                    {
                        Console.Write("no duplicate file name in transform area\n");
                        File.WriteAllBytes(transformedFileName, fileData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TransformFile: " + ex.Message);
                return;
            }
        }
*/
        //-- UTF8 Version --
        private void TransformFile(string file, int threadIndex)
        {
            Console.Write("Processing File: " + file + "\n");
            string extension = Path.GetExtension(file);
            string afterATRANSName = ExtractFileName(file);
            string transformedFileName = "";

            //string fileContent = Encoding.Unicode.GetString(fileData, 2, fileData.Length - 2);
            byte[] fileData = File.ReadAllBytes(file);
            try
            {
                if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                {
                    transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".atxt");
                }
                else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                {
                    transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".abin");
                }

                // 중복 파일 이름 확인 및 새 이름 생성
                transformedFileName = GenerateUniqueFileName(TRANSFORMEDPATH, Path.GetFileName(transformedFileName));

                // 파일 쓰기
                if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllText(transformedFileName, Encoding.Unicode.GetString(fileData, 2, fileData.Length - 2), Encoding.Unicode);
                }
                else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllBytes(transformedFileName, File.ReadAllBytes(file));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in TransformFile: " + ex.Message);
            }
        }

        /*        private void TransformFile(string file, int threadIndex)
                {
                    Console.Write("Processing File: " + file + "\n");
                    string extension = Path.GetExtension(file);
                    string afterATRANSName = ExtractFileName(file);
                    string transformedFileName = "";

                    try
                    {
                        if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                        {
                            transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".atxt");
                        }
                        else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                        {
                            transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".abin");
                        }

                        // 중복 파일 이름 확인 및 새 이름 생성
                        transformedFileName = GenerateUniqueFileName(TRANSFORMEDPATH, Path.GetFileName(transformedFileName));

                        // 파일 쓰기
                        if (extension.Equals(".abin", StringComparison.OrdinalIgnoreCase))
                        {
                            // .abin (바이너리)을 .atxt (텍스트)로 변환
                            File.WriteAllText(transformedFileName, Encoding.Unicode.GetString(File.ReadAllBytes(file)), Encoding.Unicode);
                        }
                        else if (extension.Equals(".atxt", StringComparison.OrdinalIgnoreCase))
                        {
                            // .atxt (텍스트)를 .abin (바이너리)으로 변환
                            File.WriteAllBytes(transformedFileName, Encoding.Unicode.GetBytes(File.ReadAllText(file, Encoding.Unicode)));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in TransformFile: " + ex.Message);
                    }
                }*/

        private bool checkTransformFunction(string file, int threadIndex, bool isDuplicated)
        {
            string[] errorReasons = { "Extension check Failed", "Name check Failed", "Size check Failed", "Header check Failed", "Header Name no Match" };   
           
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

            if (checkHeaderAndName(file, threadIndex, isDuplicated) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file, threadIndex, errorReasons[4]));
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
                    string fileContent = Encoding.Unicode.GetString(fileData, 2, fileData.Length - 2);

                    Console.Write("WOW : " + fileContent);
                    string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 0 && lines[0].StartsWith(headerToCheck))
                    {
                        if (lines[0].Length > headerToCheck.Length && lines[0][headerToCheck.Length] != ' ')
                        {
                            WriteLog(logFilePath, "File Header correct, it starts with [ATRANS] " + Path.GetFileName(file));
                            return true;
                        }
                        else
                        {
                            WriteLog(logFilePath, "File Header error: more than 1 space after [ATRANS]: " + Path.GetFileName(file));
                        }
                        
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

        bool checkHeaderAndName(string file, int threadIndex, bool isDuplicated)
        {
            string logFilePath = Path.Combine(LOGPATH, "log_" + Path.GetFileName(file) + ".txt");
            string fileName = Path.GetFileNameWithoutExtension(file); // 확장자를 제외한 파일명
            string headerName;
            string originalFilePath = Path.Combine(ORIGINALPATH, Path.GetFileName(file));
            
            const string fileNamePrefix = "[TargetFileName] ";
            const string headerPrefix = "[ATRANS] ";
            
            fileName = fileName.Substring(fileNamePrefix.Length); // Prefix 제거

            if (isDuplicated)
            {
                // 중복 파일의 경우, 파일 이름에서 마지막 '(' 기준으로 숫자 인덱스 제거
                int lastIndex = fileName.LastIndexOf("(");
                if (lastIndex > 0)
                {
                    fileName = fileName.Substring(0, lastIndex);
                }
            }

            byte[] fileData = File.ReadAllBytes(file);
            string fileContent = Encoding.Unicode.GetString(fileData, 2, fileData.Length - 2);
            string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            headerName = lines[0].Substring(headerPrefix.Length).Trim(); // Prefix 제거 및 공백 제거


            Console.WriteLine("header name is : " + headerName + " Filename is : " + fileName);
            if (fileName == headerName)
            {
                WriteLog(logFilePath, "File Name Contents and Header Contents matches :" + Path.GetFileName(file));
                return true;
            }
            else
            {
                WriteLog(logFilePath, "Mismatch between File Name Contents and Header Contents : " + Path.GetFileName(file));
                fileCounts[threadIndex].FailureCount++;
                File.Move(file, originalFilePath); //이거도 나중에 없애고 변환한 후에로 바꿔야함.
                UpdateFileCountLabel(threadIndex);
                return false;
            }
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

        private void UpdateFileListStatus()
        {
            try
            {
                if (fileListStatusLabel.InvokeRequired)
                {
                    fileListStatusLabel.Invoke(new Action(() => fileListStatusLabel.Text = "파일 목록 상태: " + fileList.Count + "개 파일 대기중"));
                }
                else
                {
                    fileListStatusLabel.Text = "파일 목록 상태: " + fileList.Count + "개 파일 대기중";
                }
            }
            catch (InvalidAsynchronousStateException)
            {
                // 예외 처리 로직
            }
        }

        private void UpdateFileListBox()
        {
            if (fileListBox.InvokeRequired)
            {
                fileListBox.Invoke(new Action(() => RefreshFileListBox()));
            }
            else
            {
                RefreshFileListBox();
            }
        }


        private void RefreshFileListBox()
        {
            fileListBox.Items.Clear();
            foreach (var filePath in fileList)
            {
                fileListBox.Items.Add(Path.GetFileName(filePath));
            }
        }

        private long GetDirectorySize(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return 0; // 폴더가 존재하지 않으면 0 반환
            }

            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }

        private void UpdateProgressBars()
        {
            if (!Directory.Exists(ORIGINALPATH) || !Directory.Exists(TRANSFORMEDPATH) ||
        !Directory.Exists(INPUTROUTE) || !Directory.Exists(LOGPATH))
            {
                fileListUpdateTimer.Stop();
                MessageBox.Show("경고: 하나 이상의 폴더가 삭제되어 프로그램을 종료합니다!", "폴더 삭제 경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                
                Application.Exit(); // 프로그램 종료

                return; // 폴더가 없으면 나머지 업데이트를 중단
            }
            progressBarOriginal.Value = Math.Min((int)(GetDirectorySize(ORIGINALPATH) / (1024L * 1024L)), progressBarOriginal.Maximum);
            progressBarTransformed.Value = Math.Min((int)(GetDirectorySize(TRANSFORMEDPATH) / (1024L * 1024L)), progressBarTransformed.Maximum);
            progressBarInput.Value = Math.Min((int)(GetDirectorySize(INPUTROUTE) / (1024L * 1024L)), progressBarInput.Maximum);
            progressBarLog.Value = Math.Min((int)(GetDirectorySize(LOGPATH) / (1024L * 1024L)), progressBarLog.Maximum);

            CheckForSpaceLimit();
        }

        private void CheckForSpaceLimit()
        {
            long limit = userInputSize * 1024L * 1024L; // 1GB in bytes
            if (GetDirectorySize(ORIGINALPATH) > limit ||
                GetDirectorySize(TRANSFORMEDPATH) > limit ||
                GetDirectorySize(INPUTROUTE) > limit ||
                GetDirectorySize(LOGPATH) > limit)
            {
                //isSpaceLimitWarningShown = true; // 플래그 설정
                fileListUpdateTimer.Stop();
                MessageBox.Show("경고: 저장 공간 제한에 도달했습니다!", "저장 공간 경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                Application.Exit(); // 프로그램 종료

                return; // 폴더가 없으면 나머지 업데이트를 중단
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

            DeleteFileByDate(logPath, 7);
            DeleteFileByDate(errorLogPath, 7);
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
            showErrorList newForm2 = new showErrorList(failedFiles, ERRORPATH);
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

        private bool CheckDupPaths()
        {
            string absoluteOriginalPath = Path.GetFullPath(ORIGINALPATH).TrimEnd('\\');
            string absoluteTransformedPath = Path.GetFullPath(TRANSFORMEDPATH).TrimEnd('\\');
            string absoluteInputRoute = Path.GetFullPath(INPUTROUTE).TrimEnd('\\');
            string absoluteLogPath = Path.GetFullPath(LOGPATH).TrimEnd('\\');
            // \ 이거 맨 마지막에 제외 안하면 상대경로와 절대경로를 다르게 판단합니다.
            List<string> paths = new List<string> { absoluteOriginalPath, absoluteTransformedPath, absoluteInputRoute, absoluteLogPath };

            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = i + 1; j < paths.Count; j++)
                {
                    if (paths[i].Equals(paths[j], StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Duplicate path found: " + paths[i]);
                        return true;
                    }
                }
            }
            return false;
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
                    string prevORIGINALPATH = ORIGINALPATH;
                    ORIGINALPATH = folderBrowserDialog.SelectedPath;
                    if (CheckDupPaths() == true)
                    {
                        //중복 경로가 존재하여 폴더 변경에 실패했다는 알람 1회 출력 후
                        MessageBox.Show("Failed to change the folder path because a duplicate path exists.", "Path Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ORIGINALPATH = prevORIGINALPATH;
                    }
                    UpdatePathLabel();
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
                    string prevTRANSFORMPATH = TRANSFORMEDPATH;
                    TRANSFORMEDPATH = folderBrowserDialog.SelectedPath;

                    if(CheckDupPaths() == true)
                    {
                        MessageBox.Show("Failed to change the folder path because a duplicate path exists.", "Path Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        TRANSFORMEDPATH = prevTRANSFORMPATH;
                    }
                    UpdatePathLabel();
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
                    string prevINPUT = INPUTROUTE;
                    INPUTROUTE = folderBrowserDialog.SelectedPath;

                    if (CheckDupPaths() == true)
                    {
                        MessageBox.Show("Failed to change the folder path because a duplicate path exists.", "Path Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        INPUTROUTE = prevINPUT;
                    }
                    InitializeFileSystemWatcher();  //변경된 폴더에서 watcher 재실행 (기존거 삭제)
                    UpdatePathLabel();
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
                    string prevLOG = LOGPATH;

                    // originalPath 변수에 선택된 경로 할당
                    LOGPATH = folderBrowserDialog.SelectedPath;
                    
                    if (CheckDupPaths() == true)
                    {
                        MessageBox.Show("Failed to change the folder path because a duplicate path exists.", "Path Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LOGPATH = prevLOG;

                    }
                    ERRORPATH = Path.Combine(LOGPATH, "errorLog");
                    Directory.CreateDirectory(ERRORPATH);
                    UpdatePathLabel();
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }
        private void transformedPathLabel_Click(object sender, EventArgs e)
        {
        }
        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownSize_ValueChanged(object sender, EventArgs e)
        {
            userInputSize = (long)numericUpDownSize.Value * 1024;
            progressBarOriginal.Maximum = (int)userInputSize; // 1GB, MB 단위로 표시
            progressBarTransformed.Maximum = (int)userInputSize;
            progressBarInput.Maximum = (int)userInputSize;
            progressBarLog.Maximum = (int)userInputSize;
        }
    }
}
