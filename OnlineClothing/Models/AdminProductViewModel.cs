namespace OnlineClothing.Models
{
    public class AdminProductViewModel
    {
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid? SellerId { get; set; }
        public string? SellerName { get; set; } 
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; } 
        public string? ThumbnailUrl { get; set; }
        public string? Description { get; set; }
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public decimal? FinalPrice => Price - (Price * Discount / 100);
        public int? Quantity { get; set; }
        public string? StatusName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? Status { get; set; }
        public string? RejectionReason { get; set; }
    }

}
