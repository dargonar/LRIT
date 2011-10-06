using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using Microsoft.SqlServer.Types;
using System.Globalization;
using log4net;
using Common;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de las posiciones de barcos propios.
  /// </summary>
  public class ShipPositionManager
  {

    private static readonly ILog log = LogManager.GetLogger(typeof(ShipPositionManager));

    /// <summary>
    /// Obtiene la historia de posiciones de un barco dentro de un intervalo de tiempo
    /// </summary>
    /// <param name="ship">Barco del cual se requieren las posiciones</param>
    /// <param name="startTime">Tiempo de inicio</param>
    /// <param name="endTime">Tiempo de fin</param>
    /// <returns></returns>
    public List<ShipPosition> GetShipPositionHistory(string ship, DateTime startTime, DateTime endTime)
    {
      using (var dao = new ShipPositionDataAccess())
      {
        return dao.GetShipPositionHistory(ship, startTime, endTime);
      }
    }

    /// <summary>
    /// Obtiene las ultimas posiciones de todos nuestros barcos dentro de una area circular
    /// </summary>
    /// <param name="circularArea">Area circular</param>
    /// <param name="lastPositions">Cantidad de posiciones</param>
    /// <returns></returns>
    public List<ShipPosition> GetLastPositionsInCircularArea(string circularArea, int lastPositions)
    {
      
      string center       = getCenterInCircularArea(circularArea);
      double radius_in_nm = getRadiusFromCircularArea(circularArea);
      
      //Convert to METERS
      double radius_in_meters = radius_in_nm * 1852.0;
    
      using (var dao = new ShipPositionDataAccess())
      {
      List<ShipPosition> shipsInArea = dao.GetShipsInCircularArea(center, radius_in_meters);

      return LastNPositions(lastPositions, shipsInArea);
      }
    }

    /// <summary>
    /// Obtiene las ultimas posiciones de todos nuestros barcos dentro de una area rectangular
    /// </summary>
    /// <param name="recArea">Area rectangular</param>
    /// <param name="lastPositions">Cantidad de posiciones</param>
    /// <returns></returns>
    public List<ShipPosition> GetLastPositionsInRectangularArea(string recArea, int lastPositions)
    {
      string polygon = getPolygonFromString(recArea);

      using (var dao = new ShipPositionDataAccess())
      {
        List<ShipPosition> shipsInArea = dao.GetShipsInRectangularArea(polygon);
        return LastNPositions(lastPositions, shipsInArea);
      }
    }

    private static List<ShipPosition> LastNPositions(int lastPositions, List<ShipPosition> shipsInArea)
    {
      List<ShipPosition> lastPositionsInArea = new List<ShipPosition>();
      var counts = new Dictionary<int, int>();

      foreach (ShipPosition shipPos in shipsInArea)
      {
        int stored = 0;
        if (counts.Keys.Contains(shipPos.ShipId) == true)
          stored = counts[shipPos.ShipId];

        if (stored >= lastPositions)
          continue;

        lastPositionsInArea.Add(shipPos);

        counts[shipPos.ShipId] = stored + 1;
      }

      return lastPositionsInArea;
    }

    /// <summary>
    /// Obtiene el radio de un area circular
    /// </summary>
    /// <param name="circularArea">Area circular</param>
    /// <returns>Radio</returns>
    private double getRadiusFromCircularArea(string circularArea)
    {
      string[] parts = circularArea.Split(':');
      if (parts.Length != 3)
        throw new InvalidCastException("SAR Circular area error!");

      string s = parts[2];
      log.Debug( string.Format(" getRadiusFromCircularArea [{0}]", s) );

      return double.Parse(s);
    }
    /// <summary>
    /// Obtiene el centro de un area circular
    /// </summary>
    /// <param name="circularArea">Area circular</param>
    /// <returns>Centro</returns>
    private string getCenterInCircularArea(string circularArea)
    {
      string[] parts = circularArea.Split(':');
      if (parts.Length != 3)
        throw new InvalidCastException("SAR Circular area error!");

      double lat = getDoubleFromWGS84Text(parts[0]);
      double lon = getDoubleFromWGS84Text(parts[1]);

      string s = string.Format( "POINT({0:0.00} {1:0.00})", lon, lat ).Replace(',','.');
      log.Debug(string.Format(" getCenterInCircularArea [{0}]", s));

      return s;
    }
    /// <summary>
    /// Genera un poligono a partir de un string separado por espacios
    /// </summary>
    /// <param name="rectangularArea">Vertices separados por espacios</param>
    /// <returns>Poligono</returns>
    private string getPolygonFromString(string rectangularArea)
    {
      string[] parts = rectangularArea.Split(':');
      if (parts.Length != 4)
        throw new InvalidCastException("SAR Rectangular area error!");

      double left   = getDoubleFromWGS84Text(parts[0]);
      double bottom = getDoubleFromWGS84Text(parts[1]);

      double right  = left + getDoubleFromWGS84Text(parts[2]);
      double top =    bottom + getDoubleFromWGS84Text(parts[3]);

      string s = string.Format("POLYGON( ({0:0.00} {1:0.00}* {2:0.00} {3:0.00}* {4:0.00} {5:0.00}* {6:0.00} {7:0.00}* {8:0.00} {9:0.00}) )",
        bottom, left,
        top, left,
        top, right,
        bottom, right, 
        bottom, left );

      s = s.Replace(',', '.').Replace('*', ',');

      log.Debug(string.Format("getPolygonFromString [{0}]", s));
      return s;
    }

    private double getDoubleFromWGS84Text(string wgsText)
    {
      string[] parts = wgsText.Split('.');
      if (parts.Length != 3)
        throw new InvalidCastException("WGS84 NO 3 parts!");

      double d1 = double.Parse(parts[0]) + double.Parse(parts[1]) / 60.0;
      if (parts[2] == "S" || parts[2] == "s" || parts[2] == "W" || parts[2] == "w")
        d1 *= -1;

      return d1;
    }

    /// <summary>
    /// Obtiene la ultima posicion de un barco
    /// </summary>
    /// <param name="IMONum">Numero IMO del barco</param>
    /// <returns>Ultima posicion o null si no hay posiciones</returns>
    public ShipPosition GetLastShipPosition(string IMONum)
    {
      using (var dao = new ShipPositionDataAccess())
      {
        return dao.GetLastShipPosition(IMONum);
      }
    }
    /// <summary>
    /// Verifica si un barco esta a una distancia igual o menor de un port o portfacility determinado
    /// </summary>
    /// <param name="imoNUM">Numero IMO del barco</param>
    /// <param name="place">Nombre LRIT del port o port facility</param>
    /// <param name="distance">Distancia</param>
    /// <returns>Verdadero si esta a menor o igual distancia, Falso si no</returns>
    public bool IsShipInArea(string imoNUM, string place, double distance, int ddpVersion)
    {
      using (var dao = new ShipPositionDataAccess())
      {
        return dao.IsShipInArea(imoNUM, place, distance, ddpVersion);
      }
    }

    /// <summary>
    /// Agrega una posicion para un barco determinado a la base de datos
    /// </summary>
    /// <param name="shipPosition">Objecto ShipPosition</param>
    public void Insert(ShipPosition[] shipPosition)
    {
      using (var dao = new ShipPositionDataAccess())
      {
        dao.Insert(shipPosition);
      }
    }


    /// <summary>
    /// Agrega una posicion para un barco determinado a la base de datos
    /// </summary>
    /// <param name="shipPosition">Objecto ShipPosition</param>
    public void Insert(ShipPosition shipPosition)
    {
      using (var dao = new ShipPositionDataAccess())
      {
        dao.Insert(shipPosition);
      }
    }

    public string[] GetLatLongOfPoint(ShipPosition pos)
    {
      var point = SqlGeography.STPointFromWKB(new System.Data.SqlTypes.SqlBytes(pos.Position.ToArray()), 4326);
      var latlong = new string[2];
      latlong[0] = point.Lat.ToString();
      latlong[1] = point.Long.ToString();
      return latlong;
    }
    
    public void ProcessASPPosition(PositionMessage pos)
    {
      var spos = new ShipPosition();
      spos.Rumbo      = pos.Course;
      spos.Region     = pos.OceanRegion;
      spos.Velocidad  = pos.Speed;
      spos.TimeStamp  = pos.TimeOfPosition;
      spos.TimeStampInASP   = pos.InAsp;
      spos.TimeStampOutASP  = pos.OutAsp;

      Ship ship = null;
      if (pos.DNID == "sata")
      {
        ship = ShipManager.getByISN(pos.Speed);
        spos.Velocidad = "-";
      }
      else
        ship = ShipManager.getByMemberNum(pos.MemberNumber);
      if (ship == null)
      {
        log.Error(string.Format("No hay barco registrado con Member Number {0} para la posicion recibida", pos.MemberNumber));
        return;
      }
      spos.ShipId = ship.Id;
      spos.TimeStampInDC = DateTime.UtcNow;
      var p = string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", pos.Longitude, pos.Latitude).ToCharArray();
      var sqlp = new System.Data.SqlTypes.SqlChars(p);
      var ppp = (SqlGeography.STGeomFromText(sqlp, 4326)).STAsBinary();
      spos.Position = (byte[])ppp.ToSqlBinary();
      Insert(spos);
      ShipManager.ChangeShipStatus(ShipStatus.Ok, ship);
    }
  }
}
