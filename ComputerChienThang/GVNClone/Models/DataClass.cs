using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GVNClone.Models
{
    public class DataClass
    {
        public int IDClass { get; set; }
        public double Age { get; set; }
        public double CurrentRevenue { get; set; }

        public DataClass(int _idClass, double _age, double _currentRevenue )
        {
            Age = _age;
            CurrentRevenue = _currentRevenue;
            IDClass = _idClass;
        }
    }
}