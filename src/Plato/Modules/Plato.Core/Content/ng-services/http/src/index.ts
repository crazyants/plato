
import { NgModule } from '@angular/core';
import { HttpModule, Headers, RequestOptions } from '@angular/http';

import { PlatoHttp } from "./PlatoHttp";
import { PlatoRequestOptions } from "./PlatoRequestOptions";

@NgModule({
    imports: [HttpModule],
    declarations: [],
    exports: [ ],
    providers: [
        { provide: RequestOptions, useClass: PlatoRequestOptions }
    ]
})

export class PlatoHttpModule { }