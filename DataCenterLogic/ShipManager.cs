using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using System.Text.RegularExpressions;
using log4net;
using Common;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de barcos
  /// </summary>
  
  public class ShipManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ShipManager));
    /// <summary>
    /// Obtiene un barco en base a un numero IMO.
    /// </summary>
    /// <param name="IMONum">numero IMO del barco a buscar</param>
    /// <returns>Referencia al barco</returns>
    static public Ship getByIMONum(string IMONum)
    {
      using (var sda = new ShipDataAccess())
      {
        return sda.getByIMONum(IMONum);
      }
    }

    static public Ship getByMemberNum(int member)
    {
      using (var sda = new ShipDataAccess())
      {
        return sda.getByMemberNum(member);
      }
    }

    static public Ship getByMobileNum(string mobile)
    {
      using (var sda = new ShipDataAccess())
      {
        return sda.getByMobileNum(mobile);
      }
    }


    public bool HasActiveRequest(Ship ship)
    {
      ShipDataAccess sda = null;
      ShipPositionRequestDataAccess sprda = null;

      try
      {
        sda = new ShipDataAccess();
        sprda = new ShipPositionRequestDataAccess();

        var sprs = sda.GetAllRequestsForShip(ship);

        foreach (var spr in sprs)
        {
          if (sprda.GetActiveShipPositionRequest(spr) != null)
            return true;
        }
        return false;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex);
        return false;
      }
      finally
      {
        if (sda != null)
          sda.Dispose();

        if (sprda != null)
          sprda.Dispose();
      }
    }

    public string GetShipState(Ship ship)
    {
      ShipDataAccess sda = null;
      StandingOrderDataAccess soda = null;
      
      try
      {
        sda     = new ShipDataAccess();
        soda    = new StandingOrderDataAccess();

        var spman = new ShipPositionManager();
        var ddpman = new DDPVersionManager();
        var ddpver = ddpman.GetCurrentDDPVersion();
        var lastpos = spman.GetLastShipPosition(ship.IMONum);
        List<StandingOrder> so = new List<StandingOrder>();


        if (lastpos != null)
          so = soda.GetOrdersForPosition(lastpos, ddpver);

        if (so.Count != 0)
          return "blue";

        if (sda.HasActiveRequest(ship) != 0)
          return "green";

        if (lastpos != null)
          if (DateTime.UtcNow - lastpos.TimeStamp > TimeSpan.FromDays(1))
            return "red";

        return "normal";
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex);
        return "normal";
      }
      finally
      {
        if (sda != null)
          sda.Dispose();
        if (soda != null)
        soda.Dispose();
      }
    }

    public static void ChangeShipStatus(ShipStatus status, Ship ship)
    {
      using (var sda = new ShipDataAccess())
      {
        sda.ChangeShipStatus( status , ship );
      }
    }

    public static string GetShipStatus(Ship ship)
    {
      using (var sda = new ShipDataAccess())
      {
        return sda.GetShipStatus(ship);
      }
    }


    public void ProcessPollResponse(PollResponse aspPollResponse)
    {

      switch (aspPollResponse.DeliveryNotification)
      {
        case PollResponse.EDeliveryNotification.Negative:
          {
            try
            {
              var ship = getByMobileNum(aspPollResponse.Mobile);
              ChangeShipStatus(ShipStatus.Error, ship);
              break;
            }
            catch (Exception ex)
            {
              //cancelar transaccion: no se pudo determinar para que barco corresponde el error
              // mandar cuerpo del mensaje
              break;
            }
            
          }
        case PollResponse.EDeliveryNotification.Positive:
          {
            var ship = getByMobileNum(aspPollResponse.Mobile);

            log.Info("La solicitud se envio correctamente al equipo, aguardar respuesta"); 
            break;
          }
        default :
          throw new NotImplementedException();
      }
    }


    public void CheckShipStatus()
    {
      var sda = new ShipDataAccess();

      List<Ship> ships = sda.GetAllBle();
      foreach (var ship in ships)
      {
        log.Debug("Hay barcos con respuesta demorada");
        ReprogramShip(ship);
      }
    }


    /// <summary>
    /// Reprograma el barco al periodo de request mas reciente
    /// </summary>
    /// <param name="ship">El barco a reprogramar</param>
    public static void ReprogramShip(Ship ship)
    {
      //REFACTOR: GetMinPeriodForShip en otro lado
      var asprda = new ActiveShipPositionRequestManager();
      int minPeriod = asprda.GetMinPeriodForShip(ship.IMONum, 0);

      //Quiere decir que hay que reprogramar cada 6hs
      if (minPeriod == -1)
        minPeriod = 360;

      //.AddMinutes(8) asegura un timeframe en el futuro
      var aspman = new AspManager();
      aspman.CreatePollMessage(DateTime.UtcNow.AddMinutes(8), minPeriod, ship, PollAction.Reprogram);
      ChangeShipStatus(ShipStatus.Polling, ship);
    }

    internal static Ship getByISN(string p)
    {
        using (var sda = new ShipDataAccess())
        {
            return sda.getByISN(p);
        }
    }
  }
}
