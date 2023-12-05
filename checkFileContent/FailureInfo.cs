using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
