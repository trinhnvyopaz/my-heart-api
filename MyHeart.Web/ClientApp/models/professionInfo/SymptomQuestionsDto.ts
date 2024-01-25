import SymptomQuestionsSymptomDto from "./bonds/SymptomQuestionsSymptomDto";
import SymptomQuestionsDiseaseDto from "./bonds/SymptomQuestionsDiseaseDto";

export default class SymptomQuestionsDto {
  id: number;
    text: string | null;
    symptoms: SymptomQuestionsSymptomDto[];
    diseases: SymptomQuestionsDiseaseDto[];
    symptomName: string | null;
    diseaseString: string | null;
}
