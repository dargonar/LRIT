using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DataCenterLogic;
using System.Configuration;

namespace DataCenterCore
{
  public partial class DataCenterCore : ServiceBase
  {
    private DataCenterLogic.DataCenterCore core;

    public DataCenterCore()
    {
      InitializeComponent();
    }
    /// <summary>
    /// Funcion llamada al iniciarse el servicio
    /// </summary>
    /// <param name="args">Parametros</param>
    protected override void OnStart(string[] args)
    {
      BasicConfiguration config = BasicConfiguration.FromNameValueCollection(System.Configuration.ConfigurationManager.AppSettings);
      DataCenterDataAccess.Config.ConnectionString = config.ConnectionString;

      core = new DataCenterLogic.DataCenterCore(config);
      core.Start();
    }
    /// <summary>
    /// Funcion llamada cuando el servicio es detenido
    /// </summary>
    protected override void OnStop()
    {
      core.Stop();
    }
  }
}
