
import { RouteLoaderComponent } from './libs/route-loader.component';
import { RouteLoaderService } from './libs/route-loader.service';

export * from './libs/route-loader.component';
export * from './libs/route-loader.service';

export default {
    directives: [
        RouteLoaderComponent,
        RouteLoaderService
    ]
}