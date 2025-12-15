using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RentalPropertyManagement.DAL.Enums;

namespace RentalPropertyManagement.DAL.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        // TRƯỜNG MỚI ĐÃ THÊM: Lưu mật khẩu đã băm
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }
        public string PhoneNumber { get; set; }

        // Navigation properties (Mối quan hệ 1-nhiều)
        public virtual ICollection<Contract> ContractsAsTenant { get; set; }
        public virtual ICollection<MaintenanceRequest> SubmittedRequests { get; set; }
        public virtual ICollection<MaintenanceRequest> AssignedRequests { get; set; }
    }
}