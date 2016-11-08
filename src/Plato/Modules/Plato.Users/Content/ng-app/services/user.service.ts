import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';


import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Injectable()
export class GithubService {

    private userName: string;

    // private clientId: string = '<Client Id>';
    // private clientSecret: string = '<Client Secret Key>';

    private clientId: string = '60b9f23dedffbdfc476c';
    private clientSecret: string = 'd1c186c6373f96571c0bfcf76b84e4dc6fd0c15a';
    private http: Http;

    constructor(http: Http) {
        console.log('Github Service Ready.');
        this.userName = '';
        this.http = http;
    }

    load() {

        console.log("came here in service");
        var headers = new Headers();
        headers.append('Authorization', '123123123');

        this.http.get('http://localhost:50439/api/users',
            {
                headers: headers
            })
            .map(res => console.log("Response came!!!"));

        console.log("done . . .");
    }

    getUser() {
        if (this.userName) {
            return this.http.get('http://api.github.com/users/' + this.userName
                + '?client_id=' + this.clientId
                + '&client_secret=' + this.clientSecret)
                .map(res => res.json())
                .catch(this.handleError);
        }
        // Bu şekilde de dönen değer üzerinden hatalar yakalanabilir. Ya da catch te....
        // .map(res => {
        //     console.log(res);
        //     if (res.status != 200) {
        //         throw new Error('This request has failed ' + res.status);
        //     }
        //     else {
        //         return res.json();
        //     }
        // })
    }

    getRepos() {
        if (this.userName) {
            return this.http.get('http://api.github.com/users/' + this.userName
                + '/repos?client_id=' + this.clientId
                + '&client_secret=' + this.clientSecret)
                .map(res => res.json())
                .catch(this.handleError);
        }

    }

    updateUser(userName: string) {
        this.userName = userName;
    }

    private handleError(error: any) {

        if (error.status === 401) {
            return Observable.throw(error.status);
        } else {
            return Observable.throw(error.status || 'Server error');
        }
    }
}