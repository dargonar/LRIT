using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;
using System.Configuration;

using log4net;
using log4net.Config;

//Use web.config and check for modifications
[assembly: log4net.Config.XmlConfigurator(Watch=false)]

namespace IDESimulator
{
  /// <summary>
  /// Summary description for Service1
  /// </summary>
  [WebServiceAttribute(Namespace = "http://gisis.imo.org/XML/LRIT/2008")]
  [WebServiceBindingAttribute(Name = "ideServiceBinding", Namespace = "http://gisis.imo.org/XML/LRIT/2008")]
  [SoapRpcService(RoutingStyle=SoapServiceRoutingStyle.RequestElement)]
 
  public class IDESimulator : IIdeServiceBinding
  {
    private log4net.ILog m_log = log4net.LogManager.GetLogger(typeof(IDESimulator));
    
    IDESimulator()
    {
      XmlConfigurator.Configure( new FileInfo(ConfigurationManager.AppSettings["Log4NetConfigFile"]) );
    }

    #region IIdeServiceBinding Members

    public Response ShipPositionReport(ShipPositionReportType report)
    {
      string msg = string.Format(@"DataUserRequestor:{0},DDPVersionNum:{1},ShipName:{2},
                    ReferenceId:{3},MessageId:{4},ShipborneEquipmentId:{5}, schemaVersion:{6}",
                    report.DataUserRequestor,
                    report.DDPVersionNum,
                    report.ShipName,
                    report.ReferenceId,
                    report.MessageId,
                    report.ShipborneEquipmentId  ,
                    report.schemaVersion );

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;  
    }

    public Response ShipPositionRequest(ShipPositionRequestType request)
    {
      string msg = string.Format(@"DataUserRequestor:{0},DDPVersionNum:{1},Item:{2},
                    ItemElementName:{3},MessageId:{4},Distance:{5},schemaVersion:{6}",
                    request.DataUserRequestor,
                    request.DDPVersionNum,
                    request.Item ,
                    request.ItemElementName,
                    request.MessageId,
                    request.Distance ,
                    request.schemaVersion );

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;  
    }

    public Response SARSURPICRequest(SARSURPICType request)
    {
      string msg = string.Format(@"DataUserRequestor:{0},DDPVersionNum:{1},Item:{2},
                    ItemElementName:{3},MessageId:{4},NumberOfPositions:{5},schemaVersion:{6}",
                    request.DataUserRequestor,
                    request.DDPVersionNum,
                    request.Item ,
                    request.ItemElementName,
                    request.MessageId,
                    request.NumberOfPositions,
                    request.schemaVersion );

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;    
    }

    public Response Receipt(ReceiptType receipt)
    {
      string msg = string.Format(@"DDPVersionNum:{0},Destination:{1},Message:{2},
                    MessageId:{3},Originator:{4},ReceiptCode:{5},ReferenceId:{6}",
                    receipt.DDPVersionNum ,
                    receipt.Destination,
                    receipt.Message,
                    receipt.MessageId,
                    receipt.Originator  ,
                    receipt.ReceiptCode ,
                    receipt.ReferenceId );

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;         
    }

    public Response SystemStatus(SystemStatusType systemStatus)
    {
      string msg = string.Format(@"DDPVersionNum:{0},Message:{1},MessageId:{2},
                    Originator:{3},schemaVersion:{4},test:{5},TimeStamp:{6}",
                    systemStatus.DDPVersionNum,
                    systemStatus.Message,
                    systemStatus.MessageId,
                    systemStatus.Originator,
                    systemStatus.schemaVersion,
                    systemStatus.test,
                    systemStatus.TimeStamp);

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;         
    }

    public Response JournalReport(JournalReportType JournalReport1)
    {
      string msg = string.Format(@"DDPVersionNum:{0},Message:{1},MessageId:{2},
                    Originator:{3},schemaVersion:{4},test:{5},TimeStamp:{6}",
                    JournalReport1.DDPVersionNum,
                    JournalReport1.Message,
                    JournalReport1.MessageId,
                    JournalReport1.Originator,
                    JournalReport1.schemaVersion,
                    JournalReport1.test,
                    JournalReport1.TimeStamp);

      m_log.Info( msg );

      Response response = new Response();
      response.response = responseType.Success;
      return response;      
    }

    public Response PricingRequest(PricingRequestType pricingRequest)
    {
      string.Format(@"DDPVersion:{0},MessageId:{1},MessageType:{2},
                    Originator:{3},schemaVersion:{4},test:{5},TimeStamp:{6}",
                    pricingRequest.DDPVersionNum, 
                    pricingRequest.MessageId,
                    pricingRequest.MessageType,
                    pricingRequest.Originator,
                    pricingRequest.schemaVersion,
                    pricingRequest.test,
                    pricingRequest.TimeStamp);



      Response response = new Response();
      response.response = responseType.Success;
      return response;
    }

    public Response PricingUpdate(PricingUpdateType update)
    {
      string.Format(@"DDPVersion:{0},MessageId:{1},MessageType:{2},
                    schemaVersion:{4},test:{5},TimeStamp:{6}",
                    update.DDPVersionNum, 
                    update.MessageId,
                    update.MessageType,
                    update.schemaVersion,
                    update.test,
                    update.TimeStamp);

      Response response = new Response();
      response.response = responseType.Success;
      return response;      
    }

    #endregion
  }
}
