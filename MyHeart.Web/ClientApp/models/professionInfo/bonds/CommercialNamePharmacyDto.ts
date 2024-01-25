import PharmacyDto from "../PharmacyDto";

export default class CommercialNamePharmacyDto {
    commercialNamesId: number;
    pharmacyId: number;
    bondStrength: number;
    pharmacy: PharmacyDto;

    constructor(commercialNamesId: number, bondStrength: number, pharmacy: PharmacyDto) {
        this.commercialNamesId = commercialNamesId;
        this.pharmacyId = pharmacy.id;
        this.pharmacy = pharmacy;
        this.bondStrength = bondStrength;
    }
    
}
