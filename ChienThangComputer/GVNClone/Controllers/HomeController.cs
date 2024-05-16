using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GVNClone.Models;
using System.Web.Security;
using PagedList;

namespace GVNClone.Controllers
{
    public class HomeController : Controller
    {
        dbChienThangDataContext db = new dbChienThangDataContext();
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult PartialMegaMenu() {
            return PartialView(db.SanPhams.Where(n => n.DaXoa == false));
        }


        public ActionResult PartialMegaMenuMobile()
        {
            return PartialView(db.SanPhams.Where(n => n.DaXoa == false));
        }

        [ChildActionOnly]
        public ActionResult Banner() {
            return PartialView(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.DaXoa == false));
        }

        public ActionResult PC()
        {
            return PartialView(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.DaXoa == false));
        }
        public ActionResult Laptop(int? page) {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.SoLanMua > 10 && n.LoaiDanhMuc.TenLoaiDM.Equals("Asus") && n.DanhMuc.TenDM.Equals("Laptop Gaming") && n.DaXoa == false).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Monitor(int? page) {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.LoaiSanPham.TenLoaiSP.Equals("Màn hình") && n.DaXoa == false).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult OfficeLaptop(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.DanhMuc.TenDM.Equals("Laptop Văn Phòng") && n.DaXoa == false).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Keyboard(int? page) {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(db.SanPhams.OrderByDescending(n => n.MaSP).Where(n => n.LoaiSanPham.TenLoaiSP.Equals("Bàn phím") && n.DaXoa == false).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Mouse(int? page) {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView(db.SanPhams.OrderBy(n => n.MaSP).Where(n => n.LoaiDanhMuc.TenLoaiDM.Equals("Dare U") && n.LoaiSanPham.TenLoaiSP.Contains("Chuột") && n.DaXoa == false).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult View_All_PC_BestSelling(int? page) {
            int pageSize = 18;
            int pageNumber = (page ?? 1);
            return View(db.SanPhams.Where(n => n.LoaiSanPham.TenLoaiSP.Substring(0, 2).Contains("PC") && n.SoLanMua > 10 && n.DaXoa == false).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult ListVoucher() 
        {
            return View(db.SuKienKhuyenMais.Where(n => n.NgayBatDau < DateTime.Now && DateTime.Now < n.NgayKetThuc && n.SoLuongConLai > 0));
        }

        public ActionResult ListProductVoucher(string id) {
            SuKienKhuyenMai cps = db.SuKienKhuyenMais.SingleOrDefault(n => n.MaKM.Equals(id));
            if (cps.NgayBatDau > DateTime.Now && cps.NgayKetThuc < DateTime.Now) {
                return RedirectToAction("Home");
            }
            ViewBag.Voucher = db.SuKienKhuyenMais.SingleOrDefault(n => n.MaKM.Equals(id)); // save to load single voucher for products applied
            var lst_product_max_price = db.SanPhams.AsQueryable();
            var lst_product_min_price = db.SanPhams.AsQueryable();
            if (hasVoucher(id))
            {
                ViewBag.DataIDVoucher = id; // save to call ajax filter price for list product apply the voucher
                lst_product_max_price = db.SanPhams.Where(n => n.MaKM.Equals(id)).OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Take(1);
                lst_product_min_price = db.SanPhams.Where(n => n.MaKM.Equals(id)).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Take(1);
                foreach (var item in lst_product_max_price)
                {
                    GlobalVariables.max_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
                }
                foreach (var item in lst_product_min_price)
                {
                    GlobalVariables.min_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
                }

                return View(db.SanPhams.Where(n => n.DaXoa == false && n.MaKM.Equals(id)).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
            }
            else
            {
                lst_product_max_price = db.SanPhams.OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Take(1);
                lst_product_min_price = db.SanPhams.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Take(1);
                foreach (var item in lst_product_max_price)
                {
                    GlobalVariables.max_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
                }
                foreach (var item in lst_product_min_price)
                {
                    GlobalVariables.min_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
                }
                return View(db.SanPhams.Where(n => n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
            }
        }


        public bool hasVoucher(string id) {
            var check = db.SanPhams.Where(n => n.MaKM.Equals(id));
            if (check.Count() > 0) {
                return true;
            }
            return false;
        }

        public JsonResult GetDataTimeRemainingVoucher() {
            var data = getLstVoucher();
            return Json(new { data, status = 200 }, JsonRequestBehavior.AllowGet);
        }

        public List<Voucher> getLstVoucher() {
            List<Voucher> lstVoucher = new List<Voucher>();
            var arrayVoucher = db.SuKienKhuyenMais.Where(n => n.NgayKetThuc > DateTime.Now);
            foreach (var item in arrayVoucher) {
                var voucher = new Voucher();
                voucher.Id = item.MaKM;
                voucher.totalQuantityCanUse = (int)item.SoLanSuDung;
                voucher.remainQuantityCanUse = (int)item.SoLuongConLai;
                voucher.endYear = item.NgayKetThuc.Value.Year;
                voucher.endMonth = item.NgayKetThuc.Value.Month;
                voucher.endDay = item.NgayKetThuc.Value.Day;
                voucher.endHour = item.NgayKetThuc.Value.Hour;
                voucher.endMin = item.NgayKetThuc.Value.Minute;
                lstVoucher.Add(voucher);
            }
            return lstVoucher;
        }


        [HttpPost]
        public ActionResult PartialFilterPriceVoucher(decimal min_price, decimal max_price, string idVoucher)
        {
            GlobalVariables.min_price_range_slider = min_price;
            GlobalVariables.max_price_range_slider = max_price;
            var lstProduct = db.SanPhams.Where(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= min_price && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= max_price && n.DaXoa == false && n.MaKM.Equals(idVoucher));
            if (lstProduct.Count() > 0)
            {
                return PartialView(db.SanPhams.Where(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= min_price && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= max_price && n.DaXoa == false && n.MaKM.Equals(idVoucher)).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
            }
            return PartialView(db.SanPhams.Where(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= min_price && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= max_price && n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
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