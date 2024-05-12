using Library.core.DTO;
using Library.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.core.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthDTO> RegisterAsync(RegisterDTO model);
        Task<AuthDTO> RegisterAdminAsync(RegisterDTO model);

        Task<AuthDTO> GetTokenAsync(TokenRequestDTO model);
        User AcceptUser(string id);
        User RejectUser(string id);

        User GetUser(string email);
        IEnumerable<User> GetUsers();
        Task<AuthDTO> RefreshTokenAsync(string token);
        Task<bool> Logout();

    }
}
