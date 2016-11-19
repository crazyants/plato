import { NgModule, Injectable } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule, Headers, RequestOptions } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { UsersRouterModule } from './app.routes';

// shared components
import { PagerModule } from '../../../../../plato.core/content/ng-components/pager/pager';

// shared services
import { PlatoHttpModule, PlatoHttp } from '../../../../../plato.core/content/ng-services/http/http';

// components
import { AppComponent } from './app.component';
import { LoginFormComponent } from '../public/login-form/login-form';
import { UserListComponent } from '../public/user-list/user-list';

// services
import { UserService } from '../../services/user.service';

@NgModule({ 
    imports: [
        BrowserModule,
        HttpModule,
        PlatoHttpModule,
        FormsModule,
        PagerModule,
        UsersRouterModule
    ],
    declarations: [
        AppComponent,
        UserListComponent,
        LoginFormComponent
    ],
    bootstrap: [AppComponent],
    providers: [
        PlatoHttp,
        UserService,
        { provide: LocationStrategy, useClass: HashLocationStrategy }
    ]
    
})
export class AppModule { }