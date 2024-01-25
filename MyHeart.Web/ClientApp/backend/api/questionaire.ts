import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import ClientQuestionaireDTO from "../../models/questionaire/ClientQuestionaireDTO";
import DataTableRequest from "../../models/DataTableRequest";

export default {
    async createQuestionaire(questionaire: ClientQuestionaireDTO): Promise<BaseResponse> {
        return await Api.post(`/Questionaire`, questionaire);
    },
    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/Questionaire/getDataTable', request);
    },
    async getQuestions(questionaireId: Number): Promise<BaseResponse> {
        return await Api.get('/Questionaire/getQuestions/' + questionaireId);
    },
    async getResult(questionaireId: Number): Promise<BaseResponse> {
        return await Api.get('/Questionaire/result/' + questionaireId);
    },
};
