using System.Collections.Concurrent;
using FileVersioning.Server.Application.Services;
using FileVersioning.Server.Domain;

namespace FileVersioning.Server.Storage;

public interface IFileStateStorage
{
    Task RemoveFiles(IEnumerable<TrackedFile> filePathsToRemove);
    Task AddNewFiles(IEnumerable<TrackedFile> filesToAdd);
    Task UpdateFiles(IEnumerable<TrackedFile> filesToUpdate);
    Task<IEnumerable<TrackedFile>> GetTrackedFiles(string folderPath);
    Task<bool> IsTrackedFolder(string folderPath);
}

public class FileStateStorage : IFileStateStorage
{
    private readonly ILogger<FileStateStorage> _logger;
    private static readonly ConcurrentDictionary<string, FileState> FileStatesStorage = new();

    public FileStateStorage(ILogger<FileStateStorage> logger) {
        _logger = logger;
    }

    public Task<IEnumerable<TrackedFile>> GetTrackedFiles(string folderPath) {
        var filePaths = FileStatesStorage.Keys.Where(key => key.StartsWith(Path.GetFullPath(folderPath), PathComparer.ComparisonType));

        var fileStates = new List<TrackedFile>();
        foreach (var filePath in filePaths) {
            if (FileStatesStorage.TryGetValue(filePath, out var fileState))
            {
                fileStates.Add(new TrackedFile(filePath, Path.GetFileName(filePath), fileState.LastModified, fileState.Version));
            }
        }

        return Task.FromResult(fileStates.AsEnumerable());
    }

    public Task RemoveFiles(IEnumerable<TrackedFile> filesToRemove)
    {
        foreach (var fileToRemove in filesToRemove)
        {
            FileStatesStorage.Remove(fileToRemove.Path, out _);
        }

        return Task.CompletedTask;
    }

    public Task<bool> IsTrackedFolder(string folderPath)
    {
        var isFolderTracked = FileStatesStorage.Keys.Any(key => key.StartsWith(Path.GetFullPath(folderPath), PathComparer.ComparisonType));
        return Task.FromResult(isFolderTracked);
    }

    public Task AddNewFiles(IEnumerable<TrackedFile> filesToAdd)
    {
        foreach (var fileToAdd in filesToAdd)
        {
            var fileState = new FileState { LastModified = fileToAdd.LastModified, Version = fileToAdd.Version };
            var isSuccessful = FileStatesStorage.TryAdd(fileToAdd.Path, fileState);

            if (isSuccessful is false)
            {
                _logger.LogError("Failed to add new file to the database. the file with the following path already exists. Path: {FilePath}", fileToAdd.Path);
            }
        }

        return Task.CompletedTask;
    }

    public Task UpdateFiles(IEnumerable<TrackedFile>filesToUpdate)
    {
        foreach (var fileToUpdate in filesToUpdate)
        {
            var fileState = new FileState { LastModified = fileToUpdate.LastModified, Version = fileToUpdate.Version };
            FileStatesStorage[fileToUpdate.Path] = fileState;
        }   
        
        return Task.CompletedTask;
    }
}