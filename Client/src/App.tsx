import './App.css';
import logo from './assets/images/logo.png'
import FolderAnalyzer from './components/folder-analyzer/folder-analyzer.tsx'

function App() {
    return (
        <div className={"content"}>
            <img src={logo} className="logo" alt="logo"/>
            <FolderAnalyzer/>
        </div>
    );
}

export default App;