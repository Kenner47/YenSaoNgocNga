using Microsoft.AspNetCore.Mvc;
using YenSaoNgocNga_MVC.Areas.ProductManagement.Models;
using YenSaoNgocNga_MVC.Areas.ProductManagement.Services;

namespace YenSaoNgocNga_MVC.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
    public class ProductController : Controller
    {
        private readonly IProductApiService _productApiService;

        public ProductController(IProductApiService productApiService)
        {
            _productApiService = productApiService;
        }

        // GET: /ProductManagement/Product
        public async Task<IActionResult> Index(ProductFilterViewModel filter)
        {
            try
            {
                var products = new List<ProductViewModel>();
                var categories = await _productApiService.GetCategoriesAsync();

                // Apply filters
                if (!string.IsNullOrEmpty(filter.SearchQuery))
                {
                    products = (await _productApiService.SearchProductsAsync(filter.SearchQuery)).ToList();
                }
                else if (filter.CategoryId.HasValue)
                {
                    products = (await _productApiService.GetProductsByCategoryAsync(filter.CategoryId.Value)).ToList();
                }
                else
                {
                    products = (await _productApiService.GetAllProductsAsync()).ToList();
                }

                // Apply additional filters
                if (filter.MinPrice.HasValue)
                    products = products.Where(p => p.Price >= filter.MinPrice.Value).ToList();

                if (filter.MaxPrice.HasValue)
                    products = products.Where(p => p.Price <= filter.MaxPrice.Value).ToList();

                if (!string.IsNullOrEmpty(filter.ProductType))
                    products = products.Where(p => p.ProductType.Contains(filter.ProductType, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrEmpty(filter.Grade))
                    products = products.Where(p => p.Grade == filter.Grade).ToList();

                // Apply sorting
                products = filter.SortBy.ToLower() switch
                {
                    "price" => filter.SortOrder == "desc" ? products.OrderByDescending(p => p.Price).ToList() : products.OrderBy(p => p.Price).ToList(),
                    "newest" => filter.SortOrder == "desc" ? products.OrderBy(p => p.CreatedAt).ToList() : products.OrderByDescending(p => p.CreatedAt).ToList(),
                    _ => filter.SortOrder == "desc" ? products.OrderByDescending(p => p.ProductName).ToList() : products.OrderBy(p => p.ProductName).ToList()
                };

                ViewBag.Categories = categories;
                ViewBag.Filter = filter;

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải sản phẩm: " + ex.Message;
                return View(new List<ProductViewModel>());
            }
        }

        // GET: /ProductManagement/Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productApiService.GetProductByIdAsync(id);

                if (product == null)
                {
                    TempData["Error"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction(nameof(Index));
                }

                // Get related products (same category)
                var relatedProducts = await _productApiService.GetProductsByCategoryAsync(product.CategoryId);
                ViewBag.RelatedProducts = relatedProducts.Where(p => p.ProductId != id).Take(4);

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /ProductManagement/Product/Category/1
        public async Task<IActionResult> Category(int id, string name = "")
        {
            try
            {
                var products = await _productApiService.GetProductsByCategoryAsync(id);
                var categories = await _productApiService.GetCategoriesAsync();

                ViewBag.Categories = categories;
                ViewBag.CurrentCategory = categories.FirstOrDefault(c => c.CategoryId == id);
                ViewBag.CategoryName = !string.IsNullOrEmpty(name) ? name : ViewBag.CurrentCategory?.CategoryName ?? "Danh mục";

                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /ProductManagement/Product/Search
        public async Task<IActionResult> Search(string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    TempData["Warning"] = "Vui lòng nhập từ khóa tìm kiếm";
                    return RedirectToAction(nameof(Index));
                }

                var products = await _productApiService.SearchProductsAsync(q);
                var categories = await _productApiService.GetCategoriesAsync();

                ViewBag.Categories = categories;
                ViewBag.SearchQuery = q;
                ViewBag.ResultCount = products.Count();

                return View("Index", products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tìm kiếm: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}