import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {PagerComponent } from "./pager"

@NgModule({
    // import the angular 2 common module for base directives
    imports: [CommonModule], 
    // register our components
    declarations: [PagerComponent],
    // ensure we can export our components
    exports: [PagerComponent],
    // services for our components
    providers: []
})
export class PagerModule { }