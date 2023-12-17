
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
