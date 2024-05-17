using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChienThangComputer.Models
{
    public class DataTrainingKNN
    {
        public int IDDataTraining { get; set; }
        public double Age { get; set; }
        public double CurrentRevenue { get; set; }
        public int IDClass { get; set; }

        public DataTrainingKNN(int _idDataTraining, double _age, double _currentRevenue, int _idClass) {
            IDDataTraining = _idDataTraining;
            Age = _age;
            CurrentRevenue = _currentRevenue;
            IDClass = _idClass;
        }
    }
}