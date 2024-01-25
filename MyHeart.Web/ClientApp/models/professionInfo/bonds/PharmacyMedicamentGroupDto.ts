import MedicamentGroupDto from "../MedicamentGroupDto";

export default class PharmacyMedicamentGroupDto {
    medicamentGroupId: number;
    pharmacyId: number;
    bondStrength: number;
    medicametnGroup: MedicamentGroupDto;

    constructor(pharmacyId: number, bondStrength: number, medicametnGroup: MedicamentGroupDto) {
        this.pharmacyId = pharmacyId;
        this.medicamentGroupId = medicametnGroup.id;
        this.medicametnGroup = medicametnGroup;
        this.bondStrength = bondStrength;
    }
}
