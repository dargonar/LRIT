using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataCenterLogic;

namespace DataCenterTestClient
{
  public partial class frmConfig : Form
  {
    public frmConfig()
    {
      InitializeComponent();

      //DDPImportHelper helper = new DDPImportHelper();
      //helper.Import(@"D:\wdir\LRIT\lastDoc\DDP.Full.19-65.xml");

      //DDPImportHelper helper = new DDPImportHelper();
      //helper.UpdateIncremental(File.Open(@"C:\Documents and Settings\Owner\Desktop\DDP.Incremental.xml", FileMode.Open));

      //var d = new DDPVersionManager();
      //var v = d.DDPFromDate(DateTime.UtcNow.AddYears(-1));

      DDPManager mgr = new DDPManager();
      mgr.ProcessPendingUpdates();


    }
  }
}
