using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.core.Models
{
    public class User : IdentityUser
    {
        public bool IsApproved { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        public string Role { get; set; }
        public ICollection<BorrowingHistory> BorrowingHistories { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

    }
}