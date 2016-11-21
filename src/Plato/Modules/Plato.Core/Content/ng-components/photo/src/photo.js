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
var PhotoComponent = (function () {
    function PhotoComponent(element, zone) {
        this.element = element;
        this.zone = zone;
    }
    PhotoComponent.prototype.ngOnInit = function () {
        switch (this.size) {
            case 1:
                this.imageClass = "p-photo p-photo-1x";
                break;
            case 2:
                this.imageClass = "p-photo p-photo-2x";
                break;
        }
        this.initials = "RH";
    };
    PhotoComponent.prototype.ngOnDestroy = function () {
    };
    PhotoComponent.prototype.ngOnChanges = function (changes) {
        if (this.user)
            this.init();
    };
    PhotoComponent.prototype.init = function () {
        if (this.user) {
            this.imageUrl = 'users/photo/serve?id=' + this.user.id.toString();
        }
    };
    __decorate([
        core_1.Input(), 
        __metadata('design:type', Number)
    ], PhotoComponent.prototype, "size", void 0);
    __decorate([
        core_1.Input(), 
        __metadata('design:type', Object)
    ], PhotoComponent.prototype, "user", void 0);
    PhotoComponent = __decorate([
        core_1.Component({
            selector: 'photo',
            templateUrl: './plato.core/content/ng-components/photo/src/photo.html'
        }), 
        __metadata('design:paramtypes', [core_1.ElementRef, core_1.NgZone])
    ], PhotoComponent);
    return PhotoComponent;
}());
exports.PhotoComponent = PhotoComponent;
//# sourceMappingURL=photo.js.map