import { useForm } from 'react-hook-form';
import { coursesApi } from './api';
import { useState } from 'react';

export default function CourseForm() {
  const { register, handleSubmit, reset, formState: { errors } } = useForm();
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const onSubmit = async (data) => {
    setLoading(true);
    try {
      await coursesApi.create(data);
      setMessage('Course created successfully!');
      reset();
    } catch (err) {
      setMessage('Error: ' + (err.response?.data || err.message));
    }
    setLoading(false);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="max-w-md mx-auto p-6 bg-white shadow-md">
      <h2 className="text-2xl font-bold mb-6">Create Course</h2>
      
      <div className="mb-4">
        <label className="block text-sm font-medium mb-2">Course Name</label>
        <input
          {...register('name', { required: 'Name is required' })}
          className="w-full p-3 border rounded-lg focus:ring-2 focus:ring-blue-500"
        />
        {errors.name && <p className="text-red-500 text-sm mt-1">{errors.name.message}</p>}
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium mb-2">Description</label>
        <textarea
          {...register('description')}
          rows="4"
          className="w-full p-3 border rounded-lg focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {/* Add more fields from Swagger JSON schema */}
      <div className="mb-6">
        <label className="block text-sm font-medium mb-2">Price</label>
        <input
          {...register('price', { required: true, min: 0 })}
          type="number"
          step="0.01"
          className="w-full p-3 border rounded-lg focus:ring-2 focus:ring-blue-500"
        />
      </div>

      <button
        type="submit"
        disabled={loading}
        className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg hover:bg-blue-700 disabled:opacity-50"
      >
        {loading ? 'Creating...' : 'Create Course'}
      </button>

      {message && (
        <div className="mt-4 p-3 bg-green-100 border border-green-400 text-green-700 rounded">
          {message}
        </div>
      )}
    </form>
  );
}
