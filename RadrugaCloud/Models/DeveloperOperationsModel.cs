namespace RadrugaCloud.Models
{
    /// <summary>
    /// The test azure storage model.
    /// </summary>
    public class DeveloperOperationsModel
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is error.
        /// </summary>
        /// <value><c>true</c> if this instance is error; otherwise, <c>false</c>.</value>
        public bool IsError { get; set; }

        /// <summary>
        ///     Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public string Result { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the temp images count.
        /// </summary>
        /// <value>The temp images count.</value>
        public int TempImagesCount { get; set; }

        #endregion
    }
}