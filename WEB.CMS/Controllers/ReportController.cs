using Microsoft.AspNetCore.Mvc;
using Repositories.IRepositories;
using Repositories.Repositories;
using Utilities;
using Utilities.Contants;
using WEB.CMS.Customize;
using WEB.CMS.Services;

namespace WEB.CMS.Controllers
{
    [CustomAuthorize]
    public class ReportController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IReportRepository _reportRepository;
        public ReportController(IReportRepository reportRepository, IConfiguration configuration)
        {

            _configuration = configuration; 
            _reportRepository = reportRepository;
        }
      
        public IActionResult Index()
        {
            return View();
        }
        //tông hợp
        public async Task<IActionResult> TotalVehicleInspection(string fromdate, string todate)
        {
            try
            {
                var date_time_fromdate = fromdate != null && fromdate != "" ? DateUtil.StringToDate(fromdate) : null;
                var date_time_todate = todate != null && todate != "" ? DateUtil.StringToDate(todate) : null;

                var Total = await _reportRepository.CountTotalVehicleInspectionSynthetic(date_time_fromdate, date_time_todate);
                if (Total != null )
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.SUCCESS,
                        data = Total
                    });
                }
                else
                {
                    return Ok(new
                    {
                        status = (int)ResponseType.ERROR,
                        msg = "cập nhật không thành công"
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListCartoFactory - CarController: " + ex);
            }
            return Ok(new
            {
                status = (int)ResponseType.ERROR,
               
            });
        }
        public async Task<IActionResult> GetSummaryVehicleBySite(string fromdate, string todate)
        {
            try
            {
                var date_time_fromdate = fromdate != null && fromdate != "" ? DateUtil.StringToDate(fromdate) : null;
                var date_time_todate = todate != null && todate != "" ? DateUtil.StringToDate(todate) : null;

                var data = await _reportRepository.GetSummaryVehicleBySite(date_time_fromdate, date_time_todate);
                return PartialView(data);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListCartoFactory - CarController: " + ex);
            }
            return PartialView();

        }  
        public async Task<IActionResult> GetDataBieudo(string fromdate, string todate)
        {
            try
            {
                var date_time_fromdate = fromdate != null && fromdate != "" ? DateUtil.StringToDate(fromdate) : null;
                var date_time_todate = todate != null && todate != "" ? DateUtil.StringToDate(todate) : null;

                var data = await _reportRepository.GetSummaryVehicleBySite(date_time_fromdate, date_time_todate);
                var datamodel = new
                {
                    labels = data.Select(s => s.TableName).ToArray(),
                    weightValue=data.Select(s => s.WeightValue).ToArray(),
                    totalWeightValue = data.Select(s => s.TotalTon).ToArray(),
                };
                return Ok(new
                {
                    isSuccess = true,
                    data = datamodel
                });
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ListCartoFactory - CarController: " + ex);
            }
            return PartialView();

        }

    }
}
