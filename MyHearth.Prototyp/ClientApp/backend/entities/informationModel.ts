import { UserModel } from './userModel';

export interface InformationModel {
    id: number;
    name: string;
    description: string;
    createdDate: string;
    author: UserModel;
}