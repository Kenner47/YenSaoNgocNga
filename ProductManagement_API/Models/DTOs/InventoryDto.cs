namespace ProductManagement_API.Models.DTOs
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
