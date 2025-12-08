using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class EditProductModel : PageModel
    {
        private readonly IProductService _productService;

        public EditProductModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product ProductInput { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; } // Cho phép null nếu không muốn đổi ảnh

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            var userIdStr = User.FindFirst("UserId")?.Value;

            // Kiểm tra: Có sản phẩm ko? Và sản phẩm này có phải của Seller đang đăng nhập ko?
            if (product == null || product.SellerId.ToString() != userIdStr)
            {
                return RedirectToPage("/AccessDenied"); // Hoặc về trang ManageProducts
            }

            ProductInput = product;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Bỏ qua validate các trường quan hệ
            ModelState.Remove("ProductInput.Seller");
            ModelState.Remove("ProductInput.Reviews");
            ModelState.Remove("ImageFile"); // Ảnh không bắt buộc khi sửa

            if (!ModelState.IsValid) return Page();

            // Lấy thông tin cũ từ DB để giữ lại ImageUrl cũ nếu người dùng không up ảnh mới
            var existingProduct = await _productService.GetProductByIdAsync(ProductInput.Id);
            if (existingProduct == null) return NotFound();

            // 1. Cập nhật thông tin cơ bản
            existingProduct.Name = ProductInput.Name;
            existingProduct.Description = ProductInput.Description;
            existingProduct.Price = ProductInput.Price;
            existingProduct.StockQuantity = ProductInput.StockQuantity;

            // 2. Xử lý ảnh (Nếu có up ảnh mới)
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // Lưu ảnh mới
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                existingProduct.ImageUrl = "/uploads/" + fileName;
            }

            // 3. Lưu xuống DB
            await _productService.UpdateProductAsync(existingProduct);

            return RedirectToPage("/Seller/ManageProducts");
        }
    }
}