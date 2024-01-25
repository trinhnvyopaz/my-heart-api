import PreventiveMeasuresDto from "../PreventiveMeasuresDto";

export default class MedicalFacilitiesPreventiveMeasuresDto {
    medicalFacilitiesId: number;
    preventiveMeasureId: number;
    bondStrength: number;
    preventiveMeasures: PreventiveMeasuresDto;

    constructor(medicalFacilitiesId: number, bondStrength: number, preventiveMeasures: PreventiveMeasuresDto) {
        this.medicalFacilitiesId = medicalFacilitiesId;
        this.preventiveMeasureId = preventiveMeasures.id;
        this.preventiveMeasures = preventiveMeasures;
        this.bondStrength = bondStrength;
    }
    
}
