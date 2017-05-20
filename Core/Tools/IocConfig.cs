namespace Core.Tools
{
    using System;

    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    /// <summary>
    ///     Specifies the Unity configuration for the main container.
    /// </summary>
    public class IocConfig
    {
        #region Static Fields

        /// <summary>
        /// The container.
        /// </summary>
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(
            () =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        /// <returns>
        /// The <see cref="IUnityContainer"/>.
        /// </returns>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">
        /// The unity container to configure.
        /// </param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or API controllers (unless you want to
        ///     change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            container.LoadConfiguration();
        }

        #endregion
    }
}
