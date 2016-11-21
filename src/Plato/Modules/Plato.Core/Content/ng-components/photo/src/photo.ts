import {
    Component,
    ElementRef,
    Input, Output,
    EventEmitter,
    OnDestroy, OnInit, OnChanges,
    SimpleChanges,
    NgZone
} from '@angular/core';

@Component({
    selector: 'photo',
    templateUrl: './plato.core/content/ng-components/photo/src/photo.html'
})
export class PhotoComponent implements OnDestroy, OnInit, OnChanges {


    @Input() size: number;
    @Input() user: any;

    imageUrl: string;
    imageClass: string;
    initials: string;

    constructor(
        private element: ElementRef,
        private zone: NgZone) {
        
    }

    ngOnInit() {

        switch (this.size) {
            case 1:
                this.imageClass = "p-photo p-photo-1x";
                break;
            case 2:
                this.imageClass = "p-photo p-photo-2x";
                break;
        }
        
        this.initials = "RH";

   
    }

    ngOnDestroy() {

    }

    ngOnChanges(changes: SimpleChanges) {
        if (this.user)
            this.init();
    }
    
    private init() {
        if (this.user) {
            this.imageUrl = 'users/photo/serve?id=' + this.user.id.toString();
        }
    }


}

