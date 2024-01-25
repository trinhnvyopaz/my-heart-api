import PharmacyDiseaseDto from "./bonds/PharmacyDiseaseDto";
import PharmacySymptomsDto from "./bonds/PharmacySymptomsDto";
import MedicamentGroupPharmacyDto from "./bonds/MedicamentGroupPharmacyDto";

export default class PharmacyDto {
    id: number;
    name: string | null;

    indication: PharmacyDiseaseDto[];
    contraIndication: PharmacyDiseaseDto[];
    sideEffects: PharmacySymptomsDto[];
    activeSubstance: MedicamentGroupPharmacyDto[];
    dosage: string | null;
    minimumDose: string | null;
    maximumDose: string | null;

    suklCode: number;
    strength: string;
    form: string;
    package: string;
    path: string;
    supplement: string;
    atcWho: string;
    dddamntWho: number;
    dddunWho: string;
    nameReg: string;

}
