
using checkFileContent.NonFormClasses;
using MetroFramework.Forms;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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
        private bool isRunning = true;

        private FileProcessCount[] fileCounts = new FileProcessCount[3];
        private ConcurrentQueue<FailureInfo> failedFiles = new ConcurrentQueue<FailureInfo>();
        private ConcurrentQueue<SuccessInfo> successedFiles = new ConcurrentQueue<SuccessInfo>();
        private System.Windows.Forms.Timer fileListUpdateTimer;

        private string ORIGINALPATH = "..\\DATAS\\original\\";
        private string TRANSFORMEDPATH = "..\\DATAS\\transformed\\";
        private string INPUTROUTE = "..\\DATAS\\inputRoute\\";
        private static string LOGPATH = "..\\DATAS\\log\\";
        private static string ERRORPATH = Path.Combine(LOGPATH, "errorLog");

        private static long userInputSize;

        private SettingsManager settingsManager = new SettingsManager();
        private SettingsUI settingsFormInit = new SettingsUI(LOGPATH, ERRORPATH);

        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 3; i++)
            {
                fileCounts[i] = new FileProcessCount { SuccessCount = 0, FailureCount = 0 };
            }

            RunGenerateFolder();
            userInputSize = settingsManager.Settings.UserInputSize;
            InitializeFileSystemWatcher();
            InitializeThreadsAndLabels();
            InitializeFileListUpdateTimer();
            settingsFormInit.DeleteOldLogs();
            UpdateStatus("파일 변환 전");
            UpdatePathLabel();
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void InitializeFileSystemWatcher()
        {
            if (watcher != null)
            {
                watcher.Dispose();
            }
            watcher = new FileSystemWatcher();
            watcher.Path = Path.GetFullPath(@INPUTROUTE);
            watcher.Filter = "*.*";
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;

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
            try
            {
                if (File.Exists(e.FullPath))
                {
                    fileList.Enqueue(e.FullPath);
                }
                else if (Directory.Exists(e.FullPath))
                {
                    MessageBox.Show("폴더는 처리할 수 없습니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                UpdateStatus("파일 변환 중");
            }
            catch (UnauthorizedAccessException ex)
            {

            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string filePathToRemove = e.FullPath;
            RemoveFileFromQueue(filePathToRemove);
        }

        private void RemoveFileFromQueue(string filePath)
        {
            string item;
            Queue<string> tmpQueue = new Queue<string>();

            while (fileList.TryDequeue(out item))
            {
                if (item != filePath)
                {
                    tmpQueue.Enqueue(item);
                }
            }

            string[] newQueue = tmpQueue.ToArray();
            for (int i = 0; i < newQueue.Length; i++)
            {
                fileList.Enqueue(newQueue[i]);
            }
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
                conversionThreads[i] = new Thread(() => ProcessFiles(threadIndex)); //thread start
                conversionThreads[i].Start();
                UpdateFileCountLabel(threadIndex);
            }
            progressBarOriginal.Maximum = (int)userInputSize;
            progressBarTransformed.Maximum = (int)userInputSize;
            progressBarInput.Maximum = (int)userInputSize;
            progressBarLog.Maximum = (int)userInputSize;
        }

        private void InitializeFileListUpdateTimer()
        {
            fileListUpdateTimer = new System.Windows.Forms.Timer();
            fileListUpdateTimer.Interval = 1000;
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
                    if (!WaitForIO(filePath))
                    {
                        continue;
                    }

                    UpdateFileCountLabel(threadIndex);
                    FileMetaData fileMetaData = new FileMetaData(filePath, ORIGINALPATH, LOGPATH);

                    Thread.Sleep(500);
                    UpdateThreadLabel(threadIndex, "변환 시작");
                    Thread.Sleep(1000);

                    try
                    {
                        WriteLog(fileMetaData.LogPath, "Thread index :" + threadIndex + " Starts transrform");

                        if (fileMetaData.IsDuplicated == true)
                        {
                            WriteLog(fileMetaData.LogPath, "Duplicate name found in original path, File Name changed.");
                        }
                        if (CheckTransformFunction(fileMetaData, threadIndex) == true)
                        {
                            fileCounts[threadIndex].SuccessCount++;
                            UpdateFileCountLabel(threadIndex);
                            TransformFile(fileMetaData, threadIndex);
                            WriteLog(fileMetaData.LogPath, "Thread index :" + threadIndex + ": File Transform SUCCESS :" + fileMetaData.FileName);
                            successedFiles.Enqueue(new SuccessInfo(fileMetaData.FilePath, threadIndex));
                        }
                        else
                        {
                            WriteLog(fileMetaData.LogPath, "File failed to transform, move to Error log folder");
                            string errorLogPath = Path.Combine(ERRORPATH + "\\", "ERROR_" + Path.GetFileName(fileMetaData.LogPath));
                            File.Move(fileMetaData.LogPath, errorLogPath);
                            UpdateThreadLabel(threadIndex, "변환 실패");
                            Thread.Sleep(1000);
                        }
                        File.Move(fileMetaData.FilePath, fileMetaData.OriginalPath);
                    }
                    catch (Exception ex)
                    {
                        UpdateThreadLabel(threadIndex, "변환 실패");
                        Thread.Sleep(1000);
                    }
                    UpdateThreadLabel(threadIndex, "변환 종료.");
                    Thread.Sleep(1000);
                    SleepTenSecond(threadIndex);
                    UpdateThreadLabel(threadIndex, $"변환 파일 입력 대기");
                    if (!isRunning)
                        break;
                }
                else
                {
                    if (!isRunning)
                        break;
                    Thread.Sleep(1000);
                }
            }
        }

        private bool WaitForIO(string filePath, int timeout = 30000)
        {
            const int waitDelay = 500;
            var totalWaitTime = 0;

            while (totalWaitTime < timeout)
            {
                if (IsFileReady(filePath))
                {
                    return true;
                }
                Thread.Sleep(waitDelay);
                totalWaitTime += waitDelay;
            }

            return false;
        }

        private bool IsFileReady(string filePath)
        {
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                stream.Close();
                return true;
            }
            catch (IOException)
            {
                return false;
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

        private string ExtractFileName(string filePath)
        {
            try
            {
                string firstLine = "";
                string extension = Path.GetExtension(filePath);
                if (extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    firstLine = Encoding.Unicode.GetString(fileData).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                }
                else
                {
                    firstLine = File.ReadLines(filePath, Encoding.Unicode).First();
                }
                if (firstLine.StartsWith("[ATRANS] "))
                {
                    return firstLine.Replace("[ATRANS] ", "").Trim();
                }

                return firstLine;
            }
            catch (Exception ex)
            {
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

        private void TransformFile(FileMetaData file, int threadIndex)
        {
            string afterATRANSName = ExtractFileName(file.FilePath);
            string transformedFileName = "";
            try
            {
                if (file.Extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                {
                    transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".txt");
                }
                else if (file.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    transformedFileName = Path.Combine(TRANSFORMEDPATH, afterATRANSName + ".bin");
                }
                transformedFileName = GenerateUniqueFileName(TRANSFORMEDPATH, Path.GetFileName(transformedFileName));

                if (file.Extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                {
                    if (file.FileData.Length >= 2 && file.FileData[0] == 0xFF && file.FileData[1] == 0xFE)
                    {
                        File.WriteAllText(transformedFileName, Encoding.Unicode.GetString(file.FileData, 2, file.FileData.Length - 2), Encoding.Unicode);
                    }
                    else
                    {
                        File.WriteAllText(transformedFileName, Encoding.Unicode.GetString(file.FileData), Encoding.Unicode);
                    }
                }
                else if (file.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllBytes(transformedFileName, file.FileData);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private bool CheckTransformFunction(FileMetaData file, int threadIndex)
        {
            string[] errorReasons = { "Extension check Failed", "Name check Failed", "Size check Failed", "Header check Failed", "Header Name no Match" };

            if (CheckExtension(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file.FilePath, threadIndex, errorReasons[0]));
                return false;
            }

            if (CheckFileName(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file.FilePath, threadIndex, errorReasons[1]));
                return false;
            }

            if (CheckFileSize(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file.FilePath, threadIndex, errorReasons[2]));
                return false;
            }

            if (CheckFileHeader(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file.FilePath, threadIndex, errorReasons[3]));
                return false;
            }

            if (CheckHeaderAndName(file, threadIndex) == false)
            {
                failedFiles.Enqueue(new FailureInfo(file.FilePath, threadIndex, errorReasons[4]));
                return false;
            }
            return true;
        }

        bool CheckExtension(FileMetaData file, int threadIndex)
        {
            if (file.Extension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
            {
                WriteLog(file.LogPath, "Extension check PASSED for " + file.FileName);
            }
            else if (file.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                WriteLog(file.LogPath, "Extension check PASSED for " + file.FileName);
            }
            else
            {
                HandleFailure(file, threadIndex, "File Extension Error - Invalid Extension for filename : ");
                return false;
            }
            return true;
        }

        bool CheckFileName(FileMetaData file, int threadIndex)
        {
            if (!file.FileName.StartsWith("[TargetFileName] "))
            {
                HandleFailure(file, threadIndex, "File Name Error - Invalid Header for filename : ");
                return false;
            }
            string trimmedFileName = file.FileName.Replace("[TargetFileName] ", "");
            if (string.IsNullOrWhiteSpace(trimmedFileName) || trimmedFileName.StartsWith(" ") || trimmedFileName.Equals(".bin") || trimmedFileName.Equals(".txt"))
            {
                HandleFailure(file, threadIndex, "File Name Error - Valid Header but other Header Issue for : ");
                return false;
            }
            if (trimmedFileName[0] == '(' && trimmedFileName[2] == ')' && trimmedFileName.Length == 8)
            {
                HandleFailure(file, threadIndex, "File Name Error - Valid Header but other Header Issue for : ");
                return false;
            }

            WriteLog(file.LogPath, "File Name check PASSED for " + file.FileName);
            return true;
        }

        bool CheckFileSize(FileMetaData file, int threadIndex)
        {
            FileInfo fileInfo = new FileInfo(file.FilePath);

            if (fileInfo.Length < 18)
            {
                HandleFailure(file, threadIndex, "file Size Error - Size Check Failed for : ");
                return false;
            }
            WriteLog(file.LogPath, "File Size check PASSED for " + file.FileName);
            return true;
        }

        bool CheckFileHeader(FileMetaData file, int threadIndex)
        {
            const string headerToCheck = "[ATRANS] ";
            try
            {
                string fileContent = Encoding.Unicode.GetString(file.FileData, 2, file.FileData.Length - 2);
                string firstLine;
                int firstNewLineIndex = fileContent.IndexOfAny(new[] { '\r', '\n' });
                if (firstNewLineIndex > -1)
                {
                    firstLine = fileContent.Substring(0, firstNewLineIndex);
                }
                else
                {
                    firstLine = fileContent;
                }

                if (!firstLine.StartsWith(headerToCheck))
                {
                    HandleFailure(file, threadIndex, "File Header error - Does not start with [ATRANS] for : ");
                    return false;
                }

                if (firstLine.Length <= headerToCheck.Length || firstLine[headerToCheck.Length] == ' ')
                {
                    HandleFailure(file, threadIndex, "File Header error - Incorrect format after [ATRANS] for : ");
                    return false;
                }
                WriteLog(file.LogPath, "File Header correct, it starts with [ATRANS] " + file.FileName);
                return true;
            }
            catch (Exception ex)
            {
                HandleFailure(file, threadIndex, "File Header error - Issue with File Header : " + ex.Message + "for : ");
                return false;
            }
        }

        bool CheckHeaderAndName(FileMetaData file, int threadIndex)
        {
            string fileName = file.FileNameWithoutExtension;
            string headerName;

            fileName = fileName.Substring("[TargetFileName] ".Length);
            string fileIndex = "";
            if (file.IsDuplicated == true)
            {
                int lastIndex = fileName.LastIndexOf("(");
                if (lastIndex > 0)
                {
                    fileIndex = fileName.Substring(lastIndex);
                    fileName = fileName.Substring(0, lastIndex);
                }
            }
            string fileContent = Encoding.Unicode.GetString(file.FileData, 2, file.FileData.Length - 2);
            string[] lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            headerName = lines[0].Substring("[ATRANS] ".Length);
            if (fileName == headerName)
            {
                headerName += fileIndex;
                if (file.IsDuplicated)
                {
                    UpdateFileHeaderWithIndex(file.FilePath, fileIndex);
                    file.UpdateFileData();
                }
                WriteLog(file.LogPath, "File Name Contents and Header Contents matches :" + file.FileName);
                return true;
            }
            else
            {
                HandleFailure(file, threadIndex, "File Header & Name error - Target Name And Header MisMatch for : ");
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

        private void HandleFailure(FileMetaData file, int threadIndex, string errorReason)
        {   
            fileCounts[threadIndex].FailureCount++;
            WriteLog(file.LogPath, errorReason + file.FileName);
            UpdateFileCountLabel(threadIndex);
            File.Move(file.FilePath, file.OriginalPath);
        }

        private void UpdateFileHeaderWithIndex(string file, string headerToAdd)
        {
            List<string> lines = File.ReadAllLines(file, Encoding.Unicode).ToList();
            if (lines.Count > 0)
            {
                lines[0] = lines[0] + headerToAdd;
                File.WriteAllLines(file, lines, Encoding.Unicode);
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
                return 0;
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
                Application.Exit();
                return;
            }
            progressBarOriginal.Value = Math.Min((int)(GetDirectorySize(ORIGINALPATH) / (1024L * 1024L)), progressBarOriginal.Maximum);
            progressBarTransformed.Value = Math.Min((int)(GetDirectorySize(TRANSFORMEDPATH) / (1024L * 1024L)), progressBarTransformed.Maximum);
            progressBarInput.Value = Math.Min((int)(GetDirectorySize(INPUTROUTE) / (1024L * 1024L)), progressBarInput.Maximum);
            progressBarLog.Value = Math.Min((int)(GetDirectorySize(LOGPATH) / (1024L * 1024L)), progressBarLog.Maximum);
            CheckForSpaceLimit();
        }

        private void CheckForSpaceLimit()
        {
            long limit = userInputSize * 1024L * 1024L;
            if (GetDirectorySize(ORIGINALPATH) > limit ||
                GetDirectorySize(TRANSFORMEDPATH) > limit ||
                GetDirectorySize(INPUTROUTE) > limit ||
                GetDirectorySize(LOGPATH) > limit)
            {
                fileListUpdateTimer.Stop();
                MessageBox.Show("경고: 저장 공간 제한에 도달했습니다!", "저장 공간 경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                return;
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


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
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
            catch (InvalidAsynchronousStateException ex)
            {
                MessageBox.Show("애플리케이션이 안전하게 종료되지 못했습니다. 잠시 후 다시 시도.\n" + ex.Message, "종료 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void errorLogCheck_Click(object sender, EventArgs e)
        {
            ShowErrorList newForm2 = new ShowErrorList(failedFiles, ERRORPATH);
            newForm2.ShowDialog();
        }
        private void firstThreadButton_Click(object sender, EventArgs e)
        {
            ShowEachStatus newForm2 = new ShowEachStatus(failedFiles, successedFiles, 0);
            newForm2.ShowDialog();
        }
        private void secondThreadButton_Click(object sender, EventArgs e)
        {
            ShowEachStatus newForm2 = new ShowEachStatus(failedFiles, successedFiles, 1);
            newForm2.ShowDialog();
        }
        private void thirdThreadButton_Click(object sender, EventArgs e)
        {
            ShowEachStatus newForm2 = new ShowEachStatus(failedFiles, successedFiles, 2);
            newForm2.ShowDialog();
        }

        private void DirectUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a file to upload";
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                string destinationPath = Path.Combine(INPUTROUTE, Path.GetFileName(selectedFilePath));
                try
                {
                    File.Copy(selectedFilePath, destinationPath, false);
                    MessageBox.Show("파일이 성공적으로 업로드 되었습니다.");
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
            List<string> paths = new List<string> { absoluteOriginalPath, absoluteTransformedPath, absoluteInputRoute, absoluteLogPath };
            for (int i = 0; i < paths.Count; i++)
            {
                for (int j = i + 1; j < paths.Count; j++)
                {
                    if (paths[i].Equals(paths[j], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int CountBackslashes(string path)
        {
            return path.Count(c => c == '\\');
        }

        private void SelectAndSetFolderPath(ref string path, string description)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = description;
                folderBrowserDialog.SelectedPath = path;
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {

                    string selectedPath = folderBrowserDialog.SelectedPath;
                    if (CountBackslashes(selectedPath) <= 2)
                    {
                        MessageBox.Show("유효하지 않은 경로입니다.", "Path Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string prevPath = path;
                    path = folderBrowserDialog.SelectedPath;

                    if (CheckDupPaths())
                    {
                        MessageBox.Show("경로 중복으로 인해 경로 변경에 실패합니다.", "Path Change Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        path = prevPath;
                    }
                    UpdatePathLabel();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SelectAndSetFolderPath(ref ORIGINALPATH, "원본 파일이 저장될 폴더를 선택하세요.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SelectAndSetFolderPath(ref TRANSFORMEDPATH, "변환 파일이 저장될 폴더를 선택하세요.");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SelectAndSetFolderPath(ref INPUTROUTE, "파일이 입력될 폴더를 선택하세요.");
            InitializeFileSystemWatcher();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SelectAndSetFolderPath(ref LOGPATH, "성공/실패 로그 파일이 저장될 폴더를 선택하세요.");
            ERRORPATH = Path.Combine(LOGPATH, "errorLog");
            Directory.CreateDirectory(ERRORPATH);
        }

        private void openSettingButton_Click(object sender, EventArgs e)
        {
            SettingsUI settingsForm = new SettingsUI(LOGPATH, ERRORPATH);
            settingsForm.OnApplySettings += ApplyNewSettings;
            settingsForm.Show();
        }

        public void ApplyNewSettings(long newSize)
        {
            userInputSize = newSize;
            progressBarOriginal.Maximum = (int)userInputSize;
            progressBarTransformed.Maximum = (int)userInputSize;
            progressBarInput.Maximum = (int)userInputSize;
            progressBarLog.Maximum = (int)userInputSize;
            UpdateProgressBars();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
