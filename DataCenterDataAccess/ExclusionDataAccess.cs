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
  public class ExclusionDataAccess : BaseDataAccess
  {
    public ExclusionDataAccess() : base() { }
    public ExclusionDataAccess(DBDataContext context) : base(context) { }


    /// <summary>
    /// Crea un nuevo(s) Exclusion(s) en base de datos
    /// </summary>
    /// <param name="places">Lista de places</param>
    public void Create(Exclusion[] places)
    {
      context.Exclusions.InsertAllOnSubmit(places);
      context.SubmitChanges();
    }
  }
}
