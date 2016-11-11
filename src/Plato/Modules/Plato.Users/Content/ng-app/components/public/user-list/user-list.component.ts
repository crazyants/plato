/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import "rxjs/add/operator/map";

import * as models from "../../../models/User";
import { UserService } from "../../../services/user.service";

@Component({
    selector: 'user-list',
    templateUrl: './plato.users/ng-app/components/public/user-list/user-list.html'
})

export class UserListComponent {

    @Input() viewModel: models.UserListViewModel;
    @Input() page: number = 1;
    @Input() pageSize: number = 10;
    @Output() userUpdated = new EventEmitter<models.UserListViewModel>();
    
    public totalPages: number;

    constructor(private userService: UserService) {

        this.userService = userService;
        if (this.page && this.pageSize) {
            this.init();
        }
    }
    
    ngOnInit() {
    }

    ngOnDestroy() {
    }

    prevPageClick() {
        this.page = this.page - 1;
        if (this.page < 1) {
            this.page = 1;
        }
        this.init();
    }

    nextPageClick() {
        this.page = this.page + 1;
        this.init();
    }

    setTotalPages(total: number) {
        
        this.totalPages = Math.ceil(total / this.pageSize);

    }

    init() {
    
        this.userService.get(this.page, this.pageSize)
            .subscribe(result => {
                    this.viewModel = result;
                    this.setTotalPages(result.users.total);
                    this.userUpdated.emit(this.viewModel);
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