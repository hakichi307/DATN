using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GVNClone.Models
{
    public class EuclideanDistance
    {
        public double Distance { get; set; }
        public int IDClass { get; set; }

        public EuclideanDistance(double _distance, int _idClass) {
            Distance = _distance;
            IDClass = _idClass;
        }
    }
}