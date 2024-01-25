export type FileInfo = {
    name: string;
    path: string;
    lastModified: string;
    version: number;
}

export type FolderAnalysisResult = {
    initializedFiles: FileInfo[];
    newFiles: FileInfo[];
    modifiedFiles: FileInfo[];
    deletedFiles: FileInfo[];
}