using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de MessageId
  /// </summary>
  public class MessageIdDataAccess : BaseDataAccess
  {
    public MessageIdDataAccess() : base() { }
    public MessageIdDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Obtiene el proximo numero unico de mensaje de base de datos
    /// </summary>
    /// <returns>Numero unico para usar en mensaje</returns>
    public int GetNext()
    {
        MessageId mid = context.MessageIds.First();
        int id = mid.nextId;
        mid.nextId++;
        if (mid.nextId >= 100000)
          mid.nextId = 0;
        context.SubmitChanges();
        return id;
    }
  }
}
