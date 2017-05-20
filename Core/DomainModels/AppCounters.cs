using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModels
{
    public class AppCounters
    {
        public int? RegisteredUsers { get; set; }
        public int? OneMissionPassedUsers { get; set; }
        public int? KindActionsSubmited { get; set; }
        public int? TestPassed { get; set; }
        public int? VkReposts { get; set; }
        public int? FinishedUsers { get; set; }
    }
}
