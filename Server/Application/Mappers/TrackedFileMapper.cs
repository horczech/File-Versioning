using FileVersioning.Server.Domain;

namespace FileVersioning.Server.Application.Mappers;

public static class TrackedFileMapper
{
    public static FileVersioning.Server.Domain.FileInfo ToFileInfo (this TrackedFile trackedFile) => 
        new(trackedFile.Name, trackedFile.Path, trackedFile.LastModified, trackedFile.Version);
}