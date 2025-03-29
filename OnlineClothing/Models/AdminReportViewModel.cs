namespace OnlineClothing.Models
{
    public class AdminReportViewModel
    {
        public long Id { get; set; } 
        public long? ProductId { get; set; }
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? ProductName { get; set; }
        public string? CategoryName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ProductDescription { get; set; } 
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public decimal? FinalPrice => Price - (Price * Discount / 100);
        public int? Quantity { get; set; }
        public string? Reason { get; set; } 
        public int? Status { get; set; }
        public string? StatusName { get; set; } 
        public DateTime? CreateAt { get; set; } 
    }

}
