using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class TrashController : AdminController
    {
        dbMarkLeoDataContext context = new dbMarkLeoDataContext();
        // GET: Trash
        public ActionResult Index()
        {
            GetData();
            ViewBag.ListProductDeleted = context.SanPhams.Where(n => n.DaXoa == true);
            ViewBag.ListTechNewDeleted = context.TinTucs.Where(n => n.DaXoa == true);
            return View();
        }

        [HttpPost]
        public ActionResult UndoProduct(int? id) {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại mã sản phẩm!" });
            }
            var product = context.SanPhams.SingleOrDefault(n => n.MaSP.Equals(id));
            product.DaXoa = false;
            context.SubmitChanges();
            return Json(new { statusCode = 200, message = "Khôi phục sản phẩm thành công" });
        }


        [HttpPost]
        public ActionResult UndoTechNews(int? id)
        {
            if (id == null)
            {
                return Json(new { statusCode = 404, message = "Không tồn tại mã tin tức!" });
            }
            var technew = context.TinTucs.SingleOrDefault(n => n.MaTin.Equals(id));
            technew.DaXoa = false;
            context.SubmitChanges();
            return Json(new { statusCode = 200, message = "Khôi phục bản tin thành công" });
        }
    }
}