import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import DataTableRequest from "../../models/DataTableRequest";

export default {
    async getUserReport(id: number): Promise<BaseResponse> {
        return await Api.get(`/MedicalReport/${id}`);
    },
    async getAllMedicalReports(params: DataTableRequest): Promise<BaseResponse> {
        return await Api.get('/MedicalReport', params);
    },
    async UpdateMedicalReport(id, data): Promise<BaseResponse> {
        return await Api.put(`/MedicalReport/${id}`, data);
    },
    async getMedicalReportExport(id: number) {
        var fileName = "MedicalReport_" + new Date().toLocaleString() + ".xlsx";
        await Api.download(`/MedicalReport/export/${id}`, fileName);
    },
};
