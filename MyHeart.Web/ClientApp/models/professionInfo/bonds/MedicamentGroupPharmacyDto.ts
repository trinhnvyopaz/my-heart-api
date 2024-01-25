import PharmacyDto from "../PharmacyDto";

export default class MedicamentGroupPharmacyDto {
    medicamentGroupId: number;
    pharmacyId: number;
    bondStrength: number;
    pharmacy: PharmacyDto;

    constructor(medicamentGroupId: number, bondStrength: number, pharmacy: PharmacyDto) {
        this.medicamentGroupId = medicamentGroupId;
        this.pharmacyId = pharmacy.id;
        this.pharmacy = pharmacy;
        this.bondStrength = bondStrength;
    }
}
