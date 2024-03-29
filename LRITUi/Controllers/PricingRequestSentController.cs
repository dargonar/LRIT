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
    public class PricingRequestSentController : MyController
    {
        //
        // GET: /Receipt/
        public ActionResult List()
        {
            return View();
        }

        public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord)
        {
            string[] ReqParams = { "DDPVersionNum", "MessageId", "MessageType", "Originator", "Test", "MsgInOut", "TimeStamp" };
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

            var lda = new PricingRequestSentDataAccess(context);
            var logs = lda.GetAllBetween(fromDate, toDate);

            var model = from entity in logs.OrderBy(sidx + " " + sord)
                        select new
                        {
                            DDPVersionNum = entity.DDPVersionNum,
                            MessageId = entity.MessageId,
                            MessageType = entity.MessageType,
                            Originator = entity.Originator,
                            Test = entity.test,
                            MsgInOut = entity.MsgInOut.InOut,
                            TimeStamp = entity.TimeStamp.ToString()
                        };
            return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()));
        }

    }
}
