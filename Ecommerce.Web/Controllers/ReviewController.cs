using BLL;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Khách hàng gửi đánh giá
        [HttpPost]
        public IActionResult AddReview(int productId, int rating, string comment)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            var userId = int.Parse(userIdClaim.Value);

            // Kiểm tra: Khách đã mua và đơn hàng đã hoàn tất chưa?
            // Sử dụng OrderDetailRepository trực tiếp theo hướng dẫn tối ưu hiệu năng
            var hasPurchased = _unitOfWork.OrderDetailRepository.Find(od =>
                od.ProductId == productId &&
                od.Order.CustomerId == userId &&
                od.Order.Status == OrderStatus.Completed
            ).Any();

            if (!hasPurchased)
            {
                TempData["ErrorMessage"] = "Bạn chỉ có thể đánh giá sản phẩm đã mua thành công!";
                return RedirectToAction("Details", "Home", new { id = productId });
            }

            // Kiểm tra đã đánh giá chưa (Sửa UserId -> CustomerId)
            var existingReview = _unitOfWork.ReviewRepository
                .Find(r => r.CustomerId == userId && r.ProductId == productId)
                .FirstOrDefault();

            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi.";
                return RedirectToAction("Details", "Home", new { id = productId });
            }

            var review = new Review
            {
                ProductId = productId,
                CustomerId = userId, // Sửa thành CustomerId
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            _unitOfWork.ReviewRepository.Add(review);
            _unitOfWork.Save();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá!";
            return RedirectToAction("Details", "Home", new { id = productId });
        }

        // 2. Seller phản hồi đánh giá
        [HttpPost]
        [Authorize(Roles = "Seller")]
        public IActionResult ReplyReview(int reviewId, string replyContent)
        {
            var review = _unitOfWork.ReviewRepository.GetById(reviewId);
            if (review == null) return NotFound();

            // Kiểm tra quyền sở hữu sản phẩm
            var product = _unitOfWork.ProductRepository.GetById(review.ProductId);
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);

            if (product.SellerId != currentUserId)
            {
                return Forbid();
            }

            review.SellerReply = replyContent;
            review.ReplyDate = DateTime.Now;

            _unitOfWork.ReviewRepository.Update(review);
            _unitOfWork.Save();

            return RedirectToAction("Details", "Home", new { id = review.ProductId });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}