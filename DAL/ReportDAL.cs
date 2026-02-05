using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Contants;

namespace DAL
{
    public class ReportDAL : GenericService<User>
    {
        private static DbWorker _DbWorker;
        public ReportDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }
        public async Task<TotalVehicleInspection> CountTotalVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {

                    new SqlParameter("@FromDate", FromDate==null? DateTime.Now :FromDate),
                    new SqlParameter("@ToDate", ToDate==null? DateTime.Now :ToDate),
                };
                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_CountTotalVehicleInspectionSynthetic, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<TotalVehicleInspection>();
                    return data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountTotalVehicleInspectionSynthetic - ReportDAL: " + ex);
            }
            return null;
        }
        public async Task<List<SummaryVehicleBySiteViewModel>> GetSummaryVehicleBySite(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                SqlParameter[] objParam = new SqlParameter[]
                {

                    new SqlParameter("@FromDate", FromDate==null? DateTime.Now :FromDate),
                    new SqlParameter("@ToDate", ToDate==null? DateTime.Now :ToDate),
                };
                var dt = _DbWorker.GetDataTable(StoreProcedureConstant.SP_SummaryVehicle_BySite, objParam);
                if (dt != null && dt.Rows.Count > 0)
                {
                    var data = dt.ToList<SummaryVehicleBySiteViewModel>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetSummaryVehicleBySite - ReportDAL: " + ex);
            }
            return null;
        }
    }
}
