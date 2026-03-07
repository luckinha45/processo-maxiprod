import axios from 'axios';

const webapi = axios.create({
  baseURL: 'http://localhost:5248/api/', // Base URL for all requests
  timeout: 1000,                          // Request timeout
  headers: {
    'Content-Type': 'application/json'
  }
});

export default webapi;