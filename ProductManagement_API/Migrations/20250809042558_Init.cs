using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductManagement_API.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Grade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MinStockLevel = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "CreatedAt", "Description", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Yến Sào Thô", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yến sào tự nhiên chưa qua chế biến, giữ nguyên hình dạng ban đầu", true, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Yến Sào Chưng", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yến sào đã được chưng sẵn, tiện lợi cho việc sử dụng", true, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Yến Sào Tinh Chế", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yến sào đã được tinh chế và làm sạch, chất lượng cao", true, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "Phụ Kiện Chưng Yến", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Các phụ kiện hỗ trợ chế biến yến sào như chén chưng, đường phèn", true, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "Description", "Grade", "ImageUrl", "IsActive", "Origin", "Price", "ProductName", "ProductType", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yến sào thô cao cấp từ Khánh Hòa, hạng A, trọng lượng 100g. Sản phẩm tự nhiên 100%, không tẩy trắng, giàu dinh dưỡng.", "A", "/images/products/yen-tho-kh-a-100g.jpg", true, "Khánh Hòa", 2500000m, "Yến Sào Thô Khánh Hòa Hạng A 100g", "Thô", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), 100 },
                    { 2, 2, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yến sào chưng sẵn từ Ninh Thuận, hạng B, trọng lượng 50g. Tiện lợi, dễ sử dụng.", "B", "/images/products/yen-chung-nt-b-50g.jpg", true, "Ninh Thuận", 800000m, "Yến Sào Chưng Ninh Thuận Hạng B 50g", "Chưng", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), 50 },
                    { 3, 3, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Yến sào tinh chế cao cấp từ Phú Yên, hạng A, trọng lượng 250g. Đã được làm sạch hoàn toàn.", "A", "/images/products/yen-tinh-che-py-a-250g.jpg", true, "Phú Yên", 5500000m, "Yến Sào Tinh Chế Phú Yên Hạng A 250g", "Tinh Chế", new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), 250 }
                });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "InventoryId", "MinStockLevel", "Notes", "ProductId", "Quantity", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 10, "Initial inventory setup", 1, 50, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, 5, "Initial inventory setup", 2, 30, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, 3, "Initial inventory setup", 3, 20, new DateTime(2025, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName_Unique",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsActive",
                table: "Categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_ProductId_Unique",
                table: "Inventories",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_Stock_Levels",
                table: "Inventories",
                columns: new[] { "Quantity", "MinStockLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductName",
                table: "Products",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Type_Origin",
                table: "Products",
                columns: new[] { "ProductType", "Origin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
