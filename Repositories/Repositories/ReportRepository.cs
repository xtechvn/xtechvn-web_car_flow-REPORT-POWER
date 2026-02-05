using DAL;
using Entities.ConfigModels;
using Entities.ViewModels;
using Microsoft.Extensions.Options;
using Repositories.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Repositories.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ReportDAL _ReportDAL;
      
        public ReportRepository(IOptions<DataBaseConfig> dataBaseConfig)
        {
            _ReportDAL = new ReportDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
          
        }
        public async Task<TotalVehicleInspection> CountTotalVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                return await _ReportDAL.CountTotalVehicleInspectionSynthetic(FromDate, ToDate);
            }
            catch(Exception ex)
            {
                LogHelper.InsertLogTelegram("CountTotalVehicleInspectionSynthetic - ReportRepository: " + ex);

            }
            return null;
        }
        public async Task<List<SummaryVehicleBySiteViewModel>> GetSummaryVehicleBySite(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                return await _ReportDAL.GetSummaryVehicleBySite(FromDate, ToDate);
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSummaryVehicleBySite - ReportRepository: " + ex);

            }
            return null;
        }
    }
}
