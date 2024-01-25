export default class QuestionDto {
  id: number; 
    subject: string | null;
    body: string | null;
    creationDate: string | null;
    userId: number | null;
    name: string | null;
    status: string | null;
}
export enum QuestionStatus {
    Open,
    AwaitingPatientResponse,
    AwaitingDoctorResponse,
    Closed
}
