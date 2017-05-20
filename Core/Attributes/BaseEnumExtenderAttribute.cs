namespace Core.Attributes
{
    using System;

    /// <summary>
    /// Class BaseEnumExtenderAttribute
    /// </summary>
    public abstract class BaseEnumExtenderAttribute : Attribute
    {
        #region Fields

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEnumExtenderAttribute"/> class. 
        /// </summary>
        /// <param name="text">
        /// Sets text.
        /// </param>
        protected BaseEnumExtenderAttribute(string text)
        {
            Text = text;
        }

        #endregion
    }
}