using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using log4net;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de los goviernos contratantes.
  /// Da funciones de ayuda para saber si la titulacion de un determinado contracting goverment es valida
  /// </summary>
  public class ContractingGovermentManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ContractingGovermentManager));
    /// <summary>
    /// Verifica si un contracting goverment esta titulado para recibir una posicion de un barco
    /// </summary>
    /// <param name="goverment">Contracting goverment</param>
    /// <param name="position">Posicion del barco</param>
    /// <returns>Verdadero si esta titulado, Falso si no esta titulado</returns>
    public bool IsEntitled(ContractingGoverment goverment, ShipPosition position, DDPVersion ddpVersion, bool verifyWatersOf)
    {
      return IsEntitled(goverment.Id, position.Id, ddpVersion.Id, verifyWatersOf);
    }
    /// <summary>
    /// Verifica si un contracting goverment esta titulado para recibir una posicion de un barco
    /// </summary>
    /// <param name="govermentId">ID de base de datos del Contracting goverment</param>
    /// <param name="positionId">ID de base de datos de la Posicion del barco</param>
    /// <returns>Verdadero si esta titulado, Falso si no esta titulado</returns>
    public bool IsEntitled(int govermentId, int positionId, int ddpVersionId, bool verifyWatersOf)
    {
      using (var cgda = new ContractingGovermentDataAccess())
      {
        //Cuando AccesType 5 o 3 (Port time/distance) no se verifica que este en aguas internas 
        //del gobierno que lo pide.
        if (verifyWatersOf == true)
        {
          if (cgda.IsOnWatersOf(govermentId, positionId, ddpVersionId) == false)
          {
            log.Debug(string.Format("No titulado: La posición no se encuentra en aguas del CG {0}", govermentId));
            return false;
          }
        }

        if (cgda.IsOnInternalWaterOfAnother(govermentId, positionId, ddpVersionId) == true)
        {
          log.Debug(string.Format("No titulado: La posición esta aguas del internas de otro CG distinto a {0}", govermentId));
          return false;
        }

        var mecg = cgda.GetByLRITId("1005", ddpVersionId);
        if (cgda.IsOnTerritorialWaterOf(mecg.Id, positionId, ddpVersionId) == true)
        {
          log.Debug(string.Format("No titulado: La posición esta en aguas territoriales del barco (arg 1005)", govermentId));
          return false;
        }

        log.Debug("Titulacion OK");

        return true;
      }
    }

    /// <summary>
    /// Obtiene un ContractingGoverment basado en un LRITId
    /// </summary>
    /// <param name="LRITId">LRITId del contracting goverment</param>
    /// <returns>ContractingGoverment</returns>
    public ContractingGoverment GetContractingGovermentByLRITId(string LRITId, int ddpVersionId)
    {
      using (var cgda = new ContractingGovermentDataAccess())
      {
        var cg = cgda.GetByLRITId(LRITId, ddpVersionId);
        if( cg == null )
        {
          using( var sserda = new SARServiceDataAccess() )
          {
            var sser = sserda.GetServiceByLRITId(LRITId);
            if( sser == null )
              return null;

            LRITId = sser.ContractingGoverment.LRITId.ToString();
          }
        }
        
        return cgda.GetByLRITId(LRITId, ddpVersionId);
      }
    }

    /// <summary>
    /// Obtiene un ContractingGoverment basado en un ID de base de datos
    /// </summary>
    /// <param name="cgID">ID de base de datos</param>
    /// <returns>ContractingGoverment</returns>
    public ContractingGoverment GetContractingGovermentById(int id)
    {
      using (var cgda = new ContractingGovermentDataAccess())
      {
        return cgda.GetById(id);
      }
    }


    public static Dictionary<int, string> LritIdNamePairs(int ddpVersion)
    {
      using (var cgda = new ContractingGovermentDataAccess())
      {
        var dic = new Dictionary<int, string>();
        var cgs = cgda.Getall(ddpVersion);
        foreach (var cg in cgs)
        {
          dic.Add(cg.LRITId,cg.Name);
        }

        return dic;
      }
    }



  }
}
