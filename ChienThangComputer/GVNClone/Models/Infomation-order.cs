using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GVNClone.Models {
    public class Infomation_order {
        [Required(ErrorMessage = "Họ tên không được để trống!")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email của bạn !")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ !")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại của bạn!")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Bạn ở đâu ???")]
        [StringLength(50, ErrorMessage = "Địa chỉ nhà dài dữ vậy, sao kiếm ?")]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Bạn ở tỉnh nào ?")]
        public string TinhThanh { get; set; }

        [Required(ErrorMessage = "Bạn ở huyện nào ?")]
        public string QuanHuyen { get; set; }
    }
}