import NonpharmaticTherapyDiseaseDto from "./bonds/NonpharmaticTherapyDiseaseDto";
import NonpharmaticTherapyMedicalFaclitiesDto from "./bonds/NonpharmaticTherapyMedicalFaclitiesDto";
import NonpharmaticTherapySynonymDto from "./bonds/NonpharmaticTherapySynonymDto";

export default class NonpharmacyDto {
    id: number;
    name: string | null;
    description: string | null;
    efficiency: number | null;
    hospitalizationLength: string | null;  
    complication: NonpharmaticTherapyDiseaseDto[];
    indication: NonpharmaticTherapyDiseaseDto[];
    medicalIntervention: NonpharmaticTherapyMedicalFaclitiesDto[];
    synonyms: NonpharmaticTherapySynonymDto[];
}
