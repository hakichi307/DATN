using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GVNClone.Models {
    public class Cart {
        dbChienThangDataContext db = new dbChienThangDataContext();
        public int product_ID { get; set; }
        public string product_Name { get; set; }
        public string product_Image { get; set; }
        public decimal product_Price { get; set; }
        public int product_Quantity { get; set; }
        public decimal total_Amount {
            get { return product_Price * product_Quantity; }
        }

        public Cart(int iProduct_ID) {
            product_ID = iProduct_ID;
            SanPham product = db.SanPhams.Single(pd => pd.MaSP == product_ID);
            product_Name = product.TenSP;
            product_Image = product.Pic1;
            if (product.MaGiamGia != null && product.GiamGia.NgayBatDau < DateTime.Now && product.GiamGia.NgayKetThuc > DateTime.Now)
            {
                product_Price = decimal.Parse(product.GiamGia.GiaKhuyenMai.ToString());
            }
            else {
                product_Price = decimal.Parse(product.GiaNiemYet.ToString());
            }
            product_Quantity = 1;
        }

    }
}