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

        public FileMetaData(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            Extension = Path.GetExtension(filePath);
            FileData = File.ReadAllBytes(filePath); // 주의: 대용량 파일의 경우 성능 저하 발생 가능
        }

    }
}
