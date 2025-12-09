// 1. Khởi tạo kết nối tới Hub
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/ecommerceHub")
    .withAutomaticReconnect() // Tự động kết nối lại nếu rớt mạng
    .build();

// 2. Sự kiện: CÓ ĐƠN HÀNG MỚI (Dành cho Seller)
connection.on("ReceiveOrderNotification", function (message) {
    // Chỉ hiện thông báo và reload nếu đang ở trang Quản lý đơn hàng
    if (window.location.href.includes("ManageOrders")) {
        alert("🔔 TING TING: " + message);
        location.reload();
    }
});

// 3. Sự kiện: CÓ SẢN PHẨM MỚI (Dành cho Khách hàng)
connection.on("ReceiveProductUpdate", function (message) {
    // Chỉ reload nếu đang ở Trang chủ
    var path = window.location.pathname.toLowerCase();
    if (path === "/" || path.includes("/index")) {
        // Có thể hiện Toast/Alert ở đây nếu muốn
        location.reload();
    }
});

// 4. Sự kiện: TRẠNG THÁI ĐƠN HÀNG THAY ĐỔI (Dành cho Khách hàng)
connection.on("ReceiveOrderStatusUpdate", function (orderId, newStatus) {
    // Chỉ reload nếu khách đang xem danh sách đơn hoặc chi tiết đơn
    if (window.location.href.includes("MyOrders") || window.location.href.includes("OrderDetails")) {
        // alert(`Đơn hàng #${orderId} vừa chuyển sang trạng thái: ${newStatus}`);
        location.reload();
    }
});

// 5. Bắt đầu kết nối
connection.start().then(function () {
    console.log("✅ SignalR Connected Successfully!");
}).catch(function (err) {
    console.error("❌ SignalR Connection Error: " + err.toString());
});