using Entities.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Repositories.IRepositories;
using Repositories.Repositories;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class SummaryReportController : Controller
    {
        private readonly IVehicleInspectionRepository _vehicleInspectionRepository;
        private readonly IAllCodeRepository _allCodeRepository;

        private readonly IConfiguration _configuration;
        public SummaryReportController(IVehicleInspectionRepository vehicleInspectionRepository, IConfiguration configuration, IAllCodeRepository allCodeRepository)
        {
            _vehicleInspectionRepository = vehicleInspectionRepository;
            _configuration = configuration;
            _allCodeRepository = allCodeRepository;
        }
        public async Task<IActionResult> Index()
        { 
            return View();
        }  
        public async Task<IActionResult> TotalVehicleInspection( string date)
        {
            var date_time = date != null && date != "" ? DateUtil.StringToDate(date) : null;

            var Total = await _vehicleInspectionRepository.CountTotalVehicleInspectionSynthetic(date_time, date_time);
            ViewBag.TotalData = Total;
            return View();
        }
        public async Task<IActionResult> DetailSummaryReport()
        {
            var LOAD_TYPE = await _allCodeRepository.GetListSortByName(AllCodeType.LOAD_TYPE);
            ViewBag.LOAD_TYPE = LOAD_TYPE;
            return View();
        }
        public async Task<IActionResult> DailyStatistics(SummaryReportSearchModel SearchModel)
        {
            try
            {
                var FromDate = SearchModel.FromDate != null && SearchModel.FromDate != "" ? DateUtil.StringToDate(SearchModel.FromDate) : null;
                var ToDate = SearchModel.ToDate != null && SearchModel.ToDate != "" ? DateUtil.StringToDate(SearchModel.ToDate) : null;

                var data = await _vehicleInspectionRepository.GetListVehicleInspectionSynthetic(FromDate, ToDate, SearchModel.LoadType);
                var Total = await _vehicleInspectionRepository.CountTotalVehicleInspectionSynthetic(FromDate, ToDate);
                ViewBag.TotalData = Total;
                
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DailyStatistics - SummaryReportController: " + ex);
            }
            return PartialView();
        }   
        public async Task<IActionResult> TotalWeightByTroughType(string date)
        {
            try
            {
                var date_time = date!=null && date !=""? DateUtil.StringToDate(date):null;
                var data = await _vehicleInspectionRepository.GetTotalWeightByTroughType(date_time);
                var datamodel = new TotalWeightByHourViewModel();
               
                datamodel.TroughType = data.Select(x => x.TroughType).ToArray();
                datamodel.SanLuong = data.Select(x => x.SanLuong).ToArray();
                datamodel.SoXe = data.Select(x => x.SoXe).ToArray();
                datamodel.TongGio = data.Select(x => x.TongGio).ToArray();
                datamodel.Tan_Moi_Gio = data.Select(x => x.Tan_Moi_Gio).ToArray();

                return Ok(new
                {
                    isSuccess = true,
                    data = datamodel
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByWeightGroup - SummaryReportController: " + ex);
            }
            return Ok(new
            {
                isSuccess = false,

            });
        }
        public async Task<IActionResult> GetTotalWeightByWeightGroup(string date)
        {
            try
            {
                var date_time = date!=null && date !=""? DateUtil.StringToDate(date):null;
            
                var data = await _vehicleInspectionRepository.GetTotalWeightByWeightGroup(date_time);
                var datamodel=new TotalWeightByHourViewModel();
                datamodel.CompletionHour = data.Select(x => x.CompletionHour).ToArray();
                datamodel.TotalWeightInHour = data.Select(x => x.TotalWeightInHour).ToArray();
                datamodel.WeightGroup = data.Select(x => x.WeightGroup).ToArray();
                datamodel.TotalWeightTons = data.Select(x => x.TotalWeightTons).ToArray();
                datamodel.TotalProcessMinutes = data.Select(x => x.TotalProcessMinutes).ToArray();
                datamodel.SanLuong = data.Select(x => x.SanLuong).ToArray();
                datamodel.SoPhut_Tren_Tan = data.Select(x => x.SoPhut_Tren_Tan).ToArray();
                datamodel.SoPhut_Tren_Xe = data.Select(x => x.SoPhut_Tren_Xe).ToArray();

                return Ok(new
                {
                    isSuccess = true,
                    data = datamodel
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByWeightGroup - SummaryReportController: " + ex);
            }
            return Ok(new
            {
                isSuccess = false,
               
            });
        }
        public async Task<IActionResult> GetTotalWeightByHour(string date)
        {
            try
            {
                var date_time = date != null && date != "" ? DateUtil.StringToDate(date) : null;

                var data = await _vehicleInspectionRepository.GetTotalWeightByHour(date_time);
                var datamodel = new TotalWeightByHourViewModel();
                datamodel.KhungGio = data.Select(x => x.KhungGio).ToArray();
                datamodel.SanLuong = data.Select(x => x.SanLuong).ToArray();
                datamodel.Tan_Moi_Gio = data.Select(x => x.Tan_Moi_Gio).ToArray();
            

                return Ok(new
                {
                    isSuccess = true,
                    data = datamodel
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetTotalWeightByWeightGroup - SummaryReportController: " + ex);
            }
            return Ok(new
            {
                isSuccess = false,

            });
        }
        public async Task<IActionResult> GetProductivityStatistics(string date)
        {
            try
            {
                var date_time = date != null && date != "" ? DateUtil.StringToDate(date) : null;
                var data = await _vehicleInspectionRepository.CountTotalVehicleInspectionSynthetic(date_time, date_time);
             
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetProductivityStatistics - SummaryReportController: " + ex);
            }
            return PartialView();
        }
    }
}
