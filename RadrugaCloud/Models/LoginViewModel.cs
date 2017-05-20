namespace RadrugaCloud.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class LoginViewModel
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>The login.</value>
        [Required]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [remember me].
        /// </summary>
        /// <value><c>true</c> if [remember me]; otherwise, <c>false</c>.</value>
        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}