using repository.core.Interfaces;
using Library.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Library.core.DTO;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Library.core;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace repository.EF.Repositories
{
    public class BooksRepository : BaseRepository<BorrowingHistory> ,IBooksRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public BooksRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context) {
            _httpContextAccessor = httpContextAccessor;

        }

        public string BorrowBook(BorrowBookRequestDTO borrowBookRequest)
        {
            var book = _context.Books.FirstOrDefault(b => b.BookId == borrowBookRequest.BookId);
                        
        if (book == null)
            {
                return "Request is not correct";
            }

            if (book.AvailableQuantity <= 0)
            {
                return "The book is not available for borrowing";
            }

            var id = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.PrimarySid))!.Value;

            var borrowedBooksCount = _context.BorrowingHistories.Count(b => b.UserId == id);
            if (borrowedBooksCount >= 5)
            {
                return "You have reached the maximum borrowing limit of 5 books.";
            }

            book.AvailableQuantity--;

            var borrowingEntry = new BorrowingHistory
            {
                UserId = id,
                BookId = borrowBookRequest.BookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };

            _context.BorrowingHistories.Add(borrowingEntry);

            return "Borrowed";
        }
        public List<Book> GetUserBooks( )
        {
            var id = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.PrimarySid))!.Value;

            var books = _context.BorrowingHistories
        .Include(b => b.Book)
        .Include(u => u.User)
        .Where(u => u.User.Id == id).Select(b=>b.Book)
        .ToList();

            return books;
        }


        public string ReturnBook(BorrowBookRequestDTO borrowBookRequest)
        {
            var UserId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(u => u.Type.Equals(ClaimTypes.PrimarySid))!.Value;

            var borrowingEntry = _context.BorrowingHistories.FirstOrDefault(
                entry => entry.UserId == UserId &&
                         entry.BookId == borrowBookRequest.BookId &&
                         entry.ReturnDate == null);

            if (borrowingEntry == null)
            {
                return ("No borrowing history found for the specified user and book.");
            }

            borrowingEntry.ReturnDate = DateTime.Now;

            var book = _context.Books.FirstOrDefault(b => b.BookId == borrowBookRequest.BookId);
            if (book != null)
            {
                book.AvailableQuantity++;
            }

            _context.BorrowingHistories.Remove(borrowingEntry); 


            return ("Book returned successfully.");
        }
        public void CheckOverdueBooks()
        {
            var overdueBooks = _context.BorrowingHistories
            .Where(b => b.DueDate.Date < DateTime.Now.Date && b.ReturnDate == null)
            .ToList();


            foreach (var borrowingEntry in overdueBooks)
            {
                var user = _context.Users.FirstOrDefault(u => u.Id.Equals(borrowingEntry.UserId));
                if (user != null)
                {
                    var borrowedBook = user.BorrowingHistories.FirstOrDefault(b => b.BookId == borrowingEntry.BookId);
                    if (borrowedBook != null)
                    {
                        user.BorrowingHistories.Remove(borrowedBook);
                    }
                }

                var book = _context.Books.FirstOrDefault(b => b.BookId == borrowingEntry.BookId);
                if (book != null)
                {
                    book.AvailableQuantity++;
                }


                
            }
        }
        public List<AvailableBookReportDTO> GenerateAvailableBooksReport()
        {
            var availableBooks = _context.Books
                .Where(b => b.AvailableQuantity > 0)
                .Select(b => new AvailableBookReportDTO
                {
                    BookTitle = b.Title,
                    Author = b.Author,
                    AvailableQuantity = b.AvailableQuantity
                })
                .ToList();

            return availableBooks;
        }

        
        

    }
}
