using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModels
{
    using Core.Enums;

    /// <summary>
    /// Hit request class
    /// </summary>
    public class HintRequest
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the mission identifier.
        /// </summary>
        /// <value>
        /// The mission identifier.
        /// </value>
        public string MissionId { get; set; }
        /// <summary>
        /// Gets or sets the hint identifier.
        /// </summary>
        /// <value>
        /// The hint identifier.
        /// </value>
        public string HintId { get; set; }
        /// <summary>
        /// Gets or sets the hit request status.
        /// </summary>
        /// <value>
        /// The hit request status.
        /// </value>
        public HintRequestStatus Status { get; set; }        
    }
}
