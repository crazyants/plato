import { NgModule, Injectable } from '@angular/core';
import { Headers, RequestOptions } from '@angular/http';

// extend http request options
@Injectable()
export class PlatoRequestOptions extends RequestOptions {
    headers: Headers = new Headers({
        'Auth': 'SHAREAPIKEY'
    });
}
