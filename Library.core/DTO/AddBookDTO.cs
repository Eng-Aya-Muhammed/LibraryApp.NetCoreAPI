using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.core.DTO
{
    public class AddBookDTO 
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int RackNumber { get; set; }

        public int AvailableQuantity { get; set; }
        public IFormFile Poster { get; set; }
        public IFormFile Pdf { get; set; }


    }
}
