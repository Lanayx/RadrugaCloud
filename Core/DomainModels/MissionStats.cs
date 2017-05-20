using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModels
{
    public class MissionStats
    {
        public string MissionId { get; set; }

        public int? GivenCount { get; set; }

        public int? GiveUpCount { get; set; }

        public int? DeclinedCount { get; set; }

        public int? ThreeStarsCount { get; set; }

        public int? TwoStarsCount { get; set; }

        public int? OneStarCount { get; set; }
    }
}
