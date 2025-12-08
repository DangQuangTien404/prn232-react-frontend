using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class ManageProductsModel : PageModel
    {
        private readonly IProductService _productService;

        public ManageProductsModel(IProductService productService)
        {
            _productService = productService;
        }

        public List<Product> MyProducts { get; set; } = new List<Product>();

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int sellerId))
            {
                MyProducts = await _productService.GetProductsBySellerAsync(sellerId);
            }
        }

        // XỬ LÝ XÓA SẢN PHẨM
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            // Kiểm tra bảo mật: Phải đúng là hàng của mình mới được xóa
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (product != null && userIdStr == product.SellerId.ToString())
            {
                // 1. Xóa ảnh cũ trong thư mục wwwroot (để tiết kiệm dung lượng server)
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // 2. Xóa trong Database
                await _productService.DeleteProductAsync(id);
            }

            return RedirectToPage();
        }
    }
}