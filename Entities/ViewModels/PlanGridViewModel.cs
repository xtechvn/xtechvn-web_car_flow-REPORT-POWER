using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class PlanGridViewModel
    {
        public int MonthId { get; set; }
        public int YearValue { get; set; }
        public int MonthValue { get; set; }

        public int Type { get; set; }
        public string BranchName { get; set; }

        public int WeightId { get; set; }
        public decimal WeightValue { get; set; }
    }
}

