using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Microsoft.SqlServer.Types;

namespace DataCenterDataAccess 
{
  /// <summary>
  /// Acceso a datos de PlaceData
  /// </summary>
  public class PlaceDataAccess : BaseDataAccess
  {
    public PlaceDataAccess() : base() { }
    public PlaceDataAccess(DBDataContext context) : base(context) { }


    public List<Place> GetAll(string ddpVersion)
    {
      string[] parts = ddpVersion.Split(':');
      if (parts.Length != 2) return new List<Place>();
      return context.Places.Where(p => p.ContractingGoverment.DDPVersion.regularVer == parts[0] && p.ContractingGoverment.DDPVersion.inmediateVer == parts[1] ).ToList();
    }
    /// <summary>
    /// Obtiene todos los Place de la base de datos
    /// </summary>
    /// <returns>List de Place</returns>
    public List<Place> GetAll()
    {
      return context.Places.ToList();
    }

    /// <summary>
    /// Crea un nuevo(s) Place(s) en base de datos
    /// </summary>
    /// <param name="places">Lista de places</param>
    public void Create( Place[] places )
    {
      context.Places.InsertAllOnSubmit(places);
      context.SubmitChanges();
    }

    /// <summary>
    /// Retorna true si el nombre del port o portfacility existe o false si no existe o no coincide
    /// </summary>
    /// <param name="port"></param>
    /// <returns>Nombre del port o portfacility como figura en el DDP</returns>
    public bool PortExists(string item, int ddpVersion)
    {
      return AreaExists(item, "port", ddpVersion);
    }

    public bool PortFacilityExists(string item, int ddpVersion)
    {
      return AreaExists(item, "portfacility", ddpVersion);
    }

    public bool AreaExists(string item, string itemelement, int ddpVersion)
    {
      using (SqlConnection conn = new SqlConnection(Config.ConnectionString))
      {
        conn.Open();
        string sql = string.Format("SELECT count(*) FROM Place as p left join ContractingGoverment as c on p.ContractingGovermentId = c.Id " +
                                   "where c.DDPVersionId={2} " +
                                   "AND p.PlaceStringId = '{0}' AND p.AreaType = '{1}'", item, itemelement, ddpVersion);

        SqlCommand cmd = new SqlCommand(sql, conn);
        object scalar = cmd.ExecuteScalar();
        if (scalar.ToString() == "0")
          return false;
        else return true;
      }
    }

  }
}
