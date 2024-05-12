using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Library.core.DTO
{
    public class AuthDTO
    {
            public string Message { get; set; }
            public bool IsAuthenticated { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public List<string> Roles { get; set; }
            public string Token { get; set; }
            public DateTime ExpiresOn { get; set; }
            [JsonIgnore]
            public String? RefreshToken { get; set; }
            public DateTime RefreshTokenExpiration { get; set; }
        }
    }


