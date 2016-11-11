import { Component, } from '@angular/core';
import {
    NavigationStart,
    NavigationEnd,
    NavigationCancel,
    NavigationError,
    Router,
    RouterModule,
    Event as RouterEvent
} from '@angular/router'
declare var jQuery: JQueryStatic;

@Component({
    selector: 'my-app',
    templateUrl: './plato.users/ng-app/components/app/app.html'
})
export class AppComponent {
    
    loading: boolean = true;

    constructor(private router: Router) {
        
        router.events.subscribe((event: RouterEvent) => {
         
            if (event instanceof NavigationStart) 
                this.loading = true;
            
            if (event instanceof NavigationEnd) {
                window.setTimeout(() => {
                        this.loading = false;
                    },
                    100);
            }

            if (event instanceof NavigationCancel) 
                this.loading = false;
            
            if (event instanceof NavigationError) 
                this.loading = false;
            
        });

    }

    getRouteLoaderDisplayState() {
        if (this.loading)
            return "block";
        else
            return "none";
    }

    getRouteOutletDisplayState() {
        if (this.loading)
            return "none";
        else 
            return "block";
    }
    

}