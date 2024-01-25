namespace FileVersioning.Server.Domain;

public class FileInfo
{
    public FileInfo(string name, string path, DateTime? lastModified, int? version)
    {
        Name = name;
        Path = path;
        LastModified = lastModified;
        Version = version;
    }

    public string Name { get; set; }
    public string Path { get; set; }
    public DateTime? LastModified { get; set; }
    public int? Version { get; set; }
}