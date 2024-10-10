namespace MyBestJob.BLL.Exceptions;

public class InvalidFileException(long size, string fileTpye)
    : Exception($"Uploaded file is invalid.\nSize: {size}\nType: {fileTpye}")
{
    public long Size { get; set; } = size;
    public string FileType { get; set; } = fileTpye;
}
