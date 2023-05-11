using System;
using System.Collections.Generic;
using System.Text;

namespace eSyaLaboratory.DO
{
    public class DO_ServiceTemplateFull
    {
        //Common
        public DO_ServiceCommonValues CommonValues { get; set; }
        //Short
        public DO_ShortValueHeader ShortHeader { get; set; }
        public List<DO_ShortNormalValue> l_ShortValues { get; set; }
        public List<DO_TestMethod> l_TestMethod { get; set; }
        //Long
        public List<DO_LongValue> l_LongValues { get; set; }
        //Analysis
        public List<DO_AnalysisValue> l_AnalysisValues { get; set; }
        //Descriptive
        public DO_DescriptiveResult DescriptiveResult { get; set; }

    }
}
