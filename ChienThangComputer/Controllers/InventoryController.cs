using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChienThangComputer.Models;
namespace ChienThangComputer.Controllers
{
    public class InventoryController : AdminController
    {
        dbChienThangDataContext context = new dbChienThangDataContext();
        // GET: Inventory
        public ActionResult Index()
        {
            GetData();
            return View(context.PhieuNhaps.Where(n => n.SoTienNo == 0 && n.DaXoa == false).ToList());
        }


        [HttpPost]
        public ActionResult ViewDetail(int? id)
        {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Mã phiếu nhập không tồn tại" });
            }
            var irv = context.PhieuNhaps.SingleOrDefault(n => n.MaPN.Equals(id));
            ViewBag.Supplier = context.NhaCungCaps.SingleOrDefault(n => n.MaNCC.Equals(irv.MaNCC));
            ViewBag.DetailIRV = irv;
            return PartialView(context.ChiTietPhieuNhaps.Where(n => n.MaPN.Equals(id)));
        }


        public ActionResult Delete_IRV(int? id)
        {
            var check = context.PhieuNhaps.SingleOrDefault(c => c.MaPN.Equals(id));
            if (check == null)
            {
                return Json(new { statusCode = 404, message = "Mã không tồn tại. Vui lòng kiểm tra lại!" });
            }
            check.DaXoa = true;
            context.SubmitChanges();
            return Json(new { statusCode = 200, message = "Xoá thành công" });
        }
    }
}