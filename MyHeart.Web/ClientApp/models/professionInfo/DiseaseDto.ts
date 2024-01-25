import SymptomDiseaseDto from "./bonds/SymptomDiseaseDto";
import MedicamentGroupIndicationDto from "./bonds/MedicamentGroupDiseaseIndicationDto";
import DiseaseNonpharmaticTherapyDto from "./bonds/DiseaseNonpharmaticTherapyDto";
import DiseasePreventiveMeasuresDto from "./bonds/DiseasePreventiveMeasuresDto";
import DiseaseDiseaseDto from "./bonds/DiseaseDiseaseDto";
import DiseaseSynonymDto from "./bonds/DiseaseSynonymDto";

export default class DiseaseDto {
    id: number;
    name: string | null;
    description: string | null;
    frequency: number | null;
    symptoms: SymptomDiseaseDto[];
    causes: DiseaseDiseaseDto[];
    medicamentGroup: MedicamentGroupIndicationDto[];
    nonpharmaticTherapy: DiseaseNonpharmaticTherapyDto[];
    preventiveMeasures: DiseasePreventiveMeasuresDto[];
    synonyms: DiseaseSynonymDto[];
    systemicMeasures: string | null;
}
