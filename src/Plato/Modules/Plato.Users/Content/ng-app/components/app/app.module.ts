import { NgModule, Injectable }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule, Headers, RequestOptions } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// components
import { AppComponent }   from './app.component';
import { UserListComponent } from '../public/user-list/user-list';

// services
import { UserService } from '../../services/user.service';

// extend http request options
@Injectable()
export class DefaultRequestOptions extends RequestOptions {
   headers: Headers = new Headers({
        'Auth': '1234567890'
    });
}

@NgModule({ 
    imports: [
        BrowserModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: UserListComponent },
            { path: 'users', component: UserListComponent },
            { path: 'login', component: UserListComponent }
        ])
    ],
    declarations: [
        AppComponent,
        UserListComponent
    ],
    bootstrap: [AppComponent],
    providers: [
        UserService,
        { provide: RequestOptions, useClass: DefaultRequestOptions }  
    ]
    
})
export class AppModule { }