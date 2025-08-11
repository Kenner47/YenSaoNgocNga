using Microsoft.EntityFrameworkCore;
using OrderManagement_API.Data;
using OrderManagement_API.Models.Entities;
using OrderManagement_API.Repositories.IRepository;

namespace OrderManagement_API.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDetail?> GetByIdAsync(int id)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderDetailId == id);
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .OrderByDescending(od => od.OrderDetailId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByProductIdAsync(int productId)
        {
            return await _context.OrderDetails
                .Include(od => od.Order)
                .Where(od => od.ProductId == productId)
                .OrderByDescending(od => od.OrderDetailId)
                .ToListAsync();
        }

        public async Task<OrderDetail> CreateAsync(OrderDetail orderDetail)
        {
            // Tự động tính subtotal
            orderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice;

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<OrderDetail> UpdateAsync(OrderDetail orderDetail)
        {
            // Tự động tính lại subtotal
            orderDetail.Subtotal = orderDetail.Quantity * orderDetail.UnitPrice;

            _context.Entry(orderDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return false;

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.OrderDetails.AnyAsync(od => od.OrderDetailId == id);
        }
    }
}