using PuxTask.Server.Domain;

namespace PuxTask.Server.Application.Mappers;

public static class TrackedFileMapper
{
    public static Domain.FileInfo ToFileInfo (this TrackedFile trackedFile) => 
        new(trackedFile.Name, trackedFile.Path, trackedFile.LastModified, trackedFile.Version);
}