namespace OnlineClothing.Models.ViewModelsOfAdminDashboard
{
    public class TopProductVM
    {
        public long ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? TotalSold { get; set; }
        public int? TotalRevenue { get; set; }
    }
}
