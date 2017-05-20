namespace RadrugaCloud.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin.Security;

    using RadrugaCloud.Models;

    /// <summary>
    /// Class AccountController
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Task{ActionResult}.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var isAuthenticated = model.Login == model.Password + "!" + model.Password;
                if (isAuthenticated)
                {
                    await SignInAsync(model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Недопустимое имя пользователя или пароль.");
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var identity =
                new ClaimsIdentity(
                    new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "Admin"),
                            new Claim(
                                "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                                "Custom")
                        },
                    DefaultAuthenticationTypes.ApplicationCookie);

            await
                Task.Factory.StartNew(
                    () =>
                    AuthenticationManager.SignIn(
                        new AuthenticationProperties
                            {
                                IsPersistent = isPersistent
                            },
                        new[] { identity }));
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}