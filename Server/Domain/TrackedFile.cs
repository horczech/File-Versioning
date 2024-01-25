namespace FileVersioning.Server.Domain;

public class TrackedFile
{
    public TrackedFile(string path, string name, DateTime lastModified, int version)
    {
        Path = path;
        Name = name;
        LastModified = lastModified;
        Version = version;
    }
    
    public string Path { get; set; }
    public string Name { get; set; }
    public DateTime LastModified { get; set; }
    public int Version { get; set; }
}