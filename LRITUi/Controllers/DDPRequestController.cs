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
    [Authorize(Roles = "Administrador, Operador")]
    public class DDPRequestController : MyController
    {
        public ActionResult List()
        {
            return View();
        }
        
        public ActionResult GridData(int page, int rows, string[] _search, string sidx, string sord)
        {
            string[] ReqParams = { "ArchivedDDPTimeStamp", "ArchivedDDPTimeStampSpecified", "ArchivedDDPVersionNum", "DDPVersionNum", "MessageId", "MessageType", "Originator", "test", "TimeStamp", "UpdateType", "MsgInOutId" };
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

            var lda = new DDPRequestSentDataAccess(context);
            var logs = lda.GetAllBetween(fromDate, toDate);

            var model = from entity in logs.OrderBy(sidx + " " + sord)
                        select new
                        {
                            //ArchivedDDPTimeStamp, ArchivedDDPTimeStampSpecified, ArchivedDDPVersionNum, DDPVersionNum, 
                            //MessageId, MessageType, Originator,  test, TimeStamp,UpdateType, MsgInOutId
                            ArchivedDDPTimeStamp = entity.ArchivedDDPTimeStamp.ToString(),
                            ArchivedDDPTimeStampSpecified = entity.ArchivedDDPTimeStampSpecified,
                            ArchivedDDPVersionNum = entity.ArchivedDDPTimeStampSpecified,
                            DDPVersionNum = entity.DDPVersionNum,
                            MessageId = entity.MessageId,
                            MessageType = entity.MessageType,
                            Originator = entity.Originator,
                            test = entity.test,
                            UpdateType = entity.UpdateType,
                            MsgInOut = entity.MsgInOut.InOut,
                            TimeStamp = entity.TimeStamp.ToString()
                        };
            return Json(model.ToJqGridData(page, rows, null, querys.ToArray(), columns.ToArray()), JsonRequestBehavior.AllowGet);

        }
    }
}
