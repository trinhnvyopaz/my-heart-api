import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import SymptomQuestionsDto from "../../models/professionInfo/SymptomQuestionsDto";
import SymptomQuestionsDiseaseDto from "../../models/professionInfo/bonds/SymptomQuestionsDiseaseDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";

export default {
    async getSymptomQuestions(): Promise<BaseResponse> {
        return await Api.get(`/SymptomQuestions`);
    },

    async getSymptomQuestionDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/SymptomQuestions/${id}`);
    },

    async updateSymptomQuestions(symptomQuestions: SymptomQuestionsDto): Promise<BaseResponse> {
        return await Api.put(`/SymptomQuestions`, symptomQuestions);
    },

    async createSymptomQuestions(symptomQuestions: SymptomQuestionsDto): Promise<BaseResponse> {
        return await Api.post(`/SymptomQuestions`, symptomQuestions);
    },

    async deleteSymptomQuestions(id: number): Promise<BaseResponse> {
        return await Api.delete(`/SymptomQuestions/${id}`);
    },

    async getSymptomQuestionsByDiseaseId(id: number): Promise<BaseResponse> {
        return await Api.get(`/SymptomQuestions/disease/${id}`);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/SymptomQuestions/getDataTable', request);
    },
    
    async getSymptomQuestionsBySymptomIds(list: number[]) : Promise<BaseResponse>{
        return await Api.post('/SymptomQuestions/getQuestions', list);
    },
    async updateSymtomQuestionDiseases(list: SymptomQuestionsDiseaseDto[]) : Promise<BaseResponse>{
        return await Api.put('/SymptomQuestions/updateSymtomQuestionDiseases', list)
    },
    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/SymptomQuestions/getPagedResource', request);
    },
    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/SymptomQuestions/getByIds', request);
    },
};
