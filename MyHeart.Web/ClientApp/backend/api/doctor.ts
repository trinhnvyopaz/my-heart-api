import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import RegisterDto from "../../models/user/RegisterDto";
import DoctorDetailDto from "../../models/doctor/DoctorDetailDto";

export default {

    async registerDoctor(registerDto: RegisterDto): Promise<BaseResponse> {
        return await Api.post(`/Doctor/Register`, registerDto);
    },

    async updateDoctor(doctor: DoctorDetailDto): Promise<BaseResponse> {
        return await Api.patch(`/Doctor/Update`, doctor);
    },

    async getDoctorDetail(id: number): Promise<BaseResponse> {
        return await Api.get(`/Doctor/${id}`);
    },

    async getCurrentDoctorDetail(): Promise<BaseResponse> {
        return await Api.get(`/Doctor/Current`);
    },
    async getDoctorDetailByUserId(id: number) : Promise<BaseResponse> {
        return await Api.get(`/Doctor/ByUserId/${id}`)
    }
};
