namespace PuxTask.Server.Domain;

public class FolderAnalysisResult
{
    public List<FileInfo> InitializedFiles { get; set; } = [];
    public List<FileInfo> NewFiles { get; set; } = [];
    public List<FileInfo> ModifiedFiles { get; set; } = [];
    public List<FileInfo> DeletedFiles { get; set; } = [];
}