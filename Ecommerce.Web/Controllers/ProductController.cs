using BLL;
using DAL.Entities;
using Ecommerce.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels; // Đảm bảo namespace này khớp với nơi bạn để ViewModel

namespace Web.Controllers
{
    [Authorize(Roles = "Seller")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. Danh sách sản phẩm (Chỉ hiện sản phẩm chưa bị xóa)
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdClaim.Value);

            // Lọc theo SellerId VÀ !IsDeleted
            var products = _unitOfWork.ProductRepository
                            .Find(p => p.SellerId == userId && !p.IsDeleted)
                            .ToList();

            return View(products);
        }

        // 2. Trang tạo mới (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. Xử lý tạo mới (POST)
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                // Xử lý upload ảnh
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    Description = model.Description,
                    ImageUrl = uniqueFileName,
                    SellerId = int.Parse(User.FindFirst("UserId").Value),
                    IsDeleted = false // Mặc định chưa xóa
                };

                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.Save();

                TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // 4. Trang chỉnh sửa (GET)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);

            // Tìm sản phẩm thuộc về Seller này và chưa bị xóa
            var product = _unitOfWork.ProductRepository
                            .Find(p => p.Id == id && p.SellerId == userId && !p.IsDeleted)
                            .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            // Map dữ liệu sang ViewModel
            var model = new ProductEditVM
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Description = product.Description,
                ExistingImageUrl = product.ImageUrl
            };

            return View(model);
        }

        // 5. Xử lý chỉnh sửa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst("UserId").Value);

                var product = _unitOfWork.ProductRepository
                                .Find(p => p.Id == model.Id && p.SellerId == userId)
                                .FirstOrDefault();

                if (product == null) return NotFound();

                // Cập nhật thông tin
                product.Name = model.Name;
                product.Price = model.Price;
                product.StockQuantity = model.StockQuantity;
                product.Description = model.Description;

                // Nếu có upload ảnh mới
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                    // Xóa ảnh cũ (Tùy chọn)
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        string oldFilePath = Path.Combine(uploadsFolder, product.ImageUrl);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Lưu ảnh mới
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = uniqueFileName;
                }

                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.Save();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // 6. Xóa mềm (Soft Delete)
        public IActionResult Delete(int id)
        {
            var userId = int.Parse(User.FindFirst("UserId").Value);

            var product = _unitOfWork.ProductRepository
                          .Find(p => p.Id == id && p.SellerId == userId)
                          .FirstOrDefault();

            if (product != null)
            {
                // Thay vì xóa hẳn, ta đánh dấu IsDeleted = true
                product.IsDeleted = true;

                _unitOfWork.ProductRepository.Update(product); // Gọi Update thay vì Delete
                _unitOfWork.Save();

                TempData["SuccessMessage"] = "Đã xóa sản phẩm (đưa vào thùng rác).";
            }
            return RedirectToAction("Index");
        }
    }
}