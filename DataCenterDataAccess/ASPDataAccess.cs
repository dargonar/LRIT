using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos del ASP
  /// </summary>
  class ASPDataAccess : BaseDataAccess
  {

    public ASPDataAccess() : base() { }
    public ASPDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Obtiene un ASP en base a un ID
    /// </summary>
    /// <param name="id">ID de base de datos del ASP</param>
    /// <returns>ASP</returns>
    public ASP getById(int id)
    {
        ASP asp = context.ASPs.Where( a => a.Id == id ).ToList()[0];
        return asp;
    }
  }
}
