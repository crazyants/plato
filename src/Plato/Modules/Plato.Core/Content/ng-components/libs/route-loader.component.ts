/// <reference path="../../../../../typings/index.d.ts" />

import { Component } from '@angular/core';

import { RouteLoaderService } from './route-loader.service';

@Component({
    selector: 'route-loader',
    template: `<div *ngIf="!RouteLoaderComponent.active" id="container">Nothing is Loading Now</div>`

})
    
export class RouteLoaderComponent {

    public active: boolean;

    public constructor(routeLoader: RouteLoaderService) {
        routeLoader.status.subscribe((status: boolean) => {
            this.active = status;
        });
    }
}