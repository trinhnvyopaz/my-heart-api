import DiseaseDto from "../DiseaseDto";

export default class PharmacyDiseaseDto {
    pharmacyId: number;
    diseaseId: number;
    bondStrength: number;
    disease: DiseaseDto;

    constructor(pharmacyId: number, bondStrength: number, disease: DiseaseDto) {
        this.pharmacyId = pharmacyId;
        this.diseaseId = disease.id;
        this.disease = disease;
        this.bondStrength = bondStrength;
    }
}
