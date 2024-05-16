using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class HighchartController : AdminController
    {
        dbChienThangDataContext context = new dbChienThangDataContext();

        public class StatusOrder
        {
            public int WaitForConfirmation { get; set; }
            public int WaitForGetGoods { get; set; }
            public int Delivering { get; set; }
            public int Delivered { get; set; }
            public int Canceled { get; set; }
        }


        // This is class contains name and quantity sold of product
        public class ProductInfo {
            public string Name { get; set; }
            public int QuantitySold { get; set; }

            public ProductInfo(string name, int quantitySold) {
                Name = name;
                QuantitySold = quantitySold;
            }
        }


        public class TotalOrder {
            public int Month { get; set; }
            public int Value { get; set; }
            public TotalOrder(int month, int value) {
                Month = month;
                Value = value;
            }
        }

        public class Revenue
        {
            public int Month { get; set; }
            public decimal Money { get; set; }
            public Revenue(int month, decimal money)
            {
                Month = month;
                Money = money;
            }
        }


        // View

        public ActionResult Index() {
            GetData();
            var queryMonth = context.DonDatHangs.Where(n => n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).GroupBy(n => n.NgayDat.Value.Month).Select(n => n.Key).ToList();
            List<Revenue> lstRevenue = new List<Revenue>();
            foreach (var month in queryMonth)
            {
                var obj = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year));
                decimal revenue = 0;
                foreach (var item in obj)
                {
                    revenue += (decimal)context.ChiTietDonDatHangs.Where(n => n.MaDDH == item.MaDDH).Sum(n => n.SoLuong * n.DonGia);
                }
                Revenue rv = new Revenue(month, revenue);
                lstRevenue.Add(rv);
            }
            List<TotalOrder> lstTto = new List<TotalOrder>();
            foreach (var item in queryMonth)
            {
                var itemCount = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(item) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
                TotalOrder tto = new TotalOrder(item, itemCount);
                lstTto.Add(tto);
            }
            ViewBag.RevenueStatistic = JsonConvert.SerializeObject(lstRevenue);
            ViewBag.TotalOrderStatistic = JsonConvert.SerializeObject(lstTto);
            return View();
        }



        // Handle Data

        [HttpPost]
        public ActionResult PostDataStatisticDetail(int year) {
            var queryMonth = context.DonDatHangs.Where(n => n.NgayDat.Value.Year.Equals(year)).GroupBy(n => n.NgayDat.Value.Month).Select(n => n.Key).ToList();
            List<Revenue> lstRevenue = new List<Revenue>();
            foreach (var month in queryMonth)
            {
                var obj = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year));
                decimal revenue = 0;
                foreach (var item in obj)
                {
                    revenue += (decimal)context.ChiTietDonDatHangs.Where(n => n.MaDDH == item.MaDDH).Sum(n => n.SoLuong * n.DonGia);
                }
                Revenue rv = new Revenue(month, revenue);
                lstRevenue.Add(rv);
            }
            List<TotalOrder> lstTto = new List<TotalOrder>();
            foreach (var item in queryMonth)
            {
                var itemCount = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(item) && n.NgayDat.Value.Year.Equals(year)).Count();
                TotalOrder tto = new TotalOrder(item, itemCount);
                lstTto.Add(tto);
            }
            return Json(new { arrRevenue = JsonConvert.SerializeObject(lstRevenue), arrTotalOrder = JsonConvert.SerializeObject(lstTto) });
        }


        public ActionResult GetRevenue()
        {
            var queryMonth = context.DonDatHangs.Where(n => n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).GroupBy(n => n.NgayDat.Value.Month).Select(n => n.Key).ToList();
            List<Revenue> lstRevenue = new List<Revenue>();
            foreach (var month in queryMonth)
            {
                var obj = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year));
                decimal revenue = 0;
                foreach (var item in obj)
                {
                    revenue += (decimal)context.ChiTietDonDatHangs.Where(n => n.MaDDH == item.MaDDH).Sum(n => n.SoLuong * n.DonGia);
                }
                Revenue rv = new Revenue(month, revenue);
                lstRevenue.Add(rv);
            }
            return Json(new { yAsis = JsonConvert.SerializeObject(lstRevenue) }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult PostDataGetRevenue(int year)
        {
            var queryMonth = context.DonDatHangs.Where(n => n.NgayDat.Value.Year.Equals(year)).GroupBy(n => n.NgayDat.Value.Month).Select(n => n.Key).ToList();
            List<Revenue> lstRevenue = new List<Revenue>();
            foreach (var month in queryMonth)
            {
                var obj = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year));
                decimal revenue = 0;
                foreach (var item in obj)
                {
                    revenue += (decimal)context.ChiTietDonDatHangs.Where(n => n.MaDDH == item.MaDDH).Sum(n => n.SoLuong * n.DonGia);
                }
                Revenue rv = new Revenue(month, revenue);
                lstRevenue.Add(rv);
            }
            return Json(new { yAsis = JsonConvert.SerializeObject(lstRevenue) });
        }

        public ActionResult GetStatusOrder()
        {
            int waitForConfirmationCount = context.DonDatHangs.Where(n => n.ChoXacNhan == true && n.NgayDat.Value.Month.Equals(DateTime.Now.Month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
            int waitForGetGoodsCount = context.DonDatHangs.Where(n => n.ChoLayHang == true && n.NgayDat.Value.Month.Equals(DateTime.Now.Month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
            int deliveringCount = context.DonDatHangs.Where(n => n.DangGiao == true && n.NgayDat.Value.Month.Equals(DateTime.Now.Month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
            int deliveredCount = context.DonDatHangs.Where(n => n.DaGiao == true && n.NgayDat.Value.Month.Equals(DateTime.Now.Month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
            int canceledCount = context.DonDatHangs.Where(n => n.DaHuy == true && n.NgayDat.Value.Month.Equals(DateTime.Now.Month) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
            StatusOrder st = new StatusOrder();
            st.WaitForConfirmation = waitForConfirmationCount;
            st.WaitForGetGoods = waitForGetGoodsCount;
            st.Delivering = deliveringCount;
            st.Delivered = deliveredCount;
            st.Canceled = canceledCount;
            return Json(st, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostDataGetStatusOrder(int month, int year)
        {
            int waitForConfirmationCount = context.DonDatHangs.Where(n => n.ChoXacNhan == true && n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year)).Count();
            int waitForGetGoodsCount = context.DonDatHangs.Where(n => n.ChoLayHang == true && n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year)).Count();
            int deliveringCount = context.DonDatHangs.Where(n => n.DangGiao == true && n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year)).Count();
            int deliveredCount = context.DonDatHangs.Where(n => n.DaGiao == true && n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year)).Count();
            int canceledCount = context.DonDatHangs.Where(n => n.DaHuy == true && n.NgayDat.Value.Month.Equals(month) && n.NgayDat.Value.Year.Equals(year)).Count();
            StatusOrder st = new StatusOrder();
            st.WaitForConfirmation = waitForConfirmationCount;
            st.WaitForGetGoods = waitForGetGoodsCount;
            st.Delivering = deliveringCount;
            st.Delivered = deliveredCount;
            st.Canceled = canceledCount;
            return Json(st);
        }


        public ActionResult GetTotalOrder()
        {
            var queryMonth = context.DonDatHangs.Where(n => n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).GroupBy(n => n.NgayDat.Value.Month).Select(n => n.Key).ToList();
            List<TotalOrder> lstTto = new List<TotalOrder>();
            foreach (var item in queryMonth)
            {
                var itemCount = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(item) && n.NgayDat.Value.Year.Equals(DateTime.Now.Year)).Count();
                TotalOrder tto = new TotalOrder(item, itemCount);
                lstTto.Add(tto);
            }

            return Json(new { yAsis = JsonConvert.SerializeObject(lstTto) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PostDataGetTotalOrder(int year)
        {
            var queryMonth = context.DonDatHangs.Where(n => n.NgayDat.Value.Year.Equals(year)).GroupBy(n => n.NgayDat.Value.Month).Select(n => n.Key).ToList();

            List<TotalOrder> lstTto = new List<TotalOrder>();
            foreach (var item in queryMonth)
            {
                var itemCount = context.DonDatHangs.Where(n => n.NgayDat.Value.Month.Equals(item) && n.NgayDat.Value.Year.Equals(year)).Count();
                TotalOrder tto = new TotalOrder(item, itemCount);
                lstTto.Add(tto);
            }

            return Json(new { yAsis = JsonConvert.SerializeObject(lstTto) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTopProductBestSelling() {
            var querySelect = context.SanPhams.Where(n => n.DaXoa == false).OrderByDescending(n => n.SoLanMua).Take(5);
            List<ProductInfo> lstProduct = new List<ProductInfo>();
            foreach (var item in querySelect) {
                ProductInfo pdInfo = new ProductInfo(item.TenSP, (int)item.SoLanMua);
                lstProduct.Add(pdInfo);
            }
            return Json(lstProduct, JsonRequestBehavior.AllowGet);
        }
    }
}