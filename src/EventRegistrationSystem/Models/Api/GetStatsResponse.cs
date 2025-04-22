using System.Collections.Generic;

namespace EventRegistrationSystem.Models.Api
{
    public class StatItem
    {
        public string StatLabel { get; set; }
        public int StatValue { get; set; }
    }
    
    public class GetStatsResponse
    {
        public IEnumerable<StatItem> Stats { get; set; }
    }
}
