using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.CommonModels.Results
{
    using Core.Enums;

    /// <summary>
    /// The hint request result
    /// </summary>
    public class HintRequestResult : OperationResult
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HintRequestResult"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="requestStatus">
        /// The status of hint request.
        /// </param>
        /// <param name="hint">
        /// The hint.
        /// </param>
        public HintRequestResult(OperationResultStatus status, HintRequestStatus requestStatus, string hint) : base(status)
        {            
            Hint = hint;
            RequestStatus = requestStatus;
        }

        #endregion

        #region Public Properties
               
        /// <summary>
        /// Gets or sets content of the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string Hint { get; set; }
        public HintRequestStatus RequestStatus { get; set; }

        #endregion
    }
}
