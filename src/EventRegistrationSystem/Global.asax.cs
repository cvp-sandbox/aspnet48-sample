using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EventRegistrationSystem.Utils;
using System.Linq;

namespace EventRegistrationSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LoadEnvSettings();

            AreaRegistration.RegisterAllAreas();

            // Register dependencies FIRST
            DependencyConfig.RegisterDependencies();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void LoadEnvSettings()
        {
            var envFilePath = Server.MapPath("~/.env");

            var envVariables = DotEnvReader.Load(envFilePath);

            foreach (var envVariable in envVariables)
            {
                var key = envVariable.Key;
                var value = envVariable.Value;

                if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                {
                    ConfigurationManager.AppSettings[key] = value;

                }
                else
                {
                    ConfigurationManager.AppSettings.Add(key, value);
                }
            }
        }

    }
}
