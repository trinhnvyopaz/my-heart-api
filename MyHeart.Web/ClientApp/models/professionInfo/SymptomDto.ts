import SymptomDiseaseDto from "./bonds/SymptomDiseaseDto";
import SymptomQuestionsSymptomDto from "./bonds/SymptomQuestionsSymptomDto";
import SymptomSynonymDto from "./SymptomSynonymDto";

export default class SymptomDto {
    id: number;
    name: string | null;
    description: string | null;
    diseases: SymptomDiseaseDto[];
    symptomQuestions: SymptomQuestionsSymptomDto[];
    synonyms: SymptomSynonymDto[];
}
