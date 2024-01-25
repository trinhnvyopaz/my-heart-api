import Api from "./api";
import BaseResponse from "../../models/BaseResponse";
import DataTableRequest from "../../models/DataTableRequest";
import QuestionDto from "../../models/question/QuestionDto";
import GroupedDataTableRequest from "../../models/GroupedDataTableRequest";
import QuestionCommentRequest from "../../models/QuestionCommentRequest";

export default {
    async getQuestionList(): Promise<BaseResponse> {
        return await Api.get(`/Question`);
    },
    async getOpenQuestions(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post(`/Question/open`, request);
    },
    async getOpenQuestionsForUser(id: number, request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post(`/Question/open/${id}`, request);
    },
    async getClosedQuestions(request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post(`/Question/closed`, request);
    },
    async getClosedQuestionsForUser(id: number, request: DataTableRequest): Promise<BaseResponse> {
        return await Api.post(`/Question/closed/${id}`, request);
    },
    async askQuestion(request: QuestionDto): Promise<BaseResponse> {
        return await Api.post(`/Question/ask`, request);
    },
    async getDataTable(request: GroupedDataTableRequest): Promise<BaseResponse> {
        return await Api.post('/Question/getDataTable', request);
    },
    async getQuestion(id: number): Promise<BaseResponse> {
        return await Api.get(`/Question/${id}`);
    },
    async getResponses(request: QuestionCommentRequest): Promise<BaseResponse> {
        return await Api.post('/Question/lastReplies', request);
    },
    async GetVideoRoomDetails(id: number): Promise<BaseResponse> {
        return await Api.get(`/Question/video/${id}`);
    },
    async archiveQuestion(id: Number): Promise<BaseResponse>{
        return await Api.get(`/Question/archive/${id}`);
    },
    async joinRoom(questionId: Number) : Promise<BaseResponse>{
        return await Api.get(`VideoProvider/${questionId}`)
    }
};
