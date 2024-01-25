import SymptomQuestionsDTO from "../professionInfo/SymptomQuestionsDto"

export default class ClientQuestionAnswerDTO {
    symptomQuestionId: number;
    symptomQuestion: SymptomQuestionsDTO
    userId: number;
    createdDate: Date;
    approved: boolean;
  }
  