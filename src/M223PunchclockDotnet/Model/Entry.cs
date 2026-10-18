namespace M223PunchclockDotnet.Model
{
    public class Entry
    {
        public int Id { get; set; }
        
        public DateTime CheckIn { get; set; }
        
        public DateTime CheckOut { get; set; }
    }
}
