using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Common;
using DataCenterLogic;
using log4net;
namespace ASPDC2
{
  /// <summary>
  /// Summary description for Service1
  /// </summary>
  [WebService(Namespace = "http://lrit.prefecturanaval.gov.ar/lritaspdcws")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]

  public class LRITASPDCService : System.Web.Services.WebService, Common.ILritAspDcWs
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(LRITASPDCService));
    private static string VERSION = "2.3";

    public LRITASPDCService()
    {
      DataCenterDataAccess.Config.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
    }

    [WebMethod]
    public void HeartBeat()
    {
      log.Info("ASPDC2: New HeartBeat");
      var msg = new XmlSerializerHelper<Common.HeartBeatMessage>().ToStr(new Common.HeartBeatMessage());
      QueueManager.Instance().EnqueueIn("Common.HeartBeatMessage", msg);
    }
    
    [WebMethod]
    public void MessageHandler(object msg)
    {
      log.Info("ASPDC2: MessageHandler");
      var type = msg.GetType().ToString();
      if(type == "Common.PositionMessage")
      {
        this.PositionReport((PositionMessage)msg);
      }

      if (type == "Common.PollResponse")
      {
        this.PollResponse((PollResponse)msg);
      }

      return;
    }

    [WebMethod]
    public void PollResponse(PollResponse msg)
    {
      log.Info("ASPDC2: PollResponse");
      var tmp = new XmlSerializerHelper<Common.PollResponse>().ToStr(msg);
      QueueManager.Instance().EnqueueIn("Common.PollResponse", tmp);
    }

    [WebMethod]
    public void PositionReport(PositionMessage msg)
    {
      log.Info("ASPDC2: PositionMessage");
      var tmp = new XmlSerializerHelper<Common.PositionMessage>().ToStr(msg);
      QueueManager.Instance().EnqueueIn("Common.PositionMessage", tmp);
    }
  }
}