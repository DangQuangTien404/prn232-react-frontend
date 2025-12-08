using BLL;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            var totalUsers = _unitOfWork.UserRepository.GetAll().Count();

            var orders = _unitOfWork.OrderRepository.GetAll().ToList();
            var totalOrders = orders.Count;
            var totalRevenue = orders.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.TotalAmount);
            var pendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);

            var totalProducts = _unitOfWork.ProductRepository.GetAll().Count();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.TotalProducts = totalProducts;

            return View();
        }
    }
}