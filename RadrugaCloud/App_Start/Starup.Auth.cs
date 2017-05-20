namespace RadrugaCloud
{
    using System;

    using Core.Tools;

    using Microsoft.AspNet.Identity;
    using Microsoft.Owin;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OAuth;
    using Microsoft.Practices.Unity;

    using Owin;

    /// <summary>
    /// The startup.
    /// </summary>
    public partial class Startup
    {
        static Startup()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        }

        /// <summary>
        /// Gets the O auth bearer options.
        /// </summary>
        /// <value>The O auth bearer options.</value>
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        // Дополнительные сведения о настройке проверки подлинности см. по адресу: http://go.microsoft.com/fwlink/?LinkId=301864
        #region Public Methods and Operators

        /// <summary>
        /// The configure auth.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                    {
                        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                        LoginPath = new PathString("/Account/Login"),
                        Provider = new CookieAuthenticationProvider(),
                        ExpireTimeSpan = TimeSpan.FromHours(48),
                });

            app.UseOAuthAuthorizationServer(
                new OAuthAuthorizationServerOptions
                    {
                        AllowInsecureHttp = true,
                        TokenEndpointPath = new PathString("/api/token"),
                        AccessTokenExpireTimeSpan = TimeSpan.FromHours(24),
                        Provider =
                            IocConfig.GetConfiguredContainer()
                            .Resolve<IOAuthAuthorizationServerProvider>()
                    });
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }

        #endregion
    }
}