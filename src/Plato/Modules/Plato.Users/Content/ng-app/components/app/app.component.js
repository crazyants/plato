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
var router_1 = require('@angular/router');
var AppComponent = (function () {
    function AppComponent(router) {
        var _this = this;
        this.router = router;
        this.loading = true;
        router.events.subscribe(function (event) {
            if (event instanceof router_1.NavigationStart)
                _this.loading = true;
            if (event instanceof router_1.NavigationEnd) {
                window.setTimeout(function () {
                    _this.loading = false;
                }, 100);
            }
            if (event instanceof router_1.NavigationCancel)
                _this.loading = false;
            if (event instanceof router_1.NavigationError)
                _this.loading = false;
        });
    }
    AppComponent.prototype.getRouteLoaderDisplayState = function () {
        if (this.loading)
            return "block";
        else
            return "none";
    };
    AppComponent.prototype.getRouteOutletDisplayState = function () {
        if (this.loading)
            return "none";
        else
            return "block";
    };
    AppComponent = __decorate([
        core_1.Component({
            selector: 'my-app',
            templateUrl: './plato.users/content/ng-app/components/app/app.html'
        }), 
        __metadata('design:paramtypes', [router_1.Router])
    ], AppComponent);
    return AppComponent;
}());
exports.AppComponent = AppComponent;
//# sourceMappingURL=app.component.js.map