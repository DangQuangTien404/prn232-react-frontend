using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class ProductEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 1000 VNĐ")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string Description { get; set; }

        // Không để [Required] vì user có thể giữ ảnh cũ
        public IFormFile? ImageFile { get; set; }

        // Trường này để hiển thị ảnh hiện tại
        public string? ExistingImageUrl { get; set; }
    }
}