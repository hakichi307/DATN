using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class HardCodeController : Controller
    {
        // GET: HardCode
        public ActionResult PromotionView()
        {
            // Xem khuyến mãi
            return View();
        }

        public ActionResult InstalmentPlan() {
            // Gói trả góp
            return View();
        }

        public ActionResult SwitchboardView() {

            return View();
        }

        public ActionResult Warranty() {
            // Bảo hành
            return View();
        }
        public ActionResult Transport()
        {
            // Vận chuyển
            return View();
        }
    }
}