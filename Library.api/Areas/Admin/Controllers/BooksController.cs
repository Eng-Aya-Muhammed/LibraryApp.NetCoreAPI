using Library.core.Models;
using Library.core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.core.DTO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Library.api.Areas.Admin.Controllers
{
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize(Roles = "Librarian")]

    public class BooksController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        


        private List<string> allowedImageExtensions = new List<string> { ".png", ".jpg" };
        private new string allowedPdfExtension = ".pdf";
        private long maxAllowedPosterSize = 1024 * 1024 *2; // 1MB
        private long maxAllowedPdfSize = 5 * 1024 * 1024; // 5MB
        public BooksController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        
        [HttpGet("GetById/{BookId}")]
        public IActionResult GetById(int BookId)
        {
            return Ok(unitOfWork.Books.GetById(i=>i.BookId == BookId));
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

        [HttpGet("AvailableBooksReport")]
        public IActionResult GetAvailableBooks()
        {
            var availableBooks = unitOfWork.BorrowedBooks.GenerateAvailableBooksReport();
            return Ok(availableBooks);
        }
        
        [HttpGet("SearchByISBN/{ISBN}")]
        public IActionResult SearchByISBN(string ISBN)
        {
            return Ok(unitOfWork.Books.Search(b => b.ISBN.Contains(ISBN.ToString())));
        }
        [HttpGet("SortByISBN")]
        public IActionResult SortByISBN()
        {
            return Ok(unitOfWork.Books.Sort(b=>b.ISBN));
        }
        
        [HttpGet("SearchByRackNumber/{RackNumber}")]
        public IActionResult SearchByRackNumber(int RackNumber)
        {
            var books = unitOfWork.Books.Search(b => b.RackNumber == RackNumber);
            return Ok(books);
        }
       
        [HttpGet("SortByRackNumber")]
        public IActionResult SortByRackNumber()
        {
            return Ok(unitOfWork.Books.Sort(b => b.RackNumber));
        }
        
        
        [HttpPost("AddOne")]
        public async Task<IActionResult> AddOne([FromForm] AddBookDTO bookDTO)
        {
            if (bookDTO.Poster == null || !allowedImageExtensions.Contains(Path.GetExtension(bookDTO.Poster.FileName).ToLower()))
                return BadRequest("Poster image is required and must be a .png or .jpg file!");

            if (bookDTO.Pdf == null)
                return BadRequest("PDF file is required!");

            if (Path.GetExtension(bookDTO.Pdf.FileName).ToLower() != allowedPdfExtension)
                return BadRequest("PDF file must be a .pdf file!");

            if (bookDTO.Poster.Length > maxAllowedPosterSize)
                return BadRequest("Max allowed size for the poster image is 1MB!");

            if (bookDTO.Pdf.Length > maxAllowedPdfSize)
                return BadRequest("Max allowed size for the PDF file is 5MB!");

            using var posterStream = new MemoryStream();
            using var pdfStream = new MemoryStream();

            await bookDTO.Poster.CopyToAsync(posterStream);
            await bookDTO.Pdf.CopyToAsync(pdfStream);

            var book = new Book
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                ISBN = bookDTO.ISBN,
                RackNumber =(int) bookDTO.RackNumber,
                AvailableQuantity =(int) bookDTO.AvailableQuantity,
                Poster = posterStream.ToArray(),
                Pdf = pdfStream.ToArray()
            };

            try
            {
                 unitOfWork.Books.Add(book);
                await unitOfWork.CompleteAsync();
                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the book: {ex.Message}");
            }
        }
        

        [HttpPost("UpdateOne/{bookId}")]
        public async Task<IActionResult> UpdateOne(int bookId, [FromForm] UpdateBookDTO BookDTO)
        {
            var book =  unitOfWork.Books.GetById(b=>b.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }

            // Check if poster is provided and is a valid image
            if (BookDTO.Poster != null && !allowedImageExtensions.Contains(Path.GetExtension(BookDTO.Poster.FileName).ToLower()))
                return BadRequest("Poster image must be a .png or .jpg file!");

            // Check if PDF file is provided and is valid
            if (BookDTO.Pdf != null && Path.GetExtension(BookDTO.Pdf.FileName).ToLower() != allowedPdfExtension)
                return BadRequest("PDF file must be a .pdf file!");

            // Check if poster image size is within limit
            if (BookDTO.Poster != null && BookDTO.Poster.Length > maxAllowedPosterSize)
                return BadRequest("Max allowed size for the poster image is 1MB!");

            // Check if PDF file size is within limit
            if (BookDTO.Pdf != null && BookDTO.Pdf.Length > maxAllowedPdfSize)
                return BadRequest("Max allowed size for the PDF file is 5MB!");

            book.Title = !string.IsNullOrEmpty(BookDTO.Title) ? BookDTO.Title : book.Title;
            book.Author = !string.IsNullOrEmpty(BookDTO.Author) ? BookDTO.Author : book.Author;
            book.ISBN = !string.IsNullOrEmpty(BookDTO.ISBN) ? BookDTO.ISBN : book.ISBN;
            if (!string.IsNullOrEmpty(BookDTO.RackNumber) && int.TryParse(BookDTO.RackNumber, out int rackNumber))
            {
                book.RackNumber = rackNumber;
            }
            else
            {
                book.RackNumber = book.RackNumber;
            }
            if (!string.IsNullOrEmpty(BookDTO.AvailableQuantity) && int.TryParse(BookDTO.AvailableQuantity, out int AvailableQuantity))
            {
                book.AvailableQuantity = AvailableQuantity;
            }
            else
            {
                book.AvailableQuantity =book.AvailableQuantity;
            }


            if (BookDTO.Pdf != null)
            {
                using var pdfStream = new MemoryStream();
                await BookDTO.Pdf.CopyToAsync(pdfStream);
                book.Pdf = pdfStream.ToArray();
            }
            else
            {
                book.Pdf = book.Pdf;
            }
            if (BookDTO.Poster != null)
            {
                using var pdfStream = new MemoryStream();
                await BookDTO.Poster.CopyToAsync(pdfStream);
                book.Poster = pdfStream.ToArray();
            }
            else
            {
                book.Poster = book.Poster;
            }


            try
            {
                unitOfWork.Books.Update(book);
                await unitOfWork.CompleteAsync();
                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the book: {ex.Message}");
            }
        }

        [HttpPost("DeleteOne/{bookId}")]
        public async Task<IActionResult> DeleteOne(int bookId)
        {
            if (bookId <= 0)
            {
                return NotFound();
            }

            var book = unitOfWork.Books.GetById(I => I.BookId == bookId);
            if (book == null)
            {
                return NotFound();
            }

            unitOfWork.Books.Delete(book);
            await unitOfWork.CompleteAsync();

            return Ok(book);
        }

    }
}
