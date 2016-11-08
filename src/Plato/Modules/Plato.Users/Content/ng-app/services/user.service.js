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
var GithubService = (function () {
    function GithubService(http) {
        // private clientId: string = '<Client Id>';
        // private clientSecret: string = '<Client Secret Key>';
        this.clientId = '60b9f23dedffbdfc476c';
        this.clientSecret = 'd1c186c6373f96571c0bfcf76b84e4dc6fd0c15a';
        console.log('Github Service Ready.');
        this.userName = '';
        this.http = http;
    }
    GithubService.prototype.load = function () {
        console.log("came here in service");
        var headers = new http_1.Headers();
        headers.append('Authorization', '123123123');
        this.http.get('http://localhost:50439/api/users', {
            headers: headers
        })
            .map(function (res) { return console.log("Response came!!!"); });
        console.log("done . . .");
    };
    GithubService.prototype.getUser = function () {
        if (this.userName) {
            return this.http.get('http://api.github.com/users/' + this.userName
                + '?client_id=' + this.clientId
                + '&client_secret=' + this.clientSecret)
                .map(function (res) { return res.json(); })
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
    };
    GithubService.prototype.getRepos = function () {
        if (this.userName) {
            return this.http.get('http://api.github.com/users/' + this.userName
                + '/repos?client_id=' + this.clientId
                + '&client_secret=' + this.clientSecret)
                .map(function (res) { return res.json(); })
                .catch(this.handleError);
        }
    };
    GithubService.prototype.updateUser = function (userName) {
        this.userName = userName;
    };
    GithubService.prototype.handleError = function (error) {
        if (error.status === 401) {
            return Observable_1.Observable.throw(error.status);
        }
        else {
            return Observable_1.Observable.throw(error.status || 'Server error');
        }
    };
    GithubService = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [http_1.Http])
    ], GithubService);
    return GithubService;
}());
exports.GithubService = GithubService;
//# sourceMappingURL=user.service.js.map