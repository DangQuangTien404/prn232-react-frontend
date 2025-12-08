using BLL;
using DAL.Entities; // Để dùng UserRole
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Xem danh sách user
        public IActionResult Index()
        {
            var users = _unitOfWork.UserRepository.GetAll();
            return View(users);
        }

        // --- CHỨC NĂNG MỚI: CẬP NHẬT QUYỀN (ROLE) ---
        [HttpPost]
        public IActionResult UpdateRole(int userId, UserRole newRole)
        {
            // 1. Kiểm tra không cho phép tự hạ quyền của chính mình (để tránh mất quyền Admin)
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            if (userId == currentUserId)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự thay đổi quyền của chính mình!";
                return RedirectToAction("Index");
            }

            // 2. Lấy User từ DB
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            // 3. Cập nhật Role mới
            user.Role = newRole;
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();

            TempData["SuccessMessage"] = $"Đã cập nhật quyền của tài khoản {user.Username} thành {newRole}.";
            return RedirectToAction("Index");
        }

        // Xóa tài khoản
        public IActionResult Delete(int id)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            if (id == currentUserId)
            {
                TempData["ErrorMessage"] = "Không thể xóa tài khoản đang đăng nhập!";
                return RedirectToAction("Index");
            }

            _unitOfWork.UserRepository.Delete(id);
            _unitOfWork.Save();

            TempData["SuccessMessage"] = "Đã xóa người dùng thành công.";
            return RedirectToAction("Index");
        }
    }
}