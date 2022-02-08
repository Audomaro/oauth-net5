using DTO;

namespace SVC
{
    public interface IAuthenticateService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string applicationName, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string applicationName, string ipAddress);
        void RevokeToken(string token, string ipAddress);
    }
}
