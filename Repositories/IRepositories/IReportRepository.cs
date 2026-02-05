using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IReportRepository
    {
        Task<TotalVehicleInspection> CountTotalVehicleInspectionSynthetic(DateTime? FromDate, DateTime? ToDate);
        Task<List<SummaryVehicleBySiteViewModel>> GetSummaryVehicleBySite(DateTime? FromDate, DateTime? ToDate);
    }
}
