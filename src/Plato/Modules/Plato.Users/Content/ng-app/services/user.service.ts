import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { PlatoHttp } from '../../../../plato.core/content/ng-services/http/http';


@Injectable()
export class UserService {


    private Url = "api/users";

    constructor(
        private http: Http,
        private platoHttp: PlatoHttp
    ) {
        console.log('User Service Ready.');
        this.http = http;
    }
    
    get(
        page: number,
        pageSize: number,
        sortBy: string,
        sortDesc: boolean
    ) {

        return this.platoHttp.get({
            url: this.Url,
            params: {
                page: page,
                pageSize: pageSize,
                sortBy: sortBy,
                sortDesc: sortDesc
            },
            data: {}
        });
        
        //return this.http.get('/api/users?' +
        //    'page=' + page.toString() +
        //    '&pageSize=' + pageSize.toString() +
        //    '&sortBy=' + sortBy +
        //    '&sortOrder=' + (sortDesc ? " DESC" : ""))
        //    .map(res => {
        //        return res.json();
        //    })
        //    .catch(this.handleError);
        
    }

    //getUser() {
    //    if (this.userName) {
    //        return this.http.get('http://api.github.com/users/' + this.userName
    //            + '?client_id=' + this.clientId
    //            + '&client_secret=' + this.clientSecret)
    //            .map(res => res.json())
    //            .catch(this.handleError);
    //    }
    //    // Bu şekilde de dönen değer üzerinden hatalar yakalanabilir. Ya da catch te....
    //    // .map(res => {
    //    //     console.log(res);
    //    //     if (res.status != 200) {
    //    //         throw new Error('This request has failed ' + res.status);
    //    //     }
    //    //     else {
    //    //         return res.json();
    //    //     }
    //    // })
    //}

    //getRepos() {
    //    if (this.userName) {
    //        return this.http.get('http://api.github.com/users/' + this.userName
    //            + '/repos?client_id=' + this.clientId
    //            + '&client_secret=' + this.clientSecret)
    //            .map(res => res.json())
    //            .catch(this.handleError);
    //    }

    //}

    //updateUser(userName: string) {
    //    this.userName = userName;
    //}

    private handleError(error: any) {

        if (error.status === 401) {
            return Observable.throw(error.status);
        } else {
            return Observable.throw(error.status || 'Server error');
        }
    }
}