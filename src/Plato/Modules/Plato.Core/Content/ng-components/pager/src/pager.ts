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
        
        this.buildPagingButtonts();
        this.canGoBack = this.pageLinks.length && !this.pageLinks[0].isCurrent;
        this.canGoForward = this.pageLinks.length && !this.pageLinks[this.pageLinks.length - 1].isCurrent;
    }
    
    private buildPagingButtonts() {

        let pagesToShow: number = 5;

        if (this.totalPages <= pagesToShow) {

            this.buildButtonRange(1, this.totalPages);

        } else {

            let firstPage = (this.pageIndex - 2);
            let lastPage = (this.pageIndex + 2);
            if (firstPage < 0) {
                firstPage = 1;
            }

            if (this.pageIndex === 1) {

                this.buildButtonRange(1, pagesToShow);

            } else if (this.pageIndex === 2) {

                this.buildButtonRange(1, pagesToShow);

            } else if (this.pageIndex === 3) {

                this.buildButtonRange(1, pagesToShow);

            } else if (this.pageIndex < this.totalPages - 2) {

                this.buildButtonRange(firstPage, lastPage);

            } else if (this.pageIndex === this.totalPages - 2) {

                this.buildButtonRange(this.totalPages - (pagesToShow - 1), this.pageIndex + 2);

            } else if (this.pageIndex === this.totalPages - 1) {

                this.buildButtonRange(this.totalPages - (pagesToShow - 1), this.pageIndex + 1);

            } else if (this.pageIndex === this.totalPages) {

                this.buildButtonRange(this.totalPages - (pagesToShow - 1), this.pageIndex);
            }

        }
        
    }

    private buildButtonRange(start: number, end: number) {

        this.pageLinks = [];
        for (let i = start; i <= end; i++) {
            this.pageLinks.push({
                index: i,
                text: i.toString(),
                isCurrent: i === this._pageIndex
            });
        }

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

