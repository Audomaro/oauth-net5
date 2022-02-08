using BO;

namespace HLP
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(UserEntity user);
        public int? ValidateJwtToken(string token);
        public RefreshTokenEntity GenerateRefreshToken(string application, string ipAddress);
    }
}
