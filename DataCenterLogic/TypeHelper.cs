using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  /// <summary>
  /// Clase de ayuda para transformar tipos de datos.
  /// Esta clase transforma tipo de datos del webservice a base de datos
  /// </summary>
  public class TypeHelper
  {
    /// <summary>
    /// Transforma un ReceiptType en un Receipt
    /// </summary>
    /// <param name="receipt">ReceiptType</param>
    /// <returns>Receipt</returns>
    static public Receipt Map2DB(DataCenterLogic.DataCenterTypes.ReceiptType receipt)
    {
      Receipt retReceipt      = new Receipt();

      retReceipt.DDPVersion   = receipt.DDPVersionNum;
      retReceipt.Destination  = receipt.Destination;
      retReceipt.Message      = receipt.Message;
      retReceipt.MessageId    = receipt.MessageId;
      retReceipt.MessageType  = ToInt(receipt.MessageType);
      retReceipt.Originator   = receipt.Originator;
      retReceipt.ReceiptCode  = ToInt(receipt.ReceiptCode);
      retReceipt.ReferenceId  = receipt.ReferenceId;
      retReceipt.Test         = ToInt(receipt.test);
      retReceipt.TimeStamp    = receipt.TimeStamp;
      
      return retReceipt;
    }

    /// <summary>
    /// Transforma un ReceiptType en un Receipt
    /// </summary>
    /// <param name="receipt">ReceiptType</param>
    /// <returns>Receipt</returns>
    static public Receipt Map2DB(DataCenterLogic.DataCenterTypesIDE.ReceiptType receipt)
    {
      Receipt retReceipt = new Receipt();

      retReceipt.DDPVersion = receipt.DDPVersionNum;
      retReceipt.Destination = receipt.Destination;
      retReceipt.Message = receipt.Message;
      retReceipt.MessageId = receipt.MessageId;
      retReceipt.MessageType = ToInt(receipt.MessageType);
      retReceipt.Originator = receipt.Originator;
      retReceipt.ReceiptCode = ToInt(receipt.ReceiptCode);
      retReceipt.ReferenceId = receipt.ReferenceId;
      retReceipt.Test = ToInt(receipt.test);
      retReceipt.TimeStamp = receipt.TimeStamp;

      return retReceipt;
    }


    /// <summary>
    /// Transforma un SystemStatusType en un SystemStatusSent
    /// </summary>
    /// <param name="systemStatus">SystemStatusType</param>
    /// <returns>SystemStatusSent</returns>
    static public SystemStatus Map2DB( DataCenterLogic.DataCenterTypes.SystemStatusType systemStatus )
    {
      SystemStatus retSystemStatus = new SystemStatus();

      retSystemStatus.DDPVersionNum = systemStatus.DDPVersionNum;
      retSystemStatus.Message       = systemStatus.Message;
      retSystemStatus.MessageId     = systemStatus.MessageId;
      retSystemStatus.MessageType   = ToIntStr(systemStatus.MessageType.ToString());
      retSystemStatus.Originator    = systemStatus.Originator;
      retSystemStatus.schemaVersion = systemStatus.schemaVersion;
      retSystemStatus.SystemStatus1 = ToIntStr(systemStatus.SystemStatus.ToString());
      retSystemStatus.test          = ToInt(systemStatus.test);
      retSystemStatus.TimeStamp     = systemStatus.TimeStamp;

      return retSystemStatus;
    }



    static public SystemStatus Map2DB(DataCenterLogic.DataCenterTypesIDE.SystemStatusType systemStatus)
    {
      SystemStatus retSystemStatus = new SystemStatus();

      retSystemStatus.DDPVersionNum = systemStatus.DDPVersionNum;
      retSystemStatus.Message = systemStatus.Message;
      retSystemStatus.MessageId = systemStatus.MessageId;
      retSystemStatus.MessageType = ToInt(systemStatus.MessageType);
      retSystemStatus.Originator = systemStatus.Originator;
      retSystemStatus.schemaVersion = systemStatus.schemaVersion;
      retSystemStatus.SystemStatus1 = ToInt(systemStatus.SystemStatus);
      retSystemStatus.test = ToInt(systemStatus.test);
      retSystemStatus.TimeStamp = systemStatus.TimeStamp;

      return retSystemStatus;
    }



    /// <summary>
    /// Transforma un ShipPositionReportType en un ShipPositionReport
    /// </summary>
    /// <param name="shipPositionReport">ShipPositionReportType</param>
    /// <returns>ShipPositionReport</returns>
    static public ShipPositionReport Map2DB(DataCenterLogic.DataCenterTypes.ShipPositionReportType shipPositionReport)
    {
      ShipPositionReport retShipPositionReport = new ShipPositionReport();
      retShipPositionReport.ASPId             = shipPositionReport.ASPId;
      retShipPositionReport.DataUserProvider  = shipPositionReport.DataUserProvider;
      retShipPositionReport.DataUserRequestor = shipPositionReport.DataUserRequestor;
      retShipPositionReport.DCId              = shipPositionReport.DCId;
      retShipPositionReport.DDPVersionNum     = shipPositionReport.DDPVersionNum;
      retShipPositionReport.IMONum            = shipPositionReport.IMONum;
      retShipPositionReport.Latitude          = shipPositionReport.Latitude;
      retShipPositionReport.Longitude         = shipPositionReport.Longitude;
      retShipPositionReport.MessageId         = shipPositionReport.MessageId;
      retShipPositionReport.MessageType       = ToInt(shipPositionReport.MessageType);
      retShipPositionReport.MMSINum           = shipPositionReport.MMSINum;
      retShipPositionReport.ReferenceId       = shipPositionReport.ReferenceId;
      retShipPositionReport.ResponseType      = ToInt(shipPositionReport.ResponseType);
      retShipPositionReport.schemaVersion     = shipPositionReport.schemaVersion;
      retShipPositionReport.ShipborneEquipmentId = shipPositionReport.ShipborneEquipmentId;
      retShipPositionReport.ShipName          = shipPositionReport.ShipName;
      retShipPositionReport.test              = ToInt(shipPositionReport.test);
      retShipPositionReport.TimeStamp1        = shipPositionReport.TimeStamp1;
      retShipPositionReport.TimeStamp2        = shipPositionReport.TimeStamp2;
      retShipPositionReport.TimeStamp3        = shipPositionReport.TimeStamp3;
      retShipPositionReport.TimeStamp4        = shipPositionReport.TimeStamp4;
      retShipPositionReport.TimeStamp5        = shipPositionReport.TimeStamp5;

      return retShipPositionReport;
    }

    internal static ShipPositionReport Map2DB(DataCenterLogic.DataCenterTypesIDE.ShipPositionReportType shipPositionReport)
    {
      ShipPositionReport retShipPositionReport = new ShipPositionReport();
      retShipPositionReport.ASPId = shipPositionReport.ASPId;
      retShipPositionReport.DataUserProvider = shipPositionReport.DataUserProvider;
      retShipPositionReport.DataUserRequestor = shipPositionReport.DataUserRequestor;
      retShipPositionReport.DCId = shipPositionReport.DCId;
      retShipPositionReport.DDPVersionNum = shipPositionReport.DDPVersionNum;
      retShipPositionReport.IMONum = shipPositionReport.IMONum;
      retShipPositionReport.Latitude = shipPositionReport.Latitude;
      retShipPositionReport.Longitude = shipPositionReport.Longitude;
      retShipPositionReport.MessageId = shipPositionReport.MessageId;
      retShipPositionReport.MessageType = ToInt(shipPositionReport.MessageType);
      retShipPositionReport.MMSINum = shipPositionReport.MMSINum;
      retShipPositionReport.ReferenceId = shipPositionReport.ReferenceId;
      retShipPositionReport.ResponseType = ToInt(shipPositionReport.ResponseType);
      retShipPositionReport.schemaVersion = shipPositionReport.schemaVersion;
      retShipPositionReport.ShipborneEquipmentId = shipPositionReport.ShipborneEquipmentId;
      retShipPositionReport.ShipName = shipPositionReport.ShipName;
      retShipPositionReport.test = ToInt(shipPositionReport.test);
      retShipPositionReport.TimeStamp1 = shipPositionReport.TimeStamp1;
      retShipPositionReport.TimeStamp2 = shipPositionReport.TimeStamp2;
      retShipPositionReport.TimeStamp3 = shipPositionReport.TimeStamp3;
      retShipPositionReport.TimeStamp4 = shipPositionReport.TimeStamp4;
      retShipPositionReport.TimeStamp5 = shipPositionReport.TimeStamp5;

      return retShipPositionReport;

    }

    /// <summary>
    /// Transforma un ShipPositionRequestType en un ShipPositionRequest
    /// </summary>
    /// <param name="shipPositionRequest">ShipPositionRequestType</param>
    /// <returns>ShipPositionRequest</returns>
    public static ShipPositionRequest Map2DB(DataCenterLogic.DataCenterTypes.ShipPositionRequestType shipPositionRequest)
    {
      ShipPositionRequest retShipPositionRequest = new ShipPositionRequest();
      retShipPositionRequest.AccessType = ToInt(shipPositionRequest.AccessType);
      retShipPositionRequest.DataUserProvider = shipPositionRequest.DataUserProvider;
      retShipPositionRequest.DataUserRequestor = shipPositionRequest.DataUserRequestor;
      retShipPositionRequest.DDPVersionNum = shipPositionRequest.DDPVersionNum;
      retShipPositionRequest.Distance = shipPositionRequest.Distance;
      retShipPositionRequest.IMONum = shipPositionRequest.IMONum;
      retShipPositionRequest.Item = shipPositionRequest.Item;
      retShipPositionRequest.MessageId = shipPositionRequest.MessageId;
      retShipPositionRequest.MessageType = ToInt(shipPositionRequest.MessageType);
      retShipPositionRequest.RequestType = ToInt(shipPositionRequest.RequestType);
      retShipPositionRequest.schemaVersion = shipPositionRequest.schemaVersion;

      retShipPositionRequest.StartTimeSpecified = 0;
      if( shipPositionRequest.RequestDuration != null )
        retShipPositionRequest.StartTimeSpecified = ToInt(shipPositionRequest.RequestDuration.startTimeSpecified);

      if (retShipPositionRequest.StartTimeSpecified != 0)
        retShipPositionRequest.StartTime = shipPositionRequest.RequestDuration.startTime;
      else
      {
        retShipPositionRequest.StartTime = DateTime.UtcNow;
        retShipPositionRequest.StartTimeSpecified = 1;
      }

      retShipPositionRequest.StopTimeSpecified = 0;
      if( shipPositionRequest.RequestDuration != null )
        retShipPositionRequest.StopTimeSpecified = ToInt(shipPositionRequest.RequestDuration.stopTimeSpecified);

      if (retShipPositionRequest.StopTimeSpecified != 0)
        retShipPositionRequest.StopTime = shipPositionRequest.RequestDuration.stopTime;
      else
        retShipPositionRequest.StopTime = null;

      retShipPositionRequest.test = ToInt(shipPositionRequest.test);
      retShipPositionRequest.TimeStamp = shipPositionRequest.TimeStamp;

      return retShipPositionRequest;
    }

    /// <summary>
    /// Transforma un ShipPositionRequestType en un ShipPositionRequest
    /// </summary>
    /// <param name="shipPositionRequest">ShipPositionRequestType</param>
    /// <returns>ShipPositionRequest</returns>
    public static ShipPositionRequest Map2DB(DataCenterLogic.DataCenterTypesIDE.ShipPositionRequestType shipPositionRequest)
    {
      ShipPositionRequest retShipPositionRequest = new ShipPositionRequest();
      retShipPositionRequest.AccessType = ToInt(shipPositionRequest.AccessType);
      retShipPositionRequest.DataUserProvider = shipPositionRequest.DataUserProvider;
      retShipPositionRequest.DataUserRequestor = shipPositionRequest.DataUserRequestor;
      retShipPositionRequest.DDPVersionNum = shipPositionRequest.DDPVersionNum;
      retShipPositionRequest.Distance = shipPositionRequest.Distance;
      retShipPositionRequest.IMONum = shipPositionRequest.IMONum;
      retShipPositionRequest.Item = shipPositionRequest.Item;
      retShipPositionRequest.MessageId = shipPositionRequest.MessageId;
      retShipPositionRequest.MessageType = ToInt(shipPositionRequest.MessageType);
      retShipPositionRequest.RequestType = ToInt(shipPositionRequest.RequestType);
      retShipPositionRequest.schemaVersion = shipPositionRequest.schemaVersion;

      retShipPositionRequest.StartTimeSpecified = ToInt(shipPositionRequest.RequestDuration.startTimeSpecified);
      if (retShipPositionRequest.StartTimeSpecified != 0)
        retShipPositionRequest.StartTime = shipPositionRequest.RequestDuration.startTime;
      else
        retShipPositionRequest.StartTime = null;

      retShipPositionRequest.StopTimeSpecified = ToInt(shipPositionRequest.RequestDuration.stopTimeSpecified);
      if (retShipPositionRequest.StopTimeSpecified != 0)
        retShipPositionRequest.StopTime = shipPositionRequest.RequestDuration.stopTime;
      else
        retShipPositionRequest.StopTime = null;

      retShipPositionRequest.test = ToInt(shipPositionRequest.test);
      retShipPositionRequest.TimeStamp = shipPositionRequest.TimeStamp;

      return retShipPositionRequest;
    }


    /// <summary>
    /// Transforma un PricingNotificationType en un PricingNotification
    /// </summary>
    /// <param name="pricingNotification">PricingNotificationType</param>
    /// <returns>PricingNotification</returns>
    static public PricingNotification Map2DB(DataCenterLogic.DataCenterTypes.PricingNotificationType pricingNotification)
    {
      PricingNotification retPricingNotification = new PricingNotification();

      retPricingNotification.DDPVersion  = pricingNotification.DDPVersionNum;
      retPricingNotification.Message     = pricingNotification.Message;
      retPricingNotification.MessageId   = pricingNotification.MessageId;
      retPricingNotification.MessageType = (int)pricingNotification.MessageType;
      retPricingNotification.schemaVersion = pricingNotification.schemaVersion;
      retPricingNotification.Test        = (int)pricingNotification.test;
      retPricingNotification.TimeStamp   = pricingNotification.TimeStamp;

      return retPricingNotification;
    }
    /// <summary>
    /// Transforma un PricingRequestType en un PricingRequestSent
    /// </summary>
    /// <param name="pricingRequest">PricingRequestType</param>
    /// <returns>PricingRequestSent</returns>
    static public PricingRequestSent Map2DB(DataCenterLogic.DataCenterTypesIDE.PricingRequestType pricingRequest)
    {
      PricingRequestSent retPricingRequest = new PricingRequestSent();

      retPricingRequest.DDPVersionNum = pricingRequest.DDPVersionNum;
      retPricingRequest.MessageId     = pricingRequest.MessageId;
      retPricingRequest.MessageType   = ToInt(pricingRequest.MessageType);
      retPricingRequest.Originator    = pricingRequest.Originator;
      retPricingRequest.schemaVersion = pricingRequest.schemaVersion;
      retPricingRequest.test          = ToInt(pricingRequest.test);
      retPricingRequest.TimeStamp     = pricingRequest.TimeStamp;

      return retPricingRequest;

    }
    /// <summary>
    /// Transforma un PricingUpdateType en un PricingUpdate
    /// </summary>
    /// <param name="pricingUpdate">PricingUpdateType</param>
    /// <returns>PricingUpdate</returns>
    public static PricingUpdate Map2DB(DataCenterLogic.DataCenterTypes.PricingUpdateType pricingUpdate)
    {
      PricingUpdate retPricingUpdate = new PricingUpdate();

      retPricingUpdate.DDPVersionNum = pricingUpdate.DDPVersionNum;
      retPricingUpdate.Message       = pricingUpdate.Message;
      retPricingUpdate.MessageId     = pricingUpdate.MessageId;
      retPricingUpdate.MessageType   = ToInt(pricingUpdate.MessageType);
      retPricingUpdate.PricingFile   = pricingUpdate.PricingFile;
      retPricingUpdate.schemaVersion = pricingUpdate.schemaVersion;
      retPricingUpdate.Test          = ToInt(pricingUpdate.test);
      retPricingUpdate.TimeStamp     = pricingUpdate.TimeStamp;
      
      return retPricingUpdate;
    }
    /// <summary>
    /// Transforma un PricingUpdateType en un PricingUpdateSent
    /// </summary>
    /// <param name="pricingUpdate">PricingUpdateType</param>
    /// <returns>PricingUpdateSent</returns>
    public static PricingUpdate Map2DB(DataCenterLogic.DataCenterTypesIDE.PricingUpdateType pricingUpdate)
    {
      PricingUpdate retPricingUpdate = new PricingUpdate();

      retPricingUpdate.DDPVersionNum = pricingUpdate.DDPVersionNum;
      retPricingUpdate.Message       = pricingUpdate.Message;
      retPricingUpdate.MessageId     = pricingUpdate.MessageId;
      retPricingUpdate.MessageType   = ToInt(pricingUpdate.MessageType);
      retPricingUpdate.PricingFile   = pricingUpdate.PricingFile;
      retPricingUpdate.schemaVersion = pricingUpdate.schemaVersion;
      retPricingUpdate.Test          = ToInt(pricingUpdate.test);
      retPricingUpdate.TimeStamp     = pricingUpdate.TimeStamp;
      
      return retPricingUpdate;      
    }
    /// <summary>
    /// Transforma un JournalReportType en un JournalReportSent
    /// </summary>
    /// <param name="journalReport">JournalReportType</param>
    /// <returns>JournalReportSent</returns>
    public static JournalReportSent Map2DB(DataCenterLogic.DataCenterTypesIDE.JournalReportType journalReport)
    {
      JournalReportSent retJournalReport = new JournalReportSent();

      retJournalReport.DDPVersionNum = journalReport.DDPVersionNum;
      retJournalReport.JournalFile   = journalReport.JournalFile;
      retJournalReport.Message       = journalReport.Message;
      retJournalReport.MessageId     = journalReport.MessageId;
      retJournalReport.MessageType   = ToInt(journalReport.MessageType);
      retJournalReport.Originator    = journalReport.Originator;
      retJournalReport.schemaVersion = journalReport.schemaVersion;
      retJournalReport.Test          = ToInt(journalReport.test);
      retJournalReport.TimeStamp     = journalReport.TimeStamp;

      return retJournalReport;
    }

    /// <summary>
    /// Transforma un SARSURPICType en un SARSURPICRequest
    /// </summary>
    /// <param name="SARSURPICRequest">SARSURPICType</param>
    /// <returns>SARSURPICRequest</returns>
    public static SARSURPICRequest Map2DB(DataCenterLogic.DataCenterTypes.SARSURPICType SARSURPICRequest)
    {
      SARSURPICRequest retSARSURPICRequest = new SARSURPICRequest();

      retSARSURPICRequest.DataUserRequestor = SARSURPICRequest.DataUserRequestor;
      retSARSURPICRequest.DDPVersionNum = SARSURPICRequest.DDPVersionNum;
      retSARSURPICRequest.Item = SARSURPICRequest.Item;
      retSARSURPICRequest.ItemElementName = SARSURPICRequest.ItemElementName.ToString();
      retSARSURPICRequest.MessageId = SARSURPICRequest.MessageId;
      retSARSURPICRequest.MessageType = ToInt(SARSURPICRequest.MessageType);
      retSARSURPICRequest.NumberOfPositions = int.Parse(SARSURPICRequest.NumberOfPositions);
      retSARSURPICRequest.schemaVersion = SARSURPICRequest.schemaVersion;
      retSARSURPICRequest.test = ToInt(SARSURPICRequest.test);
      retSARSURPICRequest.TimeStamp = SARSURPICRequest.TimeStamp;

      return retSARSURPICRequest;
    }

    /// <summary>
    /// Transforma un SARSURPICType en un SARSURPICRequest
    /// </summary>
    /// <param name="SARSURPICRequest">SARSURPICType</param>
    /// <returns>SARSURPICRequest</returns>
    public static SARSURPICRequest Map2DB(DataCenterLogic.DataCenterTypesIDE.SARSURPICType SARSURPICRequest)
    {
      SARSURPICRequest retSARSURPICRequest = new SARSURPICRequest();

      retSARSURPICRequest.DataUserRequestor = SARSURPICRequest.DataUserRequestor;
      retSARSURPICRequest.DDPVersionNum = SARSURPICRequest.DDPVersionNum;
      retSARSURPICRequest.Item = SARSURPICRequest.Item;
      retSARSURPICRequest.ItemElementName = SARSURPICRequest.ItemElementName.ToString();
      retSARSURPICRequest.MessageId = SARSURPICRequest.MessageId;
      retSARSURPICRequest.MessageType = ToInt(SARSURPICRequest.MessageType);
      retSARSURPICRequest.NumberOfPositions = int.Parse(SARSURPICRequest.NumberOfPositions);
      retSARSURPICRequest.schemaVersion = SARSURPICRequest.schemaVersion;
      retSARSURPICRequest.test = ToInt(SARSURPICRequest.test);
      retSARSURPICRequest.TimeStamp = SARSURPICRequest.TimeStamp;

      return retSARSURPICRequest;
    }

    /// <summary>
    /// Transforma un DDPNotificationType en un DDPNotification
    /// </summary>
    /// <param name="ddpNotification">DDPNotificationType</param>
    /// <returns>DDPNotification</returns>
    public static DDPNotification Map2DB(DataCenterLogic.DataCenterTypes.DDPNotificationType ddpNotification)
    {
      DDPNotification retDDPNotification = new DDPNotification();

      retDDPNotification.Message = ddpNotification.Message;
      retDDPNotification.MessageId = ddpNotification.MessageId;
      retDDPNotification.MessageType = ToInt(ddpNotification.MessageType);
      retDDPNotification.NewVersionNum = ddpNotification.NewVersionNum;
      retDDPNotification.schemaVersion = ddpNotification.schemaVersion;
      retDDPNotification.test = ToInt(ddpNotification.test);
      retDDPNotification.TimeStamp = ddpNotification.TimeStamp;
      retDDPNotification.UpdateType = ToInt(ddpNotification.UpdateType);

      return retDDPNotification;
    }

    /// <summary>
    /// Transforma un DDPRequestType en un DDPRequestSent
    /// </summary>
    /// <param name="ddpRequest">DDPRequestType</param>
    /// <returns>DDPRequestSent</returns>
    public static DDPRequestSent Map2DB(DataCenterLogic.DDPServerTypes.DDPRequestType ddpRequest)
    {
      DDPRequestSent retDDPRequest = new DDPRequestSent();

      retDDPRequest.ArchivedDDPTimeStamp = ddpRequest.ArchivedDDPTimeStamp.Year == 1 ? DateTime.UtcNow : ddpRequest.ArchivedDDPTimeStamp;
      retDDPRequest.ArchivedDDPTimeStampSpecified = ddpRequest.ArchivedDDPTimeStampSpecified ? 1 : 0;
      retDDPRequest.ArchivedDDPVersionNum = ddpRequest.ArchivedDDPVersionNum;
      retDDPRequest.DDPVersionNum = ddpRequest.DDPVersionNum;
      retDDPRequest.MessageId = ddpRequest.MessageId;
      retDDPRequest.MessageType = ToInt(ddpRequest.MessageType);
      retDDPRequest.Originator = ddpRequest.Originator;
      retDDPRequest.schemaVersion = ddpRequest.schemaVersion;
      retDDPRequest.test = ToInt(ddpRequest.test);
      retDDPRequest.TimeStamp = ddpRequest.TimeStamp;
      retDDPRequest.UpdateType = ToInt(ddpRequest.UpdateType);

      return retDDPRequest;
    }
    /// <summary>
    /// Transforma un DDPUpdateType en un DDPUpdate
    /// </summary>
    /// <param name="ddpUpdate">DDPUpdateType</param>
    /// <returns>DDPUpdate</returns>
    public static DDPUpdate Map2DB(DataCenterLogic.DataCenterTypes.DDPUpdateType ddpUpdate)
    {
      DDPUpdate retDDPUpdate = new DDPUpdate();

      retDDPUpdate.DDPFile = ddpUpdate.DDPFile;
      retDDPUpdate.DDPFileVersionNum = ddpUpdate.DDPFileVersionNum;
      retDDPUpdate.Message = ddpUpdate.Message;
      retDDPUpdate.MessageId = ddpUpdate.MessageId;
      retDDPUpdate.MessageType = ToInt(ddpUpdate.MessageType);
      retDDPUpdate.schemaVersion = ddpUpdate.schemaVersion;
      retDDPUpdate.test = ToInt(ddpUpdate.test);
      retDDPUpdate.TimeStamp = ddpUpdate.TimeStamp;
      retDDPUpdate.UpdateType = ToInt(ddpUpdate.UpdateType);

      return retDDPUpdate;
    }


    public static int ToInt(DataCenterLogic.DataCenterTypes.responseTypeType responseTypeType)
    {
      return ToIntStr(responseTypeType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType messageTypeType)
    {
      return ToIntStr(messageTypeType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType messageTypeType)
    {
      return ToIntStr(messageTypeType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.responseTypeType responseTypeType)
    {
      return ToIntStr(responseTypeType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType2 messageTypeType2)
    {
      return ToIntStr(messageTypeType2.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType2 messageTypeType2)
    {
      return ToIntStr(messageTypeType2.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType5 messageTypeType5)
    {
      return ToIntStr(messageTypeType5.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType7 messageTypeType7)
    {
      return ToIntStr(messageTypeType7.ToString());
    }

    public static int ToIntStr(string str)
    {
      return int.Parse(str.Substring(str.LastIndexOf("m")+1));
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType8 messageTypeType8)
    {
      return ToIntStr(messageTypeType8.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.systemStatusIndicatorType systemStatusIndicatorType)
    {
      return ToIntStr(systemStatusIndicatorType.ToString());
    }
    
    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType4 messageTypeType4)
    {
      return ToIntStr(messageTypeType4.ToString());
    }
    
    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType6 messageTypeType6)
    {
      return ToIntStr(messageTypeType6.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.testType testType)
    {
      return ToIntStr(testType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.testType testType)
    {
      return ToIntStr(testType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.receiptCodeType receiptCodeType)
    {
      return ToIntStr(receiptCodeType.ToString());
    }
 
    public static int ToInt(DataCenterLogic.DataCenterTypes.receiptCodeType receiptCodeType)
    {
      return ToIntStr(receiptCodeType.ToString());
    }
   
    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType3 messageTypeType3)
    {
      return ToIntStr(messageTypeType3.ToString());
    }
    
    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType3 messageTypeType3)
    {
      return ToIntStr(messageTypeType3.ToString());
    }


    public static int ToInt(bool p)
    {
      if ( p != false ) 
        return 1;
      
      return 0;
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.requestTypeType requestTypeType)
    {
      return ToIntStr(requestTypeType.ToString());
    }
    
    
    
    public static int ToInt(DataCenterLogic.DataCenterTypes.requestTypeType requestTypeType)
    {
      return ToIntStr(requestTypeType.ToString());
    }



    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.messageTypeType1 messageTypeType1)
    {
      return ToIntStr(messageTypeType1.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType1 messageTypeType1)
    {
      return ToIntStr(messageTypeType1.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypesIDE.accessTypeType accessTypeType)
    {
      return ToIntStr(accessTypeType.ToString());
    }



    public static int ToInt(DataCenterLogic.DataCenterTypes.accessTypeType accessTypeType)
    {
      return ToIntStr(accessTypeType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.DDPUpdateTypeUpdateType dDPUpdateTypeUpdateType)
    {
      return ToIntStr(dDPUpdateTypeUpdateType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType5 messageTypeType5)
    {
      return ToIntStr(messageTypeType5.ToString());
    }

    public static int ToInt(DataCenterLogic.DDPServerTypes.DDPRequestTypeUpdateType dDPRequestTypeUpdateType)
    {
      return ToIntStr(dDPRequestTypeUpdateType.ToString());
    }

    public static int ToInt(DataCenterLogic.DDPServerTypes.testType testType)
    {
      return ToIntStr(testType.ToString());
    }

    public static int ToInt(DataCenterLogic.DDPServerTypes.messageTypeType messageTypeType)
    {
      return ToIntStr(messageTypeType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.DDPNotificationTypeUpdateType dDPNotificationTypeUpdateType)
    {
      return ToIntStr(dDPNotificationTypeUpdateType.ToString());
    }

    public static int ToInt(DataCenterLogic.DataCenterTypes.messageTypeType4 messageTypeType4)
    {
      return ToIntStr(messageTypeType4.ToString());
    }


  }
}
