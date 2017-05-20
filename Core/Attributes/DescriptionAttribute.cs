namespace Core.Attributes
{
    /// <summary>
    /// Class DescriptionAttribute
    /// </summary>
    public class DescriptionAttribute : BaseEnumExtenderAttribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionAttribute"/> class.
        /// </summary>
        /// <param name="text">
        /// Sets text.
        /// </param>
        public DescriptionAttribute(string text)
            : base(text)
        {
        }

        #endregion
    }
}