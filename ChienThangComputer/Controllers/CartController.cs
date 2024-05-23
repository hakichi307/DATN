using ChienThangComputer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using Common;
using System.Web.Services.Description;

namespace ChienThangComputer.Controllers {
    public class CartController : Controller {
        dbChienThangDataContext db = new dbChienThangDataContext();
        public List<Cart> getCart() {
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart == null) {
                listCart = new List<Cart>();
                Session["Cart"] = listCart;
            }
            return listCart;
        }

        public ActionResult AddToCart(int id) {
            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(id));
            if (product == null) {
                Response.StatusCode = 404;
                return null;
            }
            List<Cart> listCart = getCart();
            Cart item = listCart.SingleOrDefault(it => it.product_ID.Equals(id));
            if (item != null) {
                item.product_Quantity++;
                if (product.SoLuongTon < item.product_Quantity) {
                    item.product_Quantity -= 1;
                    var errorMessage = string.Format("Sản phẩm {0} chỉ còn {1} sản phẩm", item.product_Name.ToString(), product.SoLuongTon.ToString());
                    return Json(new { success = false, message = errorMessage, quantity = Total_Quantity() });
                }
                ViewBag.Total_Quantity = Total_Quantity();
                return PartialView("CartPartial");
            }
            item = new Cart(id);
            listCart.Add(item);
            ViewBag.Total_Quantity = Total_Quantity();
            return PartialView("CartPartial");
        }



        private int Total_Quantity() {
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart == null) {
                return 0;
            }
            return listCart.Sum(n => n.product_Quantity);  
        }

        private decimal Total_Amount() {
            decimal total_amount = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null) {
                total_amount += listCart.Sum(tm => tm.total_Amount);
            }
            return total_amount;
        }

        public ActionResult CartEmpty() {
            ViewBag.Total_Quantity = Total_Quantity();
            return View();
        }
        // GET: Cart
        public ActionResult GoToCart()
        {
            if (Session["Cart"] == null) {
                return RedirectToAction("CartEmpty", "Cart");
            }
            List<Cart> listCart = getCart();
            ViewBag.Total_Amount = Total_Amount();
            ViewBag.Total_Quantity = Total_Quantity();
            return View(listCart);
        }

        public ActionResult CartPartial() {
            ViewBag.Total_Quantity = Total_Quantity();
            return PartialView();
        }

        public ActionResult UpdateCart(int iProduct_ID, FormCollection f)
        {
            if (Session["Cart"] == null)
            {
                TempData["ErrorMessage"] = "Giỏ hàng rỗng.";
                return RedirectToAction("GoToCart"); // Chuyển hướng đến trang giỏ hàng
            }

            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(iProduct_ID));
            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("GoToCart"); // Chuyển hướng đến trang giỏ hàng
            }

            List<Cart> listCart = getCart();
            Cart item = listCart.SingleOrDefault(it => it.product_ID.Equals(iProduct_ID));

            if (item != null)
            {
                int newQuantity;
                if (int.TryParse(f["txtQuantity"].ToString(), out newQuantity))
                {
                    if (product.SoLuongTon < newQuantity)
                    {
                        var errorMessage = string.Format("Sản phẩm {0} chỉ còn {1} sản phẩm", item.product_Name.ToString(), product.SoLuongTon.ToString());
                        TempData["ErrorMessage"] = errorMessage;
                        return RedirectToAction("GoToCart"); // Chuyển hướng đến trang giỏ hàng
                    }

                    item.product_Quantity = newQuantity;
                }
                else
                {
                    TempData["ErrorMessage"] = "Số lượng không hợp lệ.";
                    return RedirectToAction("GoToCart"); // Chuyển hướng đến trang giỏ hàng
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại trong giỏ hàng.";
                return RedirectToAction("GoToCart"); // Chuyển hướng đến trang giỏ hàng
            }

            return RedirectToAction("GoToCart"); // Chuyển hướng đến trang giỏ hàng sau khi cập nhật thành công
        }




        public ActionResult ItemCartRemove(int iProduct_ID) {
            if (Session["Cart"] == null) {
                return RedirectToAction("Home", "Home");
            }
            List<Cart> listCart = getCart();
            Cart item = listCart.Find(it => it.product_ID.Equals(iProduct_ID));
            if (item != null) {
                listCart.RemoveAll(it => it.product_ID.Equals(iProduct_ID));
                if (listCart.Count.Equals(0)) {
                    return RedirectToAction("CartEmpty", "Cart");
                }
                return RedirectToAction("GoToCart", "Cart");
            }
            return RedirectToAction("GoToCart", "Cart");
        }

        public ActionResult Checkout() {
            if (Session["Cart"] == null) {
                return RedirectToAction("Home", "Home");
            }
            List<Cart> listCart = getCart();
            ViewBag.Total_Amount = Total_Amount();
            return View(listCart);
        }
        [HttpPost]
        public ActionResult Checkout(KhachHang kh)
        {
            Session["Email"] = kh.Email;
            Session["Data"] = kh;
            return RedirectToAction("ConfirmCheckout");
        }

        public JsonResult ActivateDiscountCode(string code)
        {
            var totalPayment = Total_Amount();
            if (!String.IsNullOrEmpty(code)) {
                var promo = db.SuKienKhuyenMais.SingleOrDefault(n => n.MaKM.Equals(code));
                if (promo == null) {
                    return Json(new { status = 404, message = "Rất tiếc! Không tìm thấy voucher này" });
                }
                if (promo.NgayKetThuc < DateTime.Now) {
                    return Json(new { status = 404, message = "Rất tiếc! Voucher này đã hết hạn sử dụng" });
                }
                if (promo.SoLuongConLai == 0) {
                    return Json(new { status = 404, message = "Rất tiếc! Voucher này đã hết lượt sử dụng" }); 
                }
                if (totalPayment < promo.GiaTriDonHang) {
                    return Json(new { status = 406, message = "Rất tiếc! Đơn hàng của bạn không đạt giá trị tối thiểu để sử dụng mã này" });
                }
                List<Cart> lstCart = getCart();
                var lstProduct = db.SanPhams.Where(n => n.MaKM.Equals(code));
                if (lstProduct.Count() > 0)
                {
                    var countIsValid = 0;
                    foreach (var productDB in lstProduct)
                    {
                        foreach (var productCart in lstCart)
                        {
                            if (productCart.product_ID == productDB.MaSP) {
                                countIsValid++;
                            }
                        }
                    }
                    if (countIsValid == 0 || countIsValid < lstCart.Count) {
                        return Json(new { status = 406, message = "Rất tiếc! Có sản phẩm nào đó không áp dụng được mã này" });
                    }
                }
                if (promo.PhamViApDung == "Tất cả")
                {
                    totalPayment = (decimal)(totalPayment - promo.GiaTriKM);
                    return Json(new { status = 200, totalFinal = totalPayment, discount = promo.GiaTriKM, message = "Áp dụng mã khuyến mãi thành công" });
                }
                else if (promo.PhamViApDung == "Người mới")
                {
                    ThanhVien mem = Session["Account"] as ThanhVien;
                    if (mem == null) {
                        return Json(new { status = 404, message = "Vui lòng đăng nhập để sử dụng mã này" });
                    }
                    var checkCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
                    if (checkCus.Count() > 0) {
                        return Json(new { status = 406, message = "Rất tiếc! Voucher này chỉ dành cho người mới" });
                    }
                    totalPayment = (decimal)(totalPayment - promo.GiaTriKM);
                    
                    return Json(new { status = 200, totalFinal = totalPayment, discount = promo.GiaTriKM, message = "Áp dụng mã khuyến mãi thành công" });
                   
                }
            }
            return Json(new { status = false, message = "Chưa nhập mã"});
        }

        public ActionResult ConfirmCheckout()
        {
            if (Session["Cart"] == null || Session["Data"] == null)
            {
                return RedirectToAction("Home", "Home");
            }
            List<Cart> listCart = getCart();
            ViewBag.Total_Payment = Total_Amount();
            return View(listCart);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ConfirmCheckout(string note, string content, decimal? sale_price, string discountCode) {
            if (Session["Cart"] == null) {
                return RedirectToAction("Home", "Home");
            }
            var sale_price_db = sale_price != null ? sale_price : 0;
            List<Cart> listCart = getCart();
            // Lấy data khách hàng vãng lai
            KhachHang kh = Session["Data"] as KhachHang;
            ThanhVien member = Session["Account"] as ThanhVien;
            if (Session["Account"] == null)
            {
                db.KhachHangs.InsertOnSubmit(kh);
                db.SubmitChanges();
            }
            else
            {
                kh.MaThanhVien = member.MaTV;
                db.KhachHangs.InsertOnSubmit(kh);
                db.SubmitChanges();
            }
            DonDatHang ddh = new DonDatHang();
            ddh.NgayDat = DateTime.Now;
            ddh.ChoXacNhan = true;
            ddh.ChoLayHang = false;
            ddh.DangGiao = false;
            ddh.DaGiao = false;
            ddh.DaThanhToan = false;
            ddh.DaHuy = false;
            ddh.MaKH = kh.MaKH;
            ddh.GhiChu = note;
            ddh.DiaChi = kh.DiaChi;
            ddh.UuDai = sale_price_db;
            ddh.TinhThanh = kh.TinhThanh;
            ddh.QuanHuyen = kh.QuanHuyen;
            db.DonDatHangs.InsertOnSubmit(ddh);
            db.SubmitChanges();
            Session.Add("IDOrder", ddh.MaDDH);
            foreach (var item in listCart) {
                ChiTietDonDatHang ctddh = new ChiTietDonDatHang();
                ctddh.MaDDH = ddh.MaDDH;
                ctddh.MaSP = item.product_ID;
                ctddh.TenSP = item.product_Name;
                ctddh.SoLuong = item.product_Quantity;
                ctddh.DonGia = item.product_Price;
                // Lấy mã sản phẩm trong database
                SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(item.product_ID));
                // Cập nhật số lượng tồn trong database
                product.SoLuongTon -= item.product_Quantity;
                // Tăng số lần mua của sản phẩm
                product.SoLanMua += item.product_Quantity;
                db.ChiTietDonDatHangs.InsertOnSubmit(ctddh);
            }
            if (sale_price_db != 0) {
                var objPromo = db.SuKienKhuyenMais.SingleOrDefault(n => n.MaKM.Equals(discountCode));
                objPromo.SoLuongConLai -= 1;
                if (objPromo.SoLuongConLai == 0) {
                    objPromo.NgayKetThuc = DateTime.Now;
                }
            }
            db.SubmitChanges();
            var objEmail = db.MauEmails.SingleOrDefault(n => n.MaMauEmail.Equals(1));   
            var totalCart = Total_Amount();
            var totalPayment = totalCart - sale_price_db;
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{ORDERID}}", ddh.MaDDH.ToString());
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{TOTAL}}", String.Format("{0:0,0}", totalCart));
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{VOUCHER}}", String.Format("{0:0,0}", sale_price_db));
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{ADDRESS}}", kh.DiaChi);
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{CUSTOMER}}", kh.HoTen);
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{ORDERS}}", content);
            objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{TOTALPAYMENT}}", String.Format("{0:0,0}", totalPayment));
            new MailHelper().SendEmail(kh.Email, objEmail.SubjectEmail, objEmail.ContentEmail);
            return Json(new { redirectToUrl = Url.Action("Done", "Cart") });
        }
        public ActionResult Done() {
            if (Session["Cart"] == null || Session["IDOrder"] == null)
            {
                return RedirectToAction("Home", "Home");
            }
            List<Cart> listCart = getCart();
            ViewBag.Total_Amount = Total_Amount();
            Session["Cart"] = null;
            return View(listCart);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}