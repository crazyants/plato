import { NgModule, Injectable } from '@angular/core';
import { HttpModule, Headers, RequestOptions } from '@angular/http';

// extend http request options
@Injectable()
export class PlatoRequestOptions extends RequestOptions {
    headers: Headers = new Headers({
        'Auth': 'SHARED'
    });
}
