using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Microsoft.SqlServer.Types;
using System.Configuration;


namespace LRITSIGO
{
  public class Datos
  {
    public DataSet Ver_barcos_lrit()
    {
      return ObtenerDatos("SELECT * FROM LRIT_SIGO");
    }
    public DataSet Ver_barcos_lrit_filtro( string mmsi, string matricula, string nro_omi, string nombre)
    {
      string query = "SELECT * FROM LRIT_SIGO WHERE 1=1 ";

      if (mmsi != null && mmsi.Trim().Length > 0)
        query += string.Format(" AND MMSI like '%{0}%'", mmsi.Trim());

      if (matricula != null && matricula.Trim().Length > 0)
        query += string.Format(" AND MATRICULA like '%{0}%'", matricula.Trim());

      if (nro_omi != null && nro_omi.Trim().Length > 0)
        query += string.Format(" AND NRO_OMI like '%{0}%'", nro_omi.Trim());

      if (nombre != null && nombre.Trim().Length > 0)
        query += string.Format(" AND NOMBRE like '%{0}%'", nombre.Trim());

      return ObtenerDatos(query);
    }

    public DataSet Ver_barcos_lrit_historico(DateTime fromDate, DateTime toDate, string[] shipIds)
    {
      string arrShipIds = "(";
      foreach (var sid in shipIds)
        arrShipIds += sid + ",";

      arrShipIds += "-1000)";

      string query = string.Format("SELECT s.ID_Barco,sp.Id,sp.TimeStamp,sp.TimeStampInASP,sp.TimeStampOutASP,sp.TimeStampInDC,sp.Position as Position,sp.Rumbo,sp.Velocidad,sp.Destino,sp.Region " +
                                   "FROM ShipPosition as sp left join Ship as s on s.id=sp.ShipId where sp.TimeStamp >= '{0}' AND TimeStamp <= '{1}' AND s.ID_BARCO in {2}",
                                    fromDate.ToString("yyyy-MM-dd HH:mm:ss"), toDate.ToString("yyyy-MM-dd HH:mm:ss"), arrShipIds);
      return ObtenerDatos(query);
    }

    public DataSet Ver_barcos_lrit_st()
    {
      return ObtenerDatosST("SELECT * FROM LRIT_SIGO_SO WHERE FECHA_REPORTE > DATEADD(day,-1,GETUTCDATE())");
    }
    public DataSet Ver_barcos_lrit_st_filtro(string ShipName, string IMONum, string MMSINum)
    {
      string query = "SELECT * FROM LRIT_SIGO_SO WHERE FECHA_REPORTE > DATEADD(day,-1,GETUTCDATE()) ";

      if (ShipName != null && ShipName.Trim().Length > 0)
          query += string.Format(" AND ShipName like '%{0}%'", ShipName);

        if (IMONum != null && IMONum.Trim().Length > 0)
        query += string.Format(" AND IMONum like '%{0}%'", IMONum);

      if (MMSINum != null && MMSINum.Trim().Length > 0 )
        query += string.Format(" AND MMSINum like '%{0}%'", MMSINum);

      return ObtenerDatosST(query);
    }
    /*
    private DataSet ObtenerDatosST(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            Dictionary<int, string> Response = new Dictionary<int, string>();

            Response.Add(1, "Coastal");
            Response.Add(2, "Flag");
            Response.Add(3, "Port");
            Response.Add(4, "SAR");


            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();

            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            foreach (DataRow row in dt.Rows)
            {
                row["TIPO_RESPUESTA"] = Response[int.Parse(row["TIPO_RESPUESTA"].ToString())];
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }
    */

    private DataSet ObtenerDatosST(string query)
    {
      using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
      {
        Dictionary<int, string> Response = new Dictionary<int, string>();

        Response.Add(1, "Coastal");
        Response.Add(2, "Flag");
        Response.Add(3, "Port");
        Response.Add(4, "SAR");


        sqlconn.Open();
        var dsout = new DataSet();
        var dt = new DataTable();
        string sPattern = "^\\d+.\\d+.\\d+.[N|S|E|W]$";
        decimal degrees = 0;
        string[] GMS;

        string AuxLatString;
        string AuxLongString;
        decimal AuxLatString2;
        decimal AuxLongString2;

        using (var da = new SqlDataAdapter(query, sqlconn))
        {
          da.Fill(dt);
        }

        foreach (DataRow row in dt.Rows)
        {


          // AuxLatString = row["LATITUD"].ToString().Replace('.', ',');
          //AuxLongString = row["LONGITUD"].ToString().Replace('.', ',');

          AuxLatString = row["LATITUD"].ToString();
          AuxLongString = row["LONGITUD"].ToString();

          if (System.Text.RegularExpressions.Regex.IsMatch(AuxLatString, sPattern))
            try
            {
              GMS = AuxLatString.Split('.');
              degrees = decimal.Parse(GMS[0]) + decimal.Parse(GMS[1]) / 60 + decimal.Parse(GMS[2]) / 3600;
              if (new String[] { "S", "W" }.Contains(GMS[3]))
                degrees *= -1;
            }
            catch
            {
              degrees = 0;                    //El formato de la latitud o longitud no es del tipo 
            }

          row["LATITUD"] = decimal.Parse(degrees.ToString().Replace('.', ','));



          if (System.Text.RegularExpressions.Regex.IsMatch(AuxLongString, sPattern))
            try
            {
              GMS = AuxLongString.Split('.');
              degrees = decimal.Parse(GMS[0]) + decimal.Parse(GMS[1]) / 60 + decimal.Parse(GMS[2]) / 3600;
              if (new String[] { "S", "W" }.Contains(GMS[3]))
                degrees *= -1;
            }
            catch
            {
              degrees = 0;                    //El formato de la latitud o longitud no es del tipo 
            }

          row["LONGITUD"] = degrees.ToString().Replace(',', '.');
        }
        foreach (DataRow row in dt.Rows)
        {
          //row["TIPO_RESPUESTA"] = Response[int.Parse(row["TIPO_RESPUESTA"].ToString())];
        }


        dsout.Tables.Add(dt);
        return dsout;
      }
    }

    private DataSet ObtenerDatos(string query)
    {
      using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
      {
        sqlconn.Open();
        var dsout = new DataSet();
        var dt = new DataTable();
       
        var pos = new SqlGeography();
        using (var da = new SqlDataAdapter(query, sqlconn))
        {
          da.Fill(dt);
        }
        dt.Columns.Add("LATITUD", typeof(Decimal));
        dt.Columns.Add("LONGITUD", typeof(Decimal));
        foreach (DataRow row in dt.Rows)
        {
          pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["Position"]), 4326);
          row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
          row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
        }
        dt.Columns.Remove("Position");
        dsout.Tables.Add(dt);
        return dsout;
      }
    }

    public DataSet Ver_puertos_lrit_port()
    {
        return ObtenerDatos_port("select * from LRIT_SIGO_PORT");
    }
    private DataSet ObtenerDatos_port(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dt.Columns.Add("LATITUD", typeof(Decimal));
            dt.Columns.Add("LONGITUD", typeof(Decimal));
            foreach (DataRow row in dt.Rows)
            {
                pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["position"]), 4326);
                row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
                row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
            }
            dt.Columns.Remove("position");
            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_puertos_lrit_port_ID(int LRITid)
    {
        return ObtenerDatos_port_ID("select * from LRIT_SIGO_PORT where LRITid ="+ LRITid );
    }
    private DataSet ObtenerDatos_port_ID(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dt.Columns.Add("LATITUD", typeof(Decimal));
            dt.Columns.Add("LONGITUD", typeof(Decimal));
            foreach (DataRow row in dt.Rows)
            {
                pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["position"]), 4326);
                row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
                row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
            }
            dt.Columns.Remove("position");
            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_puertos_lrit_port_name(string Name_Place)
    {
        return ObtenerDatos_port_name("select * from LRIT_SIGO_PORT where Name_Place like '" + Name_Place + "'");
    }
    private DataSet ObtenerDatos_port_name(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dt.Columns.Add("LATITUD", typeof(Decimal));
            dt.Columns.Add("LONGITUD", typeof(Decimal));
            foreach (DataRow row in dt.Rows)
            {
                pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["position"]), 4326);
                row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
                row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
            }
            dt.Columns.Remove("position");
            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_puertos_lrit_port_facility()
    {
        return ObtenerDatos_port_facility("select * from LRIT_SIGO_PORT_FACILITY");
    }
    private DataSet ObtenerDatos_port_facility(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dt.Columns.Add("LATITUD", typeof(Decimal));
            dt.Columns.Add("LONGITUD", typeof(Decimal));
            foreach (DataRow row in dt.Rows)
            {
                pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["position"]), 4326);
                row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
                row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
            }
            dt.Columns.Remove("position");
            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_puertos_lrit_port_facility_ID(int LRITid)
    {
        return ObtenerDatos_port_facility_id("select * from LRIT_SIGO_PORT_FACILITY WHERE LRITid =" + LRITid);
    }
    private DataSet ObtenerDatos_port_facility_id(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dt.Columns.Add("LATITUD", typeof(Decimal));
            dt.Columns.Add("LONGITUD", typeof(Decimal));
            foreach (DataRow row in dt.Rows)
            {
                pos = SqlGeography.STGeomFromWKB(new SqlBytes((byte[])row["position"]), 4326);
                row["LATITUD"] = decimal.Parse(pos.Lat.ToString());
                row["LONGITUD"] = decimal.Parse(pos.Long.ToString());
            }
            dt.Columns.Remove("position");
            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_DDP()
    {
        return ObtenerDatosDDP("select * from LRIT_SIGO_1000MN");
    }
    private DataSet ObtenerDatosDDP(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_ContractingGoverment()
    {
        return ObtenerDatosDDP_ContractingGoverment("SELECT  Name, LRITId FROM ContractingGoverment group by Name,LRITId  order by Name ");
    }
    private DataSet ObtenerDatosDDP_ContractingGoverment(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_DDP_ID(int LRITid)
    {
        return ObtenerDatosDDP_ID("select * from LRIT_SIGO_1000MN where LRITid = "+ LRITid );
    }
    private DataSet ObtenerDatosDDP_ID(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_DDP_CUSTOM_COASTAL_AREAS()
    {
        return ObtenerDatosDDP_CUSTOM_COASTAL_AREAS("select * from LRIT_SIGO_CUSTOM_COASTAL_AREAS");
    }
    private DataSet ObtenerDatosDDP_CUSTOM_COASTAL_AREAS(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_DDP_CUSTOM_COASTAL_AREAS_ID(int LRITid)
    {
        return ObtenerDatosDDP_CUSTOM_COASTAL_AREAS_ID("select * from LRIT_SIGO_CUSTOM_COASTAL_AREAS where LRITid = " + LRITid);
    }
    private DataSet ObtenerDatosDDP_CUSTOM_COASTAL_AREAS_ID(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }
    
    public DataSet Ver_barcos_lrit_DDP_LRIT_SIGO_TERRITORIAL_SEA()
    {
        return ObtenerDatosDDP_LRIT_SIGO_TERRITORIAL_SEA("select * from LRIT_SIGO_CUSTOM_COASTAL_AREAS");
    }
    private DataSet ObtenerDatosDDP_LRIT_SIGO_TERRITORIAL_SEA(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }
      
    public DataSet Ver_barcos_lrit_DDP_LRIT_SIGO_TERRITORIAL_SEA_ID(int LRITID)
    {
        return ObtenerDatosDDP_LRIT_SIGO_TERRITORIAL_SEA_ID("select * from LRIT_SIGO_TERRITORIAL_SEA where LRITid =" + LRITID);
    }
    private DataSet ObtenerDatosDDP_LRIT_SIGO_TERRITORIAL_SEA_ID(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }

            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_DDP_LRIT_SIGO_INTERNAL_WATER()
    {
        return ObtenerDatosDDP_LRIT_SIGO_INTERNAL_WATER("select * from LRIT_SIGO_INTERNAL_WATERS");
    }
    private DataSet ObtenerDatosDDP_LRIT_SIGO_INTERNAL_WATER(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dsout.Tables.Add(dt);
            return dsout;
        }
    }

    public DataSet Ver_barcos_lrit_DDP_LRIT_SIGO_INTERNAL_WATER_ID(int LRITid)
    {
        return ObtenerDatosDDP_LRIT_SIGO_INTERNAL_WATER_ID("select * from LRIT_SIGO_INTERNAL_WATERS where LRITid= " + LRITid );
    }
    private DataSet ObtenerDatosDDP_LRIT_SIGO_INTERNAL_WATER_ID(string query)
    {
        using (var sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["mio"].ConnectionString))
        {
            sqlconn.Open();
            var dsout = new DataSet();
            var dt = new DataTable();
            var pos = new SqlGeography();
            using (var da = new SqlDataAdapter(query, sqlconn))
            {
                da.Fill(dt);
            }
            dsout.Tables.Add(dt);
            return dsout;
        }
    }
  }

}