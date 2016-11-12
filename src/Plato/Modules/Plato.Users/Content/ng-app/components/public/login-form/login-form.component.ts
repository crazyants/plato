/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import "rxjs/add/operator/map";

import * as models from "../../../models/User";
import { UserService } from "../../../services/user.service";

@Component({
    selector: 'login-form',
    templateUrl: './plato.users/content/ng-app/components/public/login-form/login-form.html'
})

export class LoginFormComponent {

    constructor(private userService: UserService) {

     
    }
    
 



}