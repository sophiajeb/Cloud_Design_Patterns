using System.Data.Entity;

namespace CQRSPattern
{
    public class BookContext: DbContext
    {
        public BookContext() : base("name=RemoteSQLStorageConnectionString")
        { 
        }

        public DbSet<Book> books { get; set; }
    }

    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Year { get; set; } // the copyright date

        public double Price { get; set; }
    }
}
