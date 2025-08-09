using System.ComponentModel.DataAnnotations;

namespace ProductManagement_API.Models.Entities
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        public int Quantity { get; set; }

        [Range(0, 1000, ErrorMessage = "Ngưỡng cảnh báo phải từ 0-1000")]
        public int MinStockLevel { get; set; } = 10; // Ngưỡng cảnh báo hết hàng

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string UpdatedBy { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        // Navigation Properties
        public Product Product { get; set; } = null!;

        // Helper Properties
        public bool IsLowStock => Quantity <= MinStockLevel;
        public bool IsOutOfStock => Quantity <= 0;
        public bool CanSell(int requestedQuantity) => Quantity >= requestedQuantity;
        public string StockStatus => IsOutOfStock ? "Hết hàng" : IsLowStock ? "Sắp hết" : "Còn hàng";
    }
}