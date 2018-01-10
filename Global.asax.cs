using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApiHash.Context;

namespace WebApiHash
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["HashContext"].ConnectionString;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<HashContext>(null);
            SqlDependency.Start(connString);
        }
        protected void Application_End()
        {
            //Stop SQL dependency
            SqlDependency.Stop(connString);
        }
    }
}
