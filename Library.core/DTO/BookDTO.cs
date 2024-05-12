using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.core.DTO
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int RackNumber { get; set; }

        public int AvailableQuantity { get; set; }
        public string Poster { get; set; }
        public string Pdf { get; set; }
    }
}
