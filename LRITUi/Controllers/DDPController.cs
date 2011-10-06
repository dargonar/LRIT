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
  public class DDPController : MyController
  {
    //
    // GET: /Receipt/
    public ActionResult List(int? ddpid)
    {
      var ddpda = new DDPVersionDataAccess(context);

      DDPVersion ddpVersion;
      if (ddpid == null)
        ddpVersion = ddpda.TodaysDDP();
      else
        ddpVersion = ddpda.GetVersionById((int)ddpid);

      ViewData["version"] = ddpVersion.regularVer + ":" + ddpVersion.inmediateVer;
      ViewData["ddpId"]   = ddpid;

      return View();
    }

    public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord, int? ddpid)
    {
      string[] ReqParams = { "Name", "DataCenterId", "LRITId" , "PlaceStringId", "PlaceName", "AreaType", "PlaceId"};
      List<string> columns = new List<string>();
      List<string> querys = new List<string>();


      //Vectores Apareados SearchQuery, Columns
      for (int i = 0; i < ReqParams.Count(); i++)
      {
        var tempValue = Request.Params[ReqParams[i]];
        if (tempValue != null)
        {
          columns.Add(ReqParams[i]);
          querys.Add(tempValue);
        }
      }

      var ddpda = new DDPVersionDataAccess(context);
      var ddp = new DDPVersion();
      if (ddpid == null)
        ddpid = ddpda.TodaysDDP().Id;

      var ddpdata = ddpda.GetAllFromVersion((int)ddpid);

      var model = from entity in ddpdata.OrderBy(sidx + " " + sord)
                  select new
                  {
                    Name = entity.Name,
                    DataCenterId = entity.DataCenterId,
                    LRITId = entity.LRITId,
                    PlaceStringId = entity.PlaceStringId,
                    PlaceName = entity.PlaceName,
                    AreaType = entity.AreaType,
                    PlaceId = entity.PlaceId.ToString(),
                  };
      return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()));
    }

  }
}
