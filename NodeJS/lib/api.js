const repo = require('./logRepo');
const express = require('express');
const weather = require('./weather');

const API_NAME = 'Chat logger API';
const API_VERSION = '0.1.0';

const api = express.Router();

// Promise with error handler
const getWeather = () => {
	return weather
		.getWeather()
		.catch((error) => console.error(error));
}

// Retrieve weather info and extende messages list with it, if succeeded.
// Otherwise send messages only to the client.
const extendAndSendMessages = async (messages, res) => {
	if (!messages) {
		res.status(404);
		return res.send();
	}

	var weatherInfo = await getWeather();

	if (weatherInfo) {
		messages.push(weatherInfo);
	}

	res.json(messages);
}

/**
 * API Info
 */
api.get('/', (req, res) => {
	res.json({ API_NAME, API_VERSION });
});

/**
 * return a list of all messages
 */
api.get('/list', async (req, res) => {
	await extendAndSendMessages(repo.listLogs(), res);
});

/**
 * returns a list of messages filtered by name (origin of the message)
 */
api.get('/list/:name', async (req, res) => {
	var messages = repo.listLogs()

	if (messages) {
		messages = messages.filter((message) => new RegExp(message.sender, 'i').test(req.params.name));
	}

	await extendAndSendMessages(messages, res);
});

/**
 * inserts a message into the db and returns a boolean for success/failure
 */
api.post('/log', (req, res) => res.send(repo.addLog(req.body)));

module.exports = {
	api,
};
