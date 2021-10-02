namespace EventSourcingPattern
{
    public class BookRentalCommand : Command
    {
        public double BookPrice { get; set; }
    }
}
