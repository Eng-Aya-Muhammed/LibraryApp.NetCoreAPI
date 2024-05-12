using Library.core;
using Library.core.DTO;
using Library.core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library.api.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BorrowBookController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public BorrowBookController(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("borrow-book")]
        public IActionResult BorrowBook([FromBody] BorrowBookRequestDTO borrowBookRequest)
        {
            string result =  unitOfWork.BorrowedBooks.BorrowBook(borrowBookRequest);

            if (result == "Borrowed")
            {
                unitOfWork.CompleteAsync().Wait();
                return Ok("Book borrowed successfully");
            }
            else if (result == "You have reached the maximum borrowing limit of 5 books.")
            {
                return BadRequest(result);
            }
            else
            {
                return NotFound(result);
            }
        }
        [HttpPost("return-book")]
        public IActionResult ReturnBook([FromBody] BorrowBookRequestDTO borrowBookRequest)
        {
            string result = unitOfWork.BorrowedBooks.ReturnBook(borrowBookRequest);

            if (result == "Book returned successfully.")
            {
                 unitOfWork.CompleteAsync().Wait();

                return Ok("Book returned successfully");
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpGet("GetUserBooks")]
        public IActionResult GetUserBooks()
        {
            var books = unitOfWork.BorrowedBooks.GetUserBooks();
            return Ok(books);
        }
        [HttpPost("CheckOverdueBooks")]
        public IActionResult CheckOverdueBooks()
        {
            try
            {
               unitOfWork.BorrowedBooks.CheckOverdueBooks();
                 unitOfWork.CompleteAsync().Wait();

                return Ok("Overdue books checked successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error checking overdue books: {ex.Message}");
            }
        }
        
    }
}
