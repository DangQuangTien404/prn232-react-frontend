namespace EShop.BLL.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalRevenue { get; set; }      // Tổng doanh thu
        public int TotalOrders { get; set; }           // Tổng số đơn hàng
        public int TotalProductsSold { get; set; }     // Tổng số sản phẩm bán ra
        public int TotalCustomers { get; set; }        // Tổng số khách hàng
    }
}