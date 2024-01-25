import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import LoginDto from "../../models/user/LoginDto";
import RegisterDto from "../../models/user/RegisterDto";
import UserDetailDto from "../../models/user/UserDetailDto";
import UserNotificationSettingsDTO from "../../models/user/UserNotificationSettingsDTO";
import GroupedDataTableRequest from "../../models/GroupedDataTableRequest";
import PasswordRecoveyDto from "../../models/user/PasswordRecoveyDto";
import MfaLoginDto from "../../models/user/MfaLoginDto";

export default {
  async login(loginDto: LoginDto): Promise<BaseResponse> {
    return await Api.post(`/User/Login`, loginDto);
    },
    async loginMfa(loginDto: MfaLoginDto): Promise<BaseResponse> {
        return await Api.post(`/User/mfaLogin`, loginDto);
    },
    async generateMfa(): Promise<BaseResponse> {
        return await Api.get(`/User/generateMfa`);
    },
    async validateMfa(code: any): Promise<BaseResponse> {
        return await Api.get(`/User/validateMfa/${code}`);
    },
    async removeMfa(code: any): Promise<BaseResponse> {
        return await Api.get(`/User/removeMfa/${code}`);
    },
    async adminRemoveMfa(code: any): Promise<BaseResponse> {
        return await Api.get(`/User/adminRemoveMfa/${code}`);
    },
    async doesUserHaveMfa(): Promise<BaseResponse> {
        return await Api.get(`/User/doesUserHaveMfa`);
    },
    async getUsers(userTypeId: number): Promise<BaseResponse> {
        return await Api.get(`/User/list/${userTypeId}`);
  },
  async getCurrentUser(): Promise<BaseResponse> {
    return await Api.get(`/User/Current`);
    },
    async getUserById(id: number): Promise<BaseResponse> {
        return await Api.get(`/User/${id}`);
    },
  async getUserDetail(id: number): Promise<BaseResponse> {
      return await Api.get(`/User/detail/${id}`);
    },
  async getUserAnamnesis(id: number): Promise<BaseResponse> {
      return await Api.get(`/User/Anamnesis/${id}`);
  },
  async registerUser(registerDto: RegisterDto): Promise<BaseResponse> {
    return await Api.post(`/User/Register`, registerDto);
    },
    //async getDoctors(): Promise<BaseResponse> {
    //    return await Api.get(`/User/Doctor`);
    //},
    async getUserQuestions(userId: number): Promise<BaseResponse> {
        return await Api.get(`/User/Questions/${userId}`);
    },
    async getQuestionComments(questionId: number): Promise<BaseResponse> {
        return await Api.get(`/User/Questions/Comments/${questionId}`);
    },
    async activateEmail(token: string): Promise<BaseResponse> {
        var response = await Api.patch(`/User/Activate/${token}`);

        return response;
    },
    async deleteUser(id: number): Promise<BaseResponse> {
        return await Api.delete(`/User/${id}`);
    },
    async updateUser(user: UserDetailDto): Promise<BaseResponse> {
        return await Api.patch(`/User/Update`, user);
    },
    async getNotificationSettings(): Promise<BaseResponse> {
        return await Api.get(`/User/NotificationSettings`);
    },
    async getNotificationSettingsById(id: number): Promise<BaseResponse> {
        return await Api.get(`/User/NotificationSettings/${id}`);
    },
    async updateNotificationSettings(notificationSettings: UserNotificationSettingsDTO): Promise<BaseResponse> {
        return await Api.put(`/User/NotificationSettings`, notificationSettings);
    },
    async getDataTable(request: GroupedDataTableRequest): Promise<BaseResponse> {
        return await Api.post('/User/getDataTable', request);
    },
    async recoverPassword(request: PasswordRecoveyDto): Promise<BaseResponse> {
        return await Api.patch('/User/recoverpassword', request);
    },
    async requestRecoverPassword(email: string): Promise<BaseResponse> {
        return await Api.postString('/User/recover', email);
    },
    async refreshLogin(): Promise<BaseResponse> {
        return await Api.get('/User/refresh');
    },
    async generateFirstMFA(id: number): Promise<BaseResponse> {
        return await Api.get(`/User/generateFirstMfa/${id}`);
    },
    async generateFirstSMS(id: number, phone: string): Promise<BaseResponse> {
        return await Api.get(`/User/generateFirstSMS/${id}/${phone}`);
    },
    async validateFirstMfa(id: number, code: string): Promise<BaseResponse> {
        return await Api.get(`/User/validateFirstMfa/${id}/${code}`);
    },
    async validateFirstSMS(id: number, code: string): Promise<BaseResponse> {
        return await Api.get(`/User/validateFirstSMS/${id}/${code}`);
    },
    async mobileLogin(): Promise<BaseResponse> {
        return await Api.get(`/User/mobileLogin`);
    },
    async loginByPhone(): Promise<BaseResponse> {
        return await Api.get(`/User/loginByPhone`);
    },
    async checkLoginByPhone(id: number, guid: string): Promise<BaseResponse> {
        console.log(`/User/loginByPhone/${id}/${guid}`);
        return await Api.get(`/User/loginByPhone/${id}/${guid}`);
    },
};
