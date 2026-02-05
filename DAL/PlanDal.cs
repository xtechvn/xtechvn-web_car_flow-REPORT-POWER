using DAL.Generic;
using DAL.StoreProcedure;
using Entities.Models;
using Entities.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace DAL
{
    public class PlanDAL : GenericService<Month>
    {
        private static DbWorker _DbWorker;

        public PlanDAL(string connection) : base(connection)
        {
            _DbWorker = new DbWorker(connection);
        }

        public async Task<Weight> GetWeightByIdAsync(int weightId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Weights
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == weightId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWeightByIdAsync - PlanDAL: " + ex);
                return null;
            }
        }

        public async Task<long> DeleteWeightAsync(int weightId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = await _DbContext.Weights.FirstOrDefaultAsync(x => x.Id == weightId);
                    if (entity == null) return 0;

                    _DbContext.Weights.Remove(entity);
                    await _DbContext.SaveChangesAsync();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteWeightAsync - PlanDAL: " + ex);
                return 0;
            }
        }

        public async Task<int> CountWeightByMonthIdAsync(int monthId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Weights
                        .AsNoTracking()
                        .CountAsync(x => x.MonthId == monthId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CountWeightByMonthIdAsync - PlanDAL: " + ex);
                return 0;
            }
        }

        public async Task<long> DeleteMonthAsync(int monthId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    var entity = await _DbContext.Months.FirstOrDefaultAsync(x => x.Id == monthId);
                    if (entity == null) return 0;

                    _DbContext.Months.Remove(entity);
                    await _DbContext.SaveChangesAsync();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("DeleteMonthAsync - PlanDAL: " + ex);
                return 0;
            }
        }

        public async Task<Month> GetMonthByIdAsync(int monthId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Months
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == monthId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetMonthByIdAsync - PlanDAL: " + ex);
                return null;
            }
        }

        public async Task<List<Weight>> GetWeightsByMonthIdAsync(int monthId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Weights
                        .AsNoTracking()
                        .Where(x => x.MonthId == monthId)
                        .OrderBy(x => x.Type)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetWeightsByMonthIdAsync - PlanDAL: " + ex);
                return new List<Weight>();
            }
        }


        public async Task<List<PlanGridViewModel>> GetAllPlanAsync(int? year, int? month, int? type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    // 1) Query Month + Weight trước (lọc trong DB)
                    var query =
                        from w in _DbContext.Weights.AsNoTracking()
                        join m in _DbContext.Months.AsNoTracking() on w.MonthId equals m.Id
                        select new
                        {
                            MonthId = m.Id,
                            m.YearValue,
                            m.MonthValue,
                            WeightId = w.Id,
                            w.Type,
                            w.WeightValue
                        };

                    if (year.HasValue) query = query.Where(x => x.YearValue == year.Value);
                    if (month.HasValue) query = query.Where(x => x.MonthValue == month.Value);
                    if (type.HasValue) query = query.Where(x => x.Type == type.Value);

                    var data = await query.ToListAsync();



                    // 2) Load AllCode BRANCH và map ở memory
                    var branches = await _DbContext.AllCodes.AsNoTracking()
                        .Where(x => x.Type == "BRANCH" && x.Status == 1)
                        .ToListAsync();

                    var branchDict = branches
    .GroupBy(x => x.CodeValue.ToString())
    .ToDictionary(g => g.Key, g => g.First().Description);

                


                    // 3) Build ViewModel
                    return data.Select(x => new PlanGridViewModel
                    {
                        MonthId = x.MonthId,
                        YearValue = x.YearValue,
                        MonthValue = x.MonthValue,
                        WeightId = x.WeightId,
                        Type = x.Type,
                        BranchName = branchDict.TryGetValue(x.Type.ToString(), out var name) ? name : $"CN {x.Type}",
                        WeightValue = x.WeightValue
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetAllPlanAsync - PlanDAL: " + ex);
                return new List<PlanGridViewModel>();
            }
        }


        // =========================
        // MONTH
        // =========================

        public async Task<bool> ExistsYearMonthAsync(int year, int month)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Months
                        .AsNoTracking()
                        .AnyAsync(x => x.YearValue == year && x.MonthValue == month);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExistsYearMonthAsync - PlanDAL: " + ex);
                return false;
            }
        }

        public async Task<bool> ExistsYearMonthExceptIdAsync(int year, int month, int exceptId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Months
                        .AsNoTracking()
                        .AnyAsync(x => x.YearValue == year && x.MonthValue == month && x.Id != exceptId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("ExistsYearMonthExceptIdAsync - PlanDAL: " + ex);
                return false;
            }
        }

        public async Task<Month> GetByIdAsync(int monthId)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Months
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == monthId);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByIdAsync(Month) - PlanDAL: " + ex);
                return null;
            }
        }

        public async Task<long> CreateAsync(Month model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Months.Add(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateAsync(Month) - PlanDAL: " + ex);
                return 0;
            }
        }

        public async Task<long> UpdateAsync(Month model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Months.Update(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateAsync(Month) - PlanDAL: " + ex);
                return 0;
            }
        }

        // =========================
        // WEIGHT
        // =========================

        public async Task<Weight> GetByMonthIdAndTypeAsync(int monthId, int type)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    return await _DbContext.Weights
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.MonthId == monthId && x.Type == type);
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("GetByMonthIdAndTypeAsync - PlanDAL: " + ex);
                return null;
            }
        }

        public async Task<long> CreateAsync(Weight model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Weights.Add(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("CreateAsync(Weight) - PlanDAL: " + ex);
                return 0;
            }
        }

        public async Task<long> UpdateAsync(Weight model)
        {
            try
            {
                using (var _DbContext = new EntityDataContext(_connection))
                {
                    _DbContext.Weights.Update(model);
                    await _DbContext.SaveChangesAsync();
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                LogHelper.InsertLogTelegram("UpdateAsync(Weight) - PlanDAL: " + ex);
                return 0;
            }
        }
    }
}
