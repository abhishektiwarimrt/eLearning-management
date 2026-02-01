import CourseForm from './components/CourseForm';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

function App() {
    return (
        <Router>
            <div className="min-h-screen bg-gray-100 py-8">
                <div className="container mx-auto px-4">
                    <Routes>
                        <Route path="/" element={<CourseForm />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
}

export default App;
