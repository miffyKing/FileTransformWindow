using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkFileContent.NonFormClasses
{
    public class AppSettings
    {
        public int ExpireDate { get; set; } = 6;
        public int UserInputSize { get; set; } = 5*1024;
    }
}
