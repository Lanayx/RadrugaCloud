namespace RadrugaCloud.Models.Api
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Class VkReference
    /// </summary>
    public class VkReference
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember(Name = "id")]
        public uint Id { get; set; }

        #endregion
    }
}