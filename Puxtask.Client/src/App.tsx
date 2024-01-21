import './App.css';
import puxLogo from './assets/images/pux-logo.png'
import FolderAnalyzer from './components/folder-analyzer/folder-analyzer.tsx'

function App() {
    return (
        <div className={"content"}>
            <img src={puxLogo} className="logo" alt="Vite logo"/>
            <FolderAnalyzer/>
        </div>
    );
}

export default App;