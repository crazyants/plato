"use strict";

module.exports = {
    entry: "./modules/plato.users/content/ng-app/components/app/main",
    output: {
        path: "./wwwroot/dist/plato.users/",
        filename: "[name].bundle.js"
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