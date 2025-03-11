namespace OnlineClothing.Models.MoMo
{
    public class MoMoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public string Currency { get; set; }
        public int Quantity { get; set; }
        public long TotalPrice { get; set; }
    }
}
