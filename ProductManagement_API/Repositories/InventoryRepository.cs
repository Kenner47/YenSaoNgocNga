using Microsoft.EntityFrameworkCore;
using ProductManagement_API.Data;
using ProductManagement_API.Models.Entities;
using ProductManagement_API.Repositories.IRepository;

namespace ProductManagement_API.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.InventoryId == id);
        }

        public async Task<Inventory?> GetByProductIdAsync(int productId)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .OrderByDescending(i => i.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Where(i => i.Quantity <= i.MinStockLevel)
                .OrderBy(i => i.Quantity)
                .ToListAsync();
        }

        public async Task<Inventory> CreateAsync(Inventory inventory)
        {
            inventory.UpdatedAt = DateTime.UtcNow;

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(inventory.InventoryId) ?? inventory;
        }

        public async Task<Inventory> UpdateAsync(Inventory inventory)
        {
            inventory.UpdatedAt = DateTime.UtcNow;
            _context.Inventories.Update(inventory);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(inventory.InventoryId) ?? inventory;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null) return false;

            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Inventories.AnyAsync(i => i.InventoryId == id);
        }

        public async Task<bool> ExistsByProductIdAsync(int productId)
        {
            return await _context.Inventories.AnyAsync(i => i.ProductId == productId);
        }
    }
}