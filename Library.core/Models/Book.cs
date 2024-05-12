using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Library.core.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int RackNumber { get; set; }
        public int AvailableQuantity { get; set; }

        public byte[] Pdf { get; set; }
        public byte[] Poster { get; set; }
        public ICollection<BorrowingHistory> BorrowingHistories { get; set; }
    }
}
