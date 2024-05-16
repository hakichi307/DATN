using GVNClone.Models.TechNewsViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace GVNClone.Controllers
{
    public class TechNewsController : Controller
    {
        dbChienThangDataContext context = new dbChienThangDataContext();
        // GET: TechNews

        public class Views
        {
            public string Category { get; set; }
            public int View { get; set; }
        }

        public void LoadData()
        {
            ViewBag.ListFeature1 = context.TinTucs.Where(n => n.MaDanhMucTin.Equals(1) && n.DaXoa == false).Take(3);
            ViewBag.ListFeature2 = context.TinTucs.Where(n => n.MaDanhMucTin.Equals(2) && n.DaXoa == false).Take(3);
            ViewBag.Categories = context.LoaiTinTucs.ToList();
            List<Views> lstView = new List<Views>();
            for (int i = 1; i <= context.LoaiTinTucs.Count(); i++) {
                Views v = new Views();
                var countView = context.TinTucs.Where(n => n.MaLoaiTin.Equals(i) && n.DaXoa == false).Sum(n => n.LuotXem);
                v.Category = context.LoaiTinTucs.SingleOrDefault(n => n.MaLoaiTin.Equals(i)).TenLoaiTin;
                v.View = int.Parse(countView.ToString());
                lstView.Add(v);
            }
            ViewBag.ListView = lstView;
        }

        public ActionResult Index()
        {
            LoadData();
            return View();
        }

        public ActionResult Home() {
            LoadData();
            return View();
        }


        public ActionResult ListTechNewsFeature() {
            LoadData();
            Random _rd = new Random();
            List<Technews> lstTechNewsFeature = new List<Technews>();
            var lstTech = context.TinTucs.Where(n => n.MaDanhMucTin.Equals(1) && n.DaXoa == false);
            foreach (var item in lstTech)
            {
                var randomNumber = _rd.Next(1, context.TinTucs.Where(n => n.DaXoa == false).Count());
                var tech = (from a in context.TinTucs
                            where a.MaTin.Equals(randomNumber) && a.DaXoa == false
                            select new Technews
                            {
                                ID = a.MaTin,
                                Image = a.HinhAnh,
                                Title = a.TieuDeTin,
                                DateUpdate = DateTime.Parse(a.NgayDang.ToString()),
                                Excerpt = a.DoanTrich
                            }).First();
                lstTechNewsFeature.Add(tech);
                if (lstTechNewsFeature.Count == 2) {
                    break;
                }
            }
            return View(lstTechNewsFeature);
        }

        public ActionResult HeroTechNews() {
            LoadData();
            Random _rd = new Random();
            var randomNumber = _rd.Next(1, context.TinTucs.Where(n => n.DaXoa == false).Count());
            var tech = (from a in context.TinTucs
                        where a.MaTin.Equals(randomNumber) && a.DaXoa == false
                        select new Technews
                        {
                            ID = a.MaTin,
                            Image = a.HinhAnh,
                            Title = a.TieuDeTin,
                            DateUpdate = DateTime.Parse(a.NgayDang.ToString()),
                            Excerpt = a.DoanTrich
                        }).First();
            return View(tech);
        }

        public ActionResult ListTechNews(int? page) {
            LoadData();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var lstTechNews = context.TinTucs.OrderBy(n => n.MaTin).Where(n => n.DaXoa == false);
            return PartialView(lstTechNews.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult ViewDetail(int? id) {
            LoadData();
            if (id == null) {
                return RedirectToAction("NotFound", "Auth");
            }
            var objTechNews = context.TinTucs.SingleOrDefault(n => n.MaTin.Equals(id) && n.DaXoa == false);
            objTechNews.LuotXem += 1;
            context.SubmitChanges();
            return View(objTechNews);
        }

        public ActionResult Relevant(int? id_TechNew, int? id_Category) {
            LoadData();
            if (id_TechNew == null || id_Category == null) {
                return RedirectToAction("NotFound", "Auth");
            }
            return View(context.TinTucs.Where(n => n.MaTin != id_TechNew && n.MaLoaiTin.Equals(id_Category) && n.DaXoa == false).Take(3));
        }


        public ActionResult SearchAction(string keyword, int? page) {
            LoadData();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.KeywordSearch = keyword;
            if (!String.IsNullOrEmpty(keyword)) {
                var query = context.TinTucs.Where(n => (n.TieuDeTin.Contains(keyword) || n.DoanTrich.Contains(keyword)) && n.DaXoa == false);
                if (query.Count() == 0) {
                    ViewBag.ResultSearch = query.Count();
                    return View();
                }
                ViewBag.ResultSearch = query.Count();
                return View(query.ToPagedList(pageNumber, pageSize));
            }
            return View(context.TinTucs.Where(n => n.DaXoa == false).ToPagedList(pageNumber,pageSize));
        }


        public ActionResult ListCategory(string category, int? page) {
            LoadData();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.Category = category;
            var query = context.TinTucs.Where(n => n.LoaiTinTuc.TenLoaiTin.Equals(category) && n.DaXoa == false).ToPagedList(pageNumber, pageSize);
            if (query.Count == 0) {
                return RedirectToAction("NotFound", "Auth");
            }
            return View(query);
        }


    }
}