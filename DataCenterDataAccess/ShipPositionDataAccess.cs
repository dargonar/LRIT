using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.SqlServer.Types;
using System.Data.Linq;
using log4net;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de ShipPosition
  /// </summary>
  public class ShipPositionDataAccess : BaseDataAccess
  {
    public ShipPositionDataAccess() : base() {}
    public ShipPositionDataAccess(DBDataContext context) : base(context) {}

    private static readonly ILog log = LogManager.GetLogger(typeof(ShipPositionDataAccess));

    /// <summary>
    /// Crea un nuevo ShipPosition en base de datos
    /// </summary>
    /// <param name="ShipPositions">Vector de ShipPosition</param>
    public void Insert(ShipPosition[] ShipPositions)
    {
      context.ShipPositions.InsertAllOnSubmit(ShipPositions);
      context.SubmitChanges();
    }

    /// <summary>
    /// Crea un nuevo ShipPosition en base de datos
    /// </summary>
    /// <param name="ShipPositions">ShipPosition</param>
    public void Insert(ShipPosition ShipPositions)
    {
      context.ShipPositions.InsertOnSubmit(ShipPositions);
      context.SubmitChanges();
    }



    /// <summary>
    /// Obtiene todas las posiciones dentro de un area circular
    /// </summary>
    /// <param name="center">Centro de area</param>
    /// <param name="radius_in_meters">Radio del area</param>
    /// <returns>Lista de posiciones</returns>
    public List<ShipPosition> GetShipsInCircularArea(string center, double radius_in_meters)
    {
      string q = string.Format("SELECT * FROM ShipPosition WHERE (geography::STGeomFromWKB(Position,4326).STDistance('{0}') < {1}) ORDER BY TimeStamp", center, radius_in_meters);
      log.Debug("GetShipsInCircularArea: " + q);
      return context.ExecuteQuery<ShipPosition>(q).ToList();
    }

    /// <summary>
    /// Obtiene todas las posiciones dentro de un area rectangular
    /// </summary>
    /// <param name="polygon">Area rectangular</param>
    /// <returns>Lista de posiciones</returns>
    public List<ShipPosition> GetShipsInRectangularArea(string polygon)
    {
      string q = string.Format("SELECT * FROM ShipPosition WHERE ( geography::STGeomFromText('{0}',4326).STIntersects(geography::STGeomFromWKB(Position,4326)) != 0) ORDER BY TimeStamp", polygon);
      log.Debug("GetShipsInRectangularArea: " + q);
      return context.ExecuteQuery<ShipPosition>(q).ToList();
    }

    /// <summary>
    /// Obtiene una lista de posiciones para un barco en un periodo de tiempo
    /// </summary>
    /// <param name="ship">Numero IMO del barco</param>
    /// <param name="startTime">Tiempo inicial</param>
    /// <param name="endTime">Tiempo final</param>
    /// <returns>Lista de posiciones</returns>
    public List<ShipPosition> GetShipPositionHistory(string shipIMONum, DateTime startTime, DateTime endTime)
    {
      DataLoadOptions options = new DataLoadOptions();
      options.LoadWith<ShipPosition>(c => c.Ship);
      context.LoadOptions = options;
      return context.ShipPositions.Where(c => c.Ship.IMONum == shipIMONum && c.TimeStampInASP > startTime && c.TimeStampInASP < endTime).ToList();
    }
    /// <summary>
    /// Obtiene la ultima posicion de un barco
    /// </summary>
    /// <param name="IMONum">Numero IMO del barco</param>
    /// <returns>Ultima posicion</returns>
    public ShipPosition GetLastShipPosition(string IMONum)
    {
      DataLoadOptions options = new DataLoadOptions();
      options.LoadWith<ShipPosition>(c => c.Ship);
      context.LoadOptions = options;
      return context.ShipPositions.OrderByDescending(c => c.TimeStamp).FirstOrDefault(c => c.Ship.IMONum == IMONum);
    }

    /// <summary>
    /// Verifica si el barco esta a una determinada distancia de un port o port facility.
    /// </summary>
    /// <param name="imoNUM">Numero IMO del barcos</param>
    /// <param name="place">PlaceStringID del port o port facility</param>
    /// <param name="distance">Distancia</param>
    /// <returns>Verdadero si el barco esta a menos de la distancia especificada, falso otra cosa.</returns>
    public bool IsShipInArea(string imoNUM, string place, double distance, int ddpVersion)
    {
      var lastPos = GetLastShipPosition(imoNUM);
      if (lastPos == null)
      {
        log.Debug(string.Format("IsShipInArea: GetLastShipPosition {0} null",imoNUM));
        return false;
      }

      var dest = context.Places.SingleOrDefault(p => p.PlaceStringId == place && p.ContractingGoverment.DDPVersionId == ddpVersion);


      if (dest == null)
      {
        log.Debug(string.Format("IsShipInArea: context.Places.SingleOrDefault {0} {1}", place, ddpVersion));
        return false;
      }

      var lastPosgeom = SqlGeography.STGeomFromWKB(new System.Data.SqlTypes.SqlBytes(lastPos.Position.ToArray()), 4326);
      var destGeom = SqlGeography.STGeomFromWKB(new System.Data.SqlTypes.SqlBytes(dest.Area.ToArray()), 4326);

      var xx = lastPosgeom.STDistance(destGeom);
      if( xx.IsNull )
      {
        log.Debug(string.Format("IsShipInArea: lastPosgeom.STDistance(destGeom) NULL"));
        return false;
      }

      double trigger_distance = distance;
      double toport_distance = xx.Value / 1852.0;

      log.Debug(string.Format("IsShipInArea: trigger_distance={0}/toport_distance={1}", trigger_distance, toport_distance));

      if (toport_distance < trigger_distance)
      {
        return true;
      }
      
      return false;
    }
  }
}
