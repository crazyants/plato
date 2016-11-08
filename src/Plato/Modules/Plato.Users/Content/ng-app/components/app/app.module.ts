import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

// components
import { AppComponent }   from './app.component';
import { UserListComponent } from '../public/user-list/user-list';

// services
import { UserService } from '../../services/user.service';


@NgModule({ 
    imports: [BrowserModule, HttpModule, FormsModule],
    declarations: [
        AppComponent,
        UserListComponent
    ],
    bootstrap: [AppComponent],
    providers: [UserService]
})
export class AppModule { }