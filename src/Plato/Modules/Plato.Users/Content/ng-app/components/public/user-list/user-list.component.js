/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />
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
// angular
var core_1 = require('@angular/core');
require("rxjs/add/operator/map");
var router_1 = require('@angular/router');
// module specific 
var models = require("../../../models/User");
var user_service_1 = require("../../../services/user.service");
var UserListComponent = (function () {
    function UserListComponent(route, router, userService) {
        this.route = route;
        this.router = router;
        this.userService = userService;
        this.userUpdated = new core_1.EventEmitter();
        this._pageIndex = 1;
        this._pageSize = 10;
        this._sortBy = "Title";
        this._sortDesc = false;
    }
    Object.defineProperty(UserListComponent.prototype, "pageIndex", {
        get: function () {
            return this._pageIndex;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(UserListComponent.prototype, "pageSize", {
        get: function () {
            return this._pageSize;
        },
        enumerable: true,
        configurable: true
    });
    UserListComponent.prototype.pagerClick = function (data) {
        this._pageIndex = data.pageIndex;
        //this.router.navigate(['Users', data.pageIndex]);
        this.refreshData();
    };
    UserListComponent.prototype.ngOnInit = function () {
        var _this = this;
        this.route.params.forEach(function (params) {
            // (+) converts string 'id' to a number
            _this._pageIndex = (params['page'] ? +params['page'] : _this._pageIndex);
            _this._pageSize = (params['pageSize'] ? +params['pageSize'] : _this._pageSize);
            if (_this._pageIndex && _this._pageSize)
                _this.refreshData();
        });
    };
    UserListComponent.prototype.sortBy = function (col) {
        this._sortDesc = col === this._sortBy ? !this._sortDesc : false;
        this._sortBy = col;
        this.refreshData();
    };
    UserListComponent.prototype.refreshData = function () {
        var _this = this;
        this.userService.get(this._pageIndex, this.pageSize, "Id", this._sortDesc)
            .subscribe(function (result) {
            _this.viewModel = result;
            _this.userUpdated.emit(_this.viewModel);
            _this.totalCount = result.users.total;
            _this.totalPages = Math.ceil(_this.totalCount / _this.pageSize);
        }, function (err) {
            console.log('err:' + err);
            _this.viewModel = null;
        }, function () { return console.log('Done'); });
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', models.UserListViewModel)
    ], UserListComponent.prototype, "viewModel", void 0);
    __decorate([
        core_1.Output(), 
        __metadata('design:type', Object)
    ], UserListComponent.prototype, "userUpdated", void 0);
    UserListComponent = __decorate([
        core_1.Component({
            selector: 'user-list',
            templateUrl: './plato.users/content/ng-app/components/public/user-list/user-list.html'
        }), 
        __metadata('design:paramtypes', [router_1.ActivatedRoute, router_1.Router, user_service_1.UserService])
    ], UserListComponent);
    return UserListComponent;
}());
exports.UserListComponent = UserListComponent;
//# sourceMappingURL=user-list.component.js.map