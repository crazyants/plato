import {
    Component,
    ElementRef,
    Input,
    Output,
    EventEmitter,
    OnDestroy,
    OnInit,
    OnChanges,
    SimpleChanges,
    NgZone
} from '@angular/core';


import "rxjs/add/operator/map";

@Component({
    selector: 'pager',
    templateUrl: './plato.core/content/ng-components/pager/src/pager.html'
})

export class PagerComponent implements OnDestroy, OnInit, OnChanges {

    @Input('totalPages') _totalPages: number = 0;
    @Input('pageIndex') _pageIndex: number = 1;
 
    @Output() pageClick = new EventEmitter<IPageClickEvent>();
    
    public pageLinks: any[];
    public canGoBack: boolean;
    public canGoForward: boolean;

    public get pageIndex() {
        return this._pageIndex;
    }
    
    public get totalPages() {
        return this._totalPages;
    }

    constructor(
        private element: ElementRef,
        private zone: NgZone) {

    }

    ngOnInit() {
        if (this.totalPages)
            this.init();
    }

    ngOnDestroy() {

    }

    ngOnChanges(changes: SimpleChanges) {
        if (this.totalPages)
            this.init();
    }
    
    private init() {
        
        this.pageLinks = [];
        for (let i = 1; i <= this.totalPages; i++) {
            this.pageLinks.push({
                index: i,
                text: i.toString(),
                isCurrent: i === this._pageIndex
            });
        }
        
        this.canGoBack = this.pageLinks.length && !this.pageLinks[0].isCurrent;
        this.canGoForward = this.pageLinks.length && !this.pageLinks[this.pageLinks.length - 1].isCurrent;
        
    }

    public goToPage(data: IPageClickEvent = { pageIndex: 1 }) {
        this.zone.run(() => this.pageClick.next(data));
    }
    
    public goToLast() {
        this.zone.run(() => this.pageClick.next(
            { pageIndex: this.totalPages }));
    }
    
}

export interface IPageClickEvent {
    pageIndex: number;
};

