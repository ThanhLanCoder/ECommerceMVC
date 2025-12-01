using System;
using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Models.ViewModels
{
    public class DangKyVM
    {
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 - 100 ký tự")]
        [DataType(DataType.Password)]
        public required string MatKhau { get; set; }

        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public required string HoTen { get; set; }

        public bool GioiTinh { get; set; } = true;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(60, ErrorMessage = "Địa chỉ tối đa 60 ký tự")]
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "Điện thoại không được bỏ trống")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Chưa đúng đinh dạng")]
        public required string DienThoai { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        public string? Hinh { get; set; }
    }
}
