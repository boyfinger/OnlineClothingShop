﻿namespace OnlineClothing.Models
{
    public class ProductViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<ProductStatus> ProductStatuses { get; set; } = new List<ProductStatus>();
        public int[] ProductStatusCases = { 1, 2 };
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchQuery { get; set; }
    }
}
