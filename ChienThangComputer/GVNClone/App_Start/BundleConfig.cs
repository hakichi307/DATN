using System.Web;
using System.Web.Optimization;

namespace GVNClone {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            bundles.Add(new ScriptBundle("~/bundles/jsClient").Include(
                        "~/Content/js/jQuery-3.5.1.min.js",
                        "~/Content/js/backtotop.js",
                        "~/Content/js/jquery-ui.js",
                        "~/Content/js/jquery.unobtrusive-ajax.min.js",
                        "~/Content/js/jquery.validate.min.js",
                        "~/Content/js/jquery.validate.unobtrusive.min.js",
                        "~/Content/js/sweetalert2@10.js",
                        "~/Content/js/slick.min.js",
                        "~/Content/js/districts.min.js",
                        "~/Content/js/fotorama.js",
                        "~/Content/js/flickity.pkgd.min.js",
                        "~/Content/js/jquery.firstVisitPopup.js",
                        "~/Content/js/index.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jsAdmin").Include(
                        "~/Content/AdminLayout/assets/vendors/popper.js/dist/umd/popper.min.js",
                        "~/Content/AdminLayout/assets/vendors/bootstrap/dist/js/bootstrap.min.js",
                        "~/Content/AdminLayout/assets/vendors/jquery-slimscroll/jquery.slimscroll.min.js",
                        "~/Content/AdminLayout/assets/vendors/DataTables/datatables.min.js",
                        "~/Content/AdminLayout/assets/vendors/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js",
                        "~/Content/js/sweetalert2@10.js",
                        "~/Content/AdminLayout/assets/vendors/jquery-validation/dist/jquery.validate.min.js",
                        "~/Content/AdminLayout/assets/js/scripts/dashboard_1_demo.js",
                        "~/Content/AdminLayout/assets/vendors/metisMenu/dist/metisMenu.min.js",
                        "~/Content/AdminLayout/assets/vendors/lightbox2-dev/dist/js/lightbox.min.js",
                        "~/Content/AdminLayout/assets/vendors/summernote/summernote.min.js",
                        "~/Content/AdminLayout/assets/js/scripts/form-plugins.js",
                        "~/Content/AdminLayout/assets/js/libs/ckfinder/ckfinder.js",
                        "~/Content/AdminLayout/assets/js/libs/ckeditor/ckeditor.js",
                        "~/Content/AdminLayout/assets/js/app.min.js"
                        ));


            bundles.Add(new StyleBundle("~/bundles/cssClient").Include(
                      "~/Content/css/jQuery-ui.css",
                      "~/Content/css/flickity.min.css",
                      "~/fonts/FontAwesome-5.15.3/css/all.min.css",
                      "~/Content/css/fotorama.css",
                      "~/Content/css/slick.css",
                      "~/Content/css/slick-theme.css",
                      "~/Content/css/main.css",
                      "~/Content/css/grid.css",
                      "~/Content/css/responsive.css"
                      ));

            bundles.Add(new StyleBundle("~/bundles/cssAdmin").Include(
                      "~/Content/AdminLayout/assets/vendors/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/AdminLayout/assets/vendors/font-awesome/css/font-awesome.min.css",
                      "~/Content/AdminLayout/assets/vendors/themify-icons/css/themify-icons.css",
                      "~/Content/AdminLayout/assets/vendors/DataTables/datatables.min.css",
                      "~/Content/AdminLayout/assets/vendors/summernote/summernote.min.css",
                      "~/Content/AdminLayout/assets/vendors/bootstrap-datepicker/dist/css/bootstrap-datepicker3.min.css",
                      "~/Content/AdminLayout/assets/vendors/lightbox2-dev/dist/css/lightbox.min.css",
                      "~/Content/AdminLayout/assets/css/main.css",
                      "~/Content/AdminLayout/assets/css/pages/mailbox.css"
                      ));
        }
    }
}
