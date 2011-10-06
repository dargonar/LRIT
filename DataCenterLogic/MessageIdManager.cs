using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de los IDs LRIT de los mensajes enviados
  /// </summary>
  public class MessageIdManager
  {
    public static string Generate()
    {
      ConfigurationManager cmgr = new ConfigurationManager();
      return Generate(cmgr.Configuration.DataCenterID);
    }
    
    
    /// <summary>
    /// Genera un nuevo ID para un mensaje LRIT
    /// </summary>
    /// <returns>El ID en formato LRIT</returns>
    public static string Generate(string lritId)
    {
      using (MessageIdDataAccess dataAccess = new MessageIdDataAccess())
      {
        string msgId = string.Format("{2}{0:yyyyMMddHHmmss}{1:00000}",
                                      DateTime.UtcNow, dataAccess.GetNext(), lritId);
        return msgId;
      }
    }
  }
}
