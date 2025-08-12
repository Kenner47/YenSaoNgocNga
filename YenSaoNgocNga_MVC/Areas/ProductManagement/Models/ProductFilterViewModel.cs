namespace YenSaoNgocNga_MVC.Areas.ProductManagement.Models
{
    public class ProductFilterViewModel
    {
        public string SearchQuery { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string SortBy { get; set; } = "name"; // name, price, newest
        public string SortOrder { get; set; } = "asc"; // asc, desc
    }
}