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
  /// <summary>
  /// Summary description for LRITSIGO
  /// </summary>
  [WebService(Namespace = "https://www.ws.prefecturanaval.gov.ar/LRITSIGO/")]
  [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
  [System.ComponentModel.ToolboxItem(false)]
  // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
  // [System.Web.Script.Services.ScriptService]
  public class LRITSIGO : System.Web.Services.WebService
  {
    [WebMethod]
    public DataSet DatasetLrit()
    {
      var datos = new Datos();
      return datos.Ver_barcos_lrit();
    }

    [WebMethod]
    public DataSet DatasetLritHistorico(DateTime fromDate, DateTime toDate, string[] shipIds)
    {
      var datos = new Datos();
      return datos.Ver_barcos_lrit_historico(fromDate, toDate, shipIds);
    }


    [WebMethod]
    public DataSet DatasetLrit_ContractingGoverment()
    {
        var datos = new Datos();
        return datos.Ver_barcos_lrit_ContractingGoverment();
    }

    [WebMethod]
    public DataSet DatasetLritFlitro(string mmsi, string matricula, string nro_omi, string nombre)
    {
      var datos = new Datos();
      return datos.Ver_barcos_lrit_filtro(mmsi, matricula, nro_omi, nombre);
    }
         
    [WebMethod]
    public DataSet DatasetLritSt()
    {
      var datos = new Datos();
      return datos.Ver_barcos_lrit_st();
    }

    [WebMethod]
    public DataSet DatasetLritStFiltro(String ShipName, string IMONum, string MMSINum)
    {
        var datos = new Datos();
        return datos.Ver_barcos_lrit_st_filtro(ShipName, IMONum, MMSINum);
    }
    
    //  [WebMethod]
    //public DataSet DatasetLritPort_facility()
    //{
    //    var datos = new Datos();
    //    return datos.Ver_puertos_lrit_port_facility();
    //}
      
    //[WebMethod]
    //public DataSet DatasetLritPort()
    //{
    //    var datos = new Datos();
    //    return datos.Ver_puertos_lrit_port();
    //}

    [WebMethod]
    public DataSet DatasetLritPort_id( int LRITid)
    {
        var datos = new Datos();
        return datos.Ver_puertos_lrit_port_ID(LRITid);
    }

    [WebMethod]
    public DataSet DatasetLritPort_name(String Name_Place)
    {
        var datos = new Datos();
        return datos.Ver_puertos_lrit_port_name(Name_Place);
    }

    [WebMethod]
    public DataSet DatasetLritPort_facility_id(int LRITid)
    {
        var datos = new Datos();
        return datos.Ver_puertos_lrit_port_facility_ID(LRITid);
    }

    //[WebMethod]
    //public DataSet DatasetLritDDP1000mn()
    //{
    //    var datos = new Datos();
    //    return datos.Ver_barcos_lrit_DDP();
    //}

    [WebMethod]
    public DataSet DatasetLritDDP1000mn_id(int LRITid)
    {
        var datos = new Datos();
        return datos.Ver_barcos_lrit_DDP_ID(LRITid);
    }
   
    //  [WebMethod]
    //public DataSet DatasetLRIT_SIGO_TERRITORIAL_SEA()
    //{
    //    var datos = new Datos();
    //    return datos.Ver_barcos_lrit_DDP_LRIT_SIGO_TERRITORIAL_SEA();
    //}

      [WebMethod]
      public DataSet DatasetLRIT_SIGO_TERRITORIAL_SEA_id(int LRITid)
      {
          var datos = new Datos();
          return datos.Ver_barcos_lrit_DDP_LRIT_SIGO_TERRITORIAL_SEA_ID(LRITid);
      }

    //[WebMethod]
    //public DataSet DatasetLritDDP_LRIT_SIGO_INTERNAL_WATER()
    //{
    //    var datos = new Datos();
    //    return datos.Ver_barcos_lrit_DDP_LRIT_SIGO_INTERNAL_WATER();
    //}

    [WebMethod]
    public DataSet DatasetLritDDP_LRIT_SIGO_INTERNAL_WATER_ID(int LRITid)
    {
        var datos = new Datos();
        return datos.Ver_barcos_lrit_DDP_LRIT_SIGO_INTERNAL_WATER_ID(LRITid );
    }

    //[WebMethod]
    //public DataSet DatasetLritDDP_CUSTOM_COASTAL_AREAS()
    //{
    //    var datos = new Datos();
    //    return datos.Ver_barcos_lrit_DDP_CUSTOM_COASTAL_AREAS();
    //}

    [WebMethod]
    public DataSet DatasetLritDDP_CUSTOM_COASTAL_AREAS_ID(int LRITid)
    {
        var datos = new Datos();
        return datos.Ver_barcos_lrit_DDP_CUSTOM_COASTAL_AREAS_ID(LRITid );
    }

  }
}

