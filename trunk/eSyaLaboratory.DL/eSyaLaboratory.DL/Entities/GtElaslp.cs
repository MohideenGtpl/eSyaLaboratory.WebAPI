using System;
using System.Collections.Generic;

namespace eSyaLaboratory.DL.Entities
{
    public partial class GtElaslp
    {
        public int BusinessKey { get; set; }
        public int ServiceId { get; set; }
        public int ProfileServiceId { get; set; }
        public int ReportingSequence { get; set; }
        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedTerminal { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedTerminal { get; set; }
    }
}
