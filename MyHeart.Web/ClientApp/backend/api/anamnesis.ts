import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import SymptomDto from "../../models/professionInfo/SymptomDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";
import { url } from "inspector";

export default {

    async getPharmacyAnamnesis(): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getPharmacyAnamnesis`);
    },
    async getPharmacyAnamnesisForPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getPharmacyAnamnesis/${id}`);
    },
    async deletePharmacyAnamnesis(id): Promise<BaseResponse> {
        return await Api.delete(`/Anamnesis/deletePharmacyAnamnesis/` + id);
    },
    async savePharmacyAnamnesis(data): Promise<BaseResponse> {
        return await Api.post('/Anamnesis/savePharmacyAnamnesis', data);
    },
    async getPharmacyByNameReg(searchString): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getPharmacyByNameReg/' + searchString);
    },
    async getPharmacyByNameRegAndStrength(nameReg, strength): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getPharmacyByNameRegAndStrength/' + nameReg + '/' + strength);
    },
    async getPersonalAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getPersonalAnamneses', null, true);
    },
    async getPersonalAnamnesesForPerson(id : number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getPersonalAnamneses/${id}`, null, true);
    },
    async savePersonalAnamnesis(data): Promise<BaseResponse> {
        return await Api.post('/Anamnesis/savePersonalAnamnesis', data);
    },
    async deletePersonalAnamnesis(id): Promise<BaseResponse> {
        return await Api.delete('/Anamnesis/deletePersonalAnamnesis/' + id);
    },
    async updatePersonalAnamnesis(id: Number, data) : Promise<BaseResponse> {
        return await Api.put(`/Anamnesis/updatePersonalAnamnesis/${id}`, data);
    },
    async getGeneralAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getGeneralAnamneses');
    },
    async getAbususAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getAbususAnamneses');
    },
    async getAbususAnamnesesFromPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getAbususAnamneses/${id}`);
    },
    async saveAbususAnamnesis(data): Promise<BaseResponse> {
        return await Api.post('/Anamnesis/saveAbususAnamnesis', data);
    },

    async getSocialAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getSocialAnamneses');
    },
    async getSocialAnamnesesFromPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getSocialAnamneses/${id}`);
    },
    async saveSocialAnamnesis(data): Promise<BaseResponse> {
        return await Api.post('/Anamnesis/saveSocialAnamnesis', data);
    },

    async getFamilyAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getFamilyAnamneses');
    },
    async getFamilyAnamnesesFromPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getFamilyAnamneses/${id}`);
    },
    async saveFamilyAnamnesis(data): Promise<BaseResponse> {
        return await Api.post('/Anamnesis/saveFamilyAnamnesis', data);
    },

    async getAllergyAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getAllergyAnamneses');
    },
    async getAllergyAnamnesesFromPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getAllergyAnamneses/${id}`);
    },
    async saveAllergyAnamnesis(data): Promise<BaseResponse> {
        return await Api.post('/Anamnesis/saveAllergyAnamnesis', data);
    },
    async deleteAllergyAnamnesis(id): Promise<BaseResponse> {
        return await Api.delete(`/Anamnesis/deleteAllergyAnamnesis/` + id);
    },

    async getDiseasesByName(searchString): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getDiseasesByName/' + searchString);
    },
    async getDiseaseAnamneses(): Promise<BaseResponse> {
        return await Api.get('/Anamnesis/getDiseaseAnamneses');
    },
    async getDiseaseAnamnesesFromPerson(id: number): Promise<BaseResponse> {
        return await Api.get(`/Anamnesis/getDiseaseAnamneses/${id}`);
    },
    async saveDiseaseAnamnesis(data): Promise<BaseResponse> {
        return await Api.post("/Anamnesis/saveDiseaseAnamnesis", data);
    },
    async deleteDiseaseAnamnesis(id): Promise<BaseResponse> {
        return await Api.delete("/Anamnesis/deleteDiseaseAnamnesis/" + id);
    },

    async exportPersonalAnamnesis() {
        await Api.download("Anamnesis/exportPersonalAnamnesis", "personal-anamnesis.csv");
    },

    async getAllDiseases() {
        return Api.get("Anamnesis/getAllDiseases");
    },

    async getDiseases(request) {
        return Api.post("Anamnesis/getDiseases", request);
    }
};
