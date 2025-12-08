using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Linq;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class CreateProductModel : PageModel
    {
        private readonly IProductService _productService;

        public CreateProductModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product ProductInput { get; set; }

        // === SỬA Ở ĐÂY: Thêm dấu ? để cho phép không chọn ảnh cũng được ===
        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // === QUAN TRỌNG: Xóa các lỗi kiểm tra không cần thiết ===
            ModelState.Remove("ProductInput.Seller");
            ModelState.Remove("ProductInput.Reviews");
            ModelState.Remove("ProductInput.ImageUrl");

            // Thêm dòng này để fix lỗi "ImageFile field is required" (Hình 2)
            ModelState.Remove("ImageFile");
            // ========================================================

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ErrorMessage = "Dữ liệu nhập không hợp lệ: " + string.Join(", ", errors);
                return Page();
            }

            // ... (Đoạn code xử lý Upload ảnh và Lưu giữ nguyên như cũ) ...

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                ProductInput.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                ProductInput.ImageUrl = "https://via.placeholder.com/300x200?text=No+Image";
            }

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int sellerId))
            {
                ProductInput.SellerId = sellerId;
                await _productService.AddProductAsync(ProductInput);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Lỗi xác thực người dùng.";
            return Page();
        }
    }
}