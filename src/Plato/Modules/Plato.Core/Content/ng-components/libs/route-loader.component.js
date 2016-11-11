/// <reference path="../../../../../typings/index.d.ts" />
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
var route_loader_service_1 = require('./route-loader.service');
var RouteLoaderComponent = (function () {
    function RouteLoaderComponent(routeLoader) {
        var _this = this;
        routeLoader.status.subscribe(function (status) {
            _this.active = status;
        });
    }
    RouteLoaderComponent = __decorate([
        core_1.Component({
            selector: 'route-loader',
            template: "<div *ngIf=\"!RouteLoaderComponent.active\" id=\"container\">Nothing is Loading Now</div>"
        }), 
        __metadata('design:paramtypes', [route_loader_service_1.RouteLoaderService])
    ], RouteLoaderComponent);
    return RouteLoaderComponent;
}());
exports.RouteLoaderComponent = RouteLoaderComponent;
//# sourceMappingURL=route-loader.component.js.map