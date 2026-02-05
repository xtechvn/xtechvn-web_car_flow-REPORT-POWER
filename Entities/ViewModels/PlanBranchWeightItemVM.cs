using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.ViewModels
{
    public class PlanItemVM
    {
        [Required]
        public int Type { get; set; } // Chi nhánh (Allcode)

        [Required]
        [Range(0, 999999999)]
        public decimal WeightValue { get; set; }
    }

    public class PlanUpsertViewModel
    {
        public int MonthId { get; set; } // nếu bạn cần, còn không thì bỏ

        [Required]
        public int YearValue { get; set; }

        [Required]
        [Range(1, 12)]
        public int MonthValue { get; set; }

        // nhiều chi nhánh + trọng lượng
        public List<PlanItemVM> Items { get; set; } = new();
    }
}
