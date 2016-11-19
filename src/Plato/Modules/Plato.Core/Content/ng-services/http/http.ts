/// <reference path="../../../../../typings/index.d.ts" />

import { PlatoHttp } from './src/PlatoHttp';
import { PlatoRequestOptions } from './src/PlatoRequestOptions';
import { IRequest } from "./src/IRequest"

export * from './src/IRequest';
export * from './src/PlatoHttp';
export * from './src/PlatoRequestOptions';
export * from './src/index';

export default {
    providers: [
        PlatoHttp
    ]
}

