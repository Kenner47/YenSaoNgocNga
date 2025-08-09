using Microsoft.AspNetCore.Mvc;
using ProductManagement_API.Models.DTOs;
using ProductManagement_API.Services.IService;

namespace ProductManagement_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAllInventories()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync();
            return Ok(inventories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryDto>> GetInventory(int id)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(id);
            if (inventory == null)
                return NotFound($"Inventory with ID {id} not found");

            return Ok(inventory);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryDto>> UpdateInventory(int id, UpdateInventoryDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var inventory = await _inventoryService.UpdateInventoryAsync(id, updateDto);
                if (inventory == null)
                    return NotFound($"Inventory with ID {id} not found");

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating inventory: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var success = await _inventoryService.DeleteInventoryAsync(id);
            if (!success)
                return NotFound($"Inventory with ID {id} not found");

            return NoContent();
        }

        [HttpGet("product/{productId}")] // Lấy thông tin kho theo sản phẩm
        public async Task<ActionResult<InventoryDto>> GetInventoryByProductId(int productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            if (inventory == null)
                return NotFound($"Inventory for product ID {productId} not found");

            return Ok(inventory);
        }

        [HttpPost("product/{productId}/adjust")] // Điều chỉnh số lượng kho theo sản phẩm
        public async Task<ActionResult> AdjustStock(int productId, AdjustInventoryDto adjustDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentInventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            if (currentInventory == null)
                return NotFound($"Inventory for product ID {productId} not found");

            if (currentInventory.Quantity + adjustDto.Adjustment < 0)
                return BadRequest($"Cannot reduce stock below 0. Current: {currentInventory.Quantity}, Adjustment: {adjustDto.Adjustment}");

            try
            {
                var success = await _inventoryService.AdjustStockAsync(productId, adjustDto);
                if (!success)
                    return NotFound($"Inventory for product ID {productId} not found");

                return Ok(new
                {
                    Message = "Stock adjusted successfully",
                    ProductId = productId,
                    Adjustment = adjustDto.Adjustment,
                    NewQuantity = currentInventory.Quantity + adjustDto.Adjustment,
                    Reason = adjustDto.Reason
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adjusting stock: {ex.Message}");
            }
        }

        [HttpGet("low-stock")] // Lấy sản phẩm trong kho có số lượng thấp
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetLowStockInventories()
        {
            var lowStockInventories = await _inventoryService.GetLowStockInventoriesAsync();
            return Ok(lowStockInventories);
        }

        [HttpGet("product/{productId}/status")] //Kiểm tra sản phẩm đó có còn hàng hay không
        public async Task<ActionResult<string>> GetStockStatus(int productId)
        {
            var status = await _inventoryService.GetStockStatusAsync(productId);
            return Ok(new { ProductId = productId, StockStatus = status });
        }

        [HttpGet("product/{productId}/can-sell/{requestedQuantity}")] // Kiểm tra sản phẩm đó có thể bán với số lượng đó không
        public async Task<ActionResult<bool>> CanSell(int productId, int requestedQuantity)
        {
            if (requestedQuantity <= 0)
                return BadRequest("Requested quantity must be greater than 0");

            var canSell = await _inventoryService.CanSellAsync(productId, requestedQuantity);
            return Ok(new
            {
                ProductId = productId,
                RequestedQuantity = requestedQuantity,
                CanSell = canSell
            });
        }

    }
}