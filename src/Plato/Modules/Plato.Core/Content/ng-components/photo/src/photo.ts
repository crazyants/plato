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
export class PhotoComponent {

    @Input() user: any;

    initials: string;

    constructor(
        private element: ElementRef,
        private zone: NgZone) {

    }

    private init() {

        if (this.user) {
            
        }

    }


}

