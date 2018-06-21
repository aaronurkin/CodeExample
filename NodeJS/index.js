const express = require('express');
const bodyParser = require('body-parser');
const { api } = require('./lib/api');

const app = express();

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));
app.use(express.static(`${__dirname}/public`));

app.use('/api', api);

/**
 * root route: renders the client (views/index)
 */
app.get('/', (req, res) => {
	res.render('index');
});

app.use((error, req, res, next) => {
	if (error) {
		console.error(error);
	}

	res.status(500);
	res.send('Something went wrong. Please, call 911');
});

app.listen(3000, () => console.log('[system]', 'the server is running...'));
