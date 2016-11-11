import { RouteLoaderComponent } from './libs/route-loader.component';
import { RouteLoaderService } from './libs/route-loader.service';

declare namespace core {

   var RouteLoaderService: RouteLoaderService;
   var RouteLoaderService: RouteLoaderService;

}

declare module "@plato/core" {
    export = core;
}
