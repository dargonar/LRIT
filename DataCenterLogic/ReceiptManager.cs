using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterLogic.DataCenterTypes;
using DataCenterDataAccess;
using log4net;

namespace DataCenterLogic
{
 
  /// <summary>
  /// Administrador de los recibos de mensajes.
  /// </summary>
  public class ReceiptManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ReceiptManager));
    /// <summary>
    /// Envia un Recibo a el Destino especificado.
    /// </summary>
    /// <param name="Destination">Destino del recibo</param>
    /// <param name="ReferenceId">ID LRIT del mensaje al que hace referencia este recibo</param>
    /// <param name="receiptCode">Codigo del recibo</param>
    /// <param name="msg">Mensaje de texto decribiendo el porque del recibo</param>
    static public void SendReceipt(string Destination, string ReferenceId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType receiptCode, string msg)
    {
      //The ship does not exists in our system, send receipt
      var configMgr = new ConfigurationManager();

      DataCenterTypesIDE.ReceiptType receipt = new DataCenterLogic.DataCenterTypesIDE.ReceiptType();
      receipt.DDPVersionNum = DDPVersionManager.currentDDP();
      receipt.Destination = Destination;
      receipt.Message = msg;
      receipt.MessageId = MessageIdManager.Generate();
      receipt.MessageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType3.Item7;
      receipt.ReceiptCode = receiptCode;
      receipt.Originator = "1005";
      receipt.ReferenceId = ReferenceId;
      receipt.schemaVersion = decimal.Parse(configMgr.Configuration.SchemaVersion);
      receipt.test = DataCenterLogic.DataCenterTypesIDE.testType.Item1;
      receipt.TimeStamp = DateTime.UtcNow;

      //Message msgout = new Message(receipt);
      //msgout.Label = "receipt";

      QueueManager.Instance().EnqueueOut("receipt", new XmlSerializerHelper<DataCenterTypesIDE.ReceiptType>().ToStr(receipt));
    }

    /// <summary>
    /// Procesa un mensaje de tipo Receipt
    /// </summary>
    /// <param name="msg">El mensaje Receipt</param>
    public void ProcessReceipt(ReceiptType receipt)
    {
      decimal? price = null;
      if( receipt.ReceiptCode == receiptCodeType.Item0 && receipt.ReferenceId.Length > 0 )
      {
        //Era un recibo correspondiente a un REQUERIMIENTO?
        var sprm = new ShipPositionRequestDataAccess();
        if (sprm.RequestExists(receipt.ReferenceId) == true)
        {
          var pman = new PricingManager();
          price = pman.GetPriceForRequest(receipt.ReferenceId, receipt.Originator);
          if (price == null)
            log.Warn(string.Format("ProcessReceipt: Se recibio un receipt codigo 0 {0}, no podemos poner precio", receipt.MessageId));
        }
        else
        {
          log.Warn(string.Format("ProcessReceipt: recibo 0 {0} no referido a Requerimiento",receipt.MessageId));
        }
      }

      using (var dao = new ReceiptDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(receipt), 0, price);
      }
      log.Info(string.Format("Receipt successfully processed: price {0}",price));
    }


  }
}
