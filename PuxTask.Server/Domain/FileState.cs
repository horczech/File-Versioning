namespace PuxTask.Server.Domain;

public class FileState
{
    public DateTime LastModified { get; set; }
    public int Version { get; set; }
}