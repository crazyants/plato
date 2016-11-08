/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import 'rxjs/add/operator/map';

import { User } from '../../../models/User';
import { UserService } from '../../../services/user.service';

@Component({
    selector: 'user-list',
    templateUrl: './plato.users/ng-app/components/public/user-list/user-list.html'
})

export class UserListComponent implements OnInit {

    @Input() user: User;
    @Output() userUpdated = new EventEmitter<User>();
    
    constructor(private userService: UserService) {
        this.userService = userService;
    }
    

    ngOnInit() {
        if (this.user) {
            this.user.user = false;
            this.init();
        }
    }


    init() {


        this.userService.get()
            .subscribe(user => {
                    this.user.user = user;
                    this.userUpdated.emit(this.user);
                },
                (err) => {
                    console.log('err:' + err);
                    this.user.user = false;
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