import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import PharmacyDto from "../../models/professionInfo/PharmacyDto";
import DataTableRequest from "../../models/DataTableRequest";
import PagedResourceRequest from "../../models/PagedResourceRequest";


export default {

    async getNonpharmacy(): Promise<BaseResponse> {
        return await Api.get(`/UserNonpharmacy`);
    },

    async getNonpharmacyForPerson(id : number): Promise<BaseResponse> {
        return await Api.get(`/UserNonpharmacy/${id}`);
    },

    async saveNonPharmacy(model): Promise<BaseResponse> {
        return await Api.post(`/UserNonpharmacy`, model);
    },

    async deleteNonpharmacy(id: number): Promise<BaseResponse> {
        return await Api.delete(`/UserNonpharmacy/${id}`);
    },

    async getNonpharmaticTherapiesByName(searchString): Promise<BaseResponse> {
        return await Api.get(`/UserNonpharmacy/getNonpharmaticTherapiesByName/` + searchString);
    },
    
};
