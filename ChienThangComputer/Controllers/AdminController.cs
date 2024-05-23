using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using System.Configuration;
using System.Web.Security;
using CaptchaMvc.HtmlHelpers;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;
using ChienThangComputer;
namespace ChienThangComputer.Controllers
{
    //[Authorize(Roles = "QuanTri")]
    public class AdminController : Controller
    {
        dbChienThangDataContext db = new dbChienThangDataContext();

        //login admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            password = GetSHA256(password);
            var query = db.ThanhViens.Where(x => x.Email == email && x.MatKhau == password).SingleOrDefault();
            if (query == null)
            {
                ViewBag.mess = "Email đăng nhập hoặc mật khẩu không đúng";
                return View();
            }
            else
            {
                if (query.MaLoaiTV != 1)
                {
                    ViewBag.mess = "Bạn không có quyền truy cập";
                    return View();
                }
            }
            Session["TaiKhoan"] = query.TaiKhoan;
            return RedirectToAction("Dashboard", "Admin");
        }

        // end login admin
        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Dashboard", "Admin");
        }

        private int Count_Order()
        {
            return db.DonDatHangs.OrderBy(c => c.MaDDH).Count();
        }

        private decimal Total_Income()
        {
            decimal totalIncome = 0;
            var check = db.ChiTietDonDatHangs.Count();
            if (check == 0)
            {
                return totalIncome;
            }
            var listDetail_Order = db.ChiTietDonDatHangs.ToList();
            foreach (var item in listDetail_Order)
            {
                if (item.DonDatHang.DaThanhToan == true)
                {
                    totalIncome += (decimal)(item.SoLuong * item.DonGia);
                }
            }
            return totalIncome;
        }

        private int Total_Member()
        {
            return db.ThanhViens.OrderBy(c => c.MaTV).Count();
        }

        public void GetData()
        {
            ViewBag.Total_Order = Count_Order();
            ViewBag.Total_Income = Total_Income();
            ViewBag.Total_Member = Total_Member();
        }

        public void LoadDataDropdownlist()
        {
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            ViewBag.MaLoaiSP = new SelectList(db.LoaiSanPhams.OrderBy(n => n.MaLoaiSP), "MaLoaiSP", "TenLoaiSP");
            ViewBag.MaLoaiDM = new SelectList(db.LoaiDanhMucs.OrderBy(n => n.MaLoaiDM), "MaLoaiDM", "TenLoaiDM");
            ViewBag.IDDM = new SelectList(db.DanhMucs.OrderBy(n => n.IDDM), "IDDM", "TenDM");
            ViewBag.MaPL = new SelectList(db.PhanLopKhachHangs.OrderBy(n => n.MaPL), "MaPL", "TenPL");
        }

        public ActionResult Dashboard()
        {
            var listLatestOrder = db.ChiTietDonDatHangs.OrderByDescending(n => n.MaChiTietDDH).ToList();
            if (listLatestOrder.Count == 0)
            {
                ViewBag.NothingToShow = 0;
            }
            GetData();
            ViewBag.Best_Sellers = db.SanPhams.OrderByDescending(n => n.SoLanMua).Take(4);
            ViewBag.ShowAllProduct = db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.DaXoa == false);
            ViewBag.ListOrder = db.DonDatHangs.OrderBy(n => n.MaDDH);
            return View(listLatestOrder);
        }


        public ActionResult AllProducts()
        {
            GetData();
            return View(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.DaXoa == false));
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            GetData();
            LoadDataDropdownlist();
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddProduct(SanPham product, HttpPostedFileBase[] fUpload)
        {
            GetData();
            LoadDataDropdownlist();
            for (int i = 0; i < fUpload.Length; i++)
            {
                if (fUpload[i] != null)
                {
                    if (fUpload[i].ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(fUpload[i].FileName);
                        var path = Path.Combine(Server.MapPath("/Images"), fileName);
                        if (System.IO.File.Exists(path))
                        {
                            //if (i == 0)
                            //{
                            //    TempData["UploadFail1"] = "Hình ảnh đã tồn tại!";
                            //    return View();
                            //}
                            //else if(i == 1){
                            //    TempData["UploadFai2"] = "Hình ảnh đã tồn tại!";
                            //    return View();
                            //}
                            //else if (i == 2)
                            //{
                            //    TempData["UploadFai3"] = "Hình ảnh đã tồn tại!";
                            //    return View();
                            //}
                            //else if (i == 3)
                            //{
                            //    TempData["UploadFai4"] = "Hình ảnh đã tồn tại!";
                            //    return View();
                            //}
                            TempData["UploadFail1"] = "Hình đã tồn tại";
                            return View();
                        }
                        else
                        {
                            fUpload[i].SaveAs(path);
                            if (i == 0)
                            {
                                product.Pic1 = fUpload[i].FileName;
                            }
                            if (i == 1)
                            {
                                product.Pic2 = fUpload[i].FileName;
                            }
                            if (i == 2)
                            {
                                product.Pic3 = fUpload[i].FileName;
                            }
                            if (i == 3)
                            {
                                product.Pic4 = fUpload[i].FileName;
                            }
                        }
                    }
                }
            }
            product.SoLuongTon = 0;
            product.SoLanMua = 0;
            product.DaXoa = false;
            product.NgayCapNhat = DateTime.Today;
            db.SanPhams.InsertOnSubmit(product);
            db.SubmitChanges();
            TempData["Added"] = "Sản phẩm đã được thêm vào hệ thống";
            ModelState.Clear();
            return RedirectToAction("Dashboard", "Admin");
        }
        [HttpGet]
        public ActionResult EditProduct(int IdProduct)
        {
            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(IdProduct));
            GetData();
            LoadDataDropdownlist();
            if (product == null)
            {
                return RedirectToAction("NotFound", "Auth");
            }
            return View(product);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditProduct(SanPham newProduct, HttpPostedFileBase[] fUpload)
        {
            SanPham productEdit = db.SanPhams.Single(pd => pd.MaSP.Equals(newProduct.MaSP));
            if (productEdit == null)
            {
                return HttpNotFound("There is something wrong. please try again");
            }
            GetData();
            LoadDataDropdownlist();
            productEdit.TenSP = newProduct.TenSP;
            productEdit.GiaNiemYet = newProduct.GiaNiemYet;
            productEdit.SoLuongTon = newProduct.SoLuongTon;
            productEdit.NgayCapNhat = DateTime.Now;
            productEdit.BaoHanh = newProduct.BaoHanh;
            productEdit.ThongSoKyThuat = newProduct.ThongSoKyThuat;
            productEdit.MoTa = newProduct.MoTa;
            productEdit.TinhTrang = newProduct.TinhTrang;
            productEdit.MaNCC = newProduct.MaNCC;
            productEdit.MaLoaiDM = newProduct.MaLoaiDM;
            productEdit.MaLoaiSP = newProduct.MaLoaiSP;
            productEdit.IDDM = newProduct.IDDM;
            productEdit.MaPL = newProduct.MaPL;
            for (int i = 0; i < fUpload.Length; i++)
            {
                if (fUpload[i] != null)
                {
                    if (fUpload[i].ContentLength > 0)
                    {
                        var filename = Path.GetFileName(fUpload[i].FileName);
                        var path = Path.Combine(Server.MapPath("/Images"), filename);
                        if (i == 0)
                        {
                            fUpload[i].SaveAs(path);
                            productEdit.Pic1 = fUpload[i].FileName;
                        }
                        if (i == 1)
                        {
                            fUpload[i].SaveAs(path);
                            productEdit.Pic2 = fUpload[i].FileName;
                        }
                        if (i == 2)
                        {
                            fUpload[i].SaveAs(path);
                            productEdit.Pic3 = fUpload[i].FileName;
                        }
                        if (i == 3)
                        {
                            fUpload[i].SaveAs(path);
                            productEdit.Pic4 = fUpload[i].FileName;
                        }
                    }
                }
            }
            db.SubmitChanges();
            TempData["Edited"] = "Sửa thông tin sản phẩm thành công";
            return RedirectToAction("AllProducts", "Admin");
        }

        [HttpPost]
        public ActionResult DeleteProduct(int? IdProduct)
        {
            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(IdProduct));
            if (product == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id sản phẩm này" });
            }
            product.DaXoa = true;
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Sản phẩm đã được xoá khỏi hệ thống" });
        }

        public ActionResult OutOfStock()
        {
            GetData();
            return View(db.SanPhams.Where(pd => pd.SoLuongTon < 10 && pd.DaXoa == false).ToList());
        }
        [HttpGet]
        public ActionResult EditQuantityInStock(int? IdProduct)
        {
            GetData();
            SanPham product = db.SanPhams.SingleOrDefault(pd => pd.MaSP.Equals(IdProduct));
            if (product == null)
            {
                return RedirectToAction("NotFound", "Auth");
            }
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            return View(product);
        }

        //Nhập hàng cho 1 sản phẩm
        [HttpPost]
        public ActionResult EditQuantityInStock(PhieuNhap irv, ChiTietPhieuNhap dirv)
        {
            GetData();
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            irv.NgayNhap = DateTime.Now;
            irv.DaXoa = false;
            irv.TongTienThanhToan = dirv.DonGiaNhap;
            irv.SoTienDaTra = dirv.DonGiaNhap;
            irv.SoTienNo = 0;
            db.PhieuNhaps.InsertOnSubmit(irv);
            db.SubmitChanges();
            dirv.MaPN = irv.MaPN;
            SanPham product = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(dirv.MaSP));
            product.SoLuongTon += dirv.SoLuongNhap;
            db.ChiTietPhieuNhaps.InsertOnSubmit(dirv);
            db.SubmitChanges();
            return View(product);
        }

        // Nhập hàng cho nhiều sản phẩm
        [HttpGet]
        public ActionResult PurchaseProducts()
        {
            GetData();
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            ViewBag.MaSP = db.SanPhams.Where(n => n.DaXoa == false);
            return View();
        }
        [HttpPost]
        public ActionResult PurchaseProducts(PhieuNhap irv, IEnumerable<ChiTietPhieuNhap> lstModel)
        {
            GetData();
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.OrderBy(n => n.MaNCC), "MaNCC", "TenNCC");
            ViewBag.MaSP = db.SanPhams.Where(n => n.DaXoa == false);
            irv.NgayNhap = DateTime.Now;
            irv.DaXoa = false;
            irv.TongTienThanhToan = lstModel.Sum(n => n.DonGiaNhap);
            if (irv.SoTienDaTra == null)
            {
                irv.SoTienDaTra = irv.TongTienThanhToan;
                irv.SoTienNo = 0;
            }
            else
            {
                irv.SoTienNo = irv.TongTienThanhToan - irv.SoTienDaTra;
            }
            db.PhieuNhaps.InsertOnSubmit(irv);
            db.SubmitChanges();
            SanPham pd;
            foreach (var item in lstModel)
            {
                pd = db.SanPhams.Single(n => n.MaSP.Equals(item.MaSP));
                pd.SoLuongTon += item.SoLuongNhap;
                item.MaPN = irv.MaPN;
            }
            db.ChiTietPhieuNhaps.InsertAllOnSubmit(lstModel);
            db.SubmitChanges();
            return View();
        }


        // Handle Order

        public ActionResult ListOfOrder()
        {
            GetData();
            return View(db.DonDatHangs.OrderBy(n => n.MaDDH).ToList());
        }
        public ActionResult OrderProcessing()
        {
            GetData();
            return View(db.DonDatHangs.Where(d => d.ChoXacNhan == true).OrderBy(d => d.MaDDH).ToList());
        }
        public ActionResult ListPresent()
        {
            GetData();
            var gifts = db.QuaTangs.OrderBy(n => n.MaQT).ToList();  // Lấy danh sách quà tặng sắp xếp theo MaQT
            return View(gifts);
        }
        [HttpPost]
        public ActionResult DetailOrder(int? id)
        {
            GetData();
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id đơn hàng!", redirectUrl = Url.Action("NotFound", "Auth") });
            }
            var discount = db.DonDatHangs.SingleOrDefault(n => n.MaDDH.Equals(id)).UuDai;
            ViewBag.Order = db.DonDatHangs.SingleOrDefault(n => n.MaDDH.Equals(id));
            ViewBag.TotalPayment = db.ChiTietDonDatHangs.Where(n => n.MaDDH.Equals(id)).Sum(n => n.SoLuong * n.DonGia) - discount;
            return View(db.ChiTietDonDatHangs.Where(n => n.MaDDH.Equals(id)).OrderByDescending(n => n.MaChiTietDDH).ToList());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ChangeStatusOrder(int id, string status, string content)
        {
            var objOrder = db.DonDatHangs.SingleOrDefault(n => n.MaDDH.Equals(id));
            if (status == "Chờ xác nhận")
            {
                objOrder.ChoXacNhan = false;
                objOrder.ChoLayHang = true;
            }
            else if (status == "Chờ lấy hàng")
            {
                objOrder.ChoLayHang = false;
                objOrder.DangGiao = true;
            }
            else if (status == "Đang giao")
            {
                objOrder.DangGiao = false;
                objOrder.DaGiao = true;
                objOrder.DaThanhToan = true;
            }
            else if (status == "cancel")
            {
                objOrder.ChoXacNhan = false;
                objOrder.ChoLayHang = false;
                objOrder.DangGiao = false;
                objOrder.DaGiao = false;
                objOrder.DaThanhToan = false;
                objOrder.DaHuy = true;
                var objOrderDetail = db.ChiTietDonDatHangs.Where(n => n.MaDDH.Equals(id));
                foreach (var item in objOrderDetail)
                {
                    SanPham product = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(item.MaSP));
                    product.SoLuongTon += item.SoLuong;
                    product.SoLanMua -= item.SoLuong;
                }
                db.SubmitChanges();
                var totalPayment = db.ChiTietDonDatHangs.Where(n => n.MaDDH.Equals(id)).Sum(n => n.SoLuong * n.DonGia);
                var objEmail = db.MauEmails.SingleOrDefault(n => n.MaMauEmail.Equals(3));
                objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{ORDERID}}", id.ToString());
                objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{ORDERS}}", content);
                objEmail.ContentEmail = objEmail.ContentEmail.Replace("{{TOTALPAYMENT}}", String.Format("{0:0,0}", totalPayment));
                new MailHelper().SendEmail(objOrder.KhachHang.Email, objEmail.SubjectEmail, objEmail.ContentEmail);
                return Json(new { statusCode = 200, message = "Đơn hàng đã được huỷ" });
            }
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Cập nhật thành công", status1 = objOrder.ChoXacNhan, status2 = objOrder.ChoLayHang, status3 = objOrder.DangGiao, status4 = objOrder.DaGiao, status5 = objOrder.DaHuy });
        }

        public ActionResult OrderExpired()
        {
            GetData();
            return View(db.DonDatHangs.Where(d => d.DaHuy == true).OrderBy(d => d.MaDDH).ToList());
        }

        public ActionResult DeleteOrder(int id)
        {
            DonDatHang order = db.DonDatHangs.SingleOrDefault(n => n.MaDDH.Equals(id));
            if (order == null)
            {
                // Nếu không tìm thấy đơn đặt hàng, chuyển hướng đến trang lỗi hoặc thông báo lỗi
                TempData["ErrorMessage"] = "Không tìm thấy đơn đặt hàng này!";
                return RedirectToAction("OrderExpired", "Admin");
            }
            // Xóa tất cả các bản ghi liên quan trong bảng "ChiTietDonDatHang"
            var chiTietDonDatHangs = db.ChiTietDonDatHangs.Where(c => c.MaDDH == id);
            db.ChiTietDonDatHangs.DeleteAllOnSubmit(chiTietDonDatHangs);

            db.DonDatHangs.DeleteOnSubmit(order);
            db.SubmitChanges();
            TempData["DeletedOrder"] = "Đơn hàng này đã được xoá khỏi hệ thống!";
            return RedirectToAction("OrderExpired", "Admin");
        }

        public ActionResult OrderShipped() //Đơn hàng dã giao
        {
            GetData();
            return View(db.DonDatHangs.Where(n => n.DaGiao == true).ToList());
        }
        public ActionResult StatisticTotalIncome()
        {
            GetData();
            ViewBag.Total_Income = Total_Income();
            return View(db.ChiTietDonDatHangs.OrderBy(n => n.MaChiTietDDH).ToList());
        }

        private decimal StatisticByMonth(int month, int year)
        {
            decimal total = 0;
            var check = db.ChiTietDonDatHangs.Count();
            if (check == 0)
            {
                return total;
            }
            else
            {
                var getList = db.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year));
                foreach (var item in getList)
                {
                    total += (decimal)(item.ChiTietDonDatHangs.Where(n => n.DonDatHang.DaGiao == true).Sum(n => n.SoLuong * n.DonGia));
                }
            }
            return total;
        }

        [HttpGet]
        public ActionResult AddPresent()
        {
            GetData();
            ViewBag.MaSP = new SelectList(db.SanPhams.OrderBy(pd => pd.MaSP).Where(n => n.DaXoa == false), "MaSP", "TenSP");
            return View();
        }
        [HttpPost]
        public ActionResult AddPresent(QuaTang present)
        {
            ViewBag.MaSP = new SelectList(db.SanPhams.OrderBy(pd => pd.MaSP).Where(n => n.DaXoa == false), "MaSP", "TenSP");
            GetData();
            var getID = int.Parse(present.MaSP.ToString());
            if (CheckNameOfPresent(present.TenQT, getID))
            {
                db.QuaTangs.InsertOnSubmit(present);
                db.SubmitChanges();
                TempData["AddedPresent"] = "Thêm quà tặng thành công";
                ModelState.Clear();
                return View();
            }
            else
            {
                ViewBag.Exist = "Tên quà tặng này đã tồn tại!";
                return View();
            }
        }

        private bool CheckNameOfPresent(string name, int id)
        {
            var check = db.QuaTangs.Where(n => n.MaSP.Equals(id) && n.TenQT.Equals(name));
            if (check.Any())
                return false;
            return true;
        }

        public ActionResult Present()
        {
            GetData();
            return View(db.SanPhams.Where(n => n.DaXoa == false).ToList());
        }

        public ActionResult ViewPresent(int id)
        {
            return PartialView(db.QuaTangs.Where(n => n.MaSP.Equals(id)).ToList());
        }

        [HttpPost]
        public ActionResult DeletePresent(int? id)
        {
            QuaTang present = db.QuaTangs.SingleOrDefault(pre => pre.MaQT.Equals(id));
            if (present == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại quà tặng này" });
            }
            db.QuaTangs.DeleteOnSubmit(present);
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Gỡ quà tặng thành công" });
        }

        public ActionResult Admin()
        {
            GetData();
            var lstAdmin = db.ThanhViens.Where(n => n.MaLoaiTV.Equals(1));
            return View(lstAdmin);
        }

        public ActionResult Customer()
        {
            GetData();
            var lstCustomer = db.ThanhViens.Where(n => n.MaLoaiTV.Equals(2));
            return View(lstCustomer);
        }

        public static string GetSHA256(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new StringBuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        [HttpGet]
        public ActionResult AddMember()
        {
            GetData();
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens.OrderBy(n => n.TenLoaiTV), "MaLoaiTV", "TenLoaiTV");
            return View();
        }
        [HttpPost]
        public ActionResult AddMember(ThanhVien newMember)
        {
            GetData();
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens.OrderBy(n => n.TenLoaiTV), "MaLoaiTV", "TenLoaiTV");
            // Check if email already exists
            var existingMember = db.ThanhViens.FirstOrDefault(tv => tv.Email == newMember.Email);
            if (existingMember != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng sử dụng một email khác.");
                return View(newMember);
            }
            newMember.Avatar = "admin_avatar.jpg";
            db.ThanhViens.InsertOnSubmit(newMember);
            newMember.MatKhau = GetSHA256(newMember.MatKhau);
            db.SubmitChanges();
            TempData["AddMemberSuccess"] = "Thêm thành viên thành công";
            ModelState.Clear();
            return RedirectToAction("Admin");
        }

        [HttpGet]
        public ActionResult EditMember(int? id)
        {
            GetData();
            if (id == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            ThanhVien member = db.ThanhViens.SingleOrDefault(n => n.MaTV.Equals(id));
            if (member == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens.OrderBy(n => n.TenLoaiTV), "MaLoaiTV", "TenLoaiTV", member.MaLoaiTV);
            return View(member);
        }

        [HttpPost]
        public ActionResult EditMember(ThanhVien member)
        {
            GetData();
            ThanhVien t = db.ThanhViens.SingleOrDefault(n => n.MaTV.Equals(member.MaTV));
            ViewBag.MaLoaiTV = new SelectList(db.LoaiThanhViens.OrderBy(n => n.TenLoaiTV), "MaLoaiTV", "TenLoaiTV", member.MaLoaiTV);
            if (t == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                t.TaiKhoan = member.TaiKhoan;
                t.MatKhau = member.MatKhau;
                t.HoTen = member.HoTen;
                t.DiaChi = member.DiaChi;
                t.Email = member.Email;
                t.SoDienThoai = member.SoDienThoai;
                t.MaLoaiTV = member.MaLoaiTV;
                db.SubmitChanges();
                TempData["EditedMember"] = "Thông tin đã được thay đổi";
                return RedirectToAction("Admin", "Admin");
            }
            return View(member);
        }
        // Xoá thành viên
        [HttpPost]
        public ActionResult DeleteMember(int id)
        {
            ThanhVien member = db.ThanhViens.SingleOrDefault(n => n.MaTV.Equals(id));
            if (member == null)
            {
                return RedirectToAction("NotFound", "Home");
            }
            db.ThanhViens.DeleteOnSubmit(member);
            db.SubmitChanges();
            TempData["DeleteMemberSuccess"] = "Xóa thành viên thành công";
            return Json(new { success = true }); // Trả về JSON để xử lý trong JavaScript
        }


        [ValidateInput(false)]
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