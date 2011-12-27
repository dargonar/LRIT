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
using System.Messaging;

namespace LRITUi.Controllers
{
  [Authorize(Roles = "Administrador, Facturador")]
    public class PricingController : MyController
    {
      public ActionResult Prices()
      {
        Price price = new Price();
        Price newprice = new Price();
        
        price.Poll = -1;
        price.PositionReport = -1;
        price.PeriodicRateChange = -1;
        price.ArchivePositionReport = -1;
        price.currency = "N/A";
        var userPrice = context.PriceUserProviders.Where( pu => pu.dataProviderID == "1005" ).SingleOrDefault();
        if (userPrice != null)
        {
          price = userPrice.Price;
          newprice = price;
        }

        newprice.effectiveDate = DateTime.UtcNow;

        ViewData["price"] = price;
        ViewData["title"] = "Precios actuales de mensajes";

        var pud = context.PricingUpdates.OrderByDescending(pu => pu.TimeStamp).Take(1).SingleOrDefault();
        if( pud != null )
          ViewData["last_update"] = pud.TimeStamp;

        return View(newprice);
      }

      public ActionResult Change(Price price)
      {
        ViewData["title"] = "Precios";
        try
        {
          var pm     = new PricingManager();
          var ddpMan = new DDPVersionManager();
          var cfgMan = new ConfigurationManager();
          var queMan = new QueueManager();

          var pud = new DataCenterLogic.DataCenterTypesIDE.PricingUpdateType();
          pud.DDPVersionNum = DDPVersionManager.currentDDP();
          pud.Message = "Price changed";
          pud.MessageId = MessageIdManager.Generate("1005");
          pud.MessageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType7.Item15;
          pud.PricingFile = pm.GeneratePriceFile(price);
          pud.ReferenceId = "";
          pud.schemaVersion = decimal.Parse(cfgMan.Configuration.SchemaVersion);
          pud.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
          pud.TimeStamp = DateTime.UtcNow;

          Message msgout = new Message(pud);
          msgout.Label = "pricingUpdate";

          string outQueue = System.Configuration.ConfigurationManager.AppSettings["CoreOutQueue"];
          QueueManager.Instance().SetOut(outQueue);
          QueueManager.Instance().EnqueueOut(msgout);

          FlashOK("El mensaje fue encolado con exito");
        }
        catch (Exception ex)
        {
          FlashOK(string.Format("Hubo un problema al enviar el mensaje => {0}",ex.Message));
        }
        
        return View();
      }
      
      public ActionResult List(int msgInOut)
      {
        ViewData["msgInOut"] = msgInOut;
        return View();
      }

      public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord, int msgInOut)
      {

        string[] ReqParams = { "MessageId", "Message", "DDPVersion", "TimeStamp" };
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

        var rda = new PricingNotificationDataAccess(context);
        var receipts = rda.GetAllBetween(msgInOut, fromDate, toDate);

        var model = from entity in receipts.OrderBy(sidx + " " + sord)
                    select new
                    {
                      MessageId = entity.MessageId,
                      Message = entity.Message,
                      DDPVersion = entity.DDPVersion,
                      TimeStamp = entity.TimeStamp.ToString(),
                    };
        return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()), JsonRequestBehavior.AllowGet);
      }
    }
}
