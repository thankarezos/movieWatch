// src/services/apiService.js
import axios from 'axios';

const API_BASE_URL = (import.meta.env.MODE === 'production' ? "/api" : 'https://moviewatch.thankarezos.com/api');

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    if (token) {
      config.headers.Authorization = token; // No 'Bearer' prefix
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
        console.log(error.response);
        
        if (error.response.status === 401) {
            console.error('Unauthorized - logging out');
            localStorage.removeItem('token');
            sessionStorage.removeItem('token');
            // window.location.href = '/';
        }
        console.error('API Error:', error.response.data);
      
    } else if (error.request) {
      console.error('Network Error:', error.request);
    } else {
      console.error('Error:', error.message);
    }
    return Promise.reject(error);
  }
);

const apiService = {
  get(endpoint, params) {
    return api.get(endpoint, { params });
  },
  post(endpoint, data) {
    return api.post(endpoint, data);
  },
  async login(username, password, rememberMe) {
    const response = await api.post('/User/login', { username, password });
    if (response.data.success) {
        const token = response.data.data.token;
        if (rememberMe) {
            localStorage.setItem('token', token);
            localStorage.setItem('username', response.data.data.user.username);
        } else {
            sessionStorage.setItem('token', token);
            sessionStorage.setItem('username', response.data.data.user.username);
        }
        
    }
    return response;
  },
  async register(username, email, password, confirmPassword) {
    const response = await api.post('/User/register', { username, email, password, confirmPassword });
    if (response.data.success) {
        sessionStorage.setItem('token', response.data.data.token);
        sessionStorage.setItem('username', response.data.data.user.username);
    }
    return response;
  },
  logout() {
    localStorage.removeItem('token');
    sessionStorage.removeItem('token');
  }

};

export default apiService;
