using System;
using CQRSPattern;

namespace EventSourcingPattern
{
    class Program
    {
        private static BookCommandService _commandService
            = new BookCommandService();

        private static BookQueryService _queryService
            = new BookQueryService();

        static void Main(string[] args)
        {
            using (EventStore store = new EventStore())
            {
                //create a new streamID. This wil be used for partitioning
                Guid customerId = Guid.NewGuid();

                _commandService.CreateBook(new BookDetails
                {
                    Id = 1,
                    Title = "Bridget Jones's Diary",
                    Author = "Sharon Maguire",
                    Description = "Bridget Jones's Diary is a 1996 novel by Helen Fielding. Written in the form of a personal diary, the novel chronicles a year in the life of Bridget Jones, a thirty-something single working woman living in London. She writes about her career, self-image, vices, family, friends, and romantic relationships.",
                    Barcode = "OL26122694M",
                    Year = 1996, //the copyright date
                    Price = 10.00
                });
                Book book1 = _queryService.GetBook(1);

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
                Book book2 = _queryService.GetBook(2);

                //create things that happened
                var rentevent1 = new BookRentalCommand { BookPrice = book1.Price, EventId = Guid.NewGuid() };
                var rentevent2 = new BookRentalCommand { BookPrice = book2.Price, EventId = Guid.NewGuid() };
                
                // customer brings one of the books he rented earlier, so we refund
                var returnevent1 = new BookReturnCommand { BookPrice = -10, EventId = Guid.NewGuid() };
                

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
    }
}
