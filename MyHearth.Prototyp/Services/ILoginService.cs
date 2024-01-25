namespace MyHeartAdvice.Services {
  public interface ILoginService {
    bool Login(dynamic loginModel);
    string CreateJwtToken();
  }
}
