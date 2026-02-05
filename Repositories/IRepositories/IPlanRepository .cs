using Entities.Models;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IRepositories
{
    public interface IPlanRepository
    {
        Task<long> CreatePlan(PlanUpsertViewModel model, string? username);
        Task<long> UpdatePlan(PlanUpsertViewModel model, string? username);
        Task<IEnumerable<PlanGridViewModel>> GetAll(int? year, int? month, int? type);
        Task<PlanUpsertViewModel> GetForEdit(int monthId);
        Task<long> DeleteWeight(int weightId);




    }
}
