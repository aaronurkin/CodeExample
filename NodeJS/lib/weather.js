const rp = require('request-promise');

const WEATHER_ENDPOINT = 'http://samples.openweathermap.org/data/2.5/weather?q=London,uk&appid=b6907d289e10d714a6e88b30761fae22';

/**
 * Sends a request to
 * @returns {Promise<Object>}
 */
async function getWeather() {
	return rp({ uri: WEATHER_ENDPOINT, json: true });
}

module.exports = {
	getWeather,
};
