import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { PlatoHttp } from "../ng-services/http/http";
import { IUser } from "../interfaces/IUser";

@Injectable()
export class UsersService {
    
    private Url = "api/users";

    constructor(private platoHttp: PlatoHttp) {
  
    }

    get(
        page: number,
        pageSize: number,
        sortBy: string,
        sortDesc: boolean
    ) {

        return this.platoHttp.get({
            url: this.Url,
            params: {
                page: page,
                pageSize: pageSize,
                sortBy: sortBy,
                sortDesc: sortDesc
            },
            data: {}
        });
        
    }




}