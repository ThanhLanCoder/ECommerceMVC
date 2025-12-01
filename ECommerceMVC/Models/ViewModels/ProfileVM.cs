
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Models.ViewModels
{
    public class ProfileVM
    {
        public required string MaKh { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public required string HoTen { get; set; }

        [Required(ErrorMessage = "Điện thoại không được bỏ trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại chưa đúng định dạng")]
        public required string DienThoai { get; set; }

        public bool GioiTinh { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [StringLength(60, ErrorMessage = "Địa chỉ tối đa 60 ký tự")]
        public string? DiaChi { get; set; }

        public string? Hinh { get; set; }

        public DateTime NgayDangKy { get; set; }

        // Statistics
        public int TongDonHang { get; set; }
        public int DonHangHoanThanh { get; set; }
        public int DonHangDangXuLy { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được bỏ trống")]
        [DataType(DataType.Password)]
        public required string MatKhauCu { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được bỏ trống")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 - 50 ký tự")]
        [DataType(DataType.Password)]
        public required string MatKhauMoi { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được bỏ trống")]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public required string XacNhanMatKhau { get; set; }
    }
}