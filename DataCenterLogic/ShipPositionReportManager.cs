using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterLogic.DataCenterTypesIDE;
using DataCenterLogic.DataCenterTypes;
using DataCenterDataAccess;
using log4net;
using System.Messaging;
using Microsoft.SqlServer.Types;

namespace DataCenterLogic
{
  class ShipPositionReportManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ShipPositionReportManager));

    public void SendReport(string requestor, ShipPosition position, string referenceId, DataCenterTypesIDE.responseTypeType responseType, DataCenterLogic.DataCenterTypesIDE.messageTypeType messageType)
    {
      //Configuracion del datacenter
      var configMgr = new ConfigurationManager();
      
      //DataCenterTypesIDE.responseTypeType responseType = getResponseTypeFromRequest(spr);
      //DataCenterLogic.DataCenterTypesIDE.messageTypeType messageType = getMessageTypeFromRequest(spr);

      //Obtengo el ship al que pertenece la posicion
      using (var shipdao = new ShipDataAccess())
      {
        Ship ship = shipdao.getById(position.ShipId);

        var shipPosReport = new DataCenterTypesIDE.ShipPositionReportType();

        var point = SqlGeography.STPointFromWKB(new System.Data.SqlTypes.SqlBytes(position.Position.ToArray()), 4326);

        shipPosReport.ASPId = configMgr.Configuration.ASPId;
        
        shipPosReport.DataUserProvider = "1005";
        //if( responseType == DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item4 )
        //  shipPosReport.DataUserProvider = "3005";

        shipPosReport.DataUserRequestor = requestor;
        shipPosReport.DCId = configMgr.Configuration.DataCenterID;
        shipPosReport.DDPVersionNum = DDPVersionManager.currentDDP();
        shipPosReport.IMONum = ship.IMONum;
        shipPosReport.Latitude = WGS84LatFormat(point.Lat.Value);
        shipPosReport.Longitude = WGS84LongFormat(point.Long.Value);
        shipPosReport.MessageId = MessageIdManager.Generate();
        shipPosReport.MessageType = messageType;
        shipPosReport.MMSINum = ship.MMSINum;
        shipPosReport.ReferenceId = referenceId;
        shipPosReport.ResponseType = responseType;
        shipPosReport.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
        shipPosReport.ShipborneEquipmentId = ship.EquipID;
        shipPosReport.ShipName = ship.Name;
        shipPosReport.test = DataCenterLogic.DataCenterTypesIDE.testType.Item1;
        shipPosReport.TimeStamp1 = position.TimeStamp;
        shipPosReport.TimeStamp2 = position.TimeStampInASP;
        shipPosReport.TimeStamp3 = position.TimeStampOutASP;
        shipPosReport.TimeStamp4 = position.TimeStampInDC;
        shipPosReport.TimeStamp5 = DateTime.UtcNow;

        log.Debug("Enqueing Report for ship: " + shipPosReport.IMONum + " Pos. Long. Lat.:" + shipPosReport.Longitude + " " + shipPosReport.Latitude + " Requestor: " + shipPosReport.DataUserRequestor);
        QueueManager.Instance().EnqueueOut(shipPosReport, "shipPositionReport");
      }
    }

    private DataCenterLogic.DataCenterTypesIDE.messageTypeType getMessageTypeFromRequest(DataCenterDataAccess.ShipPositionRequest spr)
    {
      return DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1;
      //return DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item2;
      //return DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item3;
    }

    private DataCenterLogic.DataCenterTypesIDE.responseTypeType getResponseTypeFromRequest(DataCenterDataAccess.ShipPositionRequest spr)
    {
      return DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item1;
      //return DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item2;
      //return DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item3;
      //return DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item4;
    }

    private string WGS84LongFormat(double val)
    {
      //<xs:pattern value="([0-1][0-7][0-9]\.[0-5][0-9]\.[0-9][0-9]\.[eEwW])|([0][8-9][0-9]\.[0-5][0-9]\.[0-9][0-9]\.[eEwW])|(180\.00\.00\.[eEwW])"/>
      string h = "w";
      if (val >= 0) h = "e";

      val = Math.Abs(val);
      double ival = (int)val;
      double min = (val - ival) * 60;
      double sec = 100 * (min - (int)min);

      return string.Format("{0:000}.{1:00}.{2:00}.{3}", (int)ival, (int)min, (int)sec, h);
    }

    private string WGS84LatFormat(double val)
    {
      //<xs:pattern value="([0-8][0-9]\.[0-5][0-9]\.[0-9][0-9]\.[nNsS])|(90\.00\.00\.[nNsS])"/>
      string h = "s";
      if (val >= 0) h = "n";

      val = Math.Abs(val);
      double ival = (int)val;
      double min = (val - ival) * 60;
      double sec = 100 * (min - (int)min);

      return string.Format("{0:00}.{1:00}.{2:00}.{3}", (int)ival, (int)(min), (int)sec, h);
    }

    /// <summary>
    /// Procesa un mensaje de tipo ShipPositionReport
    /// </summary>
    /// <param name="msg">El mensaje ShipPositionReport</param>
    public void ProcessShipPositionReport(DataCenterTypes.ShipPositionReportType shipPositionReport)
    {
      var configMgr = new ConfigurationManager();

      //Verifica si existe el Data User que requiere la información
      var cgm = new ContractingGovermentManager();
      var ddpm = new DDPVersionManager();

      var ddpVersion = ddpm.GetCurrentDDPVersion();

      var contractingGoverment = cgm.GetContractingGovermentByLRITId(shipPositionReport.DataUserRequestor, ddpVersion.Id);
      if (contractingGoverment == null)
      {
        string strError = string.Format("Specified LDU '{0}' does not exist", shipPositionReport.DataUserRequestor);

        //Arma mensaje de Recibo
        DataCenterLogic.DataCenterTypes.ReceiptType receipt = new DataCenterLogic.DataCenterTypes.ReceiptType();


        receipt.DDPVersionNum = DDPVersionManager.currentDDP();
        receipt.Destination = shipPositionReport.DCId;
        receipt.Message = strError;
        receipt.MessageId = MessageIdManager.Generate();
        receipt.MessageType = DataCenterLogic.DataCenterTypes.messageTypeType3.Item7;
        receipt.Originator = configMgr.Configuration.DataCenterID;
        receipt.ReceiptCode = DataCenterLogic.DataCenterTypes.receiptCodeType.Item7;
        receipt.ReferenceId = shipPositionReport.MessageId;
        receipt.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
        receipt.test = DataCenterLogic.DataCenterTypes.testType.Item1;
        receipt.TimeStamp = DateTime.UtcNow;

        Message msgout = new Message(receipt);
        msgout.Label = "receipt";

        //Encola mensaje
        QueueManager.Instance().EnqueueOut(msgout);

        log.Error(strError);
        return;
      }

      //Verifica si el existe el Id de referencia
      if (shipPositionReport.ReferenceId != "")
      {
        var sprda = new ShipPositionRequestDataAccess();
        if ( sprda.RequestExists(shipPositionReport.ReferenceId) == false )
        {
          string strError = string.Format("MessageID not found for ReferenceID '{0}'", shipPositionReport.ReferenceId);

          //Envía Recibo
          DataCenterLogic.DataCenterTypes.ReceiptType receipt = new DataCenterLogic.DataCenterTypes.ReceiptType();
          receipt.DDPVersionNum = DDPVersionManager.currentDDP();
          receipt.Destination = shipPositionReport.DCId;
          receipt.Message = strError;
          receipt.MessageId = MessageIdManager.Generate();
          receipt.MessageType = DataCenterLogic.DataCenterTypes.messageTypeType3.Item7;
          receipt.Originator = configMgr.Configuration.DataCenterID;
          receipt.ReceiptCode = DataCenterLogic.DataCenterTypes.receiptCodeType.Item7;
          receipt.ReferenceId = shipPositionReport.MessageId;
          receipt.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
          receipt.test = DataCenterLogic.DataCenterTypes.testType.Item1;
          receipt.TimeStamp = DateTime.UtcNow;

          Message msgout = new Message(receipt);
          msgout.Label = "receipt";

          //Envia mensaje
          QueueManager.Instance().EnqueueOut(msgout);

          log.Error(strError);
          return;
        }
      }

      //ReferenceID puede ser de un report o "" si es SO
      //DataUserProvider es el usuario que nos manda (deberiamos tener contrato con el)
      var pricing = new PricingManager();
      decimal? price = pricing.GetPriceForRequest(shipPositionReport.ReferenceId, shipPositionReport.DataUserProvider);

      //No tengo precio?
      if (price == null)
        log.Warn(string.Format("SendShipPositionReport: Se recibio un reporte {0} de posicion, no podemos poner precio", shipPositionReport.MessageId));

      //Save position report to DB
      using (var dao = new ShipPositionReportDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(shipPositionReport), 0, price);
      }

      log.Info(string.Format("ShipPositionReport successfully processed: price {0}", price));
    }


  }
}
