using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using log4net;
using System.Messaging;
using DataCenterLogic.DataCenterTypes;

namespace DataCenterLogic
{
  class SARSURPICManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(SARSURPICManager));
    /// <summary>
    /// Procesa un mensaje de tipo SARSurpicRequest.
    /// </summary>
    /// <param name="msg">Mensaje SARSURPICRequest</param>
    public void ProcessSARSURPICRequest(SARSURPICType SARSURPICRequest)
    {
      var configMgr = new ConfigurationManager();
      var shipPositionManager = new ShipPositionManager();

      List<ShipPosition> shipPositions;

      if (SARSURPICRequest.ItemElementName == DataCenterLogic.DataCenterTypes.ItemChoiceType1.SARCircularArea)
        shipPositions = shipPositionManager.GetLastPositionsInCircularArea(SARSURPICRequest.Item, int.Parse(SARSURPICRequest.NumberOfPositions));
      else
        shipPositions = shipPositionManager.GetLastPositionsInRectangularArea(SARSURPICRequest.Item, int.Parse(SARSURPICRequest.NumberOfPositions));

      if (shipPositions.Count == 0)
      {
        DataCenterTypesIDE.ReceiptType receipt = new DataCenterLogic.DataCenterTypesIDE.ReceiptType();
        receipt.DDPVersionNum = DDPVersionManager.currentDDP();
        receipt.Destination = SARSURPICRequest.DataUserRequestor;
        receipt.Message = "No positions";
        receipt.MessageId = MessageIdManager.Generate();
        receipt.MessageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType3.Item7;
        receipt.Originator = "1005";
        receipt.ReceiptCode = DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7;
        receipt.ReferenceId = SARSURPICRequest.MessageId;
        receipt.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
        receipt.test = DataCenterLogic.DataCenterTypesIDE.testType.Item1;
        receipt.TimeStamp = DateTime.UtcNow;

        //Receipt sent
        Message msgout = new Message(receipt);
        msgout.Label = "receipt";
        QueueManager.Instance().EnqueueOut(msgout);
      }
      else
      {
        using (var shipdao = new ShipDataAccess())
        {

          var sprm = new ShipPositionReportManager();
          foreach (ShipPosition shipPos in shipPositions)
          {
            sprm.SendReport(SARSURPICRequest.DataUserRequestor, shipPos, SARSURPICRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item4, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item3);
          }
        }
      }

      using (var dao = new SARSURPICRequestDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(SARSURPICRequest), 0);
      }
      log.Info("SARSURPICRequest successfully processed");
    }

  }
}
