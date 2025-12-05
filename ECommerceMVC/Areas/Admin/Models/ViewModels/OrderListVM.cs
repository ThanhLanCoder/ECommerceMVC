namespace ECommerceMVC.Areas.Admin.Models.ViewModels
{
    public class OrderListVM
    {
        public List<OrderItemVM> Orders { get; set; } = new List<OrderItemVM>();

        // === BỘ LỌC (Filter Properties) ===
        public string? SearchTerm { get; set; }        // Từ khóa tìm kiếm
        public int? Status { get; set; }               // Lọc theo trạng thái (0-4)
        public DateTime? FromDate { get; set; }        // Lọc từ ngày
        public DateTime? ToDate { get; set; }          // Lọc đến ngày
        public string? PaymentMethod { get; set; }     // Lọc theo thanh toán (COD/Bank)
        public int PendingCount { get; set; }          // Số đơn chờ xác nhận
        public int ShippingCount { get; set; }         // Số đơn đang giao
        public int CompletedCount { get; set; }        // Số đơn hoàn thành
        public int CancelledCount { get; set; }        // Số đơn đã hủy
        public int TotalOrders { get; set; }           // Tổng số đơn (sau khi lọc)
    }
}
