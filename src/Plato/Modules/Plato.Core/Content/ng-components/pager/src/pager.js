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
var PagerComponent = (function () {
    function PagerComponent(element, zone) {
        this.element = element;
        this.zone = zone;
        this._totalPages = 0;
        this._pageIndex = 1;
        this.pageClick = new core_1.EventEmitter();
    }
    Object.defineProperty(PagerComponent.prototype, "pageIndex", {
        get: function () {
            return this._pageIndex;
        },
        enumerable: true,
        configurable: true
    });
    Object.defineProperty(PagerComponent.prototype, "totalPages", {
        get: function () {
            return this._totalPages;
        },
        enumerable: true,
        configurable: true
    });
    PagerComponent.prototype.ngOnInit = function () {
        if (this.totalPages)
            this.init();
    };
    PagerComponent.prototype.ngOnDestroy = function () {
    };
    PagerComponent.prototype.ngOnChanges = function (changes) {
        if (this.totalPages)
            this.init();
    };
    PagerComponent.prototype.init = function () {
        this.buildPagingButtonts();
        this.canGoBack = this.pageLinks.length && !this.pageLinks[0].isCurrent;
        this.canGoForward = this.pageLinks.length && !this.pageLinks[this.pageLinks.length - 1].isCurrent;
    };
    PagerComponent.prototype.buildPagingButtonts = function () {
        var pagesToShow = 5;
        if (this.totalPages <= pagesToShow) {
            this.buildButtonRange(1, this.totalPages);
        }
        else {
            var firstPage = (this.pageIndex - 2);
            var lastPage = (this.pageIndex + 2);
            if (firstPage < 0) {
                firstPage = 1;
            }
            if (this.pageIndex === 1) {
                this.buildButtonRange(1, pagesToShow);
            }
            else if (this.pageIndex === 2) {
                this.buildButtonRange(1, pagesToShow);
            }
            else if (this.pageIndex === 3) {
                this.buildButtonRange(1, pagesToShow);
            }
            else if (this.pageIndex < this.totalPages - 2) {
                this.buildButtonRange(firstPage, lastPage);
            }
            else if (this.pageIndex === this.totalPages - 2) {
                this.buildButtonRange(this.totalPages - (pagesToShow - 1), this.pageIndex + 2);
            }
            else if (this.pageIndex === this.totalPages - 1) {
                this.buildButtonRange(this.totalPages - (pagesToShow - 1), this.pageIndex + 1);
            }
            else if (this.pageIndex === this.totalPages) {
                this.buildButtonRange(this.totalPages - (pagesToShow - 1), this.pageIndex);
            }
        }
    };
    PagerComponent.prototype.buildButtonRange = function (start, end) {
        this.pageLinks = [];
        for (var i = start; i <= end; i++) {
            this.pageLinks.push({
                index: i,
                text: i.toString(),
                isCurrent: i === this._pageIndex
            });
        }
    };
    PagerComponent.prototype.goToPage = function (data) {
        var _this = this;
        if (data === void 0) { data = { pageIndex: 1 }; }
        this.zone.run(function () { return _this.pageClick.next(data); });
    };
    PagerComponent.prototype.goToLast = function () {
        var _this = this;
        this.zone.run(function () { return _this.pageClick.next({ pageIndex: _this.totalPages }); });
    };
    __decorate([
        core_1.Input('totalPages'), 
        __metadata('design:type', Number)
    ], PagerComponent.prototype, "_totalPages", void 0);
    __decorate([
        core_1.Input('pageIndex'), 
        __metadata('design:type', Number)
    ], PagerComponent.prototype, "_pageIndex", void 0);
    __decorate([
        core_1.Output(), 
        __metadata('design:type', Object)
    ], PagerComponent.prototype, "pageClick", void 0);
    PagerComponent = __decorate([
        core_1.Component({
            selector: 'pager',
            templateUrl: './plato.core/content/ng-components/pager/src/pager.html'
        }), 
        __metadata('design:paramtypes', [core_1.ElementRef, core_1.NgZone])
    ], PagerComponent);
    return PagerComponent;
}());
exports.PagerComponent = PagerComponent;
;
//# sourceMappingURL=pager.js.map