using Microsoft.Owin;
using Owin;
using RadrugaCloud;

[assembly: OwinStartup(typeof(Startup))]

namespace RadrugaCloud
{
    /// <summary>
    /// Class Startup
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configurations the specified app.
        /// </summary>
        /// <param name="app">The app.</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
