using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using DataCenterLogic;
using DataCenterDataAccess;
using System.Configuration;

namespace GUIDDPImport
{
  public partial class frmMain : Form
  {
    private BasicConfiguration mBasicConfiguration = null;
    public frmMain()
    {
      InitializeComponent();
      mBasicConfiguration = DataCenterLogic.BasicConfiguration.FromNameValueCollection(System.Configuration.ConfigurationManager.AppSettings);
      DataCenterDataAccess.Config.ConnectionString = mBasicConfiguration.ConnectionString;
    }

    private void btnBrowse_Click(object sender, EventArgs e)
    {
      OpenFileDialog ofd = new OpenFileDialog();
      ofd.Filter = "(*.xml)|*.xml";
      
      if( ofd.ShowDialog() != DialogResult.OK )
        return;
        
      txtDDPFile.Text = ofd.FileName;
    }

    private void btnImport_Click(object sender, EventArgs e)
    {
      //try
      //{
      //  DDPImportHelper ddpImport = new DDPImportHelper();
      //  DataCenterDataAccess.DDPVersion v = new DataCenterDataAccess.DDPVersion();
      //  ddpImport.Import(txtDDPFile.Text, v );
      //}
      //catch(Exception ex)
      //{
      //  MessageBox.Show( ex.ToString() );
      //}

      //PricingManager pm = new PricingManager();
      //var p = pm.GetMyCurrentPriceFor(DataCenterDataAccess.PricesDataAccess.PriceType.PositionReport);
      //p = p;

      DBDataContext c = new DBDataContext(DataCenterDataAccess.Config.ConnectionString);
      var m = new MsgInOut();
      m.MsgType = 9999;
      m.MsgId = "ASPReprog";
      m.TimeStamp = DateTime.UtcNow;
      m.Source = "";
      m.RefId = "";
      m.InOut = 1;
      m.Price = (decimal)44.4;
      m.Destination = "";
      m.DDPVersion = "";

      c.MsgInOuts.InsertOnSubmit(m);
      c.SubmitChanges();
      c.Dispose();
    }

  }
}
