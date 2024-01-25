import AtcDto from "../AtcDto";

export default class MedicamentGroupDiseaseContraindicationDto {
    medicamentGroupId: number;
    atcId: number;
    atc: AtcDto;

    constructor(medicamentGroupId: number, atc: AtcDto) {
        this.medicamentGroupId = medicamentGroupId;
        this.atcId = atc.id;
        this.atc = atc;
    }
}
