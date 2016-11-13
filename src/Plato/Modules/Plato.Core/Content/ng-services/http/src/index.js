/*
   Component to provide paging to various lists within Plato.
*/
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
var common_1 = require('@angular/common');
var PlatoRequestOptions_1 = require("./PlatoRequestOptions");
var PlatoHttpModule = (function () {
    function PlatoHttpModule() {
    }
    PlatoHttpModule = __decorate([
        core_1.NgModule({
            // import the angular 2 common module for base directives (ngIf, ngClass etc)
            imports: [common_1.CommonModule],
            // register our components
            declarations: [PlatoRequestOptions_1.PlatoRequestOptions],
            // ensure we can export our components
            exports: [PlatoRequestOptions_1.PlatoRequestOptions],
            // services for our components
            providers: []
        }), 
        __metadata('design:paramtypes', [])
    ], PlatoHttpModule);
    return PlatoHttpModule;
}());
exports.PlatoHttpModule = PlatoHttpModule;
//# sourceMappingURL=index.js.map