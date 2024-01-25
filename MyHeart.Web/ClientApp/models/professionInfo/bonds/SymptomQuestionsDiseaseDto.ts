import DiseaseDto from "../DiseaseDto";

export default class SymptomQuestionsDiseaseDto {
    symptomQuestionsId: number;
    diseaseId: number;
    bondStrength: number;
    disease: DiseaseDto;

    constructor(symptomQuestionsId: number, bondStrength: number, disease: DiseaseDto) {
        this.symptomQuestionsId = symptomQuestionsId;
        this.diseaseId = disease.id;
        this.bondStrength = bondStrength;
        this.disease = disease;
    }
}