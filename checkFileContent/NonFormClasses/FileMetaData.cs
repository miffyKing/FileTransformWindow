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
            FilePath = CheckDupFileName(filePath, originalDir); // 중복 파일 이름 검사 및 경로 업데이트
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
            FileData = File.ReadAllBytes(FilePath); // 주의: 대용량 파일의 경우 성능 저하 발생 가능
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
                File.Move(filePath, newFilePath); // 파일 이름이 중복되는 경우 새 경로로 이동
            }

            return newFilePath;
        }
    }
}
