namespace EventSourcingPattern
{
    public class BookReturnCommand : Command
    {
        public double BookPrice { get; set; }
    }
}
