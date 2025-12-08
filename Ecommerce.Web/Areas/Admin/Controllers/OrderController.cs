using BLL;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            var orders = _unitOfWork.OrderRepository
                            .GetAll()
                            .OrderByDescending(o => o.OrderDate)
                            .ToList();

            foreach (var order in orders)
            {
                order.Customer = _unitOfWork.UserRepository.GetById(order.CustomerId);
            }

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = _unitOfWork.OrderRepository.GetById(id);
            if (order == null) return NotFound();

            order.Customer = _unitOfWork.UserRepository.GetById(order.CustomerId);

            var details = _unitOfWork.OrderDetailRepository.Find(d => d.OrderId == id).ToList();
            foreach (var item in details)
            {
                item.Product = _unitOfWork.ProductRepository.GetById(item.ProductId);
            }
            order.OrderDetails = details;

            return View(order);
        }
    }
}