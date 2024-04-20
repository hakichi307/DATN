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
            return View();
        }

        public ActionResult InstalmentPlan() {
            return View();
        }

        public ActionResult SwitchboardView() {
            return View();
        }

        public ActionResult Warranty() {
            return View();
        }
    }
}