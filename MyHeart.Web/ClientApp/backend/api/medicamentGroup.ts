import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import MedicamentGroupDto from "../../models/professionInfo/MedicamentGroupDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";
import PharmacyDiscardRequest from "../../models/PharmacyDiscardRequest";

export default {

    async getMedicamentGroups(): Promise<BaseResponse> {
        return await Api.get(`/MedicamentGroup`);
    },
   
    async getMedicamentGroupDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/MedicamentGroup/${id}`);
    },

    async createMedicamentGroup(MedicamentGroup: MedicamentGroupDto): Promise<BaseResponse> {
        return await Api.post(`/MedicamentGroup`, MedicamentGroup);
    },

    async updateMedicamentGroup(MedicamentGroup: MedicamentGroupDto): Promise<BaseResponse> {
        return await Api.put(`/MedicamentGroup`, MedicamentGroup);
    },

    async deleteMedicamentGroup(id: number): Promise<BaseResponse> {
        return await Api.delete(`/MedicamentGroup/${id}`);
    },

    async getDiscardedPharmacies(request: PharmacyDiscardRequest): Promise<BaseResponse> {
        return await Api.post(`/MedicamentGroup/getDiscardedPharmacies`, request);
    },

    async toggleDiscard(item: any): Promise<BaseResponse> {
        return await Api.post('/MedicamentGroup/togglePharmacyDiscard', item);
    },

    async getDataTable(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post('/MedicamentGroup/getDataTable', request);
    },

    async getPagedResource(request: PagedResourceRequest): Promise<BaseResponse> {
        return await Api.post('/MedicamentGroup/getPagedResource', request);
    },

    async getByIds(request: number[]): Promise<BaseResponse> {
        return await Api.post('/MedicamentGroup/getByIds', request);
    }
};
