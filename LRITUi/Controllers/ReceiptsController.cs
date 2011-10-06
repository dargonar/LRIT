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

namespace LRITUi.Controllers
{
    //public class ReceiptDateTimeFix
    //{
    //  public string MessageId   { get; set; }
    //  public string ReferenceId { get; set; }
    //  public int    ReceiptCode { get; set; }
    //  public string Destination { get; set; }
    //  public string Originator  { get; set; }
    //  public string Message     { get; set; }
    //  public string Date        { get; set; }
    //}


    [Authorize]
    public class ReceiptsController : MyController
    {

      
      
      //
      // GET: /Receipt/
      public ActionResult List(int msgInOut)
      {
        DDPVersionManager v = new DDPVersionManager();
        ViewData["msgInOut"] = msgInOut;
        ViewData["LritIDNamePairs"] = ContractingGovermentManager.LritIdNamePairs(v.GetCurrentDDPVersion().Id);
        return View();
      }

      public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord, int msgInOut)
      {
        //var ReceiptDescriptionDic = new Dictionary<int, string>();
        //ReceiptDescriptionDic.Add(0, "No titulado para recibir información");
        //ReceiptDescriptionDic.Add(1, "No hay barcos en en área SARSURPIC");
        //ReceiptDescriptionDic.Add(2, "IDE no disponible");
        //ReceiptDescriptionDic.Add(3, "DC no disponible");
        //ReceiptDescriptionDic.Add(4, "PSC no disponible");
        //ReceiptDescriptionDic.Add(5, "El barco no responde");
        //ReceiptDescriptionDic.Add(6, "El barco no está disponible");
        //ReceiptDescriptionDic.Add(7, "Falla en el sistema");
        //ReceiptDescriptionDic.Add(8, "No se pudo cargar DDP");
        //ReceiptDescriptionDic.Add(9, "Version de DDP incorrecta, mensaje descartado");

        string[] ReqParams = { "MessageId", "ReferenceId", "ReceiptCode","ReceiptDescription", "Destination", "Originator", "Message", "TimeStamp" };
        List<string> columns = new List<string>();
        List<string> querys = new List<string>();

        string tstamp = "-";
        var fromDate = new DateTime(2000, 1, 1);
        var toDate = new DateTime(2200, 1, 1);


        for (int i = 0; i < ReqParams.Count(); i++)
			  {


          if (ReqParams[i].Contains("TimeStamp"))
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


        var rda = new ReceiptDataAccess(context);
        var receipts = rda.GetAllBetween(msgInOut, fromDate, toDate);

        var model = from entity in receipts.OrderBy(sidx + " " + sord)
                    select new 
                    {
                      MessageId   = entity.MessageId,
                      ReferenceId = entity.ReferenceId,
                      ReceiptCode = entity.ReceiptCode,
                      Destination = entity.Destination,
                      Originator  = entity.Originator,
                      Message     = entity.Message,
                      TimeStamp   = entity.TimeStamp.ToString(),
                    };
        return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()));
      }
    }
}
