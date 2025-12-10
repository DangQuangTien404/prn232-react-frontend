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

        public async Task<IActionResult> OnPostAddReviewAsync(int id)
        {
            if (!User.Identity.IsAuthenticated) return RedirectToPage("/Login");

            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int customerId))
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product != null && product.SellerId == customerId)
                {
                    return RedirectToPage(new { id = id });
                }

                await _reviewService.AddReviewAsync(id, customerId, RatingInput, CommentInput);
            }

            return RedirectToPage(new { id = id }); // Load lại trang
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            return RedirectToPage("/Index");
        }
    }
}