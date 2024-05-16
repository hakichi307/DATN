using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class DebtController : AdminController
    {
        dbChienThangDataContext context = new dbChienThangDataContext();
        // GET: Debt
        public ActionResult Index()
        {
            GetData();
            return View(context.PhieuNhaps.Where(n => n.SoTienNo != 0).ToList());
        }

        [HttpPost]
        public ActionResult ViewDetail(int? id) {
            if (id == null) {
                return Json(new { statusCode = 404, message = "Mã phiếu nhập không tồn tại" });
            }
            var irv = context.PhieuNhaps.SingleOrDefault(n => n.MaPN.Equals(id));
            ViewBag.Supplier = context.NhaCungCaps.SingleOrDefault(n => n.MaNCC.Equals(irv.MaNCC));
            ViewBag.DetailIRV = irv;
            return PartialView(context.ChiTietPhieuNhaps.Where(n => n.MaPN.Equals(id)));
        }

        public ActionResult PaymentPartial() {
            return PartialView();
        }

        [HttpPost]
        public ActionResult PayPost(PhieuNhap irv)
        {
            if (String.IsNullOrEmpty(irv.MaPN.ToString()))
            {
                return RedirectToAction("NotFound", "Auth");
            }
            else if (irv.SoTienDaTra == null)
            {
                var obj = context.PhieuNhaps.SingleOrDefault(n => n.MaPN.Equals(irv.MaPN));
                obj.SoTienNo = 0;
                obj.SoTienDaTra = obj.TongTienThanhToan;
            }
            else if(irv.SoTienDaTra != null) {
                var obj = context.PhieuNhaps.SingleOrDefault(n => n.MaPN.Equals(irv.MaPN));
                if (irv.SoTienDaTra == obj.SoTienNo)
                {
                    obj.SoTienNo = 0;
                    obj.SoTienDaTra = obj.TongTienThanhToan;
                }
                else {
                    obj.SoTienNo -= irv.SoTienDaTra;
                    obj.SoTienDaTra += irv.SoTienDaTra;
                }
            }
            context.SubmitChanges();
            return RedirectToAction("Index");
        }
    }
}