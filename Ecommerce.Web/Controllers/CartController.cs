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

            if (cartItem == null) 
            {
                var product = _unitOfWork.ProductRepository.GetById(productId);
                if (product == null) return NotFound("Sản phẩm không tồn tại");

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
                    item.Quantity = quantity;
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