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
  public class SARServiceDataAccess : BaseDataAccess
  {
    public SARServiceDataAccess() : base() { }
    public SARServiceDataAccess(DBDataContext context) : base(context) { }


    /// <summary>
    /// Crea un nuevo(s) SARServices(s) en base de datos
    /// </summary>
    /// <param name="places">Lista de places</param>
    public void Create(SARService[] sarservices)
    {
      context.SARServices.InsertAllOnSubmit(sarservices);
      context.SubmitChanges();
    }

    public SARService GetServiceByLRITId(string lritID)
    {
      return context.SARServices.FirstOrDefault(s => s.LRITid == lritID);
    }
  }
}
