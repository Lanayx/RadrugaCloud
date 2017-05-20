using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadrugaCloud.Models.Api
{
    /// <summary>
    /// Class for liking/disliking kind actions
    /// </summary>
    public class KindActionIds
    {
        /// <summary>
        /// Gets or sets the kind action identifier.
        /// </summary>
        /// <value>
        /// The kind action identifier.
        /// </value>
        public string KindActionId { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public string UserId { get; set; }
    }
}