export default class VideoRoomDto {
    id: number;
    questionId: number;
    roomId: string;
    password: string;
    lastUpdateDate: string | null;
    lastUpdateUser: string | null;
}
