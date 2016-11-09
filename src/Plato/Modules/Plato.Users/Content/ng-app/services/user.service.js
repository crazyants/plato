"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var core_1 = require('@angular/core');
var http_1 = require('@angular/http');
var Observable_1 = require('rxjs/Observable');
require('rxjs/add/operator/map');
require('rxjs/add/operator/catch');
require('rxjs/add/observable/throw');
var UserService = (function () {
    function UserService(http) {
        this.http = http;
        console.log('User Service Ready.');
        this.http = http;
        this.userName = '';
    }
    UserService.prototype.get = function (page, pageSize) {
        var headers = new http_1.Headers();
        headers.append('Authorization', '123123123');
        return this.http.get('http://localhost:50439/api/users?page=' +
            page.toString() + '&pageSize=' + pageSize.toString(), {
            headers: headers
        })
            .map(function (res) {
            return res.json();
        })
            .catch(this.handleError);
    };
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
    UserService.prototype.handleError = function (error) {
        if (error.status === 401) {
            return Observable_1.Observable.throw(error.status);
        }
        else {
            return Observable_1.Observable.throw(error.status || 'Server error');
        }
    };
    UserService = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [http_1.Http])
    ], UserService);
    return UserService;
}());
exports.UserService = UserService;
//# sourceMappingURL=user.service.js.map