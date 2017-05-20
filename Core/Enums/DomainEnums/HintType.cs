namespace Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Hint types
    /// </summary>
    public enum HintType
    {

        /// <summary>
        /// The text
        /// </summary>
        [Display(Name = "Текст")]
        Text = 0,

        /// <summary>
        /// The direction (for missions around the base)
        /// </summary>
        [Display(Name = "Направление")]
        Direction = 1,

        /// <summary>
        /// The exact coordinate
        /// </summary>
        [Display(Name = "Координата")]
        Coordinate = 2
    }
}
