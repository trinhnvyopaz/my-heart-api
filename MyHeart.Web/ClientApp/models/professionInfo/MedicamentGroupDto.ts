import MedicamentGroupDiseaseContraindicationDto from "./bonds/MedicamentGroupDiseaseContraindicationDto";
import MedicamentGroupDiseaseIndicationDto from "./bonds/MedicamentGroupDiseaseIndicationDto";
import MedicamentGroupPharmacyDto from "./bonds/MedicamentGroupPharmacyDto";
import MedicamentGroupSymptomsSideEffectsDto from "./bonds/MedicamentGroupSymptomsSideEffectsDto";
import MedicamentGroupAtcsDto from "./bonds/MedicamentGroupAtcsDto"
import PharmacyDto from "./PharmacyDto";

export default class MedicamentGroupDto {
    id: number;
    name: string | null;
    description: string | null;
    contraindication: MedicamentGroupDiseaseContraindicationDto[];
    indication: MedicamentGroupDiseaseIndicationDto[];
    activeSubstance: MedicamentGroupPharmacyDto[];
    sideEffects: MedicamentGroupSymptomsSideEffectsDto[];
    atcs: MedicamentGroupAtcsDto[];
    pharmacies: PharmacyDto[];
}
