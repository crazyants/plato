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
/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />
var core_1 = require('@angular/core');
require('rxjs/add/operator/map');
var User_1 = require('../../../models/User');
var user_service_1 = require('../../../services/user.service');
var UserListComponent = (function () {
    function UserListComponent(userService) {
        this.userService = userService;
        this.userUpdated = new core_1.EventEmitter();
        this.userService = userService;
    }
    UserListComponent.prototype.ngOnInit = function () {
        if (this.user) {
            this.user.user = false;
            this.init();
        }
    };
    UserListComponent.prototype.init = function () {
        var _this = this;
        this.userService.get()
            .subscribe(function (user) {
            _this.user.user = user;
            _this.userUpdated.emit(_this.user);
        }, function (err) {
            console.log('err:' + err);
            _this.user.user = false;
        }, function () { return console.log('Done'); });
        //this._githubService.getRepos().subscribe(repos => {
        //    // console.log(repos);
        //    this.user.repos = repos;
        //    this.userUpdated.emit(this.user);
        //},
        //    (err) => {
        //        console.log('err:' + err);
        //        this.user.user = false;
        //    },
        //    () => console.log('Done')
        //);
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', User_1.User)
    ], UserListComponent.prototype, "user", void 0);
    __decorate([
        core_1.Output(), 
        __metadata('design:type', Object)
    ], UserListComponent.prototype, "userUpdated", void 0);
    UserListComponent = __decorate([
        core_1.Component({
            selector: 'user-list',
            templateUrl: './plato.users/ng-app/components/public/user-list/user-list.html'
        }), 
        __metadata('design:paramtypes', [user_service_1.UserService])
    ], UserListComponent);
    return UserListComponent;
}());
exports.UserListComponent = UserListComponent;
//# sourceMappingURL=user-list.js.map