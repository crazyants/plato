/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import 'rxjs/add/operator/map';

import * as models from '../../../models/User';
import { UserService } from '../../../services/user.service';

@Component({
    selector: 'user-list',
    templateUrl: './plato.users/ng-app/components/public/user-list/user-list.html?123123=123123'
})

export class UserListComponent {

    @Input() viewModel: models.UserListViewModel;
    //@Output() userUpdated = new EventEmitter<IUser>();


    //public viewModel: models.UserListViewModel;
    
    constructor(private userService: UserService) {
        this.userService = userService;
        this.init();
    }
    

    
    init() {
        
        this.userService.get()
            .subscribe(result => {
               
                this.viewModel = result;
                 
                    //this.userUpdated.emit(this.users);
                },
                (err) => {
                    console.log('err:' + err);
                    this.viewModel = null;
                },
                () => console.log('Done')
            );


        //this._githubService.getRepos().subscribe(repos => {
        //    // console.log(repos);
        //    this.user.repos = repos;
        //    this.userUpdated.emit(this.user);
        //},
        //    (err) => {
        //        console.log('err:' + err);
        //        this.user.user = false;
        //    },
        //    () => console.log('Done')
        //);


    }
    

}