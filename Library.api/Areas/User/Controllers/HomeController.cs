using Library.core;
using Library.core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.api.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [HttpGet("SearchByBookName/{title}")]
        public IActionResult SearchByBookName(string title)
        {
            return Ok(unitOfWork.Books.Search(b => b.Title.Contains(title.ToString())));
        }
        [HttpGet("SearchByRackNumber/{rackNumber}")]
        public IActionResult SearchByRackNumber(int rackNumber)
        {
            return Ok(unitOfWork.Books.Search(b => b.RackNumber == rackNumber));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var books = unitOfWork.Books.GetAll();
            var bookDTOs = new List<BookDTO>();

            foreach (var book in books)
            {
                var bookDTO = new BookDTO
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    ISBN = book.ISBN,
                    RackNumber = book.RackNumber,
                    AvailableQuantity = book.AvailableQuantity,
                    Poster = Convert.ToBase64String(book.Poster),
                    Pdf = Convert.ToBase64String(book.Pdf)
                };

                bookDTOs.Add(bookDTO);
            }

            return Ok(bookDTOs);
        }

    }
}
