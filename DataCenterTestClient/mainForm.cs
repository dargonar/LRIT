using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Windows.Forms;
using System.Transactions;
using System.Messaging;
using DataCenterLogic;
using log4net;
using log4net.Config;
using System.Configuration;
using DataCenterDataAccess;
using System.Globalization;
using Microsoft.SqlServer.Types;


namespace DataCenterTestClient
{
  public partial class mainForm : Form
  {
    DataCenterTypes.dcPortTypeClient client = new DataCenterTypes.dcPortTypeClient();
    
    private static readonly ILog log = LogManager.GetLogger(typeof(mainForm));
    private BasicConfiguration mBasicConfiguration = BasicConfiguration.FromNameValueCollection(System.Configuration.ConfigurationManager.AppSettings);

    public mainForm()
    {
      DataCenterDataAccess.Config.ConnectionString = mBasicConfiguration.ConnectionString;
      
      //QueueManager.Instance().SetOut( mBasicConfiguration.CoreOutQueue );
      //QueueManager.Instance().SetIn(mBasicConfiguration.CoreInQueue );
      
      //Configure logger
      XmlConfigurator.Configure(); //new System.IO.FileInfo( mBasicConfiguration.Log4NetConfigFile ) );

      client.Endpoint.Address = new System.ServiceModel.EndpointAddress( mBasicConfiguration.DCUrl );
      InitializeComponent();
    }

    private void applog(string msg)
    {
      txtLog.Text += msg;
    }


    #region DC8

    private void dc8BtnDefault_Click_1(object sender, EventArgs e)
    {
        dc8DDPVersionNum.Text = "161:557";
        dc8Originator.Text    = "4321";
        dc8MessageId.Text     = Generate();
        dc8MessageType.Text   = DataCenterTypesIDE.messageTypeType6.Item14.ToString();
        dc8schemaVersion.Text = "1.0";
        dc8Test.Text          = "1";
        dc8TimeStamp.Text     = DateTime.UtcNow.ToString();
    }

    private int nextNum = 1;
    public string Generate()
    {
      return DataCenterLogic.MessageIdManager.Generate();
      //string msgId = string.Format("4321{0:yyyyMMddHHmmss}{1:0####}",
      //                              DateTime.UtcNow, nextNum);
      //return msgId;
    }

    private void dc8BtnSend_Click(object sender, EventArgs e)
    {
      try
      {
          DataCenterTypesIDE.PricingRequestType priceQuest = new DataCenterTypesIDE.PricingRequestType();

          priceQuest.DDPVersionNum = dc8DDPVersionNum.Text;
          priceQuest.MessageId     = dc8MessageId.Text;
          priceQuest.MessageType   = DataCenterTypesIDE.messageTypeType6.Item14;
          priceQuest.Originator    = dc8Originator.Text;
          priceQuest.schemaVersion = decimal.Parse(dc8schemaVersion.Text);
          priceQuest.test          = DataCenterTestClient.DataCenterTypesIDE.testType.Item1;
          priceQuest.TimeStamp = DateTime.UtcNow;
          priceQuest.ReferenceId = dc8ReferenceId.Text;
          
          //System.Messaging.Message msg = new System.Messaging.Message(priceQuest);
          //msg.Label = "priceRequest";

          QueueManager.Instance().EnqueueOut("priceRequest", new XmlSerializerHelper<DataCenterTypesIDE.PricingRequestType>().ToStr(priceQuest));
          applog("Price request successfully enqueued\n\t" + priceQuest.ToString() );
      }
      catch(Exception ex)
      {
        applog("Error trying to send price request\n\t" + ex.ToString());
      }
    }

    #endregion 

    #region DC7
    private void dc7Send_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.PricingNotificationType pricingNotification = new DataCenterTypes.PricingNotificationType();

        pricingNotification.DDPVersionNum = dc7DDPVersionNum.Text;
        pricingNotification.Message       = dc7Message.Text;
        pricingNotification.MessageId     = dc7MessageId.Text;
        pricingNotification.MessageType   = DataCenterTestClient.DataCenterTypes.messageTypeType7.Item13;
        pricingNotification.schemaVersion = decimal.Parse(dc7schemaVersion.Text);
        pricingNotification.test          = DataCenterTestClient.DataCenterTypes.testType.Item1;
        pricingNotification.TimeStamp     = DateTime.UtcNow;

        client.PricingNotification( pricingNotification );
        applog("Pricing Notification Sent OK!\n");
      }
      catch(Exception ex)
      {
        applog("Pricing Notification ERROR \n\t" + ex.ToString());
      }
    }

    private void dc7Default_Click(object sender, EventArgs e)
    {
      dc7DDPVersionNum.Text = "161:557";
      dc7Message.Text       = "Price Notification Message Nature";
      dc7MessageId.Text     = Generate();
      dc7MessageType.Text   = DataCenterTypes.messageTypeType3.Item7.ToString();
      dc7schemaVersion.Text = "1.0";
      dc7Test.Text          = "1";
      dc7TimeStamp.Text     = DateTime.UtcNow.ToString();
    }

    #endregion

    #region DC1
    private void dc1BtnDefault_Click(object sender, EventArgs e)
    {
      var r = new Random();
      int prev = 0;
      dc1ASPId.Text             = "4321";
      dc1DataUserProvider.Text  = "4455";
      dc1DataUserRequestor.Text = "1018";
      dc1DCId.Text              = "3777";
      dc1DDPVersionNum.Text     = "161:557";
      dc1IMONum.Text            = "1234567";
      dc1Latitude.Text          = "10.10.10.n";
      dc1Longitude.Text         = "133.10.10.e";
      dc1MessageId.Text = Generate();
      dc1MessageType.Text       = "1";
      dc1MMSINum.Text           = "123456789";
      dc1ReferenceId.Text       = "";
      dc1ResponseType.Text      = "1";
      dc1schemaVersion.Text     = "1.0";
      dc1ShipborneEquipmentId.Text = "1234";
      dc1ShipName.Text          = "TSho";
      dc1Test.Text              = "1";
      dc1TimeStamp1.Text = DateTime.UtcNow.ToString(); prev = r.Next(60);
      dc1TimeStamp2.Text = DateTime.UtcNow.AddMinutes(prev).ToString(); prev = prev + r.Next(60);
      dc1TimeStamp3.Text = DateTime.UtcNow.AddMinutes(prev).ToString(); prev = prev + r.Next(60);
      dc1TimeStamp4.Text = DateTime.UtcNow.AddMinutes(prev).ToString(); prev = prev + r.Next(60);
      dc1TimeStamp5.Text = DateTime.UtcNow.AddMinutes(prev).ToString();
      
    }

    private void dc1BtnSend_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.ShipPositionReportType report = new DataCenterTypes.ShipPositionReportType();
       
        report.ASPId            = dc1ASPId.Text;
        report.DataUserProvider = dc1DataUserProvider.Text;
        report.DataUserRequestor= dc1DataUserRequestor.Text;
        report.DCId             = dc1DCId.Text;
        report.DDPVersionNum    = dc1DDPVersionNum.Text;
        report.IMONum           = dc1IMONum.Text;
        report.Latitude         = dc1Latitude.Text;
        report.Longitude        = dc1Longitude.Text;
        report.MessageId        = dc1MessageId.Text;
        report.MessageType      = (DataCenterTypes.messageTypeType)int.Parse(dc1MessageType.Text);
        report.MMSINum          = dc1MMSINum.Text;
        report.ReferenceId      = dc1ReferenceId.Text;
        report.ResponseType     = (DataCenterTypes.responseTypeType)int.Parse(dc1ResponseType.Text);
        report.schemaVersion    = decimal.Parse(dc1schemaVersion.Text);
        report.ShipborneEquipmentId = dc1ShipborneEquipmentId.Text;
        report.ShipName         = dc1ShipName.Text;
        report.test             = (DataCenterTypes.testType)int.Parse(dc1Test.Text);
        report.TimeStamp1       = DateTime.Parse( dc1TimeStamp1.Text );
        report.TimeStamp2       = DateTime.Parse( dc1TimeStamp2.Text );
        report.TimeStamp3       = DateTime.Parse( dc1TimeStamp3.Text );
        report.TimeStamp4       = DateTime.Parse( dc1TimeStamp4.Text );
        report.TimeStamp5       = DateTime.Parse( dc1TimeStamp5.Text );

        client.ShipPositionReport( report );
        applog("Position Report Sent OK!\n");
      }
      catch(Exception ex)
      {
        applog("Position Report ERROR \n\t" + ex.ToString());
      }
    }


    #endregion

    #region ToolStrip

    private void modifyReportsAsSentToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        using (DBDataContext context = new DBDataContext(Config.ConnectionString))
        {
          context.ExecuteCommand("UPDATE MsgInOut SET InOut = 1 WHERE Id IN (SELECT ShipPositionReport.MsgInOutId FROM ShipPositionReport WHERE ShipName = 'Test')");
        }
      }
      catch (Exception ex)
      {
        applog(ex.ToString());
      }
    }



    private void showToolStripMenuItem_Click(object sender, EventArgs e)
    {
      frmConfig c = new frmConfig();
      c.Show();
    }


    private void generateReportsForChartsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      for (int i = 168; i >= 0; i--)
      {
        for (int j = 0; j < 2; j++)
        {
          var r = new Random();
          int prev = 0;
          dc1ASPId.Text = "4321";
          dc1DataUserProvider.Text = "4444";
          dc1DataUserRequestor.Text = "1018";
          dc1DCId.Text = "3777";
          dc1DDPVersionNum.Text = "161:557";
          dc1IMONum.Text = "1234567";
          dc1Latitude.Text = "10.10.10.n";
          dc1Longitude.Text = "133.10.10.e";
          dc1MessageId.Text = Generate();
          dc1MessageType.Text = "1";
          dc1MMSINum.Text = "123456789";
          dc1ReferenceId.Text = "0";
          dc1ResponseType.Text = "1";
          dc1schemaVersion.Text = "1.0";
          dc1ShipborneEquipmentId.Text = "1234";
          dc1ShipName.Text = "Test";
          dc1Test.Text = "1";
          dc1TimeStamp1.Text = DateTime.UtcNow.AddDays(-i).ToString(); prev = r.Next(60);
          dc1TimeStamp2.Text = DateTime.UtcNow.AddMinutes(prev).AddDays(-i).ToString(); prev = prev + r.Next(60);
          dc1TimeStamp3.Text = DateTime.UtcNow.AddMinutes(prev).AddDays(-i).ToString(); prev = prev + r.Next(60);
          dc1TimeStamp4.Text = DateTime.UtcNow.AddMinutes(prev).AddDays(-i).ToString(); prev = prev + r.Next(60);
          dc1TimeStamp5.Text = DateTime.UtcNow.AddMinutes(prev).AddDays(-i).ToString();

          try
          {
            DataCenterTypes.ShipPositionReportType report = new DataCenterTypes.ShipPositionReportType();

            report.ASPId = dc1ASPId.Text;
            report.DataUserProvider = dc1DataUserProvider.Text;
            report.DataUserRequestor = dc1DataUserRequestor.Text;
            report.DCId = dc1DCId.Text;
            report.DDPVersionNum = dc1DDPVersionNum.Text;
            report.IMONum = dc1IMONum.Text;
            report.Latitude = dc1Latitude.Text;
            report.Longitude = dc1Longitude.Text;
            report.MessageId = dc1MessageId.Text;
            report.MessageType = (DataCenterTypes.messageTypeType)int.Parse(dc1MessageType.Text);
            report.MMSINum = dc1MMSINum.Text;
            report.ReferenceId = dc1ReferenceId.Text;
            report.ResponseType = (DataCenterTypes.responseTypeType)int.Parse(dc1ResponseType.Text);
            report.schemaVersion = decimal.Parse(dc1schemaVersion.Text);
            report.ShipborneEquipmentId = dc1ShipborneEquipmentId.Text;
            report.ShipName = dc1ShipName.Text;
            report.test = (DataCenterTypes.testType)int.Parse(dc1Test.Text);
            report.TimeStamp1 = DateTime.Parse(dc1TimeStamp1.Text);
            report.TimeStamp2 = DateTime.Parse(dc1TimeStamp2.Text);
            report.TimeStamp3 = DateTime.Parse(dc1TimeStamp3.Text);
            report.TimeStamp4 = DateTime.Parse(dc1TimeStamp4.Text);
            report.TimeStamp5 = DateTime.Parse(dc1TimeStamp5.Text);

            client.ShipPositionReport(report);
            applog("Position Report Sent OK!\n");
          }
          catch (Exception ex)
          {
            applog("Position Report ERROR \n\t" + ex.ToString());
          }
        }
      }
    }


    private void resetDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterDataAccess.DBCreate.Run(mBasicConfiguration.ConnectionString);
        txtLog.Text = "Database Recreated OK.";
      }
      catch (Exception ex)
      {
        txtLog.Text = ex.ToString();
      }
    }


    #endregion

    #region DC4
    private void dc4BtnDefault_Click(object sender, EventArgs e)
    {
        dc4DDPVersionNum.Text = "161:557";
        dc4Destination.Text   = "4321";
        dc4Message.Text       = "Text Message Indicating Nature of Receipt";
        dc4MessageId.Text = Generate();
        dc4MessageType.Text   = DataCenterTypes.messageTypeType3.Item7.ToString();
        dc4Originator.Text    = "3333";
        dc4ReceiptCode.Text   = "0";
        dc4ReferenceId.Text   = "";
        dc4schemaVersion.Text = "1.0";
        dc4test.Text          = "1";
        dc4TimeStamp.Text     = DateTime.UtcNow.ToString();
    }

    private void dc4SendReceipt_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.ReceiptType receipt = new DataCenterTypes.ReceiptType();
        
        receipt.DDPVersionNum   = dc4DDPVersionNum.Text;
        receipt.Destination     = dc4Destination.Text;
        receipt.Message         = dc4Message.Text;
        receipt.MessageId       = dc4MessageId.Text;
        receipt.MessageType     = DataCenterTypes.messageTypeType3.Item7;
        receipt.Originator      = dc4Originator.Text;
        receipt.ReceiptCode = (DataCenterTypes.receiptCodeType)Enum.Parse(typeof(DataCenterTypes.receiptCodeType), "Item" + dc4ReceiptCode.Text);
        receipt.ReferenceId     = dc4ReferenceId.Text;
        receipt.schemaVersion   = decimal.Parse(dc4schemaVersion.Text);
        receipt.test            = (DataCenterTypes.testType)int.Parse(dc4test.Text);
        receipt.TimeStamp       = DateTime.Parse(dc4TimeStamp.Text);

        client.Receipt( receipt );
        applog("Receipt Sent OK!\n");
      }
      catch(Exception ex)
      {
        applog("Receipt Sent ERROR \n\t" + ex.ToString());
      }
    }
    #endregion

    #region DC9

    private void dc9Default_Click(object sender, EventArgs e)
    {
      dc9DDPVersionNum.Text = "161:557";
      dc9Message.Text       = "Price Update Message Nature";
      dc9MessageId.Text = Generate();
      dc9MessageType.Text   = DataCenterTypes.messageTypeType8.Item15.ToString();
      dc9PricingFile.Text   = "C:\\pricingFile1.xml";
      dc9SchemaVersion.Text = "1.0";
      dc9Test.Text          = "1";
      dc9TimeStamp.Text     = DateTime.UtcNow.ToString();
    }

    private void dc9Send_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.PricingUpdateType  pricingUpdate = new DataCenterTypes.PricingUpdateType();

        pricingUpdate.DDPVersionNum = dc9DDPVersionNum.Text;
        pricingUpdate.Message       = dc9Message.Text;
        pricingUpdate.MessageId     = dc9MessageId.Text;
        pricingUpdate.MessageType   = DataCenterTypes.messageTypeType8.Item15;
        pricingUpdate.PricingFile   = System.IO.File.ReadAllBytes(dc9PricingFile.Text);
        pricingUpdate.schemaVersion = decimal.Parse(dc9SchemaVersion.Text);
        pricingUpdate.test = DataCenterTestClient.DataCenterTypes.testType.Item1;
        pricingUpdate.TimeStamp = DateTime.UtcNow;
        pricingUpdate.ReferenceId = dc9ReferenceId.Text;


        client.PricingUpdate( pricingUpdate );
        applog("Pricing Update Sent ok\n\t");
      }
      catch(Exception ex)
      {
        applog("Error Sending Pricing Update\n\t" + ex.ToString() );
      }
    }

    #endregion

    #region DC10

    private void dc10Default_Click(object sender, EventArgs e)
    {
      dc10DDPVersionNum.Text = "161:557";
      dc10Message.Text       = "Price Update Message Nature";
      dc10MessageId.Text = Generate();
      dc10MessageType.Text   = DataCenterTypesIDE.messageTypeType7.Item15.ToString();
      dc10PricingFile.Text   = "C:\\pricingFile2.xml";
      dc10SchemaVersion.Text = "1.0";
      dc10Test.Text          = "1";
      dc10TimeStamp.Text     = DateTime.UtcNow.ToString();
    }

    private void dc10Send_Click(object sender, EventArgs e)
    {
      try
      {
        using( TransactionScope ts = new TransactionScope() )
        {
          DataCenterTypesIDE.PricingUpdateType  pricingUpdate = new DataCenterTypesIDE.PricingUpdateType();

          pricingUpdate.DDPVersionNum = dc10DDPVersionNum.Text;
          pricingUpdate.Message       = dc10Message.Text;
          pricingUpdate.MessageId     = dc10MessageId.Text;
          pricingUpdate.MessageType   = DataCenterTypesIDE.messageTypeType7.Item15;
          pricingUpdate.PricingFile   = System.IO.File.ReadAllBytes(dc10PricingFile.Text);
          pricingUpdate.schemaVersion = decimal.Parse(dc10SchemaVersion.Text);
          pricingUpdate.test          =  DataCenterTestClient.DataCenterTypesIDE.testType.Item1;
          pricingUpdate.TimeStamp     = DateTime.UtcNow;
          pricingUpdate.ReferenceId = dc10ReferenceId.Text;

          
          //System.Messaging.Message msg = new System.Messaging.Message(pricingUpdate);
          //msg.Label = "pricingUpdate";

          QueueManager.Instance().EnqueueOut("pricingUpdate", new XmlSerializerHelper<DataCenterTypesIDE.PricingUpdateType>().ToStr(pricingUpdate));
          
          ts.Complete();
          applog("Pricing Update successfully enqueued\n\t" + pricingUpdate.ToString() );
        }
      }
      catch(Exception ex)
      {
        applog("Error trying to send pricing update\n\t" + ex.ToString());
      }

    }

    #endregion

    #region DC14

    private void dc14Default_Click(object sender, EventArgs e)
    {
      dc14DDPVersionNum.Text = "161:557";
      dc14Message.Text       = "Journal Report Message Nature";
      dc14MessageId.Text = Generate();
      dc14MessageType.Text   = DataCenterTypesIDE.messageTypeType7.Item15.ToString();
      dc14JournalFile.Text   = "C:\\JournalFile.xml";
      dc14SchemaVersion.Text = "1.0";
      dc14Test.Text          = "1";
      dc14Originator.Text    = "4321";
      
      dc14TimeStamp.Text     = DateTime.UtcNow.ToString();
    }

    private void dc14Send_Click(object sender, EventArgs e)
    {
      try
      {
        using( TransactionScope ts = new TransactionScope() )
        {
          DataCenterTypesIDE.JournalReportType  journalReport = new DataCenterTypesIDE.JournalReportType();

          
          journalReport.DDPVersionNum = dc14DDPVersionNum.Text;
          journalReport.Message       = dc14Message.Text;
          journalReport.MessageId     = dc14MessageId.Text;
          journalReport.MessageType   = DataCenterTypesIDE.messageTypeType5.Item12;
          journalReport.Originator    = dc14Originator.Text;
          journalReport.JournalFile   = System.IO.File.ReadAllBytes(dc14JournalFile.Text);
          journalReport.schemaVersion = decimal.Parse(dc14SchemaVersion.Text);
          journalReport.test          = (DataCenterTypesIDE.testType)int.Parse(dc14Test.Text);
          journalReport.TimeStamp     = DateTime.Parse(dc14TimeStamp.Text);
          

          //System.Messaging.Message msg = new System.Messaging.Message(journalReport);
          //msg.Label = "journalReport";

          QueueManager.Instance().EnqueueOut("journalReport", new XmlSerializerHelper<DataCenterTypesIDE.JournalReportType>().ToStr(journalReport));

          ts.Complete();
          applog("Journal Report successfully enqueued\n\t" + journalReport.ToString() );
        }
      }
      catch(Exception ex)
      {
        applog("Error trying to send journal report\n\t" + ex.ToString());
      }
    }

    #endregion

    #region DC3


    private void DC3Default_Click(object sender, EventArgs e)
    {
      dc3DataUserRequestor.Text = "1018";
      dc3DDPVersionNum.Text = "161:557";
      dc3ItemElementName.Text   = "rectangular";
      dc3MessageId.Text = Generate();
      dc3NumberOfPositions.Text = "4";
      dc3schemaVersion.Text     = "1.0";
      dc3Item.Text = "43.00.S:055.00.W:00.00.N:000.00.E";
      dc3TimeStamp.Text         = DateTime.UtcNow.ToString();
    }

    private void DC3Default2_Click(object sender, EventArgs e)
    {
      dc3DataUserRequestor.Text = "1018";
      dc3DDPVersionNum.Text = "161:557";
      dc3ItemElementName.Text = "circular";
      dc3MessageId.Text = Generate();
      dc3NumberOfPositions.Text = "4";
      dc3schemaVersion.Text = "1.0";
      dc3Item.Text = "43.00.S:055.00.W:500";
      dc3TimeStamp.Text = DateTime.UtcNow.ToString();
    }

    private void DC3Send_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.SARSURPICType SARSURPICMsg = new DataCenterTestClient.DataCenterTypes.SARSURPICType();

        SARSURPICMsg.DataUserRequestor  = dc3DataUserRequestor.Text;
        SARSURPICMsg.DDPVersionNum      = dc3DDPVersionNum.Text;
        SARSURPICMsg.Item               = dc3Item.Text;

        if( dc3ItemElementName.Text != "rectangular" )
          SARSURPICMsg.ItemElementName    = DataCenterTestClient.DataCenterTypes.ItemChoiceType1.SARCircularArea;
        else
          SARSURPICMsg.ItemElementName    = DataCenterTestClient.DataCenterTypes.ItemChoiceType1.SARRectangularArea;

        SARSURPICMsg.MessageId          = dc3MessageId.Text;
        SARSURPICMsg.MessageType        = DataCenterTestClient.DataCenterTypes.messageTypeType2.Item6;
        SARSURPICMsg.NumberOfPositions  = dc3NumberOfPositions.Text;
        SARSURPICMsg.schemaVersion      = decimal.Parse( dc3schemaVersion.Text );
        SARSURPICMsg.test               = DataCenterTestClient.DataCenterTypes.testType.Item1;
        SARSURPICMsg.TimeStamp          = DateTime.Parse( dc3TimeStamp.Text );

        client.SARSURPICRequest( SARSURPICMsg );
        applog("SARSURPIC Request sent OK");
      }
      catch(Exception ex)
      {
        applog("Error trying to send SARSURPIC Request\n\t" + ex.ToString());
      }
    }

    #endregion

    #region DC11

    private void dc11Default_Click(object sender, EventArgs e)
    {
      dc11Message.Text        = "Meeage indicating nature of update";
      dc11MessageId.Text = Generate();
      dc11MessageType.Text    = "8";
      dc11NewVersionNum.Text  = "190:1";
      dc11schemaVersion.Text  = "1.1";
      dc11test.Text           = "1";
      dc11TimeStamp.Text      = DateTime.UtcNow.ToString();
      dc11UpdateType.Text     = DataCenterLogic.DataCenterTypes.DDPNotificationTypeUpdateType.Item0.ToString();
    }

    private void dc11Send_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.DDPNotificationType ddpNotification = new DataCenterTestClient.DataCenterTypes.DDPNotificationType();

        ddpNotification.Message       = dc11Message.Text;
        ddpNotification.MessageId     = dc11MessageId.Text;
        ddpNotification.MessageType   = DataCenterTestClient.DataCenterTypes.messageTypeType4.Item8;
        ddpNotification.NewVersionNum = dc11NewVersionNum.Text;
        ddpNotification.schemaVersion = decimal.Parse(dc11schemaVersion.Text);
        ddpNotification.test          = DataCenterTestClient.DataCenterTypes.testType.Item1;
        ddpNotification.TimeStamp     = DateTime.Parse( dc11TimeStamp.Text );
        ddpNotification.UpdateType    = DataCenterTestClient.DataCenterTypes.DDPNotificationTypeUpdateType.Item1;

        client.DDPNotification( ddpNotification );
        applog("DDP Notification sent ok");
      }
      catch(Exception ex)
      {
        applog("Error sending DDP Notification\t\n" + ex.ToString() );
      }
    }
    
    #endregion

    #region DC12

    private void dc12Default_Click(object sender, EventArgs e)
    {
      dc12ArchivedDDPTimeStamp.Text           = DateTime.UtcNow.ToString();
      dc12ArchivedDDPTimeStampSpecified.Text  = bool.TrueString;
      dc12ArchivedDDPVersionNum.Text          = "161:557";
      dc12DDPVersionNum.Text                  = "161:557";
      dc12MessageId.Text                      = Generate();
      dc12MessageType.Text                    = "9";
      dc12Originator.Text                     = "ORIG";
      dc12schemaVersion.Text                  = "1.0";
      dc12test.Text                           = "1";
      dc12TimeStamp.Text                      = DateTime.UtcNow.ToString();
      dc12UpdateType.Text                     = "0";
    }

    private void dc12Send_Click(object sender, EventArgs e)
    {
      try
      {
        using( TransactionScope ts = new TransactionScope() )
        {
          DDPServerTypes.DDPRequestType ddpRequest = new DataCenterTestClient.DDPServerTypes.DDPRequestType();

          ddpRequest.ArchivedDDPTimeStamp = DateTime.Parse(dc12ArchivedDDPTimeStamp.Text).ToUniversalTime();
          ddpRequest.ArchivedDDPTimeStampSpecified = bool.Parse(dc12ArchivedDDPTimeStampSpecified.Text);
          ddpRequest.ArchivedDDPVersionNum         = dc12ArchivedDDPVersionNum.Text;
          ddpRequest.DDPVersionNum                 = dc12DDPVersionNum.Text;
          ddpRequest.MessageId                     = dc12MessageId.Text;
          ddpRequest.MessageType                   = DataCenterTestClient.DDPServerTypes.messageTypeType.Item9;
          ddpRequest.Originator                    = dc12Originator.Text;
          ddpRequest.schemaVersion                 = decimal.Parse(dc12schemaVersion.Text);
          ddpRequest.test                          = DataCenterTestClient.DDPServerTypes.testType.Item1;
          ddpRequest.TimeStamp                     = DateTime.UtcNow;
          
          switch( dc12UpdateType.Text)
          {
            case "0":
              ddpRequest.UpdateType  = DataCenterTestClient.DDPServerTypes.DDPRequestTypeUpdateType.Item0;
              break;
            case "1":
              ddpRequest.UpdateType  = DataCenterTestClient.DDPServerTypes.DDPRequestTypeUpdateType.Item1;
              break;
            case "2":
              ddpRequest.UpdateType  = DataCenterTestClient.DDPServerTypes.DDPRequestTypeUpdateType.Item2;
              break;
            case "3":
              ddpRequest.UpdateType  = DataCenterTestClient.DDPServerTypes.DDPRequestTypeUpdateType.Item3;
              break;
            case "4":
              ddpRequest.UpdateType  = DataCenterTestClient.DDPServerTypes.DDPRequestTypeUpdateType.Item4;
              break;
            default:
              MessageBox.Show("Unknown UpdateType");
              return;
          }

          //System.Messaging.Message msg = new System.Messaging.Message(ddpRequest);
          //msg.Label = "ddpRequest";

          //var serializer = new System.Xml.Serialization.XmlSerializer(msg.Body.GetType());
          //var stringWriter = new System.IO.StringWriter();
          //serializer.Serialize(stringWriter, msg.Body);

          var xmlmsg = new XmlSerializerHelper<DDPServerTypes.DDPRequestType>().ToStr(ddpRequest);
          applog( xmlmsg + "\n\n\n");
          QueueManager.Instance().EnqueueOut("ddpRequest", xmlmsg);
          
          ts.Complete();
          applog("DDP Request successfully enqueued\n\t" + ddpRequest.ToString() );
        }
      }
      catch(Exception ex)
      {
        applog("Error trying to send ddp request\n\t" + ex.ToString());
      }
    }

    #endregion

    #region DC13

    private void dc13Default_Click(object sender, EventArgs e)
    {
      dc13DDPFile.Text              = @"C:\Documents and Settings\Owner\Desktop\ddp-zipfile.zip";
      dc13DDPFileVersionNum.Text    = "191:735";
      dc13Message.Text              = "update ";
      dc13MessageId.Text = Generate();
      dc13MessageType.Text          = "10";
      dc13schemaVersion.Text        = "1.0";
      dc13test.Text                 = "1";
      dc13TimeStamp.Text            = DateTime.UtcNow.ToString();
      dc13UpdateType.Text           = "3";
    }

    private void dc13Send_Click(object sender, EventArgs e)
    {
      try
      {
        var ddpUpdate = new DataCenterTypes.DDPUpdateType();

        ddpUpdate.DDPFile           = System.IO.File.ReadAllBytes(dc13DDPFile.Text);
        ddpUpdate.DDPFileVersionNum = dc13DDPFileVersionNum.Text;
        ddpUpdate.Message           = dc13Message.Text;
        ddpUpdate.MessageId         = dc13MessageId.Text;
        ddpUpdate.MessageType       = DataCenterTestClient.DataCenterTypes.messageTypeType5.Item10;
        ddpUpdate.schemaVersion     = decimal.Parse(dc13schemaVersion.Text);
        ddpUpdate.test              = DataCenterTestClient.DataCenterTypes.testType.Item1;
        ddpUpdate.TimeStamp         = DateTime.Parse(dc13TimeStamp.Text);
        ddpUpdate.ReferenceId       = "43212010040619384200001";
        
        switch( dc13UpdateType.Text )
        {
          case "0":
            ddpUpdate.UpdateType  = DataCenterTestClient.DataCenterTypes.DDPUpdateTypeUpdateType.Item0;
            break;
          case "1":
            ddpUpdate.UpdateType  = DataCenterTestClient.DataCenterTypes.DDPUpdateTypeUpdateType.Item1;
            break;
          case "2":
            ddpUpdate.UpdateType  = DataCenterTestClient.DataCenterTypes.DDPUpdateTypeUpdateType.Item2;
            break;
          case "3":
            ddpUpdate.UpdateType  = DataCenterTestClient.DataCenterTypes.DDPUpdateTypeUpdateType.Item3;
            break;
          case "4":
            ddpUpdate.UpdateType  = DataCenterTestClient.DataCenterTypes.DDPUpdateTypeUpdateType.Item4;
            break;
          default:
            MessageBox.Show("Unknown UpdateType");
            return;
        }

        client.DDPUpdate( ddpUpdate );
        applog("DDP Update sent ok");
      }
      catch(Exception ex)
      {
        applog("Error sending DDP Update\t\n" + ex.ToString() );
      }
    }

    #endregion

    #region DC2

    

    private void dc2Send_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.ShipPositionRequestType report = new DataCenterTypes.ShipPositionRequestType();

        report.MessageType = (DataCenterTypes.messageTypeType1)Enum.Parse(typeof(DataCenterTypes.messageTypeType1), "Item" + dc2MessageType.Text);
        report.MessageId    = dc2MessageId.Text;
        report.IMONum       = dc2IMONum.Text;
        report.DataUserProvider = dc2DataUserProvider.Text;
        report.AccessType = (DataCenterTypes.accessTypeType)Enum.Parse(typeof(DataCenterTypes.accessTypeType), "Item" + dc2AccessType.Text);

        if (dc2ItemElement.Text.Trim().Length == 0)
        {
          report.Item = null;
          //report.ItemElementName = ;
          report.Distance = "0";
        }
        else
        {
          report.Item = dc2ItemField.Text;
          report.ItemElementName = (DataCenterTypes.ItemChoiceType)Enum.Parse(typeof(DataCenterTypes.ItemChoiceType), dc2ItemElement.Text);
          report.Distance = dc2Distance.Text;
        }


        report.RequestType = (DataCenterTypes.requestTypeType)Enum.Parse(typeof(DataCenterTypes.requestTypeType), "Item" + dc2RequestType.Text);
        report.RequestDuration = new DataCenterTestClient.DataCenterTypes.requestDurationType();
        report.RequestDuration.startTimeSpecified = dc2StartTimechk.Checked;
        report.RequestDuration.startTime = dc2StartTime.Value;

        report.RequestDuration.stopTime = dc2StopTime.Value;
        report.RequestDuration.stopTimeSpecified = dc2StartTimechk.Checked;

        if (report.RequestDuration.startTimeSpecified == false && report.RequestDuration.stopTimeSpecified == false)
          report.RequestDuration = null;

        report.DataUserRequestor = dc2DataUserRequestor.Text;
        report.TimeStamp = DateTime.Parse(dc2TimeStamp.Text);
        report.DDPVersionNum = dc2DDPVersionNum.Text;
        report.schemaVersion = decimal.Parse(dc2SchemaVersion.Text);
        report.test = (DataCenterTypes.testType)int.Parse(dc2Test.Text);

        client.ShipPositionRequest(report);
        applog("Position Report Sent OK!\n");
      }
      catch (Exception ex)
      {
        applog("Position Report ERROR \n\t" + ex.ToString());
      }      
    }

    private void dc2Add45secs_Click(object sender, EventArgs e)
    {
      var startt = dc2StartTime.Value.AddSeconds(45);
      dc2StopTime.Value = startt;
      txtLog.Text = dc2StopTime.Value.ToString();
    }



    private void SetUtcNow_Click(object sender, EventArgs e)
    {
      dc2StartTime.Value = DateTime.UtcNow;
      dc2StopTime.Value = DateTime.UtcNow;
    }
    


    private void dc2Add45secs_Click_1(object sender, EventArgs e)
    {
      if (txtLog.Text == "")
        dc2StopTime.Value = dc2StartTime.Value.AddSeconds(45);
      else
      {
        try
        {
          dc2StopTime.Value = DateTime.Parse(txtLog.Text).AddSeconds(45);
        }
        catch (Exception)
        {
          txtLog.Text = "";
          txtLog.Text = dc2StopTime.Value.ToString();
          dc2Add45secs_Click_1(sender, e);
        }

      }
      txtLog.Text = "";
      txtLog.Text = dc2StopTime.Value.ToString();
    }

    private void combotest_SelectedIndexChanged(object sender, EventArgs e)
    {
      switch (combotest.SelectedIndex)
      {
        case 0: //2.1
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "1";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "1:0";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 1: // 2.2 (1) Periodic 30' same ship
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "3";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 2: // 2.2 (2) Periodic 15 same ship //
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "2";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1018";
           dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 3: // 2.3 Invalid Parameter 
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = ""; 
            dc2Distance.Text = "invalid data";
            dc2RequestType.Text = "6";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = false;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = false;
            dc2DataUserRequestor.Text = "1018";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 4:  // 2.4 Ship not registered
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "99999999";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "1";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = false;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = false;
            dc2DataUserRequestor.Text = "1018";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 5: // 2.5 Non Archived not entitled //
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "1";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = false;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = false;
            dc2DataUserRequestor.Text = "1153";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 6: // 2.6 Archived not entitled //
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "7";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 7: //archived no records
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "7";
            dc2StartTime.Value = DateTime.UtcNow.AddYears(-2);
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddYears(-1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
        case 8: //archived no ships
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "69";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "7";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }
                 
         case 9: // 2.9 Archived valid
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1122334";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "7";
            dc2StartTime.Value = new DateTime(2009, 6, 8, 12, 00, 00);
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = new DateTime(2009, 7, 12, 12, 00, 00);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }

         case 10: // 2.2 (1) Periodic 30' same ship
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "4";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1065";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }

         case 11: // 2.2 (1) Periodic 30' same ship
          {
            dc2MessageType.Text = "4";
            dc2MessageId.Text = Generate();
            dc2IMONum.Text = "1";
            dc2DataUserProvider.Text = "4444";
            dc2AccessType.Text = "1";
            dc2ItemField.Text = "";
            dc2ItemElement.Text = "";
            dc2Distance.Text = "";
            dc2RequestType.Text = "3";
            dc2StartTime.Value = DateTime.UtcNow;
            dc2StartTimechk.Checked = true;
            dc2StopTime.Value = DateTime.UtcNow.AddMonths(1);
            dc2StopTimechk.Checked = true;
            dc2DataUserRequestor.Text = "1023";
            dc2TimeStamp.Text = DateTime.UtcNow.ToString();
            dc2DDPVersionNum.Text = "161:557";
            dc2Test.Text = "0";
            dc2SchemaVersion.Text = "1.0";
            break;
          }


        default:
          MessageBox.Show("Unknown UpdateType");
          return;
      }
    }

    #endregion

    #region SPG

    private void spgPreview_Click(object sender, EventArgs e)
    {
     try
     {
        bool sentidoLong = false;
        bool sentidoLat = false;
        double deltaLong = 0;
        double deltaLat = 0;
        
        ////
        double a = double.Parse(spgStartLong.Text);
        double b = double.Parse(spgEndLong.Text);
        if (a > b)
        {
          deltaLong = System.Math.Abs((a - b)) / double.Parse(spgSamples.Text);
          sentidoLong = false;
        }
        else
        {
          deltaLong = System.Math.Abs((b - a)) / double.Parse(spgSamples.Text);
          sentidoLong = true;
        }

        ////////
        a = double.Parse(spgStartLat.Text);
        b = double.Parse(spgEndLat.Text);
        if (a > b)
        {
          deltaLat = System.Math.Abs((a - b)) / double.Parse(spgSamples.Text);
          sentidoLat = false;
        }
        else
        {
          deltaLat = System.Math.Abs((b - a)) / double.Parse(spgSamples.Text);
          sentidoLat = true;
        }
        
        /////////
        TimeSpan deltaSecs = spgEndTime.Value - spgStartTime.Value;
        double deltaSecsInt =  deltaSecs.TotalSeconds / double.Parse(spgSamples.Text);
        deltaSecsInt = System.Math.Abs(deltaSecsInt);
        
        List<ShipPosition> sp = new List<ShipPosition>();

        for (int i = 0; i < int.Parse(spgSamples.Text); i++)
        {
          DataCenterDataAccess.ShipPosition p = new DataCenterDataAccess.ShipPosition();

          p.ShipId = int.Parse(spgShipId.Text);
          p.TimeStamp  = spgStartTime.Value.AddSeconds(deltaSecsInt * i);
          p.TimeStampInASP  = p.TimeStamp.AddMinutes(5);
          p.TimeStampOutASP = p.TimeStampInASP.AddMinutes(5);
          p.TimeStampInDC   = p.TimeStampOutASP.AddMinutes(5);
          
          string sLongitude,sLatitude;
          if (sentidoLong == true)
            sLongitude = (double.Parse(spgStartLong.Text) + (deltaLong * i)).ToString("F2", CultureInfo.InvariantCulture);
          else
            sLongitude = (double.Parse(spgStartLong.Text) - (deltaLong * i)).ToString("F2", CultureInfo.InvariantCulture);
          if (sentidoLat == true)
            sLatitude = (double.Parse(spgStartLat.Text) + (deltaLat * i)).ToString("F2", CultureInfo.InvariantCulture);
          else
            sLatitude = (double.Parse(spgStartLat.Text) - (deltaLat * i)).ToString("F2", CultureInfo.InvariantCulture);

          var geom = SqlGeography.STGeomFromText(new System.Data.SqlTypes.SqlChars("POINT(" + sLongitude + " " + sLatitude + ")"), 4326);
          p.Position = geom.STAsBinary().Buffer;

          sp.Add(p);
        }

        ShipPositionManager spmgr = new ShipPositionManager();
        spmgr.Insert(sp.ToArray());
      }
      catch (Exception ex)
      {
        txtLog.Text=ex.ToString();
      }
    }

    private void spgAdd_Click(object sender, EventArgs e)
    {
      spgStartLong.Text = "-20";
      spgStartLat.Text = "16";
      spgEndLong.Text = "-40";
      spgEndLat.Text = "-40";
      spgStartTime.Value = DateTime.UtcNow.AddMonths(-2);
      spgEndTime.Value = DateTime.UtcNow;
      spgShipId.Text = "1";
      spgSamples.Text = "30";
    }

    private void spgAdd2_Click(object sender, EventArgs e)
    {
      spgStartLong.Text = "40";
      spgStartLat.Text = "8";
      spgEndLong.Text = "80";
      spgEndLat.Text = "0";
      spgStartTime.Value = DateTime.UtcNow.AddMonths(-5);
      spgEndTime.Value = DateTime.UtcNow;
      spgShipId.Text = "2";
      spgSamples.Text = "40";
    }

    private void spgSetNow_Click(object sender, EventArgs e)
    {
      spgStartTime.Value = DateTime.UtcNow;
      spgEndTime.Value = DateTime.UtcNow;
    }

    #endregion


    #region System
    private void dcSSEnd_Click(object sender, EventArgs e)
    {
      try
      {
        DataCenterTypes.SystemStatusType status = new DataCenterTestClient.DataCenterTypes.SystemStatusType();

        status.MessageType = DataCenterTypes.messageTypeType6.Item11;
        status.MessageId = Generate();
        status.SystemStatus = 0;
        status.Message = "Ide is working properly";
        status.Originator = "1234";
        status.DDPVersionNum = "161:557";
        status.TimeStamp = DateTime.UtcNow;
        status.test = (DataCenterTypes.testType)int.Parse("0");
        status.schemaVersion = decimal.Parse("1.1");


        client.SystemStatus(status);
        applog("Ide Status sent ok");
      }
      catch (Exception ex)
      {
        applog("Error sending DStatus\t\n" + ex.ToString());
      }
    }

    #endregion

    private void tabDC2_Click(object sender, EventArgs e)
    {
      txtLog.Text = "";
    }

    private void txtLog_DoubleClick(object sender, EventArgs e)
    {
      txtLog.Clear();
    }

    private void btn_testQueue_Click(object sender, EventArgs e)
    {
      
      //System.Messaging.Message msg = new System.Messaging.Message("MENSAJE DE PRUEBA");
      //msg.Label = "test message";
      try
      {
        QueueManager.Instance().EnqueueOut("test message", "-no-content-");
        txtLog.Clear();
        txtLog.Text = "Enviado OK";
      }
      catch (Exception ex)
      {
        txtLog.Clear();
        txtLog.Text = "Error: " + ex.ToString();
      }
    }

 



    }
  }
