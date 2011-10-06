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

namespace CoreGUI
{
  public partial class mainForm : Form
  {
    private DataCenterCore core;
    public mainForm()
    {
      InitializeComponent();

    }

    private void mainForm_Load(object sender, EventArgs e)
    {
      BasicConfiguration config = BasicConfiguration.FromNameValueCollection(System.Configuration.ConfigurationManager.AppSettings);
      DataCenterDataAccess.Config.ConnectionString = config.ConnectionString;

      core = new DataCenterCore(config);
      core.Start();
    }

    private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      core.Stop();
    }
  }
}
