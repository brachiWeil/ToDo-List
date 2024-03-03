import axios from 'axios';


 const apiUrl ="http://localhost:5116";

// const apiUrl = process.env.REACT_APP_DEFAULT_API_URL;

axios.interceptors.response.use(
  function (response) {
    return response;
  },
  function (error) {
    console.error('Request failed:', error.message);
    return Promise.reject(error);
    
  }
);
export default {
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}/todos`)    
    return result.data;
  },

  addTask: async(name)=>{
    await axios.post(`${apiUrl}/todo`, { name: name, isComplete: false });
    console.log('addTask', name)
  },

  setCompleted: async(id, isComplete)=>{
    var i = await axios.get(`${apiUrl}/todo/${id}`);
    await axios.put(`${apiUrl}/todos/${id}`, { name: i.data.name, isComplete: isComplete });
  },

  deleteTask:async(id)=>{
    await axios.delete(`${apiUrl}/todo/${id}`);
  }
};
