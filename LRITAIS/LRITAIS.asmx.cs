using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using DataCenterDataAccess;
using Microsoft.SqlServer.Types;
using System.Configuration;
using DataCenterLogic;


namespace LRITAIS
{
  /// <summary>
  /// Summary description for LRITAIS
  /// </summary>
  [WebService(Namespace = "http://tempuri.org/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]
  // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
  // [System.Web.Script.Services.ScriptService]
  public class LRITAIS : System.Web.Services.WebService
  {

    [WebMethod]
    public DataSet GetPositions()
    {
      using (var sqlconn = new SqlConnection( System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString ))
      {
        sqlconn.Open();
        var dsout = new DataSet();
        var dt = new DataTable();
        var pos = new SqlGeography();
        using (var da = new SqlDataAdapter("SELECT * FROM AIS", sqlconn))
        {
          da.Fill(dt);
        }
        dt.Columns.Add("LATITUD", typeof(Decimal));
        dt.Columns.Add("LONGITUD", typeof(Decimal));
        foreach (DataRow row in dt.Rows)
        {
          pos = SqlGeography.STGeomFromWKB(new SqlBytes( (byte[])row["Position"] ), 4326);
          row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
          row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
        }
        dt.Columns.Remove("Position");
        dsout.Tables.Add(dt);
        return dsout;
      }
    }

    [WebMethod]
    public DataSet GetPositionsOnStandingOrder()
    {
      using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
      {
        sqlconn.Open();
        var dsout = new DataSet();
        var dt = new DataTable();
        var pos = new SqlGeography();

        ShipDataAccess sda = null;
        StandingOrderDataAccess soda = null;

        using (var da = new SqlDataAdapter("SELECT * FROM AIS", sqlconn))
        {
          da.Fill(dt);
        }
        dt.Columns.Add("LATITUD", typeof(Decimal));
        dt.Columns.Add("LONGITUD", typeof(Decimal));
        
        foreach (DataRow row in dt.Rows)
        {
          try
          {
            sda = new ShipDataAccess();
            soda = new StandingOrderDataAccess();

            var spman = new ShipPositionManager();
            var ddpman = new DDPVersionManager();
            var ddpver = ddpman.GetCurrentDDPVersion();
            var lastpos = spman.GetLastShipPosition(sda.getById((int)row["ID_BARCO"]).IMONum);
            List<StandingOrder> so = new List<StandingOrder>();
            if (lastpos != null)
              so = soda.GetOrdersForPosition(lastpos, ddpver);
            if (so.Count == 0)
            {
              dt.Rows.Remove(row);
              continue;
            }
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine(ex);
          }
          finally
          {
            sda.Dispose();
            soda.Dispose();
          }

          pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["Position"]), 4326);
          row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
          row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
        }
        dt.Columns.Remove("Position");
        dsout.Tables.Add(dt);
        return dsout;
      }
    }

  }
}

