using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Linq;
using System.Data.SqlClient;
using Microsoft.SqlServer.Types;
using DataCenterDataAccess;

namespace DBCreator
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      DataCenterDataAccess.DBCreate.Run(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
      label1.Text = "Listo!";
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }
  }
}
