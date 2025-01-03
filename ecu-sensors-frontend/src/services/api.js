import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5000/api',
});

export const getSensors = async (filters) => {
    const response = await api.get('/sensors', { params: filters });
    return response.data;
};