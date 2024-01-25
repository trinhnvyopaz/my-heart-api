import ClientQuestionaireDiseaseAverageDTO from "./ClientQuestionaireDiseaseAverageDTO";


export default class ClientQuestionaireResultDTO {
    probableDiseases: ClientQuestionaireDiseaseAverageDTO[];
    possibleDiseases: ClientQuestionaireDiseaseAverageDTO[];
    improbableDiseases: ClientQuestionaireDiseaseAverageDTO[];
}