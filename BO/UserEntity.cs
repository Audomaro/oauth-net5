using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BO
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Branch { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public List<RefreshTokenEntity> RefreshTokens { get; set; }
    }
}
