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
var platform_browser_1 = require('@angular/platform-browser');
var http_1 = require('@angular/http');
var forms_1 = require('@angular/forms');
var common_1 = require('@angular/common');
var app_routes_1 = require('./app.routes');
// shared components
var pager_1 = require('../../../../../plato.core/content/ng-components/pager/pager');
// shared services
var http_2 = require('../../../../../plato.core/content/ng-services/http/http');
// components
var app_component_1 = require('./app.component');
var login_form_1 = require('../public/login-form/login-form');
var user_list_1 = require('../public/user-list/user-list');
// services
var user_service_1 = require('../../services/user.service');
var AppModule = (function () {
    function AppModule() {
    }
    AppModule = __decorate([
        core_1.NgModule({
            imports: [
                platform_browser_1.BrowserModule,
                http_1.HttpModule,
                forms_1.FormsModule,
                pager_1.PagerModule,
                app_routes_1.UsersRouterModule
            ],
            declarations: [
                app_component_1.AppComponent,
                user_list_1.UserListComponent,
                login_form_1.LoginFormComponent
            ],
            bootstrap: [app_component_1.AppComponent],
            providers: [
                user_service_1.UserService,
                { provide: http_1.RequestOptions, useClass: http_2.PlatoRequestOptions },
                { provide: common_1.LocationStrategy, useClass: common_1.HashLocationStrategy }
            ]
        }), 
        __metadata('design:paramtypes', [])
    ], AppModule);
    return AppModule;
}());
exports.AppModule = AppModule;
//# sourceMappingURL=app.module.js.map