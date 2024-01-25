export default class MfaLoginDto {
  email: string | null;
    password: string | null;
    code: string | null;
  rememberMe: boolean = true;
}
