using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewAPI.DTOs
{
    public class TradeDTO
    {
        public int ID { get; set; }
        public string TradeName { get; set; }
        public string TradeLevel { get; set; }
        public string Languages { get; set; }
        public DateTime ActiveDate { get; set; }
        public string SyllabusName { get; set; }
        public string SyllabusFilePath { get; set; }
        public string TestPlanFilePath { get; set; }
        public string DevelopmentOfficer { get; set; }
        public string SyllabusFile { get; set; }
        public string TestPlanFile { get; set; }
        public string Manager { get; set; }

    }
}