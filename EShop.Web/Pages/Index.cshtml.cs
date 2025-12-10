using EShop.BLL.DTOs;
using EShop.BLL.Services;
using EShop.DAL.Entities;
using EShop.Web.Helpers; // Nhớ using cái này
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public List<Product> Products { get; set; }

        public async Task OnGetAsync()
        {
            Products = await _productService.GetAllProductsAsync();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(x => x.ProductId == id);
            if (existingItem != null)
            {
                existingItem.Quantity++; // Có rồi thì tăng số lượng
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToPage();
        }
    }
}