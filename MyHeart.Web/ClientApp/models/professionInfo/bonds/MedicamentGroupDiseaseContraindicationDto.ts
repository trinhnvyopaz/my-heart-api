import DiseaseDto from "../DiseaseDto";

export default class MedicamentGroupDiseaseContraindicationDto {
    medicamentGroupId: number;
    diseaseId: number;
    disease: DiseaseDto;

    constructor(medicamentGroupId: number, disease: DiseaseDto) {
        this.medicamentGroupId = medicamentGroupId;
        this.diseaseId = disease.id;
        this.disease = disease;
    }
}
