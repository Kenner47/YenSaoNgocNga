using ProductManagement_API.Models.DTOs;
using ProductManagement_API.Models.Entities;
using ProductManagement_API.Repositories.IRepository;
using ProductManagement_API.Services.IService;

namespace ProductManagement_API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<InventoryDto?> GetInventoryByIdAsync(int id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null) return null;

            return MapToDto(inventory);
        }

        public async Task<InventoryDto?> GetInventoryByProductIdAsync(int productId)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory == null) return null;

            return MapToDto(inventory);
        }

        public async Task<IEnumerable<InventoryDto>> GetAllInventoriesAsync()
        {
            var inventories = await _inventoryRepository.GetAllAsync();
            return inventories.Select(MapToDto);
        }

        public async Task<IEnumerable<InventoryDto>> GetLowStockInventoriesAsync()
        {
            var inventories = await _inventoryRepository.GetLowStockAsync();
            return inventories.Select(MapToDto);
        }

        public async Task<InventoryDto> CreateInventoryAsync(int productId, int initialStock)
        {
            var inventory = new Inventory
            {
                ProductId = productId,
                Quantity = initialStock,
                MinStockLevel = 5, // Default
                UpdatedBy = "System",
                Notes = "Initial inventory creation",
                UpdatedAt = DateTime.UtcNow
            };

            var createdInventory = await _inventoryRepository.CreateAsync(inventory);
            return MapToDto(createdInventory);
        }

        public async Task<InventoryDto?> UpdateInventoryAsync(int id, UpdateInventoryDto updateDto)
        {
            var existingInventory = await _inventoryRepository.GetByIdAsync(id);
            if (existingInventory == null) return null;

            existingInventory.Quantity = updateDto.Quantity;
            existingInventory.MinStockLevel = updateDto.MinStockLevel ?? existingInventory.MinStockLevel;
            existingInventory.Notes = updateDto.Notes;
            existingInventory.UpdatedBy = updateDto.UpdatedBy;

            var updatedInventory = await _inventoryRepository.UpdateAsync(existingInventory);
            return MapToDto(updatedInventory);
        }

        public async Task<bool> AdjustStockAsync(int productId, AdjustInventoryDto adjustDto)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory == null) return false;

            inventory.Quantity += adjustDto.Adjustment;
            inventory.Notes = adjustDto.Reason;
            inventory.UpdatedBy = adjustDto.UpdatedBy;

            await _inventoryRepository.UpdateAsync(inventory);
            return true;
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            return await _inventoryRepository.DeleteAsync(id);
        }


        // Business methods
        public async Task<bool> CanSellAsync(int productId, int requestedQuantity)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            return inventory != null && inventory.Quantity >= requestedQuantity;
        }

        public async Task<string> GetStockStatusAsync(int productId)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory == null) return "N/A";

            return GetStockStatus(inventory);
        }

        private static InventoryDto MapToDto(Inventory inventory)
        {
            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                MinStockLevel = inventory.MinStockLevel,
                IsLowStock = inventory.Quantity <= inventory.MinStockLevel,
                IsOutOfStock = inventory.Quantity <= 0,
                StockStatus = GetStockStatus(inventory),
                LastUpdated = inventory.UpdatedAt,
                UpdatedBy = inventory.UpdatedBy,
                Notes = inventory.Notes
            };
        }

        private static string GetStockStatus(Inventory inventory)
        {
            if (inventory.Quantity <= 0) return "Hết hàng";
            if (inventory.Quantity <= inventory.MinStockLevel) return "Sắp hết";
            return "Còn hàng";
        }
    }
}