using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos del ContractingGoverment
  /// </summary>
  public class ContractingGovermentDataAccess : BaseDataAccess
  {
    public ContractingGovermentDataAccess()
      : base()
    {

    }

    public ContractingGovermentDataAccess(DBDataContext context)
      : base(context)
    {

    }

    /// <summary>
    /// Crea un nuevo contracting goverment en la base de datos
    /// </summary>
    /// <param name="contractingGoverment">contractingGoverment</param>
    /// <returns>ID del registro en base de datos</returns>
    public int Create(ContractingGoverment contractingGoverment)
    {
      context.ContractingGoverments.InsertOnSubmit(contractingGoverment);
      context.SubmitChanges();
      return contractingGoverment.Id;
    }
    /// <summary>
    /// Borra todos los contracting goverments de la base de datos
    /// </summary>
    public void DropAll()
    {
      context.ContractingGoverments.DeleteAllOnSubmit(context.ContractingGoverments);
      context.SubmitChanges();
    }

    /// <summary>
    /// Retorna verdadero si la posicion para, un barco especificado, esta dentro
    /// de cualquiera de las areas del Contracting Goverment 
    /// </summary>
    /// <param name="CgId">ID del contracting goverment</param>
    /// <param name="ShipId">ID del Ship</param>
    /// <returns></returns>
    public bool IsOnWatersOf(int govermentId, int positionId, int ddpVersionId)
    {
      int[] flag = context.ExecuteQuery<int>(@"SELECT count(ShipPosition.Id) FROM "                          +Environment.NewLine+
                                            "ShipPosition, place INNER JOIN ContractingGoverment"       +Environment.NewLine+
	                                          "on (Place.ContractingGovermentId = ContractingGoverment.Id"+Environment.NewLine+
		                                            "AND ContractingGovermentId = {0})"                     +Environment.NewLine+
                                            "INNER JOIN DDPVersion"                                     +Environment.NewLine+
	                                          "on (DDPVersionId = ContractingGoverment.DDPVersionId"      +Environment.NewLine+
		                                            "AND DDPVersionId = {1})"                               +Environment.NewLine+
                                            "WHERE geography::STGeomFromWKB(ShipPosition.Position, 4326).STIntersects(geography::STGeomFromWKB(Place.Area, 4326) ) != 0"+Environment.NewLine+
                                            "AND ShipPosition.Id = {2};", govermentId, ddpVersionId, positionId).ToArray();
    
      if (flag[0] == 0)
        return false;
      return true;
    }
    /// <summary>
    /// Verifica si una determinada posicion esta dentro de las aguas territoriales de un contracting goverment
    /// distinto al que se pasa como parametro
    /// </summary>
    /// <param name="govermentId">ID de base de datos del Contracting goverment</param>
    /// <param name="positionId">ID de base de datos de la posicion</param>
    /// <returns>Verdadero si la posicion esta, falso otra cosa</returns>
    public bool IsOnInternalWaterOfAnother(int govermentId, int positionId, int ddpVersionId)
    {
        int[] flag = context.ExecuteQuery<int>(@"SELECT count(ShipPosition.Id) FROM"                             +Environment.NewLine+ 
	                                            "ShipPosition, Place INNER JOIN ContractingGoverment"         +Environment.NewLine+ 
		                                        "on (Place.ContractingGovermentId = ContractingGoverment.Id"    +Environment.NewLine+ 
			                                        "AND ContractingGoverment.Id != {0}"                          +Environment.NewLine+
			                                        "AND Place.AreaType = 'internalwaters')"                      +Environment.NewLine+ 
	                                          "INNER JOIN DDPVersion"                                         +Environment.NewLine+ 
		                                        "on (DDPVersion.Id = ContractingGoverment.DDPVersionId"         +Environment.NewLine+
			                                        "AND DDPVersion.Id = {1})"                                    +Environment.NewLine+
    	                                      "WHERE geography::STGeomFromWKB(ShipPosition.Position, 4326).STIntersects(geography::STGeomFromWKB(Place.Area, 4326) ) != 0"+Environment.NewLine+
		                                          "AND ShipPosition.Id = {2}", govermentId, ddpVersionId, positionId).ToArray();
      
        if (flag[0] == 0)
          return false;
        return true;
    }

    /// <summary>
    /// Verifica si una determinada posicion esta en las aguas territoriales de un determinado 
    /// contracting goverment
    /// </summary>
    /// <param name="govermentId">ID de base de datos del Contracting goverment</param>
    /// <param name="positionId">ID de base de datos de la posicion</param>
    /// <returns>Verdadero si la posicion esta, falso otra cosa</returns>
    public bool IsOnTerritorialWaterOf(int govermentId, int positionId, int ddpVersionId)
    {
      int[] flag = context.ExecuteQuery<int>(@"SELECT count(ShipPosition.Id) FROM" + Environment.NewLine +
                                            "ShipPosition, Place INNER JOIN ContractingGoverment" + Environment.NewLine +
                                          "on (Place.ContractingGovermentId = ContractingGoverment.Id" + Environment.NewLine +
                                            "AND ContractingGoverment.Id = {0}" + Environment.NewLine +
                                            "AND Place.AreaType = 'territorialsea')" + Environment.NewLine +
                                          "INNER JOIN DDPVersion" + Environment.NewLine +
                                          "on (DDPVersion.Id = ContractingGoverment.DDPVersionId" + Environment.NewLine +
                                            "AND DDPVersion.Id = {1})" + Environment.NewLine +
                                          "WHERE geography::STGeomFromWKB(ShipPosition.Position, 4326).STIntersects(geography::STGeomFromWKB(Place.Area, 4326) ) != 0" + Environment.NewLine +
                                            "AND ShipPosition.Id = {2}", govermentId, ddpVersionId, positionId).ToArray();

      if (flag[0] == 0)
        return false;
      return true;
    }

    public ContractingGoverment GetById(int id)
    {
      return context.ContractingGoverments.Where(cg => cg.Id == id).FirstOrDefault();
    }

    public ContractingGoverment GetByLRITId(string LRITId, int ddpVersionId)
    {
      return context.ContractingGoverments.Where(cg => cg.LRITId == int.Parse(LRITId) && cg.DDPVersion.Id == ddpVersionId).FirstOrDefault();
    }

    public List<ContractingGoverment> Getall(int ddpVersionId)
    {
      return context.ContractingGoverments.Where(cg => cg.DDPVersionId == ddpVersionId).ToList();
    }
  }
}
