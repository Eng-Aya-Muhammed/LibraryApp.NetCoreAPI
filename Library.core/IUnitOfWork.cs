using repository.core.Interfaces;
using Library.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.core.Interfaces;

namespace Library.core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Book> Books { get; }
        IBooksRepository BorrowedBooks { get;}
        Task<int> CompleteAsync();
    }
}
