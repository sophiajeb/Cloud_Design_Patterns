using System.Linq;

namespace CQRSPattern
{
    public class BookCommandService
    {
        public void CreateBook(BookDetails book)
        {
            CommandDataStore.BookDetails.Add(book);

            //update the read datastore
            ConsitencyWorker.Create(book);
        }

        public void ChangeBookYear(int id, int year)
        {
            BookDetails book = 
                CommandDataStore.BookDetails.Single(c => c.Id == id);
            if(book != null)
            {
                book.Year = year;
            }

            //update the read datastore
            ConsitencyWorker.UpdateYear(id, year);
        }

        public void DeleteBook(int id)
        {
            CommandDataStore.BookDetails
                .Remove(CommandDataStore.BookDetails.Single(c => c.Id == id));

            //update the read datastore
            ConsitencyWorker.Delete(id);
        }
    }
}
