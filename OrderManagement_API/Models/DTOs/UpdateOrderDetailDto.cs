using System.ComponentModel.DataAnnotations;

namespace OrderManagement_API.Models.DTOs
{
    public class UpdateOrderDetailDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int? Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal? UnitPrice { get; set; }
    }
}