import NonpharmacyDto from "../NonpharmacyDto";

export default class MedicalFacilitiesNonpharmaticTherapyDto {
    medicalFacilitiesId: number;
    nonpharmaticTherapyId: number;
    bondStrength: number;
    nonpharmacy: NonpharmacyDto;

    constructor(medicalFacilitiesId: number, bondStrength: number, nonpharmacy: NonpharmacyDto) {
        this.medicalFacilitiesId = medicalFacilitiesId;
        this.nonpharmaticTherapyId = nonpharmacy.id;
        this.nonpharmacy = nonpharmacy;
        this.bondStrength = bondStrength;
    }
}
