using BLL;
using DAL.Entities;
using Ecommerce.Web.Helpers;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        const string CART_KEY = "MY_CART";

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ==========================================
        // KHU VỰC CỦA KHÁCH HÀNG (CUSTOMER)
        // ==========================================

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(CART_KEY);
            if (cart == null || cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(cart);
        }
        [HttpPost]
        public IActionResult Checkout(string paymentMethod, string cardNumber, string cardHolder)
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(CART_KEY);
            if (cart == null || cart.Count == 0) return RedirectToAction("Index", "Cart");

            // VALIDATION STOCK AND SOFT DELETE
            foreach (var item in cart)
            {
                var productCheck = _unitOfWork.ProductRepository.GetById(item.ProductId);
                if (productCheck == null || productCheck.IsDeleted)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm {item.ProductName} không còn tồn tại.";
                    return RedirectToAction("Index", "Cart");
                }
                if (item.Quantity > productCheck.StockQuantity)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm {item.ProductName} không đủ hàng (chỉ còn {productCheck.StockQuantity}).";
                    return RedirectToAction("Index", "Cart");
                }
            }

            var userId = int.Parse(User.FindFirst("UserId").Value);

            string paymentInfo = "COD";
            OrderStatus initialStatus = OrderStatus.Pending;

            if (paymentMethod == "Card")
            {

                string last4Digits = cardNumber.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : "****";
                paymentInfo = $"Thẻ Tín dụng (Visa/Mastercard) - *{last4Digits}";

            }

            try
            {
                _unitOfWork.BeginTransaction();

                var order = new Order
                {
                    CustomerId = userId,
                    OrderDate = DateTime.Now,
                    Status = initialStatus,
                    PaymentMethod = paymentInfo,
                    TotalAmount = cart.Sum(p => p.Total)
                };

                _unitOfWork.OrderRepository.Add(order);
                _unitOfWork.Save();

                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    };
                    _unitOfWork.OrderDetailRepository.Add(orderDetail);

                    var product = _unitOfWork.ProductRepository.GetById(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        // Stock is already checked above, but safety check remains
                        if (product.StockQuantity < 0) product.StockQuantity = 0;
                        _unitOfWork.ProductRepository.Update(product);
                    }
                }

                _unitOfWork.Save();
                _unitOfWork.CommitTransaction();

                HttpContext.Session.Remove(CART_KEY);
                TempData["SuccessMessage"] = $"Đặt hàng thành công! Phương thức: {paymentInfo}";

                return RedirectToAction("OrderConfirmed");
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTransaction();
                TempData["ErrorMessage"] = "Đã có lỗi xảy ra trong quá trình xử lý đơn hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "Cart");
            }
        }
        public IActionResult OrderConfirmed()
        {
            return View();
        }

        public IActionResult MyOrders()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var orders = _unitOfWork.OrderRepository
                            .Find(o => o.CustomerId == userId)
                            .OrderByDescending(o => o.OrderDate)
                            .ToList();

            return View(orders);
        }

        // ==========================================
        // KHU VỰC QUẢN LÝ CỦA SELLER
        // ==========================================

        [Authorize(Roles = "Seller")]
        public IActionResult SellerOrders()
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);
            var orders = _unitOfWork.OrderRepository
                .Find(o => o.OrderDetails.Any(od => od.Product.SellerId == userId))
                .OrderByDescending(o => o.OrderDate);
            return View(orders);
        }

        [Authorize(Roles = "Seller")]
        public IActionResult SellerOrderDetails(int id)
        {
            var order = _unitOfWork.OrderRepository.GetById(id);

            if (order == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst("UserId").Value);

            // Fetch details for this order
            var details = _unitOfWork.OrderDetailRepository.Find(od => od.OrderId == id).ToList();

            // Populate Product info
            foreach (var item in details)
            {
                item.Product = _unitOfWork.ProductRepository.GetById(item.ProductId);
            }

            // Check if this seller has ANY product in this order
            if (!details.Any(d => d.Product.SellerId == userId))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            // Filter details to show ONLY products belonging to this seller
            var sellerDetails = details.Where(d => d.Product.SellerId == userId).ToList();

            order.OrderDetails = sellerDetails;

            // Manual fetch user info to fix Issue 2
            order.Customer = _unitOfWork.UserRepository.GetById(order.CustomerId);

            return View(order);
        }

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;

            _unitOfWork.OrderRepository.Update(order);
            _unitOfWork.Save();

            TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng #{orderId} thành công!";

            return RedirectToAction("SellerOrderDetails", new { id = orderId });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}