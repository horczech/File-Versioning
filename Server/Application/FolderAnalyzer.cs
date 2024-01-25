using FileVersioning.Server.Application.Services;
using FileVersioning.Server.Domain;
using FileVersioning.Server.Storage;
using MediatR;
using FileVersioning.Server.Application.Mappers;

namespace FileVersioning.Server.Application;

public class FolderAnalyzer : IRequest<FolderAnalysisResult>
{
    public string FolderPath { get; set; }
}

public class FolderAnalyzerHandler : IRequestHandler<FolderAnalyzer, FolderAnalysisResult>
{
    private readonly IFileStateStorage _fileStateStorage;
    private readonly ILogger<FolderAnalyzerHandler> _logger;

    public List<TrackedFile> FilesToAdd { get; set; } = [];
    public List<TrackedFile> FilesToUpdate { get; set; } = [];
    public List<TrackedFile> FilesToRemove { get; set; } = [];

    public FolderAnalyzerHandler(IFileStateStorage fileStateStorage, ILogger<FolderAnalyzerHandler> logger) {
        _fileStateStorage = fileStateStorage;
        _logger = logger;
    }

    public async Task<FolderAnalysisResult> Handle(FolderAnalyzer request, CancellationToken cancellationToken) {
        _logger.LogInformation("Analyzing folder path: {FolderPath}", request.FolderPath);
        var folderPath = request.FolderPath;

        if (!Directory.Exists(folderPath))
        {
            _logger.LogError("No folder found in the requested path {FolderPath}", request);
            throw new DirectoryNotFoundException();
        }

        var currentFiles = GetFileInfos(folderPath).ToList();
        var trackedFiles = (await _fileStateStorage.GetTrackedFiles(folderPath)).ToList(); //Note: only one access to storage

        // handle new untracked folder
        var isTrackedFolder = await _fileStateStorage.IsTrackedFolder(folderPath);
        if (isTrackedFolder is false)
        {
            var initializedFiles = InitializeFiles(currentFiles);
            _fileStateStorage.AddNewFiles(initializedFiles);
            
            return new FolderAnalysisResult {
                InitializedFiles = initializedFiles.Select(x => x.ToFileInfo()).ToList()
            };
        }
        
        //handle existing folder
        foreach (var currentFile in currentFiles) {
            var previousFileState = GetPreviousFileState(currentFile.FullName, trackedFiles);

            //file added
            if (previousFileState is null) {
                var addedFile = new TrackedFile(currentFile.FullName, currentFile.Name, currentFile.LastWriteTimeUtc, 1); //new files are initialized with version 1
                FilesToAdd.Add(addedFile);
                continue;
            }

            //file modified
            if (currentFile.LastWriteTimeUtc != previousFileState.LastModified)
            {
                var modifiedFile = new TrackedFile(currentFile.FullName, currentFile.Name, currentFile.LastWriteTimeUtc, previousFileState.Version + 1);
                FilesToUpdate.Add(modifiedFile);
                continue;
            }
        }

        //deleted files
        var deletedFiles = GetDeletedFiles(currentFiles, trackedFiles);
        foreach (var deletedFile in deletedFiles) {
            var removedFile = new TrackedFile(deletedFile.Path, deletedFile.Name, deletedFile.LastModified, deletedFile.Version);
            FilesToRemove.Add(removedFile);
        }
        
        //update database
        await Task.WhenAll(
            _fileStateStorage.AddNewFiles(FilesToAdd),
            _fileStateStorage.UpdateFiles(FilesToUpdate),
            _fileStateStorage.RemoveFiles(FilesToRemove)
        );
        
        return new FolderAnalysisResult
        {
            NewFiles = FilesToAdd.Select(x => x.ToFileInfo()).ToList(),
            ModifiedFiles = FilesToUpdate.Select(x => x.ToFileInfo()).ToList(),
            DeletedFiles = FilesToRemove.Select(x => x.ToFileInfo()).ToList(),
        };
    }

    private List<TrackedFile> InitializeFiles(List<System.IO.FileInfo> currentFiles)
    {
        _logger.LogInformation("Folder is untracked.");

        var initializedFiles = new List<TrackedFile>();
        foreach (var currentFile in currentFiles)
        {
            var untrackedFile = new TrackedFile(currentFile.FullName, currentFile.Name, currentFile.LastWriteTimeUtc, 1); //untracked files are initialized with version 1
            initializedFiles.Add(untrackedFile);
        }
        
        return initializedFiles;
    }

    private IEnumerable<TrackedFile> GetDeletedFiles(IEnumerable<System.IO.FileInfo> currentFiles, IEnumerable<TrackedFile> trackedFiles) => 
        trackedFiles.Where(trackedFile => currentFiles.All(currentFile => !PathComparer.Equals(currentFile.FullName, trackedFile.Path)));

    private IEnumerable<System.IO.FileInfo> GetFileInfos(string folderPath) {
        var directoryInfo = new DirectoryInfo(folderPath);
        var fileInfos = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories).AsEnumerable();

        return fileInfos;
    }

    private TrackedFile? GetPreviousFileState(string filePath, IEnumerable<TrackedFile> trackedFiles) => 
        trackedFiles.SingleOrDefault(trackedFile => PathComparer.Equals(filePath, trackedFile.Path));
}