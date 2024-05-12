using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.core.DTO
{
    public class UpdateBookDTO 
    {
        public IFormFile? Poster { get; set; }
       public IFormFile? Pdf { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public string? RackNumber { get; set; }
        public string? AvailableQuantity { get; set; }

    }
}
