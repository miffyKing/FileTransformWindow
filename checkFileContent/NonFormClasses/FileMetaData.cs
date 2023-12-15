using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkFileContent
{
    internal class FileMetaData
    {
        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public string FileNameWithoutExtension { get; private set; }
        public string Extension { get; private set; }
        public byte[] FileData { get; private set; }
        public string OriginalPath { get; private set; }
        public string LogPath { get; private set; }
        public bool IsDuplicated { get; private set; }


        public FileMetaData(string filePath, string originalDir, string logDir)
        {
            FilePath = CheckDupFileName(filePath, originalDir);
            if (FilePath != filePath)
            {
                IsDuplicated = true;
            }
            else
            {
                IsDuplicated = false;
            }

            FileName = Path.GetFileName(FilePath);
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
            Extension = Path.GetExtension(FilePath);
            FileData = File.ReadAllBytes(FilePath);
            OriginalPath = Path.Combine(originalDir, FileName);
            LogPath = Path.Combine(logDir, "log_" + FileName + ".txt");
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

            if (newFilePath != filePath)
            {
                File.Move(filePath, newFilePath);
            }
            return newFilePath;
        }
    }
}
