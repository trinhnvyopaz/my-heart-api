import SymptomDto from "../SymptomDto";

export default class PharmacySymptomsDto {
    pharmacyId: number;
    symptomId: number;  
    bondStrength: number;
    symptom: SymptomDto;

    constructor(pharmacyId: number, symptom: SymptomDto) {
        this.pharmacyId = pharmacyId;
        this.symptomId = symptom.id;
        this.symptom = symptom;
    }
}
