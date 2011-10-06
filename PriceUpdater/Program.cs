using System;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using PriceUpdaterLib;
using DataCenterDataAccess;

namespace PriceUpdaterRunner
{
  public class PriceUpdaterRunner
  {
    static void Main(string[] args)
    {
      try
      {
        string cnx = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        var p = new PriceUpdater(cnx);
        p.UpdateContracts(DateTime.UtcNow.AddDays(-1), new string[] { });
      }
      catch (Exception ex)
      {
        System.Console.WriteLine("error main: " + ex.Message + "-" + ex.ToString());
      }
    }
  }
}
