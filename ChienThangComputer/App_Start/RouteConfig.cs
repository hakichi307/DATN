using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ChienThangComputer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "FormSurvey",
                url: "tu-van-mua-hang",
                defaults: new { controller = "KNN", action = "FormSurvey", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Home",
                url: "pages/tin-cong-nghe",
                defaults: new { controller = "TechNews", action = "Home", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ListCategory",
                url: "pages/tin-cong-nghe/the-loai",
                defaults: new { controller = "TechNews", action = "ListCategory", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "RunAlgorithm",
                url: "ket-qua-tu-van",
                defaults: new { controller = "KNN", action = "RunAlgorithm", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ListVoucher",
                url: "chuong-trinh-khuyen-mai",
                defaults: new { controller = "Home", action = "ListVoucher", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ListProductVoucher",
                url: "san-pham-ap-dung/{id}",
                defaults: new { controller = "Home", action = "ListProductVoucher", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "CartEmpty",
                url: "cart-empty",
                defaults: new { controller = "Cart", action = "CartEmpty", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "DisplayAllProduct",
                url: "tat-ca-san-pham",
                defaults: new { controller = "Product", action = "DisplayAllProduct", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SignUp",
                url: "dang-ky",
                defaults: new { controller = "Auth", action = "SignUp", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SignIn",
                url: "dang-nhap",
                defaults: new { controller = "Auth", action = "SignIn", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Information",
                url: "thong-tin-ca-nhan/{id}",
                defaults: new { controller = "Auth", action = "Information", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "View_All_PC_BestSelling",
                url: "tat-ca-pc-ban-chay-nhat",
                defaults: new { controller = "Home", action = "View_All_PC_BestSelling", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Checkout",
                url: "thanh-toan",
                defaults: new { controller = "Cart", action = "Checkout", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "GoToCart",
                url: "cart",
                defaults: new { controller = "Cart", action = "GoToCart", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "Product", action = "Search", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SearchAction",
                url: "pages/tin-cong-nghe/tim-kiem",
                defaults: new { controller = "TechNews", action = "SearchAction", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "NotFound",
               url: "error",
               defaults: new { controller = "Home", action = "NotFound", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Index",
               url: "show-room",
               defaults: new { controller = "ShowRoom", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "PromotionView",
               url: "pages/tong-hop-khuyen-mai",
               defaults: new { controller = "HardCode", action = "PromotionView", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "InstalmentPlan",
               url: "pages/huong-dan-tra-gop",
               defaults: new { controller = "HardCode", action = "InstalmentPlan", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "SwitchboardView",
               url: "pages/tong-dai",
               defaults: new { controller = "HardCode", action = "SwitchboardView", id = UrlParameter.Optional }
            ); 
            routes.MapRoute(
               name: "Warranty",
               url: "pages/chinh-sach-bao-hanh",
               defaults: new { controller = "HardCode", action = "Warranty", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "AuthenticateOTP",
               url: "auth/xac-minh-otp",
               defaults: new { controller = "Auth", action = "AuthenticateOTP", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "VerifyEmail",
               url: "auth/xac-minh-email",
               defaults: new { controller = "Auth", action = "VerifyEmail", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ProductDetail",
                url: "xem-chi-tiet/{id}/{name}",
                defaults: new { controller = "Product", action = "ProductDetail", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ViewDetail",
                url: "pages/tin-cong-nghe/xem-chi-tiet/{id}",
                defaults: new { controller = "TechNews", action = "ViewDetail", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Home", id = UrlParameter.Optional }
            );
        }
    }
}
