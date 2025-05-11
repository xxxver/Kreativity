const express = require('express');
const bodyParser = require('body-parser');
const nodemailer = require('nodemailer');
const app = express();

// Middleware для парсинга JSON и x-www-form-urlencoded
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// Настройка транспорта для отправки почты через Mail.ru
const transporter = nodemailer.createTransport({
    host: 'smtp.mail.ru',
    port: 25, // Используем порт 25 для SMTP без шифрования
    secure: false, // false для отключения TLS
    auth: {
        user: 'kreatyvicompany@mail.ru',
        pass: 'ALGkiRb4kHFqtgCreuBQ' // Используйте пароль приложения
    },
    tls: {
        // Полностью отключаем проверку сертификатов
        rejectUnauthorized: false
    }
});

// Middleware для логирования входящих запросов
app.use((req, res, next) => {
    console.log('Headers:', req.headers);
    console.log('Body:', req.body);
    next();
});

// Обработчик POST-запроса для отправки кода подтверждения
app.post('/send-verification-code', async (req, res) => {
    // Проверяем, что req.body содержит данные
    if (!req.body || Object.keys(req.body).length === 0) {
        return res.status(400).send('Request body is missing');
    }

    const { email, code } = req.body;

    // Проверяем, что email и code присутствуют в запросе
    if (!email || !code) {
        return res.status(400).send('Email and code are required');
    }

    console.log('Received email:', email);
    console.log('Received code:', code);

    try {
        // Настройка параметров письма
        const mailOptions = {
            from: 'kreatyvicompany@mail.ru', // Отправитель
            to: email, // Получатель
            subject: 'Верификация', // Тема письма
            text: `Код верификации: ${code}` // Текст письма
        };

        // Отправка письма
        transporter.sendMail(mailOptions, (error, info) => {
            if (error) {
                console.error('Error sending email:', error);
                return res.status(500).send(error.toString());
            }
            console.log('Email sent:', info.response);
            res.status(200).send('Verification code sent: ' + info.response);
        });
    } catch (error) {
        console.error('Error:', error);
        return res.status(500).send(error.toString());
    }
});

// Запуск сервера на порту 3000
app.listen(3000, () => {
    console.log('Server is running on port 3000');
});
