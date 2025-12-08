using EShop.BLL.DTOs;
using EShop.BLL.Services;
using EShop.DAL.Entities;
using EShop.Web.Helpers; // Nhớ using cái này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public List<Product> Products { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _productService.GetAllProductsAsync();
        }

        // === Code xử lý thêm vào giỏ ===
        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            // 1. Lấy thông tin sản phẩm từ DB
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            // 2. Lấy giỏ hàng hiện tại từ Session (nếu chưa có thì tạo mới)
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            // 3. Kiểm tra sản phẩm đã có trong giỏ chưa
            var existingItem = cart.FirstOrDefault(x => x.ProductId == id);
            if (existingItem != null)
            {
                existingItem.Quantity++; // Có rồi thì tăng số lượng
            }
            else
            {
                // Chưa có thì thêm mới
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            // 4. Lưu ngược lại vào Session
            HttpContext.Session.SetObject("Cart", cart);

            // 5. Load lại trang
            return RedirectToPage();
        }
    }
}