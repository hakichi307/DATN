using GVNClone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc;
using CaptchaMvc.HtmlHelpers;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Common;
using GVNClone.Models.StatusOrderViewModel;

namespace GVNClone.Controllers
{
    public class AuthController : Controller
    {
        dbChienThangDataContext db = new dbChienThangDataContext();

        [HttpGet]
        public ActionResult VerifyEmail(int type) {
            if (Session["Account"] != null) return RedirectToAction("Home", "Home");
            GlobalVariables.typeEmail = type;
            return View();
        }

        [HttpPost]
        public ActionResult VerifyEmail(string email)
        {
            var checkEmailExists = db.ThanhViens.SingleOrDefault(n => n.Email.Equals(email));
            if (checkEmailExists != null)
            {
                return Json(new { exist = true, status = false });
            }
            var subject = db.LoaiMauEmails.SingleOrDefault(n => n.MaLoaiMau.Equals(GlobalVariables.typeEmail));
            Random rd = new Random();
            var randomOTPCode = rd.Next(100000, 999999 + 1);
            GlobalVariables.OTPCode = randomOTPCode;
            var obj = db.MauEmails.SingleOrDefault(n => n.MaLoai.Equals(subject.MaLoaiMau));
            var content = obj.ContentEmail.Replace("{{OTPCODE}}", randomOTPCode.ToString());
            new MailHelper().SendEmail(email, subject.TenMau, content);
            GlobalVariables.Email_Auth = email;
            return Json(new { status = true });
        }

        [HttpGet]
        public ActionResult AuthenticateOTP() {
            if (GlobalVariables.OTPCode == 0) return RedirectToAction("Home", "Home");
            return View();
        }

        [HttpPost]
        public ActionResult AuthenticateOTP(string OTP)
        {
            if (GlobalVariables.OTPCode.ToString() != OTP) {
                return Json(new { isValid = false });
            }
            GlobalVariables.OTPCode = 0;
            GlobalVariables.handleRedirect = true;
            return Json(new { isValid = true });
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            if(!GlobalVariables.handleRedirect) return RedirectToAction("Home", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(ThanhVien tv, HttpPostedFileBase fUpload)
        {
            var checkEmailExists = db.ThanhViens.SingleOrDefault(n => n.Email.Equals(tv.Email));
            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                if (fUpload != null)
                {
                    if (fUpload.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(fUpload.FileName);
                        tv.Avatar = fileName;
                    }
                }
                else
                {
                    tv.Avatar = "anonymousUser.png";
                }
                //tv.MatKhau = GetMD5(tv.MatKhau);
                tv.MatKhau = GetSHA256(tv.MatKhau);
                tv.MaLoaiTV = 2;
                tv.Email = GlobalVariables.Email_Auth;
                db.ThanhViens.InsertOnSubmit(tv);
                db.SubmitChanges();
                var objTypeEmail = db.MauEmails.SingleOrDefault(n => n.MaMauEmail.Equals(2));
                new MailHelper().SendEmail(GlobalVariables.Email_Auth, objTypeEmail.SubjectEmail, objTypeEmail.ContentEmail);
                GlobalVariables.Email_Auth = "";
                GlobalVariables.handleRedirect = true;
                return RedirectToAction("SignIn");
            }
            TempData["CaptchaError"] = "Mã bảo vệ không đúng";
            return View();
        }

        //create a string MD5
        //public static string GetMD5(string str)
        //{
        //    using (MD5 md5 = MD5.Create())
        //    {
        //        byte[] fromData = Encoding.UTF8.GetBytes(str);
        //        byte[] targetData = md5.ComputeHash(fromData);

        //        StringBuilder byte2String = new StringBuilder();
        //        for (int i = 0; i < targetData.Length; i++)
        //        {
        //            byte2String.Append(targetData[i].ToString("x2"));
        //        }
        //        return byte2String.ToString();
        //    }
        //}

        // Tạo một string hash với SHA-256
        public static string GetSHA256(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] fromData = Encoding.UTF8.GetBytes(str);
                byte[] targetData = sha256.ComputeHash(fromData);

                StringBuilder byte2String = new StringBuilder();
                for (int i = 0; i < targetData.Length; i++)
                {
                    byte2String.Append(targetData[i].ToString("x2"));
                }
                return byte2String.ToString();
            }
        }
        [HttpGet]
        public ActionResult SignIn()
        {
            if (Session["Account"] != null) return RedirectToAction("Home", "Home");
            return View();
        }


        [HttpPost]
        public ActionResult SignIn(FormCollection f)
        {
            string sUserName = f["Email"].ToString();
            string sPassword = f["MatKhau"].ToString();
            //var _password = GetMD5(sPassword);
            var _password = GetSHA256(sPassword); // Sử dụng SHA-256
            ThanhVien tv = db.ThanhViens.SingleOrDefault(member => member.Email.Equals(sUserName) && member.MatKhau.Equals(_password));
            if (tv != null)
            {
                Session["Account"] = tv;
                Session.Add("UserName", tv.TaiKhoan);
                Session.Add("IdUser", tv.MaTV);
                Session.Add("Avatar", tv.Avatar);
                Session.Add("Email", tv.Email);
                var lstRoles = db.LoaiThanhVien_Quyens.Where(n => n.MaLoaiTV.Equals(tv.MaLoaiTV));
                string roles = ""; // Tạo ra biến để lưu trữ các quyền khi đăng nhập
                foreach (var item in lstRoles)
                {
                    roles += item.Quyen.MaQuyen + ",";
                }
                roles = roles.Substring(0, roles.Length - 1); // Cắt đi dấu phẩy
                Authentication(tv.Email, roles);
                if (roles.Equals("QuanTri"))
                {
                    return Content("<script>window.location.href = '/admin/dashboard'</script>");
                }
                return Content("<script>checkPreviousPage();</script>");
            }
            return Content("Thông tin đăng nhập không hợp lệ.");
        }



        public void Authentication(string email, string roles)
        {
            FormsAuthentication.Initialize(); // Khởi tạo
            var ticket = new FormsAuthenticationTicket(
                                                        1, //version
                                                        email, // name: Vì Email chỉ có 1 không có email trùng nên mình truyền vào                                  email (Quan trọng)
                                                        DateTime.Now, // begin
                                                        DateTime.Now.AddHours(3), // timeout, tính từ lúc bắt đầu, có hiệu lực 3h
                                                        false, //remember ? 
                                                        roles, // Chuỗi quyền được cấp (Quan trọng)
                                                        FormsAuthentication.FormsCookiePath //Đường dẫn cookie, có thể tự truyền                                                                vào, ở đây mình để mặc định (nên)
                                                        );
            /*
                Khởi tạo cookie
                Tham số 1: name (tự đặt tên tuỳ ý, mình vẫn để mặc định)
                Tham số 2: Truyền vào giá trị để mã hoá, tăng cường bảo mật, tránh hacker :DD
            */
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            if (ticket.IsPersistent)
            { // Nếu có duy trì đăng nhập
                cookie.Expires = ticket.Expiration; // Set timeout cho cookie
            }
            Response.Cookies.Add(cookie); // Server sẽ lưu cookie vào storage
        }

        public ActionResult SignOut()
        {
            Session["Account"] = null;
            GlobalVariables.handleRedirect = true;
            FormsAuthentication.SignOut();
            return RedirectToAction("Home", "Home");
        }


        public ActionResult NotFound()
        {
            return View();
        }


        public ActionResult Information(int? id)
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Home", "Home");
            }
            if (id == null)
            {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = db.ThanhViens.SingleOrDefault(n => n.MaTV.Equals(id));
            if (mem == null)
            {
                return RedirectToAction("NotFound");
            }
            return View(mem);
        }

        [HttpPost]
        public ActionResult Information(ThanhVien tv, HttpPostedFileBase fUpload)
        {
            ThanhVien member = db.ThanhViens.SingleOrDefault(mem => mem.MaTV.Equals(int.Parse(Session["IdUser"].ToString())));
            Session["UserName"] = tv.TaiKhoan;
            member.HoTen = tv.HoTen;
            member.DiaChi = tv.DiaChi;
            member.TaiKhoan = tv.TaiKhoan;
            member.SoDienThoai = tv.SoDienThoai;
            if (fUpload != null)
            {
                if (fUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fUpload.FileName);
                    member.Avatar = fileName;
                    Session["Avatar"] = fileName;
                }
            }
            Session["Account"] = member;
            db.SubmitChanges();
            return View();
        }


        [HttpPost]
        public ActionResult ChangePassword(string newPassword) {
            ThanhVien member = db.ThanhViens.SingleOrDefault(mem => mem.MaTV.Equals(int.Parse(Session["IdUser"].ToString())));
            member.MatKhau = GetSHA256(newPassword); // Sử dụng SHA-256 
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Đổi mật khẩu thành công", redirectUrl = Url.Action("Information", "Auth", new { id = member.MaTV }) });
        }

        [HttpPost]
        public ActionResult CheckCurrentPassword(string curPassword)
        {
            ThanhVien member = db.ThanhViens.SingleOrDefault(mem => mem.MaTV.Equals(int.Parse(Session["IdUser"].ToString())));
            if (member.MatKhau == GetSHA256(curPassword)) {
                return Json(new { message = "Ok" });
            }
            return Json(new { message = "Your current password is not valid!" });
        }


        public ActionResult ProcessImage(HttpPostedFileBase fileUpload) {
            if (fileUpload != null)
            {
                if (fileUpload.ContentLength > 0)
                {
                    var urlImage = "/Images/";
                    fileUpload.SaveAs(Server.MapPath("/Images/" + fileUpload.FileName));
                    return Json(new { statusCode = 200, data = urlImage + fileUpload.FileName });
                }
            }
            return Json(new { statusCode = 404, message = "Lỗi tên file ảnh" });
        }


        public bool isEvaluated(int id) {
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objRate = db.DanhGias.Where(n => n.MaTV.Equals(mem.MaTV) && n.MaSP.Equals(id));
            if (objRate.Count() == 0) {
                return false;
            }
            return true;
        }


        public ActionResult AllOrderPartialView() {
            if (Session["Account"] == null) {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            var detailOrder = db.ChiTietDonDatHangs;
            
            List<Order> lstOrder = new List<Order>();
            foreach (var cus in objCus) {
                foreach (var itemCart in detailOrder) {
                    if (itemCart.DonDatHang.MaKH.Equals(cus.MaKH)) {
                        Order objCart = new Order();
                        objCart.product_id = int.Parse(itemCart.MaSP.ToString());
                        objCart.product_thumb = itemCart.SanPham.Pic1;
                        objCart.product_name = itemCart.SanPham.TenSP;
                        objCart.product_price = decimal.Parse(itemCart.DonGia.ToString());
                        objCart.product_quantity = int.Parse(itemCart.SoLuong.ToString());
                        objCart.WaitForConfirmation = bool.Parse(itemCart.DonDatHang.ChoXacNhan.ToString());
                        objCart.WaitForGetGoods = bool.Parse(itemCart.DonDatHang.ChoLayHang.ToString());
                        objCart.Delivering = bool.Parse(itemCart.DonDatHang.DangGiao.ToString());
                        objCart.Delivered = bool.Parse(itemCart.DonDatHang.DaGiao.ToString());
                        objCart.Canceled = bool.Parse(itemCart.DonDatHang.DaHuy.ToString());
                        objCart.isEvaluated = isEvaluated(int.Parse(itemCart.MaSP.ToString()));
                        lstOrder.Add(objCart);
                    }
                }
            }
            ViewBag.AllOrder = lstOrder;
            return PartialView();
        }


        public ActionResult WaitForConfirmationOrderPartialView()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            var detailOrder = db.ChiTietDonDatHangs;

            List<Order> lstOrder = new List<Order>();
            foreach (var cus in objCus)
            {
                foreach (var itemCart in detailOrder)
                {
                    if (itemCart.DonDatHang.ChoXacNhan == true && itemCart.DonDatHang.MaKH.Equals(cus.MaKH)) {
                        Order objCart = new Order();
                        objCart.product_id = int.Parse(itemCart.MaSP.ToString());
                        objCart.product_thumb = itemCart.SanPham.Pic1;
                        objCart.product_name = itemCart.SanPham.TenSP;
                        objCart.product_price = decimal.Parse(itemCart.DonGia.ToString());
                        objCart.product_quantity = int.Parse(itemCart.SoLuong.ToString());
                        objCart.WaitForConfirmation = bool.Parse(itemCart.DonDatHang.ChoXacNhan.ToString());
                        objCart.WaitForGetGoods = bool.Parse(itemCart.DonDatHang.ChoLayHang.ToString());
                        objCart.Delivering = bool.Parse(itemCart.DonDatHang.DangGiao.ToString());
                        objCart.Delivered = bool.Parse(itemCart.DonDatHang.DaGiao.ToString());
                        objCart.Canceled = bool.Parse(itemCart.DonDatHang.DaHuy.ToString());
                        objCart.isEvaluated = isEvaluated(int.Parse(itemCart.MaSP.ToString()));
                        lstOrder.Add(objCart);
                    }
                }
            }
            ViewBag.WaitForConfirmationOrder = lstOrder;
            return PartialView();
        }

        public ActionResult WaitForGetGoodsOrderPartialView()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            var detailOrder = db.ChiTietDonDatHangs;

            List<Order> lstOrder = new List<Order>();
            foreach (var cus in objCus)
            {
                foreach (var itemCart in detailOrder)
                {
                    if (itemCart.DonDatHang.ChoLayHang == true && itemCart.DonDatHang.MaKH.Equals(cus.MaKH))
                    {
                        Order objCart = new Order();
                        objCart.product_id = int.Parse(itemCart.MaSP.ToString());
                        objCart.product_thumb = itemCart.SanPham.Pic1;
                        objCart.product_name = itemCart.SanPham.TenSP;
                        objCart.product_price = decimal.Parse(itemCart.DonGia.ToString());
                        objCart.product_quantity = int.Parse(itemCart.SoLuong.ToString());
                        objCart.WaitForConfirmation = bool.Parse(itemCart.DonDatHang.ChoXacNhan.ToString());
                        objCart.WaitForGetGoods = bool.Parse(itemCart.DonDatHang.ChoLayHang.ToString());
                        objCart.Delivering = bool.Parse(itemCart.DonDatHang.DangGiao.ToString());
                        objCart.Delivered = bool.Parse(itemCart.DonDatHang.DaGiao.ToString());
                        objCart.Canceled = bool.Parse(itemCart.DonDatHang.DaHuy.ToString());
                        objCart.isEvaluated = isEvaluated(int.Parse(itemCart.MaSP.ToString()));
                        lstOrder.Add(objCart);
                    }
                }
            }
            ViewBag.WaitForGetGoodsOrder = lstOrder;
            return PartialView();
        }


        public ActionResult DeliveringOrderPartialView()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            var detailOrder = db.ChiTietDonDatHangs;

            List<Order> lstOrder = new List<Order>();
            foreach (var cus in objCus)
            {
                foreach (var itemCart in detailOrder)
                {
                    if (itemCart.DonDatHang.DangGiao == true && itemCart.DonDatHang.MaKH.Equals(cus.MaKH))
                    {
                        Order objCart = new Order();
                        objCart.product_id = int.Parse(itemCart.MaSP.ToString());
                        objCart.product_thumb = itemCart.SanPham.Pic1;
                        objCart.product_name = itemCart.SanPham.TenSP;
                        objCart.product_price = decimal.Parse(itemCart.DonGia.ToString());
                        objCart.product_quantity = int.Parse(itemCart.SoLuong.ToString());
                        objCart.WaitForConfirmation = bool.Parse(itemCart.DonDatHang.ChoXacNhan.ToString());
                        objCart.WaitForGetGoods = bool.Parse(itemCart.DonDatHang.ChoLayHang.ToString());
                        objCart.Delivering = bool.Parse(itemCart.DonDatHang.DangGiao.ToString());
                        objCart.Delivered = bool.Parse(itemCart.DonDatHang.DaGiao.ToString());
                        objCart.Canceled = bool.Parse(itemCart.DonDatHang.DaHuy.ToString());
                        objCart.isEvaluated = isEvaluated(int.Parse(itemCart.MaSP.ToString()));
                        lstOrder.Add(objCart);
                    }
                }
            }
            ViewBag.DeliveringOrder = lstOrder;
            return PartialView();
        }


        public ActionResult DeliveredOrderPartialView()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            var detailOrder = db.ChiTietDonDatHangs;

            List<Order> lstOrder = new List<Order>();
            foreach (var cus in objCus)
            {
                foreach (var itemCart in detailOrder)
                {
                    if (itemCart.DonDatHang.DaGiao == true && itemCart.DonDatHang.MaKH.Equals(cus.MaKH))
                    {
                        Order objCart = new Order();
                        objCart.product_id = int.Parse(itemCart.MaSP.ToString());
                        objCart.product_thumb = itemCart.SanPham.Pic1;
                        objCart.product_name = itemCart.SanPham.TenSP;
                        objCart.product_price = decimal.Parse(itemCart.DonGia.ToString());
                        objCart.product_quantity = int.Parse(itemCart.SoLuong.ToString());
                        objCart.WaitForConfirmation = bool.Parse(itemCart.DonDatHang.ChoXacNhan.ToString());
                        objCart.WaitForGetGoods = bool.Parse(itemCart.DonDatHang.ChoLayHang.ToString());
                        objCart.Delivering = bool.Parse(itemCart.DonDatHang.DangGiao.ToString());
                        objCart.Delivered = bool.Parse(itemCart.DonDatHang.DaGiao.ToString());
                        objCart.Canceled = bool.Parse(itemCart.DonDatHang.DaHuy.ToString());
                        objCart.isEvaluated = isEvaluated(int.Parse(itemCart.MaSP.ToString()));
                        lstOrder.Add(objCart);
                    }
                }
            }
            ViewBag.DeliveredOrder = lstOrder;
            return PartialView();
        }

        public ActionResult CanceledOrderPartialView()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("NotFound");
            }
            ThanhVien mem = (ThanhVien)Session["Account"];
            var objCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            var detailOrder = db.ChiTietDonDatHangs;

            List<Order> lstOrder = new List<Order>();
            foreach (var cus in objCus)
            {
                foreach (var itemCart in detailOrder)
                {
                    if (itemCart.DonDatHang.DaHuy == true && itemCart.DonDatHang.MaKH.Equals(cus.MaKH))
                    {
                        Order objCart = new Order();
                        objCart.product_id = int.Parse(itemCart.MaSP.ToString());
                        objCart.product_thumb = itemCart.SanPham.Pic1;
                        objCart.product_name = itemCart.SanPham.TenSP;
                        objCart.product_price = decimal.Parse(itemCart.DonGia.ToString());
                        objCart.product_quantity = int.Parse(itemCart.SoLuong.ToString());
                        objCart.WaitForConfirmation = bool.Parse(itemCart.DonDatHang.ChoXacNhan.ToString());
                        objCart.WaitForGetGoods = bool.Parse(itemCart.DonDatHang.ChoLayHang.ToString());
                        objCart.Delivering = bool.Parse(itemCart.DonDatHang.DangGiao.ToString());
                        objCart.Delivered = bool.Parse(itemCart.DonDatHang.DaGiao.ToString());
                        objCart.Canceled = bool.Parse(itemCart.DonDatHang.DaHuy.ToString());
                        objCart.isEvaluated = isEvaluated(int.Parse(itemCart.MaSP.ToString()));
                        lstOrder.Add(objCart);
                    }
                }
            }
            ViewBag.CancelOrder = lstOrder;
            return PartialView();
        }

    }
}