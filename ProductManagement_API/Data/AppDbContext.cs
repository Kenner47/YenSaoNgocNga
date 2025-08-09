using Microsoft.EntityFrameworkCore;
using ProductManagement_API.Models.Entities;

namespace ProductManagement_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);

                // Configure decimal precision
                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);

                // Configure string properties
                entity.Property(p => p.ProductName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Description)
                      .HasMaxLength(1000);

                entity.Property(p => p.ProductType)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Origin)
                      .HasMaxLength(100);

                entity.Property(p => p.Grade)
                      .HasMaxLength(10);

                entity.Property(p => p.ImageUrl)
                      .HasMaxLength(500);

                // Relationships
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Inventory)
                      .WithOne(i => i.Product)
                      .HasForeignKey<Inventory>(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Indexes for performance
                entity.HasIndex(p => p.ProductName)
                      .HasDatabaseName("IX_Products_ProductName");

                entity.HasIndex(p => p.CategoryId)
                      .HasDatabaseName("IX_Products_CategoryId");

                entity.HasIndex(p => p.IsActive)
                      .HasDatabaseName("IX_Products_IsActive");

                entity.HasIndex(p => new { p.ProductType, p.Origin })
                      .HasDatabaseName("IX_Products_Type_Origin");
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);

                entity.Property(c => c.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Description)
                      .HasMaxLength(500);

                // Unique constraint on CategoryName
                entity.HasIndex(c => c.CategoryName)
                      .IsUnique()
                      .HasDatabaseName("IX_Categories_CategoryName_Unique");

                entity.HasIndex(c => c.IsActive)
                      .HasDatabaseName("IX_Categories_IsActive");
            });

            // Inventory Configuration
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(i => i.InventoryId);

                entity.Property(i => i.UpdatedBy)
                      .HasMaxLength(100);

                entity.Property(i => i.Notes)
                      .HasMaxLength(1000);

                // Unique constraint on ProductId (1-1 relationship)
                entity.HasIndex(i => i.ProductId)
                      .IsUnique()
                      .HasDatabaseName("IX_Inventories_ProductId_Unique");

                // Index for low stock queries
                entity.HasIndex(i => new { i.Quantity, i.MinStockLevel })
                      .HasDatabaseName("IX_Inventories_Stock_Levels");
            });

            // Seed initial data
            SeedInitialData(modelBuilder);
        }

        private void SeedInitialData(ModelBuilder modelBuilder)
        {
            // 👈 SỬA: Dùng static datetime thay vì DateTime.UtcNow
            var staticDateTime = new DateTime(2025, 8, 9, 0, 0, 0, DateTimeKind.Utc);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Yến Sào Thô",
                    Description = "Yến sào tự nhiên chưa qua chế biến, giữ nguyên hình dạng ban đầu",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Yến Sào Chưng",
                    Description = "Yến sào đã được chưng sẵn, tiện lợi cho việc sử dụng",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Yến Sào Tinh Chế",
                    Description = "Yến sào đã được tinh chế và làm sạch, chất lượng cao",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Phụ Kiện Chưng Yến",
                    Description = "Các phụ kiện hỗ trợ chế biến yến sào như chén chưng, đường phèn",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                }
            );

            // Seed sample Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    ProductName = "Yến Sào Thô Khánh Hòa Hạng A 100g",
                    Description = "Yến sào thô cao cấp từ Khánh Hòa, hạng A, trọng lượng 100g. Sản phẩm tự nhiên 100%, không tẩy trắng, giàu dinh dưỡng.",
                    Price = 2500000m,
                    CategoryId = 1,
                    ProductType = "Thô",
                    Origin = "Khánh Hòa",
                    Weight = 100,
                    Grade = "A",
                    ImageUrl = "/images/products/yen-tho-kh-a-100g.jpg",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Yến Sào Chưng Ninh Thuận Hạng B 50g",
                    Description = "Yến sào chưng sẵn từ Ninh Thuận, hạng B, trọng lượng 50g. Tiện lợi, dễ sử dụng.",
                    Price = 800000m,
                    CategoryId = 2,
                    ProductType = "Chưng",
                    Origin = "Ninh Thuận",
                    Weight = 50,
                    Grade = "B",
                    ImageUrl = "/images/products/yen-chung-nt-b-50g.jpg",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Yến Sào Tinh Chế Phú Yên Hạng A 250g",
                    Description = "Yến sào tinh chế cao cấp từ Phú Yên, hạng A, trọng lượng 250g. Đã được làm sạch hoàn toàn.",
                    Price = 5500000m,
                    CategoryId = 3,
                    ProductType = "Tinh Chế",
                    Origin = "Phú Yên",
                    Weight = 250,
                    Grade = "A",
                    ImageUrl = "/images/products/yen-tinh-che-py-a-250g.jpg",
                    IsActive = true,
                    CreatedAt = staticDateTime,
                    UpdatedAt = staticDateTime
                }
            );

            // Seed sample Inventories
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory
                {
                    InventoryId = 1,
                    ProductId = 1,
                    Quantity = 50,
                    MinStockLevel = 10,
                    UpdatedBy = "System",
                    Notes = "Initial inventory setup",
                    UpdatedAt = staticDateTime
                },
                new Inventory
                {
                    InventoryId = 2,
                    ProductId = 2,
                    Quantity = 30,
                    MinStockLevel = 5,
                    UpdatedBy = "System",
                    Notes = "Initial inventory setup",
                    UpdatedAt = staticDateTime
                },
                new Inventory
                {
                    InventoryId = 3,
                    ProductId = 3,
                    Quantity = 20,
                    MinStockLevel = 3,
                    UpdatedBy = "System",
                    Notes = "Initial inventory setup",
                    UpdatedAt = staticDateTime
                }
            );
        }
    }
}