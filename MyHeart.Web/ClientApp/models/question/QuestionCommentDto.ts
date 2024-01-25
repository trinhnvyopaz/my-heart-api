export default class QuestionCommentDto {
    id: number;
    text: string | null;
    senderId: number;
    questionId: number;
    creationDate: string | null;
    lastUpdateDate: string | null;
    lastUpdateUser: string | null;
}
