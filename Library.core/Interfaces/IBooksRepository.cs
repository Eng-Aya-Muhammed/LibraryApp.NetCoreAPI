using Library.core.DTO;
using Library.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repository.core.Interfaces
{
    public interface IBooksRepository : IBaseRepository<BorrowingHistory>
    {
        string BorrowBook( BorrowBookRequestDTO borrowBookRequest);
        string ReturnBook(BorrowBookRequestDTO borrowBookRequest);
        List<Book> GetUserBooks();
        List<AvailableBookReportDTO> GenerateAvailableBooksReport();
        void CheckOverdueBooks();





    }
}
