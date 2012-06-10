using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using log4net;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Checksums;
using DataCenterLogic.DataCenterTypes;
using System.IO;


namespace DataCenterLogic
{
  public class PricingManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(PricingManager));
  
    //public void GetProice

    /// <summary>
    /// Procesa un mensaje de tipo PricingUpdate
    /// </summary>
    /// <param name="msg">El mensaje PricingUpdate</param>
    public void ProcessPricingUpdate(PricingUpdateType pricingUpdate)
    {
      PricingImportHelper importer = new PricingImportHelper();
      importer.Import( new MemoryStream(pricingUpdate.PricingFile) );
        
      using (var dao = new PricingUpdateDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(pricingUpdate), 0);
      }
      log.Info("ProcessPricingUpdate: PricingUpdate successfully processed");
    }

    public decimal? GetMyCurrentPriceFor(PricesDataAccess.PriceType type)
    {
      using (var dao = new PricesDataAccess())
      {
        return dao.GetPrice("1005", type);
      }
    }

    public decimal? AddASPReprogrMessage(int inout, string source, string dest)
    {
      var oprice = GetPricesForLDU(dest);
      decimal? price = null;
      if( oprice != null )
      {
        price = oprice.PeriodicRateChange;
      }
      else
      {
        log.Error(string.Format("AddASPReprogrMessage: Unable to get price for {0}!", source));
      }

      var m = new MsgInOut();
      m.MsgType     = 9999;
      m.MsgId       = "ASPReprog";
      m.TimeStamp   = DateTime.UtcNow;
      m.Source      = source;
      m.RefId       = "";
      m.InOut       = inout;
      m.Price       = price;
      m.Destination = dest;
      m.DDPVersion  = "";
      
      using (var dao = new MsgInOutDataAccess())
      {
        dao.Create(m);
      }

      return price;
    }

    /// <summary>
    /// Procesa un mensaje de tipo PricingNotification
    /// </summary>
    /// <param name="msg">El mensaje PricingNotification</param>
    public void ProcessPricingNotification(PricingNotificationType pricingNotification)
    {
      ConfigurationManager cmgr = new ConfigurationManager();

      DataCenterTypesIDE.PricingRequestType preq = new DataCenterLogic.DataCenterTypesIDE.PricingRequestType();
      preq.DDPVersionNum = DDPVersionManager.currentDDP();
      preq.MessageId = MessageIdManager.Generate();
      preq.MessageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType6.Item14;
      preq.Originator = cmgr.Configuration.DataCenterID;
      preq.ReferenceId = pricingNotification.MessageId;
      preq.schemaVersion = decimal.Parse(cmgr.Configuration.SchemaVersion);
      preq.test = DataCenterLogic.DataCenterTypesIDE.testType.Item0;
      preq.TimeStamp = DateTime.UtcNow;

      //Enqueue DDPrequest
      //Message msgout = new Message(preq);
      //msgout.Label = "priceRequest";

      QueueManager.Instance().EnqueueOut("priceRequest", new XmlSerializerHelper<DataCenterTypesIDE.PricingRequestType>().ToStr(preq));
      
      using (PricingNotificationDataAccess dao = new PricingNotificationDataAccess())
      {
        dao.Create(TypeHelper.Map2DB(pricingNotification), 0);
      }
      log.Info("PricingNotification successfully processed");
    }

    private Price GetPricesForLDU(string provider)
    {
      using (var pda = new PricesDataAccess())
      {
        return pda.GetPrice(provider);
      }
    }

    public decimal? GetPriceForReceipt(string provider)
    {
      using (var pda = new PricesDataAccess())
      {
        var prices = pda.GetPrice(provider);
        if (prices == null)
        {
          log.Error(string.Format("GetPriceForReceipt: price not found for LDU {0}", provider));
          return null;
        }

        return prices.PositionReport;
      }
    }

    public decimal? GetPriceForRequest(string requestMsgId, string ldu)
    {
      decimal? price = null;
      
      var prices = GetPricesForLDU(ldu);
      if (prices == null)
      {
        log.Info("GetPriceForRequest: GetPricesForLDU: no prices found for " + ldu);
        return price;
      }

      var sprm = new ShipPositionRequestManager();
      var request = sprm.GetByMessageID(requestMsgId);

      if (request == null)
      {
        //No existe el mensaje que origina este reporte ==> es por standing order
        price = prices.PositionReport;
        log.Info("GetPriceForRequest: Request not found, using price = PositionReport");
      }
      else if (request.MessageType == 5)
      {
        //Was it a SAR request?
        price = 0;
        log.Info("GetPriceForRequest: 0 price => SAR Message");
      }
      else if (request.RequestType == 7)
      {
        //El mensaje que origina es un pedido de datos archivados.
        price = prices.ArchivePositionReport;
        log.Info("GetPriceForRequest: ArchivedPosition, rt=7");
      }
      else if (request.RequestType == 1)
      {
        //El mensaje que origina es un one time poll.
        price = prices.Poll;
        log.Info("GetPriceForRequest: Poll, rt=1");
      }
      else
      {
        //Cualquier otra cosa ==> precio defecto
        log.Info(string.Format("GetPriceForRequest: defaulting to PositionReport, rt=({0})", request.RequestType));
        price = prices.PositionReport;
      }
      
      return price;
    }


    public byte[] GeneratePriceFile(Price price)
    {
      var cfgman = new ConfigurationManager();
      
      var pfile = new PricingFile();
      pfile.PriceList = new PricingFilePriceList[] { new PricingFilePriceList() };
      pfile.PriceList[0].BreakDown = new PricingFilePriceListBreakDown[] { new PricingFilePriceListBreakDown() } ;
      pfile.PriceList[0].BreakDown[0].ArchivePositionReport = (float)price.ArchivePositionReport;
      pfile.PriceList[0].BreakDown[0].currency              = price.currency;
      pfile.PriceList[0].BreakDown[0].PeriodicRateChange    = (float)price.PeriodicRateChange;
      pfile.PriceList[0].BreakDown[0].Poll                  = (float)price.Poll;
      pfile.PriceList[0].BreakDown[0].PositionReport        = (float)price.PositionReport;

      pfile.PriceList[0].dataCentreID           = cfgman.Configuration.DataCenterID;
      pfile.PriceList[0].dataProviderID         = "1005";
      pfile.PriceList[0].effectiveDate          = price.effectiveDate;
      pfile.PriceList[0].effectiveDateSpecified = true;
      pfile.PriceList[0].issueDate              = DateTime.UtcNow;
      pfile.PriceList[0].issueDateSpecified     = true;
      
      pfile.schemaVersion = decimal.Parse(cfgman.Configuration.SchemaVersion);

      var serializer = new System.Xml.Serialization.XmlSerializer(typeof(PricingFile));
      var sin = new MemoryStream();
      serializer.Serialize(sin, pfile);

      var sout = new MemoryStream();
      var oZipStream = new ZipOutputStream(sout); // create zip stream
      var oZipEntry = new ZipEntry("pricing-file.xml");
      oZipStream.PutNextEntry(oZipEntry);
      var rawb = sin.ToArray();
      oZipStream.Write(rawb, 0, rawb.Length);
      oZipStream.Finish();
      oZipStream.Close();

      return sout.ToArray();
    }
  }
}
