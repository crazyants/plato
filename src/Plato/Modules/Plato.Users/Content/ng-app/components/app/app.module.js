"use strict";
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
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
var platform_browser_1 = require('@angular/platform-browser');
var http_1 = require('@angular/http');
var forms_1 = require('@angular/forms');
var router_1 = require('@angular/router');
var common_1 = require('@angular/common');
// components
var app_component_1 = require('./app.component');
var login_form_component_1 = require('../public/login-form/login-form.component');
var user_list_component_1 = require('../public/user-list/user-list.component');
// services
var user_service_1 = require('../../services/user.service');
// extend http request options
var DefaultRequestOptions = (function (_super) {
    __extends(DefaultRequestOptions, _super);
    function DefaultRequestOptions() {
        _super.apply(this, arguments);
        this.headers = new http_1.Headers({
            'Auth': '1234567890'
        });
    }
    DefaultRequestOptions = __decorate([
        core_1.Injectable(), 
        __metadata('design:paramtypes', [])
    ], DefaultRequestOptions);
    return DefaultRequestOptions;
}(http_1.RequestOptions));
exports.DefaultRequestOptions = DefaultRequestOptions;
var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                http_1.HttpModule,
                forms_1.FormsModule,
                router_1.RouterModule.forRoot([
                    { path: '', component: user_list_component_1.UserListComponent },
                    { path: 'users', component: user_list_component_1.UserListComponent },
                    { path: 'login', component: login_form_component_1.LoginFormComponent }
                ])
            ],
            declarations: [
                app_component_1.AppComponent,
                user_list_component_1.UserListComponent,
                login_form_component_1.LoginFormComponent
            ],
            bootstrap: [app_component_1.AppComponent],
            providers: [
                user_service_1.UserService,
                { provide: http_1.RequestOptions, useClass: DefaultRequestOptions },
                { provide: common_1.LocationStrategy, useClass: common_1.HashLocationStrategy }
            ]
        }), 
        __metadata('design:paramtypes', [])
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map