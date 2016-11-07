import { NgModule }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent }   from './app.component';
import { ForumListComponent } from '../public/forum-list/forum-list';



@NgModule({
    imports: [BrowserModule],
    declarations: [
        AppComponent,
        ForumListComponent
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }