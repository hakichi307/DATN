
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using PagedList;
using GVNClone.Models;

namespace GVNClone.Controllers
{
    public class ProductController : Controller
    {

        dbMarkLeoDataContext db = new dbMarkLeoDataContext();

        public ActionResult DisplayAllProduct(int? page)
        {
            var allProduct = db.SanPhams.Where(n => n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet));
            var min_price = db.SanPhams.Where(n => n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet));
            var max_price = db.SanPhams.Where(n => n.DaXoa == false).OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet));

            // Reset GlobalVariables 
            GlobalVariables.ID_TypeProduct = 0;
            GlobalVariables.ID_TypeCategory = 0;
            GlobalVariables.ID_Category = 0;

            foreach (var item in min_price.Take(1))
            {
                GlobalVariables.min_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
            }
            foreach (var item in max_price.Take(1))
            {
                GlobalVariables.max_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
            }
            int pageSize = 40; // Số sản phẩm hiển thị trên trang
            int pageNumber = (page ?? 1); // Số trang hiện tại, nếu page k có giá trị sẽ mặc định là 1
            return View(allProduct.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ProductDetail(int id, string name)
        {
            SanPham product = db.SanPhams.SingleOrDefault(n => n.MaSP.Equals(id) && n.DaXoa == false);
            if (product == null)
            {
                return RedirectToAction("NotFound", "Auth");
            }
            ViewBag.QuaTang = db.QuaTangs.Where(qt => qt.MaSP.Equals(id)).ToList();
            ViewBag.Rating = db.DanhGias.Where(n => n.MaSP.Equals(id)).OrderByDescending(n => n.MaBL);
            NumberStarRatingAverage(id);
            ViewBag.isLogin =  HandlerDisplayChatStarRating(id);
            Session["CountRating"] = db.DanhGias.Where(n => n.MaSP.Equals(id)).Count();
            return View(product);
        }

        public ActionResult Revelant(int? category, int? id) {
            return PartialView(db.SanPhams.Where(n => n.MaLoaiSP.Equals(category) && !n.MaSP.Equals(id) && n.DaXoa == false).OrderBy(n => n.TenSP));
        }

        public ActionResult ProductFilter(int? maLoaiDM, int? IDDM, int? id)
        {
            // Initialize ID_TypeProduct
            GlobalVariables.ID_TypeProduct = 0;

            var min_price = db.SanPhams.AsQueryable();
            var max_price = db.SanPhams.AsQueryable();
            if (maLoaiDM == null && IDDM == null)
            {
                ViewBag.Temp = "";
            }
            else
            {
                GlobalVariables.ID_TypeCategory = (int)maLoaiDM;
                GlobalVariables.ID_Category = (int)IDDM;
            }
            var filterSP = db.SanPhams.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false);
            if (id != null)
            {
                GlobalVariables.ID_TypeProduct = (int)id;
                min_price = db.SanPhams.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(n => n.MaLoaiSP.Equals(GlobalVariables.ID_TypeProduct) && n.DaXoa == false);
                max_price = db.SanPhams.OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(n => n.MaLoaiSP.Equals(GlobalVariables.ID_TypeProduct) && n.DaXoa == false);
                foreach (var item in min_price.Take(1)) 
                {
                    GlobalVariables.min_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
                }
                foreach (var item in max_price.Take(1))
                {
                    GlobalVariables.max_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
                }
                return View(db.SanPhams.Where(n => n.MaLoaiSP.Equals(GlobalVariables.ID_TypeProduct) && n.DaXoa == false));
            }
            if (filterSP.Count() == 1)
            {
                foreach (var item in filterSP)
                {
                    return RedirectToRoute("ProductDetail", new { @id = item.MaSP, @name = item.TenSP.Replace(" ", "-") });
                }
            }
            foreach (var item in filterSP.Take(1))
            {
                GlobalVariables.min_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
            }
            foreach (var item in filterSP.OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Take(1))
            {
                GlobalVariables.max_price_range_slider = (item.MaGiamGia != null && item.GiamGia.NgayBatDau < DateTime.Now && item.GiamGia.NgayKetThuc > DateTime.Now ? (decimal)item.GiamGia.GiaKhuyenMai : (decimal)item.GiaNiemYet);
            }
            return View(filterSP);
        }

        [HttpPost]
        public ActionResult PartialFilterPrice(decimal min_price, decimal max_price) {
            GlobalVariables.min_price_range_slider = min_price;
            GlobalVariables.max_price_range_slider = max_price;
            if (GlobalVariables.ID_Category != 0 && GlobalVariables.ID_TypeCategory != 0 && GlobalVariables.ID_TypeProduct == 0) {
                return PartialView(db.SanPhams.Where(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= GlobalVariables.min_price_range_slider && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= GlobalVariables.max_price_range_slider && n.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && n.IDDM.Equals(GlobalVariables.ID_Category) && n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
            }
            if (GlobalVariables.ID_TypeProduct != 0) {
                return PartialView(db.SanPhams.Where(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= min_price && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= max_price && n.MaLoaiSP.Equals(GlobalVariables.ID_TypeProduct) && n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
            }
            return PartialView(db.SanPhams.Where(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= min_price && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= max_price && n.DaXoa == false).OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)));
        }

        public ActionResult Sort(string sort)
        {
            var products = db.SanPhams.AsQueryable();
            if (GlobalVariables.min_price_range_slider != 0 && GlobalVariables.max_price_range_slider != 0 && GlobalVariables.ID_Category != 0 && GlobalVariables.ID_TypeCategory != 0)
            {
                switch (sort)
                {
                    case "price-ascending":
                        {
                            products = products.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) >= GlobalVariables.min_price_range_slider && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "price-descending":
                        {
                            products = products.OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) >= GlobalVariables.min_price_range_slider && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "title-ascending":
                        {
                            products = products.OrderBy(n => n.TenSP).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) >= GlobalVariables.min_price_range_slider && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "title-descending":
                        {
                            products = products.OrderByDescending(n => n.TenSP).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) >= GlobalVariables.min_price_range_slider && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "best-selling":
                        {
                            products = products.Where(n => n.SoLanMua > 10).OrderBy(n => n.MaSP).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) >= GlobalVariables.min_price_range_slider && (sp.MaGiamGia == null ? sp.GiaNiemYet : sp.GiamGia.GiaKhuyenMai) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                }
            }
            else
            {
                switch (sort)
                {
                    case "price-ascending":
                        {
                            products = products.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false);
                            break;
                        };
                    case "price-descending":
                        {
                            products = products.OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false);
                            break;
                        };
                    case "title-ascending":
                        {
                            products = products.OrderBy(n => n.TenSP).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false);
                            break;
                        };
                    case "title-descending":
                        {
                            products = products.OrderByDescending(n => n.TenSP).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false);
                            break;
                        };
                    case "best-selling":
                        {
                            products = products.Where(n => n.SoLanMua > 10).OrderBy(n => n.MaSP).Where(sp => sp.MaLoaiDM.Equals(GlobalVariables.ID_TypeCategory) && sp.IDDM.Equals(GlobalVariables.ID_Category) && sp.DaXoa == false);
                            break;
                        };
                }
            }
            return PartialView(products);
        }
        
        [HttpPost]
        public JsonResult SortAllProduct(string sort)
        {
            var products = db.SanPhams.AsQueryable();
            if (GlobalVariables.min_price_range_slider != 0 && GlobalVariables.max_price_range_slider != 0)
            {
                switch (sort)
                {
                    case "price-ascending":
                        {
                            products = products.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(n => n.DaXoa == false && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= GlobalVariables.min_price_range_slider && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "price-descending":
                        {
                            products = products.OrderByDescending(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).Where(n => n.DaXoa == false && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= GlobalVariables.min_price_range_slider && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "title-ascending":
                        {
                            products = products.OrderBy(n => n.TenSP).Where(n => n.DaXoa == false && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= GlobalVariables.min_price_range_slider && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "title-descending":
                        {
                            products = products.OrderByDescending(n => n.TenSP).Where(n => n.DaXoa == false && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= GlobalVariables.min_price_range_slider && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= GlobalVariables.max_price_range_slider);
                            break;
                        };
                    case "best-selling":
                        {
                            products = products.Where(n => n.SoLanMua > 10 && n.DaXoa == false && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) >= GlobalVariables.min_price_range_slider && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= GlobalVariables.max_price_range_slider).OrderBy(n => n.MaSP);
                            break;
                        };
                }
            }
            Session["ListProductSorted"] = products;
            Session["TypeSort"] = sort;
            return Json(new { statusCode = 200, redirectUrl = Url.Action("ViewSortAllProduct", "Product") });
        }

        public ActionResult ViewSortAllProduct(int? page) {
            int pageSize = 40; // Số sản phẩm hiển thị trên trang
            int pageNumber = (page ?? 1); // Số trang hiện tại, nếu page k có giá trị sẽ mặc định là 1
            IEnumerable<SanPham> lst = Session["ListProductSorted"] as IEnumerable<SanPham>;
            return View(lst.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Search(string keyword, int? page)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.keyword = keyword;
                var result = from sp in db.SanPhams where (sp.TenSP.Contains(keyword) || sp.DanhMuc.TenDM.Contains(keyword)) && sp.DaXoa == false select sp;
                if (result.Count() == 0)
                {
                    ViewBag.Result = result.Count();
                    return View();
                }
                int pageSize = 28;
                int pageNumber = (page ?? 1);
                return View(result.OrderBy(n => (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet)).ToPagedList(pageNumber, pageSize));
            }
            ViewBag.Result = 0;
            return RedirectToAction("DisplayAllProduct");
        }

        public JsonResult GetDataFromSearchAutoComplete(string prefix)
        {
            var data = db.SanPhams.Where(sp => (sp.TenSP.Contains(prefix) || sp.DanhMuc.TenDM.Contains(prefix)) && sp.DaXoa == false).Select(n => new { image = n.Pic1, name = n.TenSP, id = n.MaSP, price_original = n.GiaNiemYet, price_sale = (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) }).Take(5);
            return Json(new { data, status = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Rating(int id_product, string content, int starRating)
        {
            if (Session["Account"] == null) {
                return Json(new { status = 301 });
            }
            ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.MaTV.Equals(int.Parse(Session["IdUser"].ToString())));

            DanhGia rateUser = db.DanhGias.SingleOrDefault(n => n.MaTV.Equals(int.Parse(Session["IdUser"].ToString())) && n.MaSP.Equals(id_product));

            if (rateUser != null) {
                rateUser.NoiDungBL = content;
                rateUser.CommentedOn = DateTime.Now;
                rateUser.Rating = starRating;
                db.SubmitChanges();
                return Json(new { status = 200 });
            }

            DanhGia newRating = new DanhGia();
            newRating.MaTV = int.Parse(Session["IdUser"].ToString());
            newRating.MaSP = id_product;
            newRating.NoiDungBL = content;
            newRating.CommentedOn = DateTime.Now;
            newRating.Rating = starRating;
            Session["NameUserRating"] = tv.HoTen;
            Session["IdProductRating"] = newRating.MaSP;
            Session["ContentRating"] = newRating.NoiDungBL;
            Session["CommentedOn"] = newRating.CommentedOn;
            Session["StarRating"] = newRating.Rating;
            Session["CountRating"] = db.DanhGias.Count();
            db.DanhGias.InsertOnSubmit(newRating);
            db.SubmitChanges();
            NumberStarRatingAverage(id_product);
            return PartialView("RatingPartial");
        }


        public void NumberStarRatingAverage(int idProduct) {
            var star1 = db.DanhGias.Where(n => n.Rating.Equals(1) && n.MaSP.Equals(idProduct)).Count();
            var star2 = db.DanhGias.Where(n => n.Rating.Equals(2) && n.MaSP.Equals(idProduct)).Count();
            var star3 = db.DanhGias.Where(n => n.Rating.Equals(3) && n.MaSP.Equals(idProduct)).Count();
            var star4 = db.DanhGias.Where(n => n.Rating.Equals(4) && n.MaSP.Equals(idProduct)).Count();
            var star5 = db.DanhGias.Where(n => n.Rating.Equals(5) && n.MaSP.Equals(idProduct)).Count();
            var scoreTotal = 1 * star1 + 2 * star2 + 3 * star3 + 4 * star4 + 5 * star5;
            var responseTotal = star1 + star2 + star3 + star4 + star5;
            if (responseTotal.Equals(0))
            {
                Session["NumberStar"] = 0;
            }
            else { 
                Session["NumberStar"] = Math.Round((decimal)scoreTotal / responseTotal);
            }
        }

        public ActionResult RatingPartial()
        {
            return PartialView();
        }

        public JsonResult GetDataProgressStarRating(int id) {
            var data = ListProgressRating(id);
            return Json(new { data, status = true }, JsonRequestBehavior.AllowGet);
        }


        public List<double> ListProgressRating(int id) {
            List<double> lstProg = new List<double>();
            var totalCommented = db.DanhGias.Where(n => n.MaSP.Equals(id)).Count();

            if (totalCommented.Equals(0)) {
                return lstProg;
            }

            var star5Count = db.DanhGias.Where(n => n.Rating.Equals(5) && n.MaSP.Equals(id)).Count();
            var star4Count = db.DanhGias.Where(n => n.Rating.Equals(4) && n.MaSP.Equals(id)).Count();
            var star3Count = db.DanhGias.Where(n => n.Rating.Equals(3) && n.MaSP.Equals(id)).Count();
            var star2Count = db.DanhGias.Where(n => n.Rating.Equals(2) && n.MaSP.Equals(id)).Count();
            var star1Count = db.DanhGias.Where(n => n.Rating.Equals(1) && n.MaSP.Equals(id)).Count();

            var calcStar5 = ((double)(star5Count) / totalCommented) * 100;
            var calcStar4 = ((double)(star4Count) / totalCommented) * 100;
            var calcStar3 = ((double)(star3Count) / totalCommented) * 100;
            var calcStar2 = ((double)(star2Count) / totalCommented) * 100;
            var calcStar1 = ((double)(star1Count) / totalCommented) * 100;

            lstProg.Add(calcStar5);
            lstProg.Add(calcStar4);
            lstProg.Add(calcStar3);
            lstProg.Add(calcStar2);
            lstProg.Add(calcStar1);
            return lstProg;
        }


        public bool HandlerDisplayChatStarRating(int id) {
            if (Session["Account"] == null) return false;
            ThanhVien mem = (ThanhVien)Session["Account"];
            var checkCus = db.KhachHangs.Where(n => n.MaThanhVien.Equals(mem.MaTV));
            if (checkCus.Count() == 0) return false;
            var count = 0;
            var ct = db.ChiTietDonDatHangs;
            foreach (var cus in checkCus) {
                foreach (var item in ct) {
                    if (item.MaSP.Equals(id) && item.DonDatHang.MaKH.Equals(cus.MaKH) && item.DonDatHang.DaThanhToan == true) {
                        count++;
                    }
                }
            }
            if (count == 0) return false;
            return true;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (db != null)
        //            db.Dispose();
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}