using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using DataCenterDataAccess;

namespace SchedulerGUI
{
  public partial class frmMain : Form
  {
    private DataCenterLogic.SchedulerManager schedulerManager = new DataCenterLogic.SchedulerManager();
    public frmMain()
    {
      InitializeComponent();

      schedulerManager.mBasicConfiguration = DataCenterLogic.BasicConfiguration.FromNameValueCollection( ConfigurationManager.AppSettings );
      DataCenterDataAccess.Config.ConnectionString = schedulerManager.mBasicConfiguration.ConnectionString;

      schedulerManager.Start();
    }

    private void frmMain_Load(object sender, EventArgs e)
    {
      schedulerManager.OnSystemStatusSent += new DataCenterLogic.SchedulerManager.SystemStatusSent(schedulerManager_OnSystemStatusSent);
      Log("Scheduler started");
    }

    void schedulerManager_OnSystemStatusSent(bool sentOk, object desc)
    {
      this.BeginInvoke( new OnSystemStatusSentDelegate(OnSystemStatusSent) , new object[] { sentOk, desc } );
    }

    private delegate void OnSystemStatusSentDelegate(bool sentOk, object desc);
    void OnSystemStatusSent(bool sentOK, object desc)
    {
      if( sentOK == false )
      {
        Log( "System status sent to out queue failed" );
        Log( desc.ToString() );
      }
      else
      {
        Log( "System status sento to out queue OK" );
      }
    }

    private void Log(string msg)
    {
      lstEvents.Items.Add( string.Format("{0}-{1}", DateTime.UtcNow, msg ) );
    }

    void schedulerManager_OnSystemStatusSent()
    {
      
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      schedulerManager.Stop();
    }

    private void button1_Click(object sender, EventArgs e)
    {
    }

  }
}
