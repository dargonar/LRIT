using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using DataCenterLogic;

namespace TxGUI
{
  public partial class frmMain : Form
  {
    private delegate void OnMessageSentToIDEDelegate(string str);

    public frmMain()
    {
      InitializeComponent();
      
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      OutputMessageManager msgMgr = new OutputMessageManager();
      msgMgr.mBasicConfiguration = BasicConfiguration.FromNameValueCollection(ConfigurationManager.AppSettings);

      DataCenterDataAccess.Config.ConnectionString = msgMgr.mBasicConfiguration.ConnectionString;
      
      msgMgr.Start();
      msgMgr.OnMessageSentToIDE += new OutputMessageManager.MessageSentToIDE(msgMgr_OnMessageSentToIDE);
    }

    void msgMgr_OnMessageSentToIDE(string label)
    {
      this.BeginInvoke( new OnMessageSentToIDEDelegate(OnMessageSentToIDE) , new object[] { label } );
    }

    void OnMessageSentToIDE(string str)
    {
      listBox1.Items.Add( string.Format("Message sent to IDE successfull. type:{0}", str ));
    }

  }
}
