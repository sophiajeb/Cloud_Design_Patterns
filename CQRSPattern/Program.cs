using System;
using System.Collections.Generic;

namespace CQRSPattern
{
    class Program
    {
        private static BookCommandService _commandService
            = new BookCommandService();
        private static BookQueryService _queryService
            = new BookQueryService();

        static void Main(string[] args)
        {
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true);
            StorageService storageService = new StorageService();

            Console.WriteLine("Press Key to create a book...");
            Console.ReadLine();

            _commandService.CreateBook(new BookDetails
            {
                Id = 1,
                Title = @"Bridget Jones Diary",
                Author = "Sharon Maguire",
                Description = @"Bridget Jones Diary is a 1996 novel by Helen Fielding. Written in the form of a personal diary, the novel chronicles a year in the life of Bridget Jones, a thirty-something single working woman living in London. She writes about her career, self-image, vices, family, friends, and romantic relationships.",
                Barcode = "OL26122694M",
                Year = 1996, //the copyright date
                Price = 10.00
            });

            _commandService.CreateBook(new BookDetails
            {
                Id = 2,
                Title = "Harry Potter and the Deathly Hallows",
                Author = "J. K. Rowling",
                Description = "Harry Potter is leaving Privet Drive for the last time. \n  "
                            + "But as he climbs into the sidecar of Hagrid’s motorbike and they take to the skies,\n "
                            + " he knows Lord Voldemort and the Death Eaters will not be far behind.\n "
                            + "The protective charm that has kept him safe until now is broken.\n "
                            + "But the Dark Lord is breathing fear into everything he loves. \n "
                            + "And he knows he can’t keep hiding. To stop Voldemort, Harry knows \n"
                            + "he must find the remaining Horcruxes and destroy them.",
                Barcode = "OL32050722M",
                Year = 2018, //the copyright date
                Price = 22.00
            });

            Console.WriteLine("Press Key to read the first book info...");
            Console.ReadLine();

            Book book = _queryService.GetBook(1);

            Console.WriteLine("Title: " + book.Title);
            Console.WriteLine("Copyright Year: " + book.Year);
            Console.WriteLine("Price: " + book.Price);

            Console.WriteLine("Press Key to update book year...");
            Console.ReadLine();

            _commandService.ChangeBookYear(book.Id, 2007);

            Console.WriteLine("Press Key to get books from 2007...");
            Console.ReadLine();

            List<Book> books = _queryService.GetBooksFromYear(2007);
            foreach (var b in books)
            {
                Console.WriteLine("2007 Book Title: " + b.Title);
            }

            Console.ReadLine();

            Console.WriteLine("Storing the books to the database!");
            storageService.StoreBook(book);
            storageService.StoreBook(_queryService.GetBook(2));

            //using (var db = new BookContext())
            //{
            //    Console.WriteLine("Storing the books to the database!");
            //    db.books.Add(book);
            //    db.books.Add(_queryService.GetBook(2));
            //    db.SaveChanges();
            //}

            storageService.DisconnectStorageService();
        }
    }
}
