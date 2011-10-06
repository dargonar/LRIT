using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataCenterQueues;
using System.Messaging;
using System.Transactions;

namespace DataCenterOutQueueAdder
{
  public partial class frmMain : Form
  {
    MessageQueue queue = QueueManager.GetQueue( Queues.CORE_OUT );

    public frmMain()
    {
      InitializeComponent();
    }

    private void btnAddPriceRequest_Click(object sender, EventArgs e)
    {
      using( TransactionScope ts = new TransactionScope() )
      {
        IDEDataTypes.PricingRequestType priceQuest = new DataCenterOutQueueAdder.IDEDataTypes.PricingRequestType();
        priceQuest.DDPVersionNum = "1.0";
        priceQuest.MessageId     = "mid";
        priceQuest.MessageType   = DataCenterOutQueueAdder.IDEDataTypes.messageTypeType6.Item14;
        priceQuest.Originator    = "ABC";
        priceQuest.schemaVersion = 2.1M;
        priceQuest.test          = DataCenterOutQueueAdder.IDEDataTypes.testType.Item0;
        priceQuest.TimeStamp     = DateTime.UtcNow;
        
        System.Messaging.Message msg = new System.Messaging.Message(priceQuest);
        msg.Label = "priceRequest";

        queue.Send(msg, MessageQueueTransactionType.Automatic);
        ts.Complete();
      }

      

    }
  }
}
