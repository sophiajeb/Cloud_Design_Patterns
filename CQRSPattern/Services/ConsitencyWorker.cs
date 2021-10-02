using System.Linq;

namespace CQRSPattern
{
    public static class ConsitencyWorker
    {
        public static void Create(BookDetails book)
        {
            QueryDataStore.Books.Add(
                new Book
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    Year = book.Year
                }
            ) ;
        }

        public static void UpdateYear(int id, int year)
        {
            Book book = QueryDataStore.Books.Single(c => c.Id == id);
            if (book != null)
            {
                book.Year = year;
            }
        }

        public static void Delete(int id)
        {
            QueryDataStore.Books
                .Remove(QueryDataStore.Books.Single(c => c.Id == id));
        }
    }
}
