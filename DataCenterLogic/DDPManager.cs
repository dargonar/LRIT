using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using log4net;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.IO;
using DataCenterLogic.DataCenterTypes;
using System.Xml;
using System.Transactions;

namespace DataCenterLogic
{
  public class DDPManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(DDPManager));
    public void ProcessPendingUpdates()
    {
      DDPImportHelper helper = new DDPImportHelper();
      using (TransactionScope ts = new TransactionScope(TransactionScopeOption.RequiresNew, TimeSpan.FromMinutes(10)))
      {
        try
        {
          var ddpvermgr = new DDPVersionManager();

          var pendings = PendingUpdateManager.GetPendingUpdates();
          foreach (var pending in pendings)
          {
            DDPVersion ver=null;
            if (pending.type == 0)
              ver = ddpvermgr.GetInmediateDDPVersion(pending.targetVersion);
            else
              ver = ddpvermgr.GetRegularDDPVersion(pending.targetVersion);

            if (ver != null)
            {
              log.Info(string.Format("ProcessPendingUpdates: version {0} already exists skping ...",pending.targetVersion));
              continue;
            }

            helper.UpdateIncrementalOrRegular(pending);
          }

          using (var d = new PendingUpdatesDataAccess())
          {
            d.Remove(pendings);
          }

        }
        catch(Exception ex)
        {
          log.Error("ProcessPendingUpdates: error!", ex);
        }
        ts.Complete();
      }
    }
    
    /// <summary>
    /// Procesa un mensaje de tipo DDP Update.
    /// Esta funcion utiliza el DDPImportHelper para incorporar la informacion del DDP en formato XML a la base de datos.
    /// </summary>
    /// <param name="msg">Mensaje de DDP Update</param>
    public void ProcessDDPUpdate(DDPUpdateType ddpUpdate)
    {
      DDPUpdate dbupd;
      using (DDPUpdateDataAccess dao = new DDPUpdateDataAccess())
      {
        dbupd = dao.Create(TypeHelper.Map2DB(ddpUpdate), 0);
      }

      if (ddpUpdate.UpdateType != DDPUpdateTypeUpdateType.Item4 )
      {
        //Full??
        if (ddpUpdate.UpdateType == DDPUpdateTypeUpdateType.Item3)
        {
          ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = new ZipFile(new MemoryStream(ddpUpdate.DDPFile));

          var ddpVersion = InsertCompleteDDP(ddpUpdate, DateTime.UtcNow.AddYears(-100));

          log.Info("ProcessDDPUpdate: Starting full");
          DDPImportHelper importer = new DDPImportHelper();
          ddpVersion.published_at = importer.Import(zipFile.GetInputStream(0), ddpVersion);
          zipFile.Close();

          using (DDPVersionDataAccess ddpverda = new DDPVersionDataAccess())
          {
            ddpverda.update(ddpVersion);
          }
        }
        else
        {
          //Salvamos como updates pendientes y luego el scheduler procesa (incrementales y regulares)
          ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = new ZipFile(new MemoryStream(ddpUpdate.DDPFile));
          DDPImportHelper importer = new DDPImportHelper();
          importer.SavePendingUpdates(zipFile.GetInputStream(0), dbupd.Id);
          log.Info("ProcessDDPUpdate: Saved in pending updates..");
          zipFile.Close();
        }
      }
      else
      {
        log.Info("ProcessDDPUpdate: Not processing archived or mixed incr/reg ddpupdate");
      }

      log.Info("ProcessDDPUpdate: DDPUpdate finished");
    }

    /// <summary>
    /// Procesa un mensaje de tipo DDP Notification.
    /// Esta funcion incorpora el mensaje a la base de datos y pide al DDP un requerimiento de actualizacion
    /// </summary>
    /// <param name="msg">Mensaje DDPNotification</param>
    public void ProcessDDPNotification(DDPNotificationType ddpNotification)
    {
      
      var ddpRequest = new DataCenterLogic.DDPServerTypes.DDPRequestType();

      ConfigurationManager configMgr = new ConfigurationManager();

      var dver = new DDPVersionDataAccess();
      var ver = dver.TodaysDDP();

      ddpRequest.ArchivedDDPTimeStamp = DateTime.UtcNow;
      ddpRequest.ArchivedDDPTimeStampSpecified = false;
      ddpRequest.ArchivedDDPVersionNum = null;
      ddpRequest.DDPVersionNum = ver.regularVer + ":" + ver.inmediateVer;
      ddpRequest.MessageId = MessageIdManager.Generate();
      ddpRequest.MessageType = DataCenterLogic.DDPServerTypes.messageTypeType.Item9;
      ddpRequest.Originator = configMgr.Configuration.DataCenterID;
      ddpRequest.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
      ddpRequest.test = DataCenterLogic.DDPServerTypes.testType.Item1;
      ddpRequest.TimeStamp = DateTime.UtcNow;

      //Si el notification es 0 (Regular) pedimos regular
      if (ddpNotification.UpdateType == DDPNotificationTypeUpdateType.Item0)
        ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item0;
      
      //Si el notification es 1 (Inmediate) pedimos inmediate
      if (ddpNotification.UpdateType == DDPNotificationTypeUpdateType.Item1)
        ddpRequest.UpdateType = DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType.Item1;

      //Enqueue DDPrequest
      //Message msgout = new Message(ddpRequest);
      //msgout.Label = "ddpRequest";

      QueueManager.Instance().EnqueueOut("ddpRequest", new XmlSerializerHelper<DataCenterLogic.DDPServerTypes.DDPRequestType>().ToStr(ddpRequest));

      using (DDPNotificationDataAccess dao = new DDPNotificationDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(ddpNotification), 0);
      }
      log.Info("DDPNotification successfully processed");
    }

    static public DDPVersion InsertCompleteDDP(DDPUpdateType ddp, DateTime publishDate)
    {
      return InsertCompleteDDP(ddp.DDPFileVersionNum, publishDate, ddp.DDPFile);
    }
    
    static public DDPVersion InsertCompleteDDP(string version, DateTime publishDate, byte[] file)
    {
      var ddpVersion = new DDPVersion();
      using (var ddpvda = new DDPVersionDataAccess())
      {
        ddpVersion.published_at = publishDate;
        ddpVersion.received_at = DateTime.UtcNow;
        ddpVersion.regularVer = version.Split(':')[0];
        ddpVersion.inmediateVer = version.Split(':')[1];
        ddpVersion.DDPFile = file;
        ddpvda.InsertCompleteDDP(ddpVersion);
        return ddpVersion;
      }
    }

    
    public void MakeDDPRequest(DDPServerTypes.DDPRequestType ddpRequest)
    {
      ConfigurationManager configMgr = new ConfigurationManager();

      var ddpverda = new DDPVersionManager();
      var ddpver = ddpverda.DDPFromDate(DateTime.UtcNow);

      //Fill necesary parameters
      ddpRequest.DDPVersionNum = ddpver.regularVer + ":" + ddpver.inmediateVer;
      ddpRequest.MessageId = MessageIdManager.Generate();
      ddpRequest.MessageType = DataCenterLogic.DDPServerTypes.messageTypeType.Item9;
      ddpRequest.Originator = configMgr.Configuration.DataCenterID;
      //ddpRequest.ReferenceId
      ddpRequest.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
      ddpRequest.TimeStamp = DateTime.UtcNow;

      //Enqueue DDPrequest
      //Message msgout = new Message(ddpRequest);
      //msgout.Label = "ddpRequest";
      //return ddpRequest;
    }

  }
}
