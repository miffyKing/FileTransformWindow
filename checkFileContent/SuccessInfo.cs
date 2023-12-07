using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SuccessInfo
{
    public string FileName { get; set; }
    public int ThreadIndex { get; set; }

    public SuccessInfo(string fileName, int threadIndex)
    {
        FileName = fileName;
        ThreadIndex = threadIndex;
    }
}
