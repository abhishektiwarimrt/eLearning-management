import axios from 'axios';

const api = axios.create({
    baseURL: window.BASE_URL || 'https://localhost:7001',  // Your API
});

export const coursesApi = {
    create: (courseData) => api.post('/Courses', courseData),
    getAll: () => api.get('/Courses'),
};
