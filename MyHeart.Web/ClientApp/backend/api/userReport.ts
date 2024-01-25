import Api from "./api";
import BaseResponse from "../../models/BaseResponse";

export default {
    async getUserReports(): Promise<BaseResponse> {
        return await Api.get('/UserReports/getUserReports');
    },
    async getUserReportsForUser(id :number): Promise<BaseResponse> {
        return await Api.get(`/UserReports/getUserReports/${id}`);
    },
    async saveUserReport(data): Promise<BaseResponse> {
        return await Api.post("/UserReports/saveUserReport", data);
    },
    async deleteUserReport(id): Promise<BaseResponse> {
        return await Api.delete("/UserReports/deleteUserReport/" + id);
    },

    async downloadReport(id, filename) {
        await Api.download("UserReports/downloadReportFile/" + id, filename);
    },

    async updateUserReport(data): Promise<BaseResponse>{
        return await Api.post('/UserReports/updateUserReport', data)
    },
};
