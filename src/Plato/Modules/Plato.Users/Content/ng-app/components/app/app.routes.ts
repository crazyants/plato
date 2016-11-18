import { Routes, RouterModule } from '@angular/router';

import { LoginFormComponent } from '../public/login-form/login-form';
import { UserListComponent } from '../public/user-list/user-list';

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