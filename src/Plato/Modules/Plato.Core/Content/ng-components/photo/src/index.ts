
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PhotoComponent } from "./photo"

@NgModule({
    // import the angular 2 common module for base directives (ngIf, ngClass etc)
    imports: [CommonModule], 
    // register our components
    declarations: [PhotoComponent],
    // what we want to export externally
    exports: [PhotoComponent],
    // services for our components
    providers: []
})
export class PhotoModule { }