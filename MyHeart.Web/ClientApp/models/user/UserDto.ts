export default class UserDto {
    id: number;
    firstName: string | null;
    lastName: string | null;
    email: string | null;
    userType: UserType | null;
    stripeId: string;
    userTypes: string | null;
}
export enum UserType {
    SuperAdmin = 0,
    Admin = 1,
    Doctor = 2,
    Patient = 3,
    DataManager = 4
}
