import React, {useState} from 'react';
import axios, {AxiosError} from 'axios';
import './folder-analysis.css';
import { FolderAnalysisResult, FileInfo } from '../../models/types.ts';

const FolderAnalyzer: React.FC = () => {
    const [folderPath, setFolderPath] = useState<string>('');
    const [showErrorMessage, setShowErrorMessage] = useState<boolean>(false);
    const [analysisResult, setAnalysisResult] = useState<FolderAnalysisResult | null>(null);
    const [loading, setLoading] = useState<boolean>(false);
    const [fetchError, setFetchError] = useState<string | null>(null);
    const handleAnalyzeClick = async () => {
        setLoading(true);
        setAnalysisResult(null);
        setFetchError(null);

        if (!folderPath){
            setShowErrorMessage(true);
            setLoading(false);
            return
        }else{
            setShowErrorMessage(false);
        }
        
        try {
            const response = await axios.post<FolderAnalysisResult>('http://localhost:5085/api/file-analyzer/analyze', { folderPath: folderPath });
            setAnalysisResult(response.data);
        } catch (error: any | AxiosError) {
            setFetchError(error.response ? error.response.data : error.message);
            console.log(`Failed to load files. Error message: ${error.message}.`)
        } finally {
            setLoading(false);
        }
    };

    const renderFileList = (files: FileInfo[]) => {
        if (files.length === 0) {
            return <p className={"no_changes"}>no changes</p>;
        }

        return (
            <ul>
                {files.map(file => (
                    <li key={file.path}>
                        <p className={"file_name"}>{file.name}</p>
                        {file.version && (<p className={"version"}>(version: {file.version})</p>)}
                    </li>
                ))}
            </ul>
        );
    };

    return (
        <div className={"folder-analyzer"}>
            <h1 className={"title"}>Folder Analyzer</h1>

            <div className={"path-input"}>
                <input
                    type='text'
                    value={folderPath}
                    onChange={(e) => setFolderPath(e.target.value)}
                    placeholder='Enter a folder path'
                    className={"path-input-textbox"}
                />
                {showErrorMessage && (<p className={"error"}>Error: Folder path must be filled.</p>)}
            </div>
            <button type="button" onClick={handleAnalyzeClick} disabled={loading}
                    className={"analyze-btn"}>
                Analyze
            </button>

            <div>
                {loading && <p>Loading data...</p>}
                {fetchError && <p className={"fetch_error"}>Failed to load the files from the provided folder.</p>}
            </div>
            
            {analysisResult && (
                <div className={"folder-analyzer-results"}>
                    <h3 className={"title"}>Analysis Results</h3>

                    {analysisResult.initializedFiles.length > 0 && (
                        <div className={"item"}>
                            <h4>Current files in folder:</h4>
                            {renderFileList(analysisResult.initializedFiles)}
                        </div>
                    )}


                    <div className={"item"}>
                        <h4>New Files:</h4>
                        {renderFileList(analysisResult.newFiles)}
                    </div>

                    <div className={"item"}>
                        <h4>Modified Files:</h4>
                        {renderFileList(analysisResult.modifiedFiles)}
                    </div>

                    <div className={"item"}>
                        <h4>Deleted Files:</h4>
                        {renderFileList(analysisResult.deletedFiles)}
                    </div>
                </div>
            )}
        </div>
    )
}

export default FolderAnalyzer;

