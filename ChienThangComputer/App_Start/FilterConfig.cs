using System.Web;
using System.Web.Mvc;

namespace ChienThangComputer
{
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
