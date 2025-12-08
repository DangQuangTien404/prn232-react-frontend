using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShop.Web.Pages
{
    public class OrderSuccessModel : PageModel
    {
        public int OrderId { get; set; }
        public void OnGet(int id)
        {
            OrderId = id;
        }
    }
}