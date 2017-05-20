using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AzureTablesObjects.TableEntities
{
    using Core.Enums;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The hint request azure
    /// </summary>    
    public class HintRequestAzure : TableEntity
    {       
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HintRequestAzure"/> class.
        /// </summary>
        public HintRequestAzure()
        {
            PartitionKey = Guid.NewGuid().ToString("N");
            RowKey = AzureTableConstants.HintRequestRowKey;            
        }

        #endregion

        #region Public Properties

        // Common

        /// <summary>
        /// The indentifer of user who doing request
        /// </summary>
        public string UserId{ get; set; }

        /// <summary>
        /// The hint mission identifier.
        /// </summary>        
        public string MissionId { get; set; }


        /// <summary>
        /// The hint identifier.
        /// </summary>
        public string HintId { get; set; }


        /// <summary>
        /// The request result status.
        /// </summary>        
        public int Status { get; set; }

        #endregion
    }
}
