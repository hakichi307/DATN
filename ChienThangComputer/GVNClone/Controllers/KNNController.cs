using GVNClone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GVNClone.Controllers
{
    public class KNNController : Controller
    {
        readonly dbChienThangDataContext db = new dbChienThangDataContext();


        public struct MinMaxRevenue
        {
            public double min;
            public double max;
        }

        public struct MinMaxAge
        {
            public double min;
            public double max;
        }

        public struct MostFrequent
        {
            public int firstNumber;
            public int secondNumber;
        }

        public struct PointsCenterCirle
        {
            public double age;
            public double revenue;
        }

        public struct DistanceTwoCircle
        {
            public double distance1;
            public int class1;
            public double distance2;
            public int class2;
        }

        // GET: KNN
        public ActionResult FormSurvey()
        {
            return View(db.LoaiSanPhams);
        }

        public ActionResult RunAlgorithm(double age, double curRevenue, LoaiSanPham typeProduct)
        {

            // Initial list
            List<DataTrainingKNN> lstData = new List<DataTrainingKNN>();

            List<DataClass> lstDataClass = new List<DataClass>();

            // First. Get data training from sql and push into list
            var lstDataDB = db.KhachHangTrainings.ToList();

            var lstDataClassDB = db.PhanLopKhachHangs.ToList();

            foreach (var item in lstDataDB)
            {
                DataTrainingKNN dtTrainItem = new DataTrainingKNN(item.MaKHPL, (int)item.Tuoi, (double)item.MucTaiChinh, (int)item.MaPL);
                lstData.Add(dtTrainItem);
            }

            foreach (var item in lstDataClassDB)
            {
                DataClass dtClass = new DataClass(item.MaPL, (double)item.Tuoi, (double)item.MucTaiChinh);
                lstDataClass.Add(dtClass);
            }

            // Normalization revenue and age from list data

            List<DataTrainingKNN> lstNormalizated = NormalizationListData(lstData);

            // Normalization single attribute revenue
            double norRevenue = NormalizationSingleRevenue(curRevenue, lstData);

            // Normalization single attribute age
            double norAge = NormalizationSingleAge(age, lstData);

            // Compute distance
            List<EuclideanDistance> lstEudis = ComputeDistance(lstNormalizated, norRevenue, norAge);

            /*---------------------------Method---------------------------*/

            // Method 1:

            // Choose distance
            double d = Math.Round(lstEudis.Average(n => n.Distance), 2);
            var result = lstEudis.Where(n => n.Distance <= d).OrderBy(n => n.Distance).ToList();


            //Method 2:
            //Take k nearest neighbor

            // Pick a k, k must be odd, not too big
            //int k = 7;

            //var lstOrderBy = lstEudis.OrderBy(n => n.Distance).ToList();

            //var result = lstOrderBy.Take(k).ToList();


            /*---------------------------Method---------------------------*/

            // Initialize an array and push the class
            int[] a = new int[result.Count];

            for (var i = 0; i < result.Count; i++)
            {
                a[i] = result[i].IDClass;
            }

            //-------------------------------------------

            // Find the final class
            //int finalClass = FindNumAppearsTheMost(a);

            var objNumberFrequent = TwoNumFrequent(a);

            // Center circle point in data
            var objPoint1 = FindCenterCircle(objNumberFrequent.firstNumber, lstDataClass);

            var objPoint2 = FindCenterCircle(objNumberFrequent.secondNumber, lstDataClass);

            var res = ComputeDistanceBetweenTwoCircle(norAge, norRevenue, objPoint1, objPoint2, objNumberFrequent.firstNumber, objNumberFrequent.secondNumber);

            int finalClass = res.distance1 < res.distance2 ? res.class1 : res.class2;

            // Query
            var query = db.SanPhams.Where(n => n.MaPL.Equals(finalClass) && n.MaLoaiSP.Equals(typeProduct.MaLoaiSP) && (n.MaGiamGia != null && n.GiamGia.NgayBatDau < DateTime.Now && n.GiamGia.NgayKetThuc > DateTime.Now ? n.GiamGia.GiaKhuyenMai : n.GiaNiemYet) <= (decimal)curRevenue && n.DaXoa == false);
            Session["finalClass"] = finalClass;
            Session["KNNClass"] = db.PhanLopKhachHangs.SingleOrDefault(n => n.MaPL.Equals(finalClass)).TenPL;
            return View(query);
        }

        // Chuẩn hoá đơn dữ liệu doanh thu khách hàng post data

        public double NormalizationSingleRevenue(double revenue, List<DataTrainingKNN> lstData)
        {
            MinMaxRevenue resultValue = FindMinMaxRevenue(lstData);
            double result = Math.Round(((revenue - resultValue.min) / (resultValue.max - resultValue.min)), 2);
            return result;
        }

        // Chuẩn hoá đơn dữ liệu tuổi khách hàng post data
        public double NormalizationSingleAge(double age, List<DataTrainingKNN> lstData)
        {
            MinMaxAge resultAge = FindMinMaxAge(lstData);
            double result = Math.Round(((age - resultAge.min) / (resultAge.max - resultAge.min)), 2);
            return result;
        }


        // Chuẩn hoá dữ liệu trường thuộc tính doanh thu và tuổi trong danh sách training
        public List<DataTrainingKNN> NormalizationListData(List<DataTrainingKNN> lstData)
        {
            MinMaxAge resultAge = FindMinMaxAge(lstData);
            MinMaxRevenue resultRevenue = FindMinMaxRevenue(lstData);
            List<DataTrainingKNN> lstNormalizated = new List<DataTrainingKNN>();
            foreach (var item in lstData)
            {
                var age = Math.Round(((item.Age - resultAge.min) / (resultAge.max - resultAge.min)), 2);
                var revenue = Math.Round(((item.CurrentRevenue - resultRevenue.min) / (resultRevenue.max - resultRevenue.min)), 2);
                DataTrainingKNN newData = new DataTrainingKNN(item.IDDataTraining, age, revenue, item.IDClass);
                lstNormalizated.Add(newData);
            }
            return lstNormalizated;
        }


        public MinMaxRevenue FindMinMaxRevenue(List<DataTrainingKNN> lstData)
        {
            MinMaxRevenue values = new MinMaxRevenue();
            foreach (var item in lstData.OrderBy(n => n.CurrentRevenue).Take(1))
            {
                values.min = item.CurrentRevenue;
            }
            foreach (var item in lstData.OrderByDescending(n => n.CurrentRevenue).Take(1))
            {
                values.max = item.CurrentRevenue;
            }

            return values;
        }

        public MinMaxAge FindMinMaxAge(List<DataTrainingKNN> lstData)
        {
            MinMaxAge values = new MinMaxAge();
            foreach (var item in lstData.OrderBy(n => n.Age).Take(1))
            {
                values.min = item.Age;
            }
            foreach (var item in lstData.OrderByDescending(n => n.Age).Take(1))
            {
                values.max = item.Age;
            }

            return values;
        }



        public MinMaxRevenue FindMinMaxRevenueClass(List<DataClass> lstData)
        {
            MinMaxRevenue values = new MinMaxRevenue();
            foreach (var item in lstData.OrderBy(n => n.CurrentRevenue).Take(1))
            {
                values.min = item.CurrentRevenue;
            }
            foreach (var item in lstData.OrderByDescending(n => n.CurrentRevenue).Take(1))
            {
                values.max = item.CurrentRevenue;
            }

            return values;
        }

        public MinMaxAge FindMinMaxAgeClass(List<DataClass> lstData)
        {
            MinMaxAge values = new MinMaxAge();
            foreach (var item in lstData.OrderBy(n => n.Age).Take(1))
            {
                values.min = item.Age;
            }
            foreach (var item in lstData.OrderByDescending(n => n.Age).Take(1))
            {
                values.max = item.Age;
            }

            return values;
        }

        // Method choose k nearest neighbor
        public int FindNumAppearsTheMost(int[] array)
        {

            var max = (array.GroupBy(x => x)
                .Select(x => new { num = x, cnt = x.Count() })
                .OrderByDescending(g => g.cnt)
                .Select(g => g.num).First());

            return max.Key;
        }

        // Find two number frequent the most in an array
        public MostFrequent TwoNumFrequent(int[] array)
        {
            MostFrequent mf = new MostFrequent();
            var firstNum = (array.GroupBy(x => x)
                .Select(x => new { num = x, cnt = x.Count() })
                .OrderByDescending(g => g.cnt)
                .Select(g => g.num).First());

            var secondNum = (array.GroupBy(x => x)
                .Select(x => new { num = x, cnt = x.Count() })
                .Take(2)
                .OrderBy(g => g.cnt)
                .Select(x => x.num).First());

            mf.firstNumber = firstNum.Key;
            mf.secondNumber = secondNum.Key;
            return mf;
        }


        public PointsCenterCirle FindCenterCircle(int IDClass, List<DataClass> lstData)
        {
            PointsCenterCirle pt = new PointsCenterCirle();
            MinMaxAge resultAge = FindMinMaxAgeClass(lstData);
            MinMaxRevenue resultRevenue = FindMinMaxRevenueClass(lstData);
            PhanLopKhachHang objClass = db.PhanLopKhachHangs.SingleOrDefault(n => n.MaPL.Equals(IDClass));
            pt.age = Math.Round((double)((objClass.Tuoi - resultAge.min) / (resultAge.max - resultAge.min)), 2);
            pt.revenue = Math.Round((double)((objClass.MucTaiChinh - resultRevenue.min) / (resultRevenue.max - resultRevenue.min)), 2);
            return pt;
        }


        public DistanceTwoCircle ComputeDistanceBetweenTwoCircle(double norAge, double norRevenue, PointsCenterCirle objPoint1, PointsCenterCirle objPoint2, int class1, int class2)
        {
            DistanceTwoCircle dt = new DistanceTwoCircle();
            double distance1 = Math.Round(Math.Sqrt(Math.Pow((norAge - objPoint1.age), 2) + Math.Pow((norRevenue - objPoint1.revenue), 2)), 2);
            double distance2 = Math.Round(Math.Sqrt(Math.Pow((norAge - objPoint2.age), 2) + Math.Pow((norRevenue - objPoint2.revenue), 2)), 2);
            dt.distance1 = distance1;
            dt.distance2 = distance2;
            dt.class1 = class1;
            dt.class2 = class2;
            return dt;
        }


        public List<EuclideanDistance> ComputeDistance(List<DataTrainingKNN> lstNormalizated, double norRevenue, double norAge)
        {

            List<EuclideanDistance> lstEuDis = new List<EuclideanDistance>();

            foreach (var item in lstNormalizated.OrderBy(n => n.IDDataTraining))
            {
                double distance = Math.Round(Math.Sqrt(Math.Pow(norAge - item.Age, 2) + Math.Pow(norRevenue - item.CurrentRevenue, 2)), 2);
                int idClass = item.IDClass;
                EuclideanDistance dt = new EuclideanDistance(distance, idClass);
                lstEuDis.Add(dt);
            }

            return lstEuDis;
        }
    }
}