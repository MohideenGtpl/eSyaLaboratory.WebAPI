using System;
using System.Collections.Generic;

namespace eSyaLaboratory.DL.Entities
{
    public partial class GtElasan
    {
        public int BusinessKey { get; set; }
        public int ServiceId { get; set; }
        public int SerialNumber { get; set; }
        public string Heading { get; set; }
        public string TestParameterDesc { get; set; }
        public int? Unit { get; set; }
        public string NormalValues { get; set; }
        public int? TestMethod { get; set; }
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
