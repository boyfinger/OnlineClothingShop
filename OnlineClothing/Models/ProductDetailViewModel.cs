namespace OnlineClothing.Models
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; } 
        public List<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
