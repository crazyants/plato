import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

@Injectable()
export class UserService {

    private userName: string;
    
    constructor(private http: Http) {
        console.log('User Service Ready.');
        this.http = http;
        this.userName = '';
    }


    get(page: number, pageSize: number) {
        
        const headers = new Headers();
        headers.append('Authorization', '123123123');
        
        return this.http.get('http://localhost:50439/api/users?page=' +
            page.toString() + '&pageSize=' + pageSize.toString(),
            {
                headers: headers
            })
            .map(res => {
                return res.json();
            })
            .catch(this.handleError);
        
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