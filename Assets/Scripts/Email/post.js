const axios = require('axios');

const data = {
    email: 'xaiverty@gmail.com',
    code: '123456'
};

axios.post('https://your-api-gateway-url/send-verification-code', data)
    .then(response => {
        console.log(response.data);
    })
    .catch(error => {
        console.error(error);
    });