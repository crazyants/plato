

export interface IUser {
    Id: number;
    UserName: string;
    Email: string;
    DisplayName: string;
    SamAccountName: string;
    NormalizedUserName: string;
    NormalizedEmail: string;
    PasswordHash: string,
    EmailConfirmed: boolean;
    SecurityStamp: string;
    PhoneNumber: string;
    PhoneNumberConfirmed: boolean;
    TwoFactorEnabled: boolean;
    LockoutEnd: string;
    LockoutEnabled: boolean;
    AccessFailedCount: number;
}