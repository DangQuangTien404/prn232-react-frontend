namespace EShop.DAL.Enums
{
    public enum OrderStatus
    {
        Pending = 1,          // Mới tạo, chờ thanh toán
        Paid = 2,             // Đã thanh toán
        Processing = 3,       // Seller xác nhận
        Shipped = 4,          // Đã giao vận chuyển
        Completed = 5,        // Giao thành công
        Cancelled = 0
    }
}