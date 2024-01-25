export default interface BaseResponse {
  success: boolean;
  message: string;
  status: number;
  errors: object;
  data: any;
}
