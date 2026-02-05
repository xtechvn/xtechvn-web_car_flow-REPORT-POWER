using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Weight
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public decimal WeightValue { get; set; }
        public int Type { get; set; } //Chi nhánh

        public virtual Month Month { get; set; }
    }
}
