using EShop.BLL.DTOs;
using EShop.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages
{
    public class CartModel : PageModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal GrandTotal => CartItems.Sum(x => x.Total);

        public void OnGet()
        {
            CartItems = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        }

        // Chức năng Xóa sản phẩm khỏi giỏ
        public IActionResult OnPostRemove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.SetObject("Cart", cart);
            }
            return RedirectToPage();
        }
    }
}