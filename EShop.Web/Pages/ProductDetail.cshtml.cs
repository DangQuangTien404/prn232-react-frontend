using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EShop.Web.Pages
{
    public class ProductDetailModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public ProductDetailModel(IProductService productService, IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        public Product Product { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();

        // Các biến để binding form đánh giá
        [BindProperty]
        public int RatingInput { get; set; }
        [BindProperty]
        public string CommentInput { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductByIdAsync(id);
            if (Product == null) return NotFound();

            Reviews = await _reviewService.GetProductReviewsAsync(id);
            return Page();
        }

        // Xử lý khi khách hàng gửi đánh giá
        public async Task<IActionResult> OnPostAddReviewAsync(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToPage("/Login");

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int customerId))
            {
                await _reviewService.AddReviewAsync(id, customerId, RatingInput, CommentInput);
            }

            return RedirectToPage(new { id = id }); // Load lại trang
        }

        // Xử lý thêm vào giỏ (Copy từ trang Index qua để dùng được ở đây luôn)
        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            // (Bạn có thể gọi lại logic AddToCart giống IndexModel, hoặc điều hướng về Index để xử lý)
            // Để đơn giản, ta tạm thời redirect về Index rồi thêm sau, 
            // hoặc bạn copy hàm OnPostAddToCart từ Index.cshtml.cs sang đây.
            return RedirectToPage("/Index");
        }
    }
}