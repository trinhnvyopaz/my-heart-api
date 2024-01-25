import SymptomDto from "../SymptomDto";

export default class SymptomQuestionsSymptomDto {
    symptomQuestionsId: number;
    symptomId: number;
    bondStrength: number;
    symptom: SymptomDto;

    constructor(symptomQuestionsId: number, bondStrength: number, symptoms: SymptomDto) {
        console.log(symptoms)
        this.symptomQuestionsId = symptomQuestionsId;
        this.symptomId = symptoms.id;
        this.symptom = symptoms;
        this.bondStrength = bondStrength;
    }
    
}
