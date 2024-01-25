import ClientQuestionAnswerDTO from "./ClientQuestionAnswerDTO";
import UserDTO from "../user/UserDto"

export default class ClientQuestionaireDTO {
    userId: number;
    user: UserDTO;
    createdDate: Date;
    questionAnswers:   ClientQuestionAnswerDTO[] 
  }
  