using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChienThangComputer.Models {
    public class Infomation {

        [Required(ErrorMessage = "Họ tên không được để trống!")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Vui lòng nhập đầy đủ họ va tên !")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Tài khoản không được để trống!")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Bạn ở đâu ???")]
        [StringLength(50, ErrorMessage = "Địa chỉ nhà dài dữ vậy, sao kiếm ?")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [RegularExpression("^.*(?=.{8,})(?=.*[\\d])(?=.*[A-Z]).*$", ErrorMessage = "Mật khẩu phải có ít nhất 8 kí tự trong đó có ít nhất 1 chữ hoa và 1 số từ 0-9")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu không khớp")]
        public string ReTypePassword { get; set; }

    }
}