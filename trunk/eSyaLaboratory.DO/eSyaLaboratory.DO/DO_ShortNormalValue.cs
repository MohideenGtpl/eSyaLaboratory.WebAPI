using System;
using System.Collections.Generic;
using System.Text;

namespace eSyaLaboratory.DO
{
    public class DO_ShortNormalValue
    {
        public int BusinessKey { get; set; }
        public int ServiceId { get; set; }
        public int SerialNumber { get; set; }
        public string TestParameter { get; set; }
        public string Sex { get; set; }
        public int StartAge { get; set; }
        public string StartAgeType { get; set; }
        public int EndAge { get; set; }
        public string EndAgeType { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string NormalValues { get; set; }
        public decimal? HypoValue { get; set; }
        public decimal? HyperValue { get; set; }
        public decimal? Desirable { get; set; }
        public decimal? BorderLine { get; set; }
        public decimal? HighRisk { get; set; }

        public bool ActiveStatus { get; set; }
        public string FormId { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string TerminalID { get; set; }
    }
}
