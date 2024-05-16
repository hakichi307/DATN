using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class CRUDTechNewsController : AdminController
    {
        dbChienThangDataContext context = new dbChienThangDataContext();
        // GET: CRUDTechNews
        public ActionResult ViewAll()
        {
            GetData();
            return View(context.TinTucs.Where(n => n.DaXoa == false));
        }

        [HttpGet]
        public ActionResult DetailTechNew(int? id)
        {
            GetData();
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại id tin tức" });
            }
            var objTechNew = context.TinTucs.SingleOrDefault(n => n.MaTin.Equals(id) && n.DaXoa == false);
            ViewBag.MaLoaiTin = new SelectList(context.LoaiTinTucs.OrderBy(n => n.MaLoaiTin), "MaLoaiTin", "TenLoaiTin");
            ViewBag.MaDanhMucTin = new SelectList(context.DanhMucTins.OrderBy(n => n.MaDanhMucTin), "MaDanhMucTin", "TenDM");
            return View(objTechNew);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditTechNews(TinTuc technew, HttpPostedFileBase fUpload) {
            TinTuc objTechNew = context.TinTucs.SingleOrDefault(tn => tn.MaTin.Equals(technew.MaTin));
            if (objTechNew == null)
            {
                return HttpNotFound("There is something wrong. please try again");
            }
            objTechNew.TieuDeTin = technew.TieuDeTin;
            objTechNew.DoanTrich = technew.DoanTrich;
            objTechNew.NoiDung = technew.NoiDung;
            if (fUpload != null)
            {
                if (fUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fUpload.FileName);
                    objTechNew.HinhAnh = "/Images/" + fileName;
                }
            }
            objTechNew.MaLoaiTin = technew.MaLoaiTin;
            objTechNew.MaDanhMucTin = technew.MaDanhMucTin;
            objTechNew.DaXoa = technew.DaXoa;
            context.SubmitChanges();
            TempData["ConfigTechNewsSuccess"] = "Chỉnh sửa bảng tin thành công";
            return RedirectToAction("ViewAll");
        }

        [HttpGet]
        public ActionResult AddTechNew() {
            GetData();
            ViewBag.MaLoaiTin = new SelectList(context.LoaiTinTucs.OrderBy(n => n.MaLoaiTin), "MaLoaiTin", "TenLoaiTin");
            ViewBag.MaDanhMucTin = new SelectList(context.DanhMucTins.OrderBy(n => n.MaDanhMucTin), "MaDanhMucTin", "TenDM");
            return View();
        }


        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AddTechNew(TinTuc technews, HttpPostedFileBase fUpload) {
            GetData();
            ViewBag.MaLoaiTin = new SelectList(context.LoaiTinTucs.OrderBy(n => n.MaLoaiTin), "MaLoaiTin", "TenLoaiTin");
            ViewBag.MaDanhMucTin = new SelectList(context.DanhMucTins.OrderBy(n => n.MaDanhMucTin), "MaDanhMucTin", "TenDM");
            if (fUpload != null)
            {
                if (fUpload.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fUpload.FileName);
                    technews.HinhAnh = "/Images/" + fileName;
                }
            }
            technews.LuotXem = 0;
            technews.DaXoa = false;
            technews.NgayDang = DateTime.Today;
            context.TinTucs.InsertOnSubmit(technews);
            context.SubmitChanges();
            TempData["Added"] = "Đã thêm vào bảng tin thành công";
            ModelState.Clear();
            return RedirectToAction("ViewAll");
        }

        [HttpPost]
        public ActionResult DeteleTechNew(int? id) {
            if (id == null) {
                return Json(new { statusCode = 404, message = "Không tồn tại id bảng tin" });
            }
            var objTechNew = context.TinTucs.SingleOrDefault(n => n.MaTin.Equals(id));
            objTechNew.DaXoa = true;
            context.SubmitChanges();
            return Json(new { statusCode = 200, message = "Bảng tin đã được xoá khỏi hệ thống" });
        }
    }
}