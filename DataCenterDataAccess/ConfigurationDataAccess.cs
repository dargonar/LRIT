using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de Configuracion
  /// </summary>
  public class ConfigurationDataAccess : BaseDataAccess
  {
    public ConfigurationDataAccess() : base(){}
    public ConfigurationDataAccess(DBDataContext context) : base() { }
    
    /// <summary>
    /// Lee el primer registro de la tabla de configuracion
    /// </summary>
    /// <returns>Configuracin del LRIT Data Center</returns>
    public Configuration Read()
    {
      return context.Configurations.First();//.Where( c => c.Id == 1 ).ToList()[0];
    }

  }
}
