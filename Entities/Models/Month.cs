using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Month
    {
        public Month()
        {
            Weights = new HashSet<Weight>();
        }

        public int Id { get; set; }
        public int YearValue { get; set; }
        public int MonthValue { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<Weight> Weights { get; set; }
    }
}
