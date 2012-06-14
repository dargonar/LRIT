using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading;
using System.Transactions;
using DataCenterDataAccess;
using DataCenterLogic;
using log4net;
using DataCenterLogic.DataCenterTypes;

namespace DataCenterLogic
{
  /// <summary>
  /// Manejador de los mensajes entrantes al DataCenter
  /// Esta clase es la responsable de sacar todos los mensajes de la cola de entrada y procesarlos.
  /// Estos mensajes pueden ser provenientes del IDE o del ASP
  /// </summary>
  public class InputMessageManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(InputMessageManager));

    /// <summary>
    /// Variable privada del Singleton 
    /// </summary>
    private static InputMessageManager coreMessageManager = null;

    /// <summary>
    /// Funcion del patron singleton para obtener una instancia unica
    /// </summary>
    /// <returns>Instancia al InputMessageManager</returns>
    public static InputMessageManager instance()
    {
      if( coreMessageManager == null )
        coreMessageManager = new InputMessageManager();

      return coreMessageManager;
    }

    /// <summary>
    /// Informacion de configuracion
    /// </summary>
    public BasicConfiguration mBasicConfiguration = new BasicConfiguration();

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
      log.Info("Starting InputMessage Manager");
      while (mRun == true)
      {
        try
        {
          while (try_to_load_message() == true)
          {
            Thread.Sleep(100);
          }

        }
        catch (Exception ex)
        {
          log.Error("CheckQueue => Loop error", ex);
        }
        
        //Wait 
        for(int i=0; i<10 && mRun == true; i++)
          Thread.Sleep(1000);
      }

      log.Info("Finishing InputMessage Manager");
    }

    private bool try_to_load_message()
    {
      bool retval = false;
      using (DBDataContext ctx = new DBDataContext(mBasicConfiguration.ConnectionString))
      {
        var msg = ctx.core_ins.OrderBy(c => c.created_at).FirstOrDefault();
        if (msg != null)
        {
          retval = true;
          int wrong_count = 0;
          bool res = ProcessMessage(msg, ctx);
          while (res == false && ++wrong_count < 3)
            res = ProcessMessage(msg, ctx);

          if (res == false)
            DiscardMessage(msg, ctx);
        }
      }

      return retval;
    }

    private void DiscardMessage(core_in msg, DBDataContext ctx)
    {
      try
      {
        ctx.core_ins.DeleteOnSubmit(msg);
        ctx.SubmitChanges();

        log.Error("Unable to process msg: " + (msg != null ? msg.message : "null msg"));
      }
      catch
      {
        log.Error("Unable to dump msg");
      }
    }

    private bool ProcessMessage(core_in msg, DBDataContext ctx)
    {
      try
      {
        // --- Procesamos un mensaje ---
        using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(15)))
        {
          queue_PeekCompleted(msg);
          ctx.core_ins.DeleteOnSubmit(msg);
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
    
    static private void CheckQueueStub(object o)
    {
      ((InputMessageManager)o).CheckQueue();
    }

    /// <summary>
    /// Esta funcion es llamada cuando hay un mensaje nuevo en la cola de entrada
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Async events</param>
    private void queue_PeekCompleted(core_in msg)
    {
      string msgId = "NA";
      if (msg.msgtype == "receipt")
      {
        log.Debug("Se detecto receipt en queue...");
        var receipt = new XmlSerializerHelper<DataCenterLogic.DataCenterTypes.ReceiptType>().FromStr(msg.message);
        msgId = receipt.MessageId;
        var rman = new ReceiptManager();
        rman.ProcessReceipt(receipt);
      }
      else
        if (msg.msgtype == "pricingNotification")
        {
          log.Debug("Se detecto pricingNotification en queue...");
          //var pricingNotification = (PricingNotificationType)(msg.Body);
          var pricingNotification = new XmlSerializerHelper<PricingNotificationType>().FromStr(msg.message);
          msgId = pricingNotification.MessageId;
          var pman = new PricingManager();
          pman.ProcessPricingNotification(pricingNotification);
        }
        else
          if (msg.msgtype == "shipPositionReport")
          {
            log.Debug("Se detecto shipPositionReport en queue...");
            //var shipPositionReport = (DataCenterLogic.DataCenterTypes.ShipPositionReportType)(msg.Body);
            var shipPositionReport = new XmlSerializerHelper<DataCenterLogic.DataCenterTypes.ShipPositionReportType>().FromStr(msg.message);
            msgId = shipPositionReport.MessageId;
            var sprman = new ShipPositionReportManager();
            sprman.ProcessShipPositionReport(shipPositionReport);
          }
          else
            if (msg.msgtype == "pricingUpdate")
            {
              log.Debug("Se detecto pricingUpdate en queue...");
              //var pricingUpdate = (DataCenterLogic.DataCenterTypes.PricingUpdateType)(msg.Body);
              var pricingUpdate = new XmlSerializerHelper<DataCenterLogic.DataCenterTypes.PricingUpdateType>().FromStr(msg.message);
              msgId = pricingUpdate.MessageId;
              var pman = new PricingManager();
              pman.ProcessPricingUpdate(pricingUpdate);
            }
            else
              if (msg.msgtype == "SARSURPICRequest")
              {
                log.Debug("Se detecto SARSURPICRequest en queue...");
                //var SARSURPICRequest = (DataCenterLogic.DataCenterTypes.SARSURPICType)(msg.Body);
                var SARSURPICRequest = new XmlSerializerHelper<DataCenterLogic.DataCenterTypes.SARSURPICType>().FromStr(msg.message);
                msgId = SARSURPICRequest.MessageId;
                var ssman = new SARSURPICManager();
                ssman.ProcessSARSURPICRequest(SARSURPICRequest);
              }
              else
                if (msg.msgtype == "ddpNotification")
                {
                  log.Debug("Se detecto ddpNotification en queue...");
                  //var ddpNotification = (DataCenterLogic.DataCenterTypes.DDPNotificationType)(msg.Body);
                  var ddpNotification = new XmlSerializerHelper<DataCenterLogic.DataCenterTypes.DDPNotificationType>().FromStr(msg.message);
                  msgId = ddpNotification.MessageId;
                  var DDPman = new DDPManager();
                  DDPman.ProcessDDPNotification(ddpNotification);
                }
                else
                  if (msg.msgtype == "ddpUpdate")
                  {
                    log.Debug("Se detecto ddpUpdate en queue...");
                    var DDPman = new DDPManager();
                    //var ddpUpdate = (DataCenterTypes.DDPUpdateType)(msg.Body);
                    var ddpUpdate = new XmlSerializerHelper<DataCenterTypes.DDPUpdateType>().FromStr(msg.message);
                    msgId = ddpUpdate.MessageId;
                    DDPman.ProcessDDPUpdate(ddpUpdate);
                  }
                  else
                    if (msg.msgtype == "shipPositionRequest")
                    {
                      log.Debug("Se detecto shipPositionRequest en queue...");
                      //var shipPositionRequest = (DataCenterTypes.ShipPositionRequestType)(msg.Body);
                      var shipPositionRequest = new XmlSerializerHelper<DataCenterTypes.ShipPositionRequestType>().FromStr(msg.message);
                      msgId = shipPositionRequest.MessageId;
                      var sprm = new ShipPositionRequestManager();
                      sprm.ProcessShipPositionRequest(shipPositionRequest);
                    }
                    else
                      if (msg.msgtype == "systemStatus")
                      {
                        log.Debug("Se detecto systemStatus en queue...");
                        //var systemStatus = (DataCenterTypes.SystemStatusType)(msg.Body);
                        var systemStatus = new XmlSerializerHelper<DataCenterTypes.SystemStatusType>().FromStr(msg.message);
                        msgId = systemStatus.MessageId;
                        var ssm = new SystemStatusManager();
                        ssm.ProcessSystemStatus(systemStatus);
                      }
                      else
                        if (msg.msgtype == "Common.PositionMessage")
                        {
                          log.Debug("mensaje del ASP: Position Message");
                          //var aspPos = (Common.PositionMessage)(msg.Body);
                          var aspPos = new XmlSerializerHelper<Common.PositionMessage>().FromStr(msg.message);
                          var spm = new ShipPositionManager();
                          spm.ProcessASPPosition(aspPos);
                        }
                        else
                          if (msg.msgtype == "Common.PollResponse")
                          {
                            log.Debug("mensaje del ASP: PollResponse");
                            //var aspPollResponse = (Common.PollResponse)(msg.Body);
                            var aspPollResponse = new XmlSerializerHelper<Common.PollResponse>().FromStr(msg.message);
                            var spm = new ShipManager();
                            spm.ProcessPollResponse(aspPollResponse);
                          }
                          else
                            if (msg.msgtype == "Common.HeartBeatMessage")
                            {
                              log.Debug("mensaje del ASP: HeartBeat");
                              //var aspHb = (Common.HeartBeatMessage)(msg.Body);
                              var aspHb = new XmlSerializerHelper<Common.HeartBeatMessage>().FromStr(msg.message);
                              var spm = new SystemStatusManager();
                              spm.ProcessAspHeartBeat(aspHb);
                            }
                            else
                            {
                              log.Error(string.Format("Mensaje no conocido en cola '{0}'", msg.msgtype));
                            }

      //Dump message to disk
      try
      {
        if (System.Configuration.ConfigurationManager.AppSettings["save_messages"] == "yes")
        {
          string folder = System.Configuration.ConfigurationManager.AppSettings["save_folder"];
          if (folder == string.Empty)
            folder = "c:\\msgs";

          string fullpath = string.Format("{0}\\{1:yyyyMMdd}\\in", folder, DateTime.UtcNow);
          Directory.CreateDirectory(fullpath);

          File.WriteAllText(string.Format("{0}\\{1}-{2}.txt", fullpath, msg.msgtype, msgId), msg.message);
        }
      }
      catch (Exception ex)
      {
        log.Error("Error intentando guardar mensaje de entrada en disco", ex);
      }
    }

    public void Stop()
    {
      mRun = false;
    }
  }
}
