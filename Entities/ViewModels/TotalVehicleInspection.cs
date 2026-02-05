using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class TotalVehicleInspection
    {
        public double TotalCar { get; set; }//Tổng số xe theo ngày
        public double TotalCarCompleted { get; set; }//Tổng số xe đã lấy hàng
        public double TotalCarUnfinished { get; set; }//Tổng số xe chưa lấy hàng
        public double TotalCarArriving16h { get; set; }//Tổng số xe đến lấy hàng sau 16h
        public double TotalTroughType3 { get; set; }//Tổng số bản ghi theo từng loại máng
        public double TotalTroughType4 { get; set; }
        public double TotalTroughType5 { get; set; }
        public double TotalTroughType6 { get; set; }
        public double TotalTroughType7 { get; set; }
        public double TotalTroughType8 { get; set; }
        public double TotalWeightTroughType { get; set; }//TÍNH TỔNG TRỌNG LƯỢNG
        public double TotalWeightTroughType3 { get; set; }
        public double TotalWeightTroughType4 { get; set; }
        public double TotalWeightTroughType5 { get; set; }
        public double TotalWeightTroughType6 { get; set; }
        public double TotalWeightTroughType7 { get; set; }
        public double TotalWeightTroughType8 { get; set; }
        public double TotalTimeInHour { get; set; }
        public double AvgTimePerCompletedCar_Hour { get; set; }
        public DateTime MaxVehicleTimeOutToday { get; set; }
        public double TotalTimeWorkInHour { get; set; }
        public double AverageProductivity { get; set; }
        public DateTime VehicleWeighingTimeComplete { get; set; }

    }
}
