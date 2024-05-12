using repository.core;
using repository.core.Interfaces;
using Library.core.Models;
using repository.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.core;
using Library.core.Interfaces;
using Library.EF.Repositories;
using Library.core.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace repository.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private readonly ApplicationDbContext _context;

        public IBaseRepository<Book> Books { get; private set; }
        public IBooksRepository BorrowedBooks { get; private set; }
        

        public UnitOfWork(ApplicationDbContext context , IHttpContextAccessor httpContextAccessor
            )
        {
            _context = context;
            
            _httpContextAccessor = httpContextAccessor;
            Books = new BaseRepository<Book>(_context);
            BorrowedBooks = new BooksRepository(_context , _httpContextAccessor);

        }



        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();

        }
    }
}
