namespace OnlineClothing.Models
{
    public class HomeViewModel
    {
        public List<Product> hotProducts { get; set; } = new List<Product>();
        public List<Product> saleProducts { get; set; } = new List<Product>();
    }
}
