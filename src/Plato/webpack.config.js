"use strict";

/* 
 * 
 * You must first install webpack and ts-loader...
 * 
 * npm install webpack -g
 * npm install ts-loader -g
 * 
 */

module.exports = {
    entry: "./plato.core.ts",
    output: {
        path: "./wwwroot/webpack",
        filename: "bundle.js"
    },
    module: {
        loaders: [
            {
                test: /\.ts?$/,
                loader: "ts-loader"
            }
        ]
    }
};