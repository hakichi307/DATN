using GVNClone.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace GVNClone
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["HomNay"] = 0;
            Application["HomQua"] = 0;
            Application["TuanNay"] = 0;
            Application["TuanTruoc"] = 0;
            Application["ThangNay"] = 0;
            Application["ThangTruoc"] = 0;
            Application["TatCa"] = 0;
            Application["visitors_online"] = 0;
        }

        protected void Session_Start()
        {
            Application.Lock();
            Application["visitors_online"] = Convert.ToInt32(Application["visitors_online"]) + 1;
            Application.UnLock();
            try
            {
                DataBindSQL mThongKe = new DataBindSQL();
                DataTable dtb = mThongKe.TableThongKe();
                if (dtb.Rows.Count > 0)
                {
                    Application["HomNay"] = long.Parse("0" + dtb.Rows[0]["HomNay"]).ToString("#,###");
                    Application["HomQua"] = long.Parse("0" + dtb.Rows[0]["HomQua"]).ToString("#,###");
                    Application["TuanNay"] = long.Parse("0" + dtb.Rows[0]["TuanNay"]).ToString("#,###");
                    Application["TuanTruoc"] = long.Parse("0" + dtb.Rows[0]["TuanTruoc"]).ToString("#,###");
                    Application["ThangNay"] = long.Parse("0" + dtb.Rows[0]["ThangNay"]).ToString("#,###");
                    Application["ThangTruoc"] = long.Parse("0" + dtb.Rows[0]["ThangTruoc"]).ToString("#,###");
                    Application["TatCa"] = long.Parse("0" + dtb.Rows[0]["TatCa"]).ToString("#,###");
                }
                dtb.Dispose();
                mThongKe = null;
            }
            catch { }
        }

        protected void Session_End()
        {
            Application.Lock();
            Application["visitors_online"] = Convert.ToInt32(Application["visitors_online"]) - 1;
            Application.UnLock();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
            var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null) {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value); // Giải mã giá trị khi nãy mã hoá
                var roles = authTicket.UserData.Split(new Char[] { ',' });
                var userPrintcipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);
                Context.User = userPrintcipal;
            }
        }
    }
}
