import { Injectable } from '@angular/core';
import { Http, HttpModule, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { IRequest } from './IRequest';

@Injectable()
export class PlatoHttp {

    constructor(private http: Http) {
        this.http = http;
    }

    get(request: IRequest): Observable<any> {
  
        const params = new URLSearchParams();
        params.set('page', request.params.page.toString()); 
        params.set('pageSize', request.params.pageSize.toString());
        params.set('sortBy', request.params.sortBy);
        params.set('sortOrder', (request.params.sortDesc ? "DESC" : ""));
        
        return this.http.get(request.url, { search: params })
            .map(res => {
                return res.json();
            })
            .catch(this.handleError);

    }
    
    private handleError(error: any) {

        if (error.status === 401) {
            return Observable.throw(error.status);
        } else {
            return Observable.throw(error.status || 'Server error');
        }
    }


}