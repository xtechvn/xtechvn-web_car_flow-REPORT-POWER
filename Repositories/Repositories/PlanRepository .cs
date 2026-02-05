using DAL;
using Entities.ConfigModels;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
    public class PlanRepository : IPlanRepository
    {
        private readonly PlanDAL _planDAL;

        public PlanRepository(IHttpContextAccessor context, IOptions<DataBaseConfig> dataBaseConfig,  IConfiguration configuration) 
        {
            _planDAL = new PlanDAL(dataBaseConfig.Value.SqlServer.ConnectionString);
        }
        public async Task<IEnumerable<PlanGridViewModel>> GetAll(int? year, int? month, int? type)
        {
            try
            {
                return await _planDAL.GetAllPlanAsync(year, month, type);
            }
            catch { throw; }
        }
        public async Task<long> DeleteWeight(int weightId)
        {
            try
            {
                // 1) lấy weight để biết MonthId
                var w = await _planDAL.GetWeightByIdAsync(weightId);
                if (w == null) return -1;

                // 2) xóa weight
                var delW = await _planDAL.DeleteWeightAsync(weightId);
                if (delW <= 0) return 0;

                // 3) nếu tháng không còn weight nào -> xóa luôn Month
                var count = await _planDAL.CountWeightByMonthIdAsync(w.MonthId);
                if (count == 0)
                {
                    await _planDAL.DeleteMonthAsync(w.MonthId);
                }

                return 1;
            }
            catch { throw; }
        }

        public async Task<PlanUpsertViewModel> GetForEdit(int monthId)
        {
            try
            {
                var month = await _planDAL.GetMonthByIdAsync(monthId);
                if (month == null) return null;

                var weights = await _planDAL.GetWeightsByMonthIdAsync(monthId);

                var model = new PlanUpsertViewModel
                {
                    MonthId = month.Id,
                    YearValue = month.YearValue,
                    MonthValue = month.MonthValue,
                    Items = weights.Select(w => new PlanItemVM
                    {
                        Type = w.Type,
                        WeightValue = w.WeightValue
                    }).ToList()
                };

                return model;
            }
            catch { throw; }
        }


        public async Task<long> CreatePlan(PlanUpsertViewModel model, string? username)
        {
            try
            {
                // ✅ validate trùng tháng khi create
                var existed = await _planDAL.ExistsYearMonthAsync(model.YearValue, model.MonthValue);
                if (existed) return -3;

                var month = new Month
                {
                    YearValue = model.YearValue,
                    MonthValue = model.MonthValue,
                    CreatedDate = DateTime.Now,
                    CreatedBy = username
                };

                var monthId = await _planDAL.CreateAsync(month);
                if (monthId <= 0) return 0;

                foreach (var item in model.Items)
                {
                    if (item.Type <= 0) continue;

                    await _planDAL.CreateAsync(new Weight
                    {
                        MonthId = (int)monthId,
                        Type = item.Type,
                        WeightValue = item.WeightValue
                    });
                }

                return monthId;
            }
            catch { throw; }
        }

        public async Task<long> UpdatePlan(PlanUpsertViewModel model, string? username)
        {
            try
            {
                var month = await _planDAL.GetByIdAsync(model.MonthId);
                if (month == null) return -1;

                // Nếu cho phép đổi Year/Month khi update => phải validate trùng với tháng khác
                if (month.YearValue != model.YearValue || month.MonthValue != model.MonthValue)
                {
                    var existed = await _planDAL.ExistsYearMonthExceptIdAsync(model.YearValue, model.MonthValue, model.MonthId);
                    if (existed) return -3; // đổi sang tháng đã có
                }

                month.YearValue = model.YearValue;
                month.MonthValue = model.MonthValue;
                month.UpdatedDate = DateTime.Now;
                month.UpdatedBy = username;

                await _planDAL.UpdateAsync(month);

                foreach (var item in model.Items)
                {
                    if (item.Type <= 0) continue;

                    var w = await _planDAL.GetByMonthIdAndTypeAsync(model.MonthId, item.Type);
                    if (w == null)
                    {
                        await _planDAL.CreateAsync(new Weight
                        {
                            MonthId = model.MonthId,
                            Type = item.Type,
                            WeightValue = item.WeightValue
                        });
                    }
                    else
                    {
                        w.WeightValue = item.WeightValue;
                        await _planDAL.UpdateAsync(w);
                    }
                }

                return model.MonthId;
            }
            catch { throw; }
        }

    }
}
