using System;

namespace SCED.Models
{
    public class SettlementRecord
    {
        public DateTime PeriodEnd { get; set; }
        public TimeSpan Period { get; set; }
        public double PowerMW { get; set; }
        public string UtilityName { get; set; }
        public string EnergySource { get; set; }
    }
}