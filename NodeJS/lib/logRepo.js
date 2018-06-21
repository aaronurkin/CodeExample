const JsonDB = require('node-json-db');

const DB_NAME = 'chatLog';
const db = new JsonDB(DB_NAME, true, false);

/**
 * returns an array of all logs saved in db
 */
function listLogs() {
    try {
        return db.getData("/chat/messages");
    } catch (error) {
        console.error(error);
        return undefined;
    }
}

/**
 * accepts a message object and saves it to db
 * @param message
 * return Boolean success
 */
function addLog(message) {
    try {
        db.push("/chat/messages[]", message);
        return true;
    } catch (error) {
        console.error(error);
        return false;
    }
}

module.exports = {
    addLog,
    listLogs
};
