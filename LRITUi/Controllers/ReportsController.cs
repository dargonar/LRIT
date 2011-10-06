using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Data.Linq;
using DataCenterLogic;
using DataCenterDataAccess;
using System.Linq.Dynamic;
using GridDemo.Models;
using System.Threading;
using System.Globalization;
using LRITUi.Controllers;

namespace LRITUI.Controllers
{
    [Authorize]
    public class ReportsController : MyController
    {
      //
      // GET: /Receipt/
      public ActionResult Sent(string refid)
      {
        DDPVersionManager v = new DDPVersionManager();

        if (refid != null)
          ViewData["referenceId"] = refid;
        ViewData["msgInOut"] = 1;
        ViewData["LritIDNamePairs"] = ContractingGovermentManager.LritIdNamePairs(v.GetCurrentDDPVersion().Id);
        return View("List");
      }

      public ActionResult Received(string refid)
      {
        DDPVersionManager v = new DDPVersionManager();

        if (refid != null)
          ViewData["referenceId"] = refid;
        ViewData["msgInOut"] = 0;
        ViewData["LritIDNamePairs"] = ContractingGovermentManager.LritIdNamePairs(v.GetCurrentDDPVersion().Id);       
        return View("List");
      }

      public ActionResult ReceivedFromSO(string refid)
      {
        DDPVersionManager v = new DDPVersionManager();

        if (refid != null)
          ViewData["referenceId"] = refid;
        ViewData["LritIDNamePairs"] = ContractingGovermentManager.LritIdNamePairs(v.GetCurrentDDPVersion().Id);
        ViewData["msgInOut"]    = 0;
        ViewData["referenceId"] = 0;
        return View("List");
      }

      public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord, int msgInOut)
      {

          string[] ReqParams = { "IMONum" , "ShipName", "DataUserRequestor", "DataUserProvider", "Latitude", "Longitude", "ResponseType", "ReferenceId" , "TimeStamp1", "MessageId" };
          List<string> columns = new List<string>();
          List<string> querys = new List<string>();

          string tstamp = "-";
          var fromDate = new DateTime(2000, 1, 1);
          var toDate = new DateTime(2200, 1, 1);

          //Vectores Apareados SearchQuery, Columns
          for (int i = 0; i < ReqParams.Count(); i++)
          {
            
            
            if (ReqParams[i].Contains("TimeStamp1"))
            {
              tstamp = Request.Params[ReqParams[i]];
              if (tstamp != null)
              {
                var dates = tstamp.Split('-');
                fromDate = DateTime.Parse(dates[0]);
                if (dates.Length == 1)
                  toDate = fromDate.AddDays(1);
                else
                  toDate = DateTime.Parse(dates[1]);
              }
              continue;
            }


            var tempValue = Request.Params[ReqParams[i]];
            if (tempValue != null)
            {
              columns.Add(ReqParams[i]);
              querys.Add(tempValue);
            }
          }
          
          
          
          var sprda = new ShipPositionReportDataAccess(context);
          var reports = sprda.GetAllBetween(msgInOut, fromDate, toDate);

          var model = from entity in reports.OrderBy(sidx + " " + sord)
                      select new
                      {
                        IMONum = entity.IMONum,
                        ShipName = entity.ShipName,
                        DataUserRequestor = entity.DataUserRequestor,
                        DataUserProvider = entity.DataUserProvider,
                        Latitude = entity.Latitude,
                        Longitude = entity.Longitude,
                        ResponseType = entity.ResponseType,
                        ReferenceId = entity.ReferenceId,
                        MessageId = entity.MessageId,
                        TimeStamp1 = entity.TimeStamp1.ToString(),
                      };

          

          return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()));
        }

    }
  
}
