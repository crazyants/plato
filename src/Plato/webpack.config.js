
/* 
 * 
 * You must first install webpack and ts-loader...
 * 
 * npm install webpack -g
 * npm install ts-loader -g
 * 
 */
 
var environment = (process.env.NODE_ENV || "development").trim();

if (environment === "development") {
    module.exports = require('./webpack.dev.js');
} else {
    module.exports = require('./webpack.prod.js');
}