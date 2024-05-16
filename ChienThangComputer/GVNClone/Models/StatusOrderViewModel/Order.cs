using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GVNClone.Models.StatusOrderViewModel
{
    public class Order
    {
        public int product_id { get; set; }
        public string product_thumb { get; set; }
        public string product_name { get; set; }
        public int product_quantity { get; set; }
        public decimal product_price { get; set; }
        public decimal total_Amount
        {
            get { return product_price * product_quantity; }
        }

        public bool WaitForConfirmation { get; set; }
        public bool WaitForGetGoods { get; set; }
        public bool Delivering { get; set; }
        public bool Delivered { get; set; }
        public bool Canceled { get; set; }

        public bool isEvaluated { get; set; }
    }
}