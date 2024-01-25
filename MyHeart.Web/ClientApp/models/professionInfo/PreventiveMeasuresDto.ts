import PreventiveMeasuresDiseaseDto from "./bonds/PreventiveMeasuresDiseaseDto";
import PreventiveMeasuresMedicalFacilitiesDto from "./bonds/PreventiveMeasuresMedicalFacilitiesDto";
import PreventiveMeasuresSynonymDto from "./PreventiveMeasuresSynonymDto";

export default class PreventiveMeasuresDto {
    id: number;
    name: string | null;
    description: string | null;
    indication: PreventiveMeasuresDiseaseDto[];
    workspaceList: PreventiveMeasuresMedicalFacilitiesDto[];
    synonyms: PreventiveMeasuresSynonymDto[];
}
