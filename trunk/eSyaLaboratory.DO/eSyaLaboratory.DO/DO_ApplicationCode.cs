﻿using System;
using System.Collections.Generic;
using System.Text;

namespace eSyaLaboratory.DO
{
    public class DO_ApplicationCode
    {
        public int ApplicationCode { get; set; }
        public int CodeType { get; set; }
        public string CodeDesc { get; set; }
        public bool DefaultStatus { get; set; }
        public bool UsageStatus { get; set; }
        public bool ActiveStatus { get; set; }
    }
}
