import CourseForm from './components/CourseForm.jsx';
import './App.css';

function App() {
  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-2xl mx-auto">
        <h1 className="text-4xl font-bold text-center mb-12">Create New Course</h1>
        <CourseForm />
      </div>
    </div>
  );
}
export default App;
