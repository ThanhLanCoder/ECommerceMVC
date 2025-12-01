using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.Models.ViewModels
{
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 - 50 ký tự")]
        [DataType(DataType.Password)]
        public required string MatKhau { get; set; }
    }
}
