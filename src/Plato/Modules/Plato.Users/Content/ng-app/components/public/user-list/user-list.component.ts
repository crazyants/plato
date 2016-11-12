/// <reference path="../../../../../../../typings/globals/core-js/index.d.ts" />

// angular
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import "rxjs/add/operator/map";
import { Router, ActivatedRoute, Params } from '@angular/router';

// common
import { IPageClickEvent } from '../../../../../../plato.core/content/ng-components/pager/pager';

// module specific 
import * as models from "../../../models/User";
import { UserService } from "../../../services/user.service";

@Component({
    selector: 'user-list',
    templateUrl: './plato.users/content/ng-app/components/public/user-list/user-list.html'
})

export class UserListComponent implements OnInit {

    @Input() viewModel: models.UserListViewModel;
    @Output() userUpdated = new EventEmitter<models.UserListViewModel>();
    
    private _pageIndex = 1;
    private _pageSize = 10;
    private _sortBy = "Title";
    private _sortDesc = false;

    public totalCount: number;
    public totalPages: number;

    public get pageIndex() {
        return this._pageIndex;
    }

    public get pageSize() {
        return this._pageSize;
    }
 
    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private userService: UserService) {
        

    }

    pagerClick(data: IPageClickEvent) {
        this._pageIndex = data.pageIndex;
        //this.router.navigate(['Users', data.pageIndex]);
        this.refreshData();
    }
    
    ngOnInit() {

        this.route.params.forEach((params: Params) => {

            // (+) converts string 'id' to a number
            this._pageIndex = (params['page'] ? +params['page'] : this._pageIndex); 
            this._pageSize = (params['pageSize'] ? +params['pageSize'] : this._pageSize);
       
            if (this._pageIndex && this._pageSize) 
                this.refreshData();

        });

    
    }

    public sortBy(col: string) {
        this._sortDesc = col === this._sortBy ? !this._sortDesc : false;
        this._sortBy = col;
        this.refreshData();
    }
    
    refreshData() {

        this.userService.get(this._pageIndex, this.pageSize, "Id", this._sortDesc)
            .subscribe(result => {

                    this.viewModel = result;
                    this.userUpdated.emit(this.viewModel);

                    this.totalCount = result.users.total;
                    this.totalPages = Math.ceil(this.totalCount / this.pageSize);
                
                },
                (err) => {
                    console.log('err:' + err);
                    this.viewModel = null;
                },
                () => console.log('Done')
            );

    }


}