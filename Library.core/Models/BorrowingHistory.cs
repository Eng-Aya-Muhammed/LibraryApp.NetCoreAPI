using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.core.Models
{
    public class BorrowingHistory
    {
        public int BorrowingHistoryId { get; set; }
        public string UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public User User { get; set; } 
        public Book Book { get; set; }
    }
}
