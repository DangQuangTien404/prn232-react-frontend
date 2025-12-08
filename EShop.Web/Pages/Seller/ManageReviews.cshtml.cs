using EShop.BLL.Services;
using EShop.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages.Seller
{
    [Authorize(Roles = "Seller")]
    public class ManageReviewsModel : PageModel
    {
        private readonly IReviewService _reviewService;

        public ManageReviewsModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public List<Review> Reviews { get; set; } = new List<Review>();

        public async Task OnGetAsync()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdStr, out int sellerId))
            {
                Reviews = await _reviewService.GetReviewsBySellerAsync(sellerId);
            }
        }

        public async Task<IActionResult> OnPostReplyAsync(int reviewId, string replyContent)
        {
            if (!string.IsNullOrWhiteSpace(replyContent))
            {
                await _reviewService.ReplyReviewAsync(reviewId, replyContent);
            }
            return RedirectToPage();
        }
    }
}