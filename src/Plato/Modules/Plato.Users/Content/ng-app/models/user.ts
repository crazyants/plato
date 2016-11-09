
export interface IUser {
    id: number;
    userName: string;
    displayName: string;
}


export class UserListViewModel {
    constructor(
        public data: any[],
        public statusCode: number,
        public message: string
    ) { }
}