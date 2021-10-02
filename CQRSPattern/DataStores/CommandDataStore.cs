using System.Collections.Generic;

namespace CQRSPattern
{
    public static class CommandDataStore
    {
        static CommandDataStore()
        {
            BookDetails = new List<BookDetails>();
        }
        public static List<BookDetails> BookDetails { get; set; }
    }
}
