using System.Collections.Generic;

namespace CQRSPattern
{
    public static class QueryDataStore
    {
        static QueryDataStore()
        {
            Books = new List<Book>();
        }

        public static List<Book> Books { get; set; }
    }
}
