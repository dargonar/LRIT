using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Configuration;
using DataCenterDataAccess;

namespace LRITUI
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



      routes.MapRoute(
        "Audit",                                      // Route name
        "Audit/{action}/{time}",                   // URL with parameters
        new { controller = "Audit", action = "DayStat", time = "ShipAsp" }
      );

      routes.MapRoute(
        "Ship",                                              // Route name
        "Ship/{action}/{id}",                           // URL with parameters
        new {controller = "Ship", action = "Edit", id = (string)null }  // Parameter defaults
      );

      routes.MapRoute(
        "DDPList",                                      // Route name
        "DDP/{action}/{ddpid}",                   // URL with parameters
        new { controller = "DDP", action = "List", ddpid = (string)null }
      );
      
      
      routes.MapRoute(
        "Reports",                                      // Route name
        "Reports/{action}/{msgInOut}/{unreferenced}",                   // URL with parameters
        new { controller = "Reports", action = "List", msgInOut = 0, unreferenced = "false" }
      );

      routes.MapRoute(
        "Receipts",                                      // Route name
        "Receipts/{action}/{msgInOut}",                   // URL with parameters
        new { controller = "Receipts", action = "List", msgInOut = 0 }
      );

      routes.MapRoute(
        "Pricing",                                      // Route name
        "Pricing/{action}/{msgInOut}",                   // URL with parameters
        new { controller = "Pricing", action = "List", msgInOut = 0 }
      );

      routes.MapRoute(
        "Requests",                                      // Route name
        "Requests/{action}/{msgInOut}/{refid}",                   // URL with parameters
        new { controller = "Requests", action = "List", msgInOut = 0 , refid = (string)null }
      );

      routes.MapRoute(
        "NewSarRequest",                                      // Route name
        "Requests/{action}",                   // URL with parameters
        new { controller = "Requests", action = "NewRectangularSarsurpic" }
      );

      routes.MapRoute(
        "NewRequest",                                      // Route name
        "NewRequest/{accessType}/{requestType}",                   // URL with parameters
        new { controller = "Requests", action = "New", requestType = 0, accessType = 0 }
      );

      routes.MapRoute(
        "Account",                                      // Route name
        "Account/{action}/{id}",                   // URL with parameters
        new { controller = "Account", action = "List", id = "" }
      );

      //Contratos
      routes.MapRoute(
        "Contract",                                      // Route name
        "Contract/{action}",                             // URL with parameters
        new { controller = "Contract", action = "List" }
      );
      
      //Facturas
      routes.MapRoute(
        "Invoce",                                      // Route name
        "Invoce/{action}",                             // URL with parameters
        new { controller = "Invoce", action = "List" }
      );
      
      routes.MapRoute(
        "Default",                                              // Route name
        "{controller}/{action}",                           // URL with parameters
        new { controller = "Home", action = "Index" }  // Parameter defaults
      );

    }

    protected void Application_Start()
    {
      DataCenterDataAccess.Config.ConnectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
      RegisterRoutes(RouteTable.Routes);
      
    }
  }
}