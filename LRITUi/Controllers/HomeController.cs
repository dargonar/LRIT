using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataCenterDataAccess;
using System.Data.Linq;
using DataCenterLogic;
using LRITUi;
using LRITUi.Controllers;

namespace LRITUI.Controllers
{
  [Authorize]
  [HandleError]
  public class HomeController : MyController
  {
    public ActionResult Index()
    {
      if (User.IsInRole("ExternoVerificarFacturas"))
        return RedirectToAction("List", "ExternalInvoice");

      if (User.IsInRole("Facturador"))
        return RedirectToAction("List", "Contract");

      // Barcos
      var sda = new ShipDataAccess(context);
      var sman = new ShipManager();
      var spman = new ShipPositionManager();
      var asprda = new ActiveShipPositionRequestDataAccess(context);
      List<Ship> barcos = sda.GetAll();
      string[] latlong = { "N/A", "N/A" };

      int i = 0;
      foreach (Ship barco in barcos)
      {
        ViewData[string.Format("State{0}", i)] = "normal"; //sman.GetShipState(barco);

        DataCenterDataAccess.ShipPosition LastPos = spman.GetLastShipPosition(barco.IMONum);
        if (LastPos == null)
        {
          latlong[0] = "N/A";
          latlong[1] = "N/A";
        }
        else latlong = spman.GetLatLongOfPoint(LastPos);

        ViewData[string.Format("Nombre{0}", i)] = barco.Name.ToString();
        ViewData[string.Format("OMIId{0}", i)] = barco.IMONum.ToString();
        if (LastPos != null)
        {
          string temp = latlong[0].Length > 6 ? latlong[0].Remove(6) : latlong[0];
          string temp2 = latlong[1].Length > 6 ? latlong[1].Remove(6) : latlong[1];
          ViewData[string.Format("UltimaPosicion{0}", i)] = "Lat: " + temp + " Long: " + temp2;
          ViewData[string.Format("Fecha{0}", i)] = LastPos.TimeStamp.ToString();
        }
        else
        {
          ViewData[string.Format("UltimaPosicion{0}", i)] = "Lat: N/A Long: N/A";
          ViewData[string.Format("Fecha{0}", i)] = "N/A";
        }
        i++;
      }
      ViewData["total_barcos"] = i;
      //ViewData["total_barcos"] = 0;


      //Status

      int status, aspStatus;
      TimeSpan since, aspSince;
      var ssman = new SystemStatusManager();

      /*
      ssman.getLastStatus(out status, out since);
      ssman.GetLastAspStatus(out aspStatus, out aspSince);

      ViewData["IDEStatus"] = status;
      ViewData["IDESince"] = since;
      ViewData["DDPStatus"] = status;
      ViewData["DDPSince"] = since;
      ViewData["ASPStatus"] = aspStatus;
      ViewData["ASPSince"] = aspSince;

      ViewData["AsprCount"] = asprda.Count();

      // Tráfico
      
      var tman = new TrafficManager();
      var ts = tman.GetTrafficStats();

      ViewData["InMonth"] = ts.InMonth;
      ViewData["InWeek"] = ts.InWeek;
      ViewData["InDay"] = ts.InDay;
      ViewData["OutMonth"] = ts.OutMonth;
      ViewData["OutWeek"] = ts.OutWeek;
      ViewData["OutDay"] = ts.OutDay;
      */


      return View();
    }


    public ActionResult About()
    {
      return View();
    }


    public ContentResult Perchar(int id)
    {
      ContentResult c = new ContentResult();
      c.Content = "Voy a borrar el id " + id.ToString();
      return c; 
    }


  }
}
