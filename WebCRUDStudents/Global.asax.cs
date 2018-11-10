using DAL;
using System;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebCRUDStudents
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            DBType typeOfDB = DBType.MySQL;
            var stringTypeOfDB = WebConfigurationManager.AppSettings["DBType"];
            if (Enum.TryParse(stringTypeOfDB, out typeOfDB)) {
                DBConnection.Type = typeOfDB;
                DBConnection.ConnectionString = WebConfigurationManager.ConnectionStrings[stringTypeOfDB].ConnectionString;
            }

            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.Indented;

            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
                (new Newtonsoft.Json.Converters.StringEnumConverter());
        }
    }
}
