namespace Core.DomainModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class Question
    /// </summary>
    public class Question
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Question name (short description)
        /// </summary>
        /// <value>The name.</value>
        [Required]
        [DisplayName("Название")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Question string
        /// </summary>
        [Required]
        [DisplayName("Текст вопроса")]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets List of options
        /// </summary>
        public List<QuestionOption> Options { get; set; }
    }
}
