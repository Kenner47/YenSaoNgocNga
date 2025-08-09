using ProductManagement_API.Models.DTOs;

namespace ProductManagement_API.Services.IService
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync();
        Task<InventoryDto?> GetInventoryByIdAsync(int id);
        Task<InventoryDto?> GetInventoryByProductIdAsync(int productId);
        Task<IEnumerable<InventoryDto>> GetLowStockInventoriesAsync();
        Task<InventoryDto> CreateInventoryAsync(int productId, int initialStock);
        Task<InventoryDto?> UpdateInventoryAsync(int id, UpdateInventoryDto updateDto);
        Task<bool> AdjustStockAsync(int productId, AdjustInventoryDto adjustDto);
        Task<bool> DeleteInventoryAsync(int id);

        // Business methods
        Task<bool> CanSellAsync(int productId, int requestedQuantity);
        Task<string> GetStockStatusAsync(int productId);
    }
}