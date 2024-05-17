using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChienThangComputer.Models
{
    public class Voucher
    {
        public string Id { get; set; }
        public int totalQuantityCanUse { get; set; }
        public int remainQuantityCanUse { get; set; }
        public int endYear { get; set; }
        public int endMonth { get; set; }
        public int endDay { get; set; }
        public int endHour { get; set; }
        public int endMin { get; set; }
    }
}