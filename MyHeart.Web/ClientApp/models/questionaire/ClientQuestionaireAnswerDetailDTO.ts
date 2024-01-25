import SymptomQuestionsDiseaseDto from "./bonds/SymptomQuestionsDiseaseDto";

export default class ClientQuestionaireAnswerDetailDTO {
    approved: boolean;
    symptomName: string | null;
    text: string | null;
    symptomDisease: SymptomQuestionsDiseaseDto;
  }
  
