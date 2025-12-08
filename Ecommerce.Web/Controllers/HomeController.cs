using BLL;
using Web.Models; 
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq; 

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository
                            .Find(p => !p.IsDeleted) 
                            .ToList();

            foreach (var product in products)
            {
                product.Reviews = _unitOfWork.ReviewRepository.Find(r => r.ProductId == product.Id).ToList();
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);

            if (product == null || product.IsDeleted)
            {
                return NotFound();
            }

            var reviews = _unitOfWork.ReviewRepository
                            .Find(r => r.ProductId == id)
                            .OrderByDescending(r => r.CreatedAt)
                            .ToList();

            foreach (var review in reviews)
            {
                review.Customer = _unitOfWork.UserRepository.GetById(review.CustomerId);
            }

            ViewBag.Reviews = reviews;

            if (reviews.Any())
            {
                ViewBag.AverageRating = reviews.Average(r => r.Rating);
                ViewBag.ReviewCount = reviews.Count;
            }
            else
            {
                ViewBag.AverageRating = 0;
                ViewBag.ReviewCount = 0;
            }

            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}