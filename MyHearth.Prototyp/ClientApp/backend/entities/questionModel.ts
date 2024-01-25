import { UserModel } from "./userModel";
import { MessageModel } from "./messageModel";

export interface QuestionModel {
    id: number;
    subject: string;
    messages: MessageModel[];
    doctor: UserModel;
    client: UserModel;
}