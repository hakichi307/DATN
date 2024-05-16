using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class SaleOffController : AdminController
    {
        dbChienThangDataContext db = new dbChienThangDataContext();
        public ActionResult Index()
        {
            GetData();
            ViewBag.MaSP = db.SanPhams.Where(n => n.DaXoa == false);
            return View(db.GiamGias.ToList());
        }
        //Danh sách sự kiện khuyến mãi
        public ActionResult EventSale()
        {
            GetData();
            var eventSales = db.SuKienKhuyenMais.ToList(); // Retrieve all promotional events

            if (eventSales == null || !eventSales.Any())
            {
                ViewBag.ErrorMessage = "No promotion events available.";
            }

            return View(eventSales); // Pass the list of promotional events to the view
        }

        [HttpPost]
        public ActionResult SaleOffPricePartial(int? id)
        {
            try
            {
                if (id == null) {
                    return Json(new { statusCode = 404, message = "Không tồn tại id!" });
                }
                ViewBag.LstVoucherSalePrice = db.GiamGias.Where(n => n.NgayBatDau < DateTime.Now && DateTime.Now < n.NgayKetThuc).ToList();
                return PartialView(db.SanPhams.Single(n => n.MaSP.Equals(id) && n.DaXoa == false));
            }
            catch (Exception ex) {
                return Json(new { statusCode = 404, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult HandleSalePrice(int? id_product, int? id_sale_price)
        {
            try
            {
                if (id_product == null || id_sale_price == null) 
                {
                    return Json(new { statusCode = 404, message = "Không tồn tại id" });
                }
                var objProduct = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(id_product));
                var objSalePrice = db.GiamGias.SingleOrDefault(n => n.MaGiamGia.Equals(id_sale_price));
                if (objProduct.GiaNiemYet <= objSalePrice.GiaKhuyenMai) 
                {
                    return Json(new { statusCode = 404, message = "Giá khuyến mãi không được lớn hơn hoặc bằng giá gốc!" });
                }
                objProduct.MaGiamGia = id_sale_price;
                db.SubmitChanges();
                return Json(new { statusCode = 200, message = "Cập nhật thành công" });
            }
            catch (Exception ex) 
            {
                return Json(new { statusCode = 404, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteSalePrice(int? id) {
            try
            {
                if (id == null) 
                {
                    return Json(new { statusCode = 404, message = "Không tồn tại id" });
                }
                var objProduct = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(id));
                objProduct.MaGiamGia = null;
                db.SubmitChanges();
                return Json(new { statusCode = 200, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = 200, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ViewLstProductApplied(int? id)
        {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id" });
            }
            return PartialView(db.SanPhams.Where(n => n.DaXoa == false && n.MaGiamGia.Equals(id)).ToList());
        }

        [HttpGet]
        public ActionResult EditVoucherSalePrice(int? id) {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id" });
            }
            return PartialView(db.GiamGias.SingleOrDefault(n => n.MaGiamGia.Equals(id)));
        }

        [HttpPost]

        public JsonResult UpdateSalePrice(int? id, decimal sale_price, string timeStart, string timeEnd) {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id" });
            }
            var obj = db.GiamGias.SingleOrDefault(n => n.MaGiamGia.Equals(id));
            obj.GiaKhuyenMai = sale_price;
            obj.NgayBatDau = DateTime.Parse(timeStart);
            obj.NgayKetThuc = DateTime.Parse(timeEnd);
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Cập nhật thành công" });
        }

        [HttpPost]
        public ActionResult RemoveApply(int? id)
        {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id" } );
            }
            var objProduct = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(id));
            objProduct.MaGiamGia = null;
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Thành công" });
        }


        [HttpPost]
        public ActionResult DeletePriceSale(int? id)
        {
            if (id == null)
            {
                return Json(new { statusCode = 404 });
            }
            var objProduct = db.SanPhams.Where(n => n.MaGiamGia.Equals(id));
            foreach (var item in objProduct)
            {
                item.MaGiamGia = null;
            }
            db.SubmitChanges();
            var obj = db.GiamGias.SingleOrDefault(n => n.MaGiamGia.Equals(id));
            db.GiamGias.DeleteOnSubmit(obj);
            db.SubmitChanges();
            return Json(new { statusCode = 200, message = "Xoá thành công" });
        }


        public ActionResult CreateVoucher()
        {
            GetData();
            ViewBag.LstProduct = db.SanPhams.Where(n => n.DaXoa == false);
            return View();
        }


        [HttpPost]
        public ActionResult CreateVoucher(SuKienKhuyenMai cps, IEnumerable<SanPham> lstModel)
        {
            GetData();
            ViewBag.LstProduct = db.SanPhams.Where(n => n.DaXoa == false);
            SuKienKhuyenMai checkId = db.SuKienKhuyenMais.SingleOrDefault(n => n.MaKM.Equals(cps.MaKM));
            if (checkId != null)
            {
                TempData["MessageCpsError"] = "Mã đã tồn tại";
                return View();



            }
            cps.SoLuongConLai = cps.SoLanSuDung;
            if (lstModel != null)
            {
                foreach (var item in lstModel)
                {
                    SanPham product = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(item.MaSP));
                    product.MaKM = cps.MaKM;
                }
            }
            db.SuKienKhuyenMais.InsertOnSubmit(cps);
            db.SubmitChanges();
            TempData["MessageCpsSuccess"] = "Tạo thành công";
            return View();
        }


    }
}