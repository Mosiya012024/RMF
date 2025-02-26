import axios from 'axios';
import store from '../redux/store/store';
import { setAPIResponseCode } from '../redux/actions/APIResponseCode';

// Create an axios instance
const axiosInstance = axios.create({
  //baseURL: 'https://localhost:44356',
  baseURL: 'https://localhost:7253',
});

// Request interceptor to add Authorization header
axiosInstance.interceptors.request.use(
  config => {
    const token = localStorage.getItem('userToken'); // Or wherever you store your token
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle errors globally
axiosInstance.interceptors.response.use(
  (response) => {return response},
  (error) => {
    console.log('Request failed:', error);
    if(error.response.status === 401) {
        store.dispatch(setAPIResponseCode(401));
    }
    
    throw new Error('I crashed'); // This will be caught by the Error Boundary
    //return Promise.reject(error); // Propagate the error
  }
);

export default axiosInstance;
