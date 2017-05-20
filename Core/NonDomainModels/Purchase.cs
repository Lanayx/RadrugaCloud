namespace Core.NonDomainModels
{
    using Enums;

    /// <summary>
    /// Class for carring purchase info from client to server
    /// </summary>
    public class Purchase
    {
        /// <summary>
        /// Gets or sets the product identifier in the specified store.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductId { get; set; }
        /// <summary>
        /// Gets or sets the receipt data.
        /// </summary>
        /// <value>
        /// The receipt data.
        /// </value>
        public string ReceiptData { get; set; }
        /// <summary>
        /// Gets or sets the type of the store.
        /// </summary>
        /// <value>
        /// The type of the store.
        /// </value>
        public StoreType StoreType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this purchase is in test mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is test; otherwise, <c>false</c>.
        /// </value>
        public bool IsTest { get; set; }
        /// <summary>
        /// Gets or sets the signature (Android only)
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string Signature { get; set; }
    }
}
