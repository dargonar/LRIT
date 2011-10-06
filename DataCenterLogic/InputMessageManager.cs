using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Messaging;
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
    /// Referencia a la cola de entrada
    /// </summary>
    private MessageQueue inputQueue = null;
 
    private int wrong_count = 0;

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
    public void Start()
    {
      ///Open input and ouput queue
      inputQueue = QueueManager.Instance().GetInQueue();
      
      ///Start async read of message from queue
      inputQueue.PeekCompleted += new PeekCompletedEventHandler(queue_PeekCompleted);
      inputQueue.BeginPeek();
    }

    /// <summary>
    /// Esta funcion es llamada cuando hay un mensaje nuevo en la cola de entrada
    /// </summary>
    /// <param name="sender">Object Sender</param>
    /// <param name="e">Async events</param>
    private void queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
    {
      using( TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(15) ) )
      {
        Message msg = null;
        string msgId = "NA";
        try
        {
          msg = inputQueue.Receive( MessageQueueTransactionType.Automatic );

          if( msg.Label == "receipt" )
          {
            log.Debug("Se detecto receipt en queue...");
            var receipt = (DataCenterLogic.DataCenterTypes.ReceiptType)(msg.Body);
            msgId = receipt.MessageId;
            var rman = new ReceiptManager();
            rman.ProcessReceipt(receipt);
          }
          else 
          if ( msg.Label == "pricingNotification" )
          {
            log.Debug("Se detecto pricingNotification en queue...");
            var pricingNotification = (PricingNotificationType)(msg.Body);
            msgId = pricingNotification.MessageId;
            var pman = new PricingManager();
            pman.ProcessPricingNotification(pricingNotification);
          }
          else
          if ( msg.Label == "shipPositionReport" )
          {
            log.Debug("Se detecto shipPositionReport en queue...");
            var shipPositionReport = (DataCenterLogic.DataCenterTypes.ShipPositionReportType)(msg.Body);
            msgId = shipPositionReport.MessageId;
            var sprman = new ShipPositionReportManager();
            sprman.ProcessShipPositionReport(shipPositionReport);
          }
          else
          if ( msg.Label == "pricingUpdate" )
          {
            log.Debug("Se detecto pricingUpdate en queue...");
            var pricingUpdate = (DataCenterLogic.DataCenterTypes.PricingUpdateType)(msg.Body);
            msgId = pricingUpdate.MessageId;
            var pman = new PricingManager();
            pman.ProcessPricingUpdate(pricingUpdate);
          }
          else
          if ( msg.Label == "SARSURPICRequest" )
          {
            log.Debug("Se detecto SARSURPICRequest en queue...");
            var SARSURPICRequest = (DataCenterLogic.DataCenterTypes.SARSURPICType)(msg.Body);
            msgId = SARSURPICRequest.MessageId;
            var ssman = new SARSURPICManager();
            ssman.ProcessSARSURPICRequest(SARSURPICRequest);
          }
          else
          if ( msg.Label == "ddpNotification" )
          {
            log.Debug("Se detecto ddpNotification en queue...");
            var ddpNotification = (DataCenterLogic.DataCenterTypes.DDPNotificationType)(msg.Body);
            msgId = ddpNotification.MessageId;
            var DDPman = new DDPManager();
            DDPman.ProcessDDPNotification(ddpNotification);
          }
          else
          if ( msg.Label == "ddpUpdate" )
          {
            log.Debug("Se detecto ddpUpdate en queue...");
            var DDPman = new DDPManager();
            var ddpUpdate = (DataCenterTypes.DDPUpdateType)(msg.Body);
            msgId = ddpUpdate.MessageId;
            DDPman.ProcessDDPUpdate(ddpUpdate);
          }
          else
          if (msg.Label == "shipPositionRequest")
          {
            log.Debug("Se detecto shipPositionRequest en queue...");
            var shipPositionRequest = (DataCenterTypes.ShipPositionRequestType)(msg.Body);
            msgId = shipPositionRequest.MessageId;
            var sprm = new ShipPositionRequestManager();
            sprm.ProcessShipPositionRequest(shipPositionRequest);
          }
          else
          if (msg.Label == "systemStatus")
          {
            log.Debug("Se detecto systemStatus en queue...");
            var systemStatus = (DataCenterTypes.SystemStatusType)(msg.Body);
            msgId = systemStatus.MessageId;
            var ssm = new SystemStatusManager();
            ssm.ProcessSystemStatus(systemStatus);
          }
          else
          if (msg.Label == "")
          {
            log.Debug("Se detecto mensaje del ASP en queue... tipo:");
            var type = msg.Body.GetType().ToString();
            
            switch (type)
            {
              case "Common.PositionMessage": 
                {
                  log.Debug("Position Message");
                  var aspPos = (Common.PositionMessage)(msg.Body);
                  var spm = new ShipPositionManager();
                  spm.ProcessASPPosition(aspPos);
                  break; 
                }
              case "Common.PollResponse": 
                {
                  log.Debug("PollResponse");
                  var aspPollResponse = (Common.PollResponse)(msg.Body);
                  var spm = new ShipManager();
                  spm.ProcessPollResponse(aspPollResponse);
                  break;
                }
              case "Common.HeartBeatMessage":
                {
                  log.Debug("HeartBeat");
                  var aspHb = (Common.HeartBeatMessage)(msg.Body);
                  var spm = new SystemStatusManager();
                  spm.ProcessAspHeartBeat(aspHb);
                  break;
                }
              default: 
                { 
                  break; 
                }
            }
          }
          else
          {
            log.Error(string.Format("Mensaje no conocido en cola '{0}'", msg.Label));
          }

          scope.Complete();
          wrong_count = 0;

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

              string xmlstr = OutputMessageManager.messageToString(msg);
              string lbl = msg.Label;
              if (lbl == "")
                lbl = msg.Body.GetType().ToString();
              File.WriteAllText(string.Format("{0}\\{1}-{2}.txt", fullpath, lbl, msgId), xmlstr);
            }
          }
          catch (Exception ex)
          {
            log.Error("Error intentando guardar mensaje de entrada en disco", ex);
          }

        }
        catch(Exception ex)
        {
          wrong_count++;
          try
          {
            log.Error("Error handling message" + Environment.NewLine + Environment.NewLine + ex + Environment.NewLine + Environment.NewLine);
          }
          catch
          {

          }
          
          //Poison messages
          if (wrong_count > 3)
          {
            wrong_count = 0;
            try
            {
              var serializer = new System.Xml.Serialization.XmlSerializer(msg.Body.GetType());
              var stringWriter = new System.IO.StringWriter();
              serializer.Serialize(stringWriter, msg.Body);

              scope.Complete();
              log.Error("MSGDUMP: " + stringWriter.ToString());
            }
            catch(Exception ex2)
            {
              log.Error("UNABLE TO DUMP MESSAGE: " + ex.Message, ex2);
            }
          }
        }
      }

      inputQueue.BeginPeek();
    }
 

    public void Stop()
    {
      
    }
  }
}
