using BLL;
using Ecommerce.Web.Helpers; 
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        const string CART_KEY = "MY_CART";

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CartItemVM> GetCart()
        {
            var cart = HttpContext.Session.Get<List<CartItemVM>>(CART_KEY);
            if (cart == null)
            {
                cart = new List<CartItemVM>();
            }
            return cart;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var cart = GetCart();
            var cartItem = cart.SingleOrDefault(p => p.ProductId == productId);

            var product = _unitOfWork.ProductRepository.GetById(productId);
            if (product == null || product.IsDeleted) return NotFound("Sản phẩm không tồn tại hoặc đã bị xóa");

            // Check stock quantity
            int currentQuantityInCart = cartItem != null ? cartItem.Quantity : 0;
            if (currentQuantityInCart + quantity > product.StockQuantity)
            {
                TempData["ErrorMessage"] = $"Số lượng yêu cầu vượt quá tồn kho (Còn lại: {product.StockQuantity})";
                return RedirectToAction("Index", "Home");
            }

            if (cartItem == null)
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null)
                {
                    int userId = int.Parse(userIdClaim.Value);
                    if (product.SellerId == userId)
                    {
                        TempData["ErrorMessage"] = "Bạn không thể mua sản phẩm của chính mình!";
                        return RedirectToAction("Index", "Home");
                    }
                }

                cartItem = new CartItemVM
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity
                };
                cart.Add(cartItem);
            }
            else 
            {
                cartItem.Quantity += quantity;
            }

            HttpContext.Session.Set(CART_KEY, cart);

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ!";

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductId == productId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    var product = _unitOfWork.ProductRepository.GetById(productId);
                    if (product != null)
                    {
                        if (quantity > product.StockQuantity)
                        {
                            item.Quantity = product.StockQuantity;
                            TempData["ErrorMessage"] = $"Kho chỉ còn {product.StockQuantity} sản phẩm. Đã cập nhật số lượng về mức tối đa.";
                        }
                        else
                        {
                            item.Quantity = quantity;
                        }
                    }
                    else
                    {
                         // Product might be deleted
                         cart.Remove(item);
                         TempData["ErrorMessage"] = "Sản phẩm không còn tồn tại.";
                    }
                }
                else
                {
                    cart.Remove(item);
                }
                HttpContext.Session.Set(CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CART_KEY);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}