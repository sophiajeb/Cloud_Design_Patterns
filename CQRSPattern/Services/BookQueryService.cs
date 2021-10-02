using System.Collections.Generic;
using System.Linq;

namespace CQRSPattern
{
    public class BookQueryService
    {
        public Book GetBook(int id)
        {
            return QueryDataStore.Books.Single(c => c.Id == id);
        }

        public List<Book> GetBooksFromYear(int year)
        {
            return QueryDataStore.Books.Where(c => c.Year.Equals(year)).ToList();
        }
    }
}
