using System;
using RentalPropertyManagement.DAL.Enums;
using System.ComponentModel.DataAnnotations; // Thêm using cho Validation

namespace RentalPropertyManagement.BLL.DTOs
{
    public class ContractDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Property ID là bắt buộc.")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Tenant ID là bắt buộc.")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số tiền thuê là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tiền thuê phải lớn hơn 0.")]
        public decimal RentAmount { get; set; }

        public ContractStatus Status { get; set; } = ContractStatus.Active; // Mặc định là Active khi tạo

        // (OPTIONAL) Thêm các trường hiển thị hỗ trợ cho View
        public string TenantName { get; set; }
        public string PropertyAddress { get; set; }
    }
}