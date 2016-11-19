


export interface IRequest {
    url: string;
    params: IRequestParams;
    data: any;
}

export interface IRequestParams {
    page: number;
    pageSize: number;
    sortBy: string;
    sortDesc: boolean;
}