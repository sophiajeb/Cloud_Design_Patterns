using CQRSPattern;
using EventSourcingPattern;
using RetryPattern;
using System;
using System.Collections.Generic;
using StorageService = RetryPattern.StorageService;

namespace MainApp
{
    class Program
    {
        private static BookCommandService _commandService
            = new BookCommandService();
        private static BookQueryService _queryService
            = new BookQueryService();

        static void Main(string[] args)
        {
            CQRSPattern();
            EventSourcingPattern();
            RetryPattern();
        }

        static void CQRSPattern()
        {
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true);
            StorageService storageService = new StorageService();

            Console.WriteLine("Press Key to create a book...");
            Console.ReadLine();

            storageService.clearBooks();

            _commandService.CreateBook(new BookDetails
            {
                Id = 1,
                Title = "Bridget Jones Diary",
                Author = "Sharon Maguire",
                Description = "Bridget Jones Diary is a 1996 novel by Helen Fielding. Written in the form of a personal diary, the novel chronicles a year in the life of Bridget Jones, a thirty-something single working woman living in London. She writes about her career, self-image, vices, family, friends, and romantic relationships.",
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

            Console.WriteLine("Storing the books to the database!");
            storageService.StoreBook(book);
            storageService.StoreBook(_queryService.GetBook(2));

            Console.ReadLine();
        }

        static void EventSourcingPattern()
        {
            using (EventStore store = new EventStore())
            {
                //create a new streamID. This wil be used for partitioning
                Guid customerId = Guid.NewGuid();

                Book book1 = _queryService.GetBook(1);
                Book book2 = _queryService.GetBook(2);

                //create things that happened
                var rentevent1 = new BookRentalCommand { BookPrice = book1.Price, EventId = Guid.NewGuid() };
                var rentevent2 = new BookRentalCommand { BookPrice = book2.Price, EventId = Guid.NewGuid() };

                // customer brings one of the books he rented earlier, so we refund
                var returnevent1 = new BookReturnCommand { BookPrice = -10, EventId = Guid.NewGuid() };

                Console.WriteLine("customer with Id : " + customerId + " rent two books");
                Console.WriteLine("First book Title : " + book1.Title);
                Console.WriteLine("First book price : " + book1.Price);
                Console.WriteLine("Second book Title : " + book2.Title);
                Console.WriteLine("Second book price : " + book2.Price);
                double sum = book1.Price + book2.Price;
                Console.WriteLine("Sum of books that he paid " + sum);
                Console.WriteLine("Customer brings one of the books he rented earlier, so we refund");


                //commit those things
                store.AppendToStream(customerId, rentevent1);
                store.AppendToStream(customerId, rentevent2);
                store.AppendToStream(customerId, returnevent1);

                //replay the events and get the current state of the data
                double stateOfData = store.GetBalance(customerId);

                Console.WriteLine("Balance: " + stateOfData);
                Console.ReadLine();
            }

        }

        static void RetryPattern()
        {
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableRetryLogic", true);
            int counter = 10;
            StorageService storageService = new StorageService();
            
            while (counter > 0)
            {
                int result = storageService.ReadWriteToRemoteStorage(counter);
                Console.WriteLine(result);
                counter--;
            }

            storageService.DisconnectStorageService();
            Console.ReadLine();
        }
    }
        
}
