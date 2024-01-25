import TherapyNewsDiseaseDto from "./bonds/TherapyNewsDiseaseDto";

export default class TherapyNewsDto {
  id: number;
    text: string | null;
    webLink: string | null;
    diseases: TherapyNewsDiseaseDto[];
    lastUpdateDate: string | null;
    lastUpdateUser: string | null;
}
