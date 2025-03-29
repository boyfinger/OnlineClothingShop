namespace OnlineClothing.Models.ViewModelsOfAdminDashboard
{
    public class SellerRevenueVM
    {
        public Guid? SellerId { get; set; }
        public string? SellerName { get; set; }
        public int? Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}
