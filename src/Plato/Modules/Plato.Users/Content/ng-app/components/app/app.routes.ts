import { Routes, RouterModule } from '@angular/router';

import { LoginFormComponent } from '../public/login-form/login-form.component';
import { UserListComponent } from '../public/user-list/user-list.component';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'Users',
        pathMatch: 'full'
    },
    //{ path: '', component: UserListComponent },
    { path: 'Users', component: UserListComponent },
    { path: 'Login', component: LoginFormComponent },
    { path: 'Users/:page', component: UserListComponent }
];

export const UsersRouterModule = RouterModule.forRoot(routes, { useHash: true });