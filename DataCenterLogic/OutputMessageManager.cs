using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.IO;
using DataCenterDataAccess;
using DataCenterLogic;
using log4net;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de los mensajes salientes de DataCenter.
  /// Esta clase lee los mensajes de la cola de salida y los envia al IDE / DDP / ASP segun corresponda
  /// </summary>
  public class OutputMessageManager
  {
    public delegate void MessageSentToIDE(string label);
    public event MessageSentToIDE OnMessageSentToIDE;

    private DataCenterTypesIDE.idePortTypeClient ideClient = new DataCenterLogic.DataCenterTypesIDE.idePortTypeClient("idePort");
    private DDPServerTypes.ddpPortTypeClient ddpClient = new DataCenterLogic.DDPServerTypes.ddpPortTypeClient("ddpPort");

    private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
    {
      return true;
    }

    public OutputMessageManager()
    {
      ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
    }

    /// <summary>
    /// Referencia a la instancia de log
    /// </summary>
    private static readonly ILog log = LogManager.GetLogger(typeof(OutputMessageManager));

    /// <summary>
    /// Variable privada del Singleton 
    /// </summary>
    private static OutputMessageManager outputMessageManager = null;

    /// <summary>
    /// Funcion del patron singleton para obtener una instancia unica
    /// </summary>
    /// <returns>Instancia al OutputMessageManager</returns>
    public static OutputMessageManager instance()
    {
      if( outputMessageManager == null )
        outputMessageManager = new OutputMessageManager();

      return outputMessageManager;
    }

    /// <summary>
    /// Informacion de configuracion
    /// </summary>
    public BasicConfiguration mBasicConfiguration;

    /// <summary>
    /// Inicializa el InputMessageManager y comienza a leer de la cola de entrada asincronicamente
    /// </summary>
    static private Thread mCheckQueueThread = new Thread(CheckQueueStub);
    static bool mRun = true;

    public void Start()
    {
      mCheckQueueThread.Start(this);
    }

    private void CheckQueue()
    {
      while (mRun == true)
      {
        try
        {
          core_out msg = null;
          using (DBDataContext ctx = new DBDataContext(mBasicConfiguration.ConnectionString))
          {
            msg = ctx.core_outs.OrderBy(c => c.created_at).SingleOrDefault();
            while (msg != null)
            {
              int wrong_count = 0;
              bool res = ProcessMessage(msg, ctx);
              while (res == false && ++wrong_count < 3)
                res = ProcessMessage(msg, ctx);

              if (res == false)
                DiscardMessage(msg, ctx);

              msg = ctx.core_outs.OrderBy(c => c.created_at).SingleOrDefault();
            }
          }
        }
        catch (Exception ex)
        {
          log.Error("CheckQueue => Loop error", ex);
        }

        //Wait 
        for (int i = 0; i < 10 && mRun == true; i++)
          Thread.Sleep(1000);
      }
    }

    static private void CheckQueueStub(object o)
    {
      ((OutputMessageManager)o).CheckQueue();
    }

    private void DiscardMessage(core_out msg, DBDataContext ctx)
    {
      try
      {
        ctx.core_outs.DeleteOnSubmit(msg);
        ctx.SubmitChanges();

        log.Error("Unable to process msg: " + (msg != null ? msg.message : "null msg"));
      }
      catch
      {
        log.Error("Unable to dump msg");
      }
    }

    private bool ProcessMessage(core_out msg, DBDataContext ctx)
    {
      try
      {
        // --- Procesamos un mensaje ---
        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(15)))
        {
          queue_PeekCompleted(msg);
          ctx.core_outs.DeleteOnSubmit(msg);
          ctx.SubmitChanges();

          scope.Complete();
        }

        return true;
      }
      catch (Exception ex)
      {
        log.Error("ProcessMessage", ex);
      }

      return false;
    }

    /// <summary>
    /// Esta funcion es llamada cuando hay un mensaje nuevo en la cola de entrada
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Async events</param>
    void queue_PeekCompleted(core_out msg)
    {
      string msgId = "N-A";
      if( msg.msgtype == "priceRequest" )
      {
        msgId = SendPriceRequest(msg);
      }
      else if ( msg.msgtype == "systemStatus" )
      {
        msgId = SendSystemStatus(msg);
      }
      else if ( msg.msgtype == "receipt" )
      {
        msgId = SendReceipt(msg);
      }
      else if (msg.msgtype == "pricingUpdate")
      {
        msgId = SendPricingUpdate(msg);
      }
      else if (msg.msgtype == "journalReport")
      {
        msgId = SendJournalReport(msg);
      }
      else if ( msg.msgtype == "shipPositionReport" )
      {
        msgId = SendShipPositionReport(msg);
      }
      else if ( msg.msgtype == "ddpRequest" )
      {
        msgId = SendDDPRequest(msg);
      }
      else if (msg.msgtype == "shipPositionRequest")
      {
        msgId = SendShipPositionRequest(msg);
      }
      else if (msg.msgtype == "SARSURPICRequest")
      {
        msgId = SendSARSurpicRequest(msg);
      }
      else
      {
        log.Error("Unknow message in out queue " + msg.msgtype);
      }

      if ( System.Configuration.ConfigurationManager.AppSettings["save_messages"] == "yes" )
      {
        string folder = System.Configuration.ConfigurationManager.AppSettings["save_folder"];
        if( folder == string.Empty )
          folder = "c:\\msgs";

        string fullpath = string.Format("{0}\\{1:yyyyMMdd}\\out", folder, DateTime.UtcNow);
        Directory.CreateDirectory(fullpath);

        File.WriteAllText(string.Format("{0}\\{1}-{2}.txt", fullpath, msg.msgtype, msgId), msg.message);
      }

      if( OnMessageSentToIDE != null )
        OnMessageSentToIDE(msg.msgtype);
    }

    private string SendPricingRequest(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.PricingRequestType pricingRequest = (DataCenterLogic.DataCenterTypesIDE.PricingRequestType)(msg.Body);
      var pricingRequest = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.PricingRequestType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        pricingRequest.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.PricingRequest(pricingRequest);
        log.Info("SendPricingRequest: pricingRequest sent to IDE");
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new PricingRequestSentDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(pricingRequest), 1);
      }
      log.Info("pricingRequest stored");
      return pricingRequest.MessageId;      
      
    }

    private string SendShipPositionRequest(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.ShipPositionRequestType shipPositionRequest = (DataCenterLogic.DataCenterTypesIDE.ShipPositionRequestType)(msg.Body);
      var shipPositionRequest = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.ShipPositionRequestType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        shipPositionRequest.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.ShipPositionRequest(shipPositionRequest);
        log.Info("SendShipPositionRequest: shipPositionRequest sent to IDE");
      }

      if (ShipPositionRequestHelper.IsPeriodicRequest(shipPositionRequest.RequestType))
      {
        //Es periodico, y pedimos sacarlo de la frecuencia estandard 6hs (RequestType==6), y no es SAR (msgtype==5)?
        //Debemos imputarnos la presunta reprogramacion PRICING
        if (shipPositionRequest.RequestType !=  DataCenterLogic.DataCenterTypesIDE.requestTypeType.Item6 &&
            shipPositionRequest.MessageType ==  DataCenterLogic.DataCenterTypesIDE.messageTypeType1.Item5 )
        {
          /************************************************/
          var pmgr = new PricingManager();
          decimal? price = pmgr.AddASPReprogrMessage(0, shipPositionRequest.DataUserRequestor, shipPositionRequest.DataUserProvider);
          log.Info(string.Format("SendShipPositionRequest: Reprogramacion presunta par {0} => precio={1}", shipPositionRequest.MessageId, price));
          /************************************************/
        }
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new ShipPositionRequestDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(shipPositionRequest), 1);
      }
      log.Info("ShipPositionRequest stored");
      return shipPositionRequest.MessageId;      
    }

    private string SendSARSurpicRequest(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.SARSURPICType SARSurpicReq = (DataCenterLogic.DataCenterTypesIDE.SARSURPICType)(msg.Body);
      var SARSurpicReq = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.SARSURPICType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        SARSurpicReq.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.SARSURPICRequest(SARSurpicReq);
        log.Info("SendSARSurpicRequest: SARSurpicReq sent to IDE");
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new SARSURPICRequestDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(SARSurpicReq), 1);
      }
      log.Info("SARSurpicReq stored");
      return SARSurpicReq.MessageId;      
    }

    /// <summary>
    /// Envia un DDP Request al DDP
    /// </summary>
    /// <param name="msg"></param>
    private string SendDDPRequest(core_out msg)
    {
      //DDPServerTypes.DDPRequestType ddpRequest = (DDPServerTypes.DDPRequestType)(msg.Body);
      var ddpRequest = new XmlSerializerHelper<DDPServerTypes.DDPRequestType>().FromStr(msg.message);

      //Send to ddp
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        ddpRequest.test = DataCenterLogic.DDPServerTypes.testType.Item0;
        ddpClient.DDPRequest(ddpRequest);
        log.Info("SendDDPRequest: ddpRequest sent to DDP");
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new DDPRequestSentDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(ddpRequest), 1);
      }
      log.Info("DDPRequest stored");
      return ddpRequest.MessageId;
    }

    /// <summary>
    /// Envia un mensaje de tipo ShipPositionReport al IDE
    /// </summary>
    /// <param name="msg">Mensaje ShipPositionReport</param>
    private string SendShipPositionReport(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.ShipPositionReportType  shipPositionReport = (DataCenterLogic.DataCenterTypesIDE.ShipPositionReportType)(msg.Body);
      var shipPositionReport = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.ShipPositionReportType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        shipPositionReport.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.ShipPositionReport(shipPositionReport);
        log.Info("SendShipPositionReport: shipPositionReport sent to IDE");
      }

      //ReferenceID puede ser de un report o "" si es SO
      //DataUserProvider deberia ser siempre 1005
      var pricing = new PricingManager();
      decimal? price = pricing.GetPriceForRequest(shipPositionReport.ReferenceId, shipPositionReport.DataUserProvider);

      //No tengo precio y es Es una reporte NO SAR?
      if (price == null)
        log.Warn(string.Format("SendShipPositionReport: Se manda un reporte {0} de posicion sin precio", shipPositionReport.MessageId));

      //Guarda el mensaje en la base de datos
      using (var dao = new ShipPositionReportDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(shipPositionReport), 1, price);
      }
      log.Info(string.Format("ShipPositionReport stored: price {0}",price));
      return shipPositionReport.MessageId;
    }

    /// <summary>
    /// Envia un mensaje de tipo JournalReport al IDE
    /// </summary>
    /// <param name="msg">Mensaje JournalReport</param>
    private string SendJournalReport(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.JournalReportType journalReport = (DataCenterLogic.DataCenterTypesIDE.JournalReportType)(msg.Body);
      var journalReport = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.JournalReportType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        journalReport.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.JournalReport(journalReport);
        log.Info("SendJournalReport: journalReport sent to IDE");
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new JournalReportSentDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(journalReport), 1);
      }
      log.Info("JournalReport stored");
      return journalReport.MessageId;
    }

    /// <summary>
    /// Envia un mensaje de tipo PricingUpdate al IDE
    /// </summary>
    /// <param name="msg">Mensaje PricingUpdate</param>
    private string SendPricingUpdate(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.PricingUpdateType  pricingUpdate = (DataCenterLogic.DataCenterTypesIDE.PricingUpdateType)(msg.Body);
      var pricingUpdate = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.PricingUpdateType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        pricingUpdate.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.PricingUpdate(pricingUpdate);
        log.Info("SendPricingUpdate: pricingUpdate sent to IDE");
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new PricingUpdateDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(pricingUpdate), 0);
      }
      log.Info("PricingUpdate stored");
      return pricingUpdate.MessageId;
    }

    /// <summary>
    /// Envia un mensaje de tipo Receipt al IDE
    /// </summary>
    /// <param name="msg">Mensaje Receipt</param>
    private string SendReceipt(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.ReceiptType receipt = (DataCenterLogic.DataCenterTypesIDE.ReceiptType)(msg.Body);
      var receipt = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.ReceiptType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        receipt.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.Receipt(receipt);
        log.Info("SendReceipt: receipt sent to IDE");
      }

      //Get price for receipt
      //El codigo 0 dice todo, si es codigo 0 quiere decir que este recibo se esta mandando por que no se pudo mandar una posicion
      //Por lo tanto el request debe existir o sino fue de una standing order.
      decimal? price = null;
      if (receipt.ReceiptCode == 0)
      {
        var pman = new PricingManager();
        price = pman.GetPriceForRequest(receipt.ReferenceId, receipt.Originator);
        if (price == null)
          log.Warn(string.Format("SendReceipt: Se manda un receipt codigo 0 {0} sin precio", receipt.MessageId));
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new ReceiptDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(receipt), 1, price);
      }
      
      log.Info(string.Format("Receipt stored: price {0}", price));
      return receipt.MessageId;
    }

    /// <summary>
    /// Envia un mensaje de tipo SystemStatus
    /// </summary>
    /// <param name="msg"></param>
    private string SendSystemStatus(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.SystemStatusType systemStatus = (DataCenterLogic.DataCenterTypesIDE.SystemStatusType)(msg.Body);
      var systemStatus = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.SystemStatusType>().FromStr(msg.message);

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        systemStatus.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.SystemStatus(systemStatus);
        log.Info("SendSystemStatus: systemStatus sent to IDE");
      }

      //Guarda el mensaje en la base de datos
      using (var dao = new SystemStatusDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(systemStatus), 1);
      }
      log.Info("SystemStatus stored");
      
      return systemStatus.MessageId;
    }

    /// <summary>
    /// Envia un mensaje de tipo PriceRequest
    /// </summary>
    /// <param name="msg">Mensaje PriceRequest</param>
    private string SendPriceRequest(core_out msg)
    {
      //DataCenterLogic.DataCenterTypesIDE.PricingRequestType priceRequest = (DataCenterLogic.DataCenterTypesIDE.PricingRequestType)(msg.Body);
      var priceRequest = new XmlSerializerHelper<DataCenterLogic.DataCenterTypesIDE.PricingRequestType>().FromStr(msg.message);

      //Guarda el mensaje en la base de datos

      //Send to IDE
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        priceRequest.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
        ideClient.PricingRequest(priceRequest);
        log.Info("SendPriceRequest: priceRequest sent to IDE");
      }

      using (var dao = new PricingRequestSentDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(priceRequest), 1);
      }
      log.Info("PriceRequest stored");
      
      return priceRequest.MessageId;
    }

    public void Stop()
    {
      mRun = false;      
    }
  }
}
