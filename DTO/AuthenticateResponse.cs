using BO;
using System.Text.Json.Serialization;

namespace DTO
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Branch { get; private set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string JwtToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        public AuthenticateResponse(UserEntity user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Username = user.Username;
            Branch = user.Branch;
            DisplayName = user.DisplayName;
            Email = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
