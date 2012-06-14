using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Messaging;
using DataCenterDataAccess;
using DataCenterLogic;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Transactions;
using System.Text.RegularExpressions;

namespace LRITDataCenter
{
  /// <summary>
  /// Summary description for LRITDataCenter
  /// </summary>
  [WebServiceAttribute(Namespace = "http://gisis.imo.org/XML/LRIT/2008")]
  [WebServiceBindingAttribute(Name = "dcServiceBinding", Namespace = "http://gisis.imo.org/XML/LRIT/2008")]
  [SoapRpcService(RoutingStyle=SoapServiceRoutingStyle.RequestElement)]

  public class LRITDataCenterWebService : IDcServiceBinding
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(LRITDataCenterWebService));
    private BasicConfiguration mBasicConfiguration = BasicConfiguration.FromNameValueCollection( System.Configuration.ConfigurationManager.AppSettings );

    private static string VERSION = "1.3.1";

    public LRITDataCenterWebService()
    {
      XmlConfigurator.Configure();
      DataCenterDataAccess.Config.ConnectionString = mBasicConfiguration.ConnectionString;
      //QueueManager.Instance().SetIn(mBasicConfiguration.CoreInQueue);
    }

    #region IDcServiceBinding Members
    
    [Validation]
    public Response ShipPositionReport(ShipPositionReportType shipPositionReport)
    {
      log.Info("New ShipPositionReport message arrived");
      validateSchemaVersion(shipPositionReport.schemaVersion);

      //Create message and enqueue it
      //Message msg = new Message(shipPositionReport);
      //msg.Label = "shipPositionReport";

      EnqueueMessageInQueue("shipPositionReport", new XmlSerializerHelper<ShipPositionReportType>().ToStr(shipPositionReport));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("ShipPositionReport first validation OK, enqueued");
      return response;
    }
    
    [Validation]
    public Response ShipPositionRequest(ShipPositionRequestType shipPositionRequest)
    {
      log.Info("New shipPosition request arrived");
      validateSchemaVersion(shipPositionRequest.schemaVersion);

      //Create message and enqueue it
      //Message msg = new Message(shipPositionRequest);
      //msg.Label = "shipPositionRequest";

      EnqueueMessageInQueue("shipPositionRequest", new XmlSerializerHelper<ShipPositionRequestType>().ToStr(shipPositionRequest));

      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("shipPosition Request first validation OK, enqueued");
      return response;      
    }

    [Validation]
    public Response SARSURPICRequest(SARSURPICType SARSURPICRequest)
    {
      log.Info("New SARSURPIC request arrived");
      validateSchemaVersion(SARSURPICRequest.schemaVersion);

      //Create message and enqueue it
      //Message msg = new Message(SARSURPICRequest);
      //msg.Label = "SARSURPICRequest";

      EnqueueMessageInQueue("SARSURPICRequest", new XmlSerializerHelper<SARSURPICType>().ToStr(SARSURPICRequest));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("SARSURPIC Request first validation OK, enqueued");
      return response;
    }

    [Validation]
    public Response Receipt(ReceiptType receipt)
    {
      log.Info("New Receipt message arrived");
      validateSchemaVersion(receipt.schemaVersion);

      //Create message and enqueue it
      //Message msg = new Message(receipt);
      //msg.Label = "receipt";

      EnqueueMessageInQueue("receipt", new XmlSerializerHelper<ReceiptType>().ToStr(receipt));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("Receipt first validation OK, enqueued");
      return response;
    }

    [Validation]
    public Response DDPNotification(DDPNotificationType ddpNotification)
    {
      log.Info("New DDPNotification message arrived");
      validateSchemaVersion(ddpNotification.schemaVersion);

      //Create message and enqueue it
      Message msg = new Message(ddpNotification);
      msg.Label = "ddpNotification";

      EnqueueMessageInQueue("ddpNotification", new XmlSerializerHelper<DDPNotificationType>().ToStr(ddpNotification));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("DDPNotificationMessage first validation OK, enqueued");
      return response;      
    }

    [Validation]
    public Response DDPUpdate(DDPUpdateType ddpUpdate)
    {
      log.Info("New DDPUpdate message arrived");
      validateSchemaVersion(ddpUpdate.schemaVersion);

      //Create message and enqueue it
      //Message msg = new Message(ddpUpdate);
      //msg.Label = "ddpUpdate";

      EnqueueMessageInQueue("ddpUpdate", new XmlSerializerHelper<DDPUpdateType>().ToStr(ddpUpdate));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("DDPUpdate Message first validation OK, enqueued");
      return response;
    }
    
    [Validation]
    public Response SystemStatus(SystemStatusType SystemStatus1)
    {
      log.Info("New System Status message arrived");
      validateSchemaVersion(SystemStatus1.schemaVersion);
      
      //Create message and enqueue it
      //Message msg = new Message(SystemStatus1);
      //msg.Label = "systemStatus";

      EnqueueMessageInQueue("systemStatus", new XmlSerializerHelper<SystemStatusType>().ToStr(SystemStatus1));

      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("System Status Message first validation OK, enqueued");
      return response;
    }

    [Validation]
    public Response PricingNotification(PricingNotificationType pricingNotification)
    {
      log.Info("New PricingNotification message arrived");
      validateSchemaVersion(pricingNotification.schemaVersion);
  
      //Create message and enqueue it
      //Message msg = new Message(pricingNotification);
      //msg.Label = "pricingNotification";

      EnqueueMessageInQueue("pricingNotification", new XmlSerializerHelper<PricingNotificationType>().ToStr(pricingNotification));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("PricingNotification first validation OK, enqueued");
      return response;
    }

    [Validation]
    public Response PricingUpdate(PricingUpdateType pricingUpdate)
    {
      log.Info("New PricingUpdate message arrived");
      validateSchemaVersion(pricingUpdate.schemaVersion);

      //Create message and enqueue it
      //Message msg = new Message(pricingUpdate);
      //msg.Label = "pricingUpdate";

      EnqueueMessageInQueue("pricingUpdate", new XmlSerializerHelper<PricingUpdateType>().ToStr(pricingUpdate));
      
      //Build response
      Response response = new Response();
      response.response = responseType.Success;
      log.Info("PricingUpdate first validation OK, enqueued");
      return response;
    }

    #endregion

    #region Enqueue Function

    private void EnqueueMessageInQueue(string label, string xmlmessage)
    {
      using (TransactionScope ts = new TransactionScope())
      {
        try
        {
          QueueManager.Instance().EnqueueIn(label, xmlmessage);
        }
        catch(Exception ex)
        {
          throw ex;
        }

        ts.Complete();
      }
    }

    #endregion

    #region Validate Schema Function
    private void validateSchemaVersion(decimal version)
    {
      var c = new DataCenterLogic.ConfigurationManager();
      if (version != decimal.Parse(c.Configuration.SchemaVersion))
      {
        log.Error("validateSchemaVersion: Sending exception to IDE");
        throw new SoapException("Invalid Schema Version", new System.Xml.XmlQualifiedName("err") );
      }

    }
    #endregion

  }
}
