using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de la configuracion LRIT a nivel sistema.
  /// Lee el unico registro existente en la tabla Configuration de la DB y lo mapea en una propiedad publica readonly.
  /// </summary>
  public class ConfigurationManager
  {
    private Configuration mConfig = new Configuration();
    public Configuration Configuration 
    { 
      get 
      { 
        return mConfig; 
      } 
    }

    public void Refresh()
    {
      using (ConfigurationDataAccess cdao = new ConfigurationDataAccess())
      {
        mConfig = cdao.Read();
      }
    }


    public ConfigurationManager()
    {
      Refresh();
    }
  }
}
