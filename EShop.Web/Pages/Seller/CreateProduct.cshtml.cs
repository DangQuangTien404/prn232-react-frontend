using EShop.BLL.Services;
using EShop.DAL.Entities;
using EShop.Web.Hubs; // Thêm dòng này
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR; // Thêm dòng này
using System.IO;
using System.Linq;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class CreateProductModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IHubContext<ECommerceHub> _hubContext; // 1. Khai báo Hub

        // 2. Inject Hub vào Constructor
        public CreateProductModel(IProductService productService, IHubContext<ECommerceHub> hubContext)
        {
            _productService = productService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public Product ProductInput { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("ProductInput.Seller");
            ModelState.Remove("ProductInput.Reviews");
            ModelState.Remove("ProductInput.ImageUrl");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ErrorMessage = "Dữ liệu nhập không hợp lệ: " + string.Join(", ", errors);
                return Page();
            }

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

                // Lưu sản phẩm vào DB
                await _productService.AddProductAsync(ProductInput);

                // 3. CODE MỚI: Gửi tín hiệu SignalR cập nhật sản phẩm
                await _hubContext.Clients.All.SendAsync("ReceiveProductUpdate", "Có sản phẩm mới vừa lên kệ!");

                return RedirectToPage("/Index"); // Chuyển về trang quản lý của Seller (hoặc trang chủ)
            }

            ErrorMessage = "Lỗi xác thực người dùng.";
            return Page();
        }
    }
}