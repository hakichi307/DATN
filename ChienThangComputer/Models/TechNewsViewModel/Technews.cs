using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChienThangComputer.Models.TechNewsViewModel
{
    public class Technews
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public DateTime DateUpdate { get; set; }
        public string Excerpt { get; set; }

        public List<Technews> lst { get; set; }
    }
}