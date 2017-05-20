namespace RadrugaCloud
{
    using System.Web.Http;
    using System.Web.Http.ExceptionHandling;

    using Core.Tools;

    using Microsoft.Owin.Security.OAuth;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// The web api config.
    /// </summary>
    public static class WebApiConfig
    {
        #region Public Methods and Operators

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public static void Register(HttpConfiguration config)
        {
            // Конфигурация и службы Web API
            // Настройка Web API для использования только проверки подлинности посредством маркера-носителя.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Маршруты веб-API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}/{id}",
                new { action = "Get", id = RouteParameter.Optional });

            // There can be multiple exception loggers. (By default, no exception loggers are registered.)
            config.Services.Add(typeof(IExceptionLogger), IocConfig.GetConfiguredContainer().Resolve<IExceptionLogger>());

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new RuDateTimeConverter());

        }

        class RuDateTimeConverter : Newtonsoft.Json.Converters.IsoDateTimeConverter
        {
            public RuDateTimeConverter()
            {
                DateTimeFormat = "dd.MM.yyyy";
            }
        }

        #endregion
    }
}