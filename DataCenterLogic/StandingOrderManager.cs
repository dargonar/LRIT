using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using System.Messaging;
using log4net;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de los standing orders
  /// </summary>
  public class StandingOrderManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(StandingOrderManager));
    /// <summary>
    /// Verifica si la ultima posicion de cada uno de nuestros barcos esta en alguna standing order.
    /// Tinen en cuenta hay un STOP para ese requestor.
    /// Tiene en cuenta la titulacion
    /// </summary>
    public void Process()
    {
      ShipDataAccess sda = null;

      try
      {
        sda = new ShipDataAccess();

        var cgm = new ContractingGovermentManager();
        var ddpm = new DDPVersionManager();
        var ddpVersion = ddpm.GetCurrentDDPVersion();

        log.Info("Procesando standing orders.");
        foreach (Ship ship in sda.GetAll())
        {
          ShipPosition pos = null;
          using (var spda = new ShipPositionDataAccess())
          {
            pos = spda.GetLastShipPosition(ship.IMONum);
          }
          
          if (pos == null)
          {
            log.Info(string.Format("No hay ultima posicion para {0}", ship.IMONum));
            continue;
          }

          List<StandingOrder> orders = new List<StandingOrder>();
          using (var soda = new StandingOrderDataAccess())
          {
            orders = soda.GetOrdersForPosition(pos, ddpVersion);
          }

          if (orders.Count == 0)
          {
            log.Info(string.Format("No hay ninguna SO para {0},posid:{1},ddpVer:{2}", ship.IMONum, pos.Id, ddpVersion.Id));
            continue;
          }

          var sprm = new ShipPositionReportManager();

          foreach (StandingOrder order in orders)
          {
            var cg = order.Place.ContractingGoverment;

            //Skipeo las SO de argentina (no chekeo si mis barcos estan en mis SO)
            if (cg.LRITId == 1005)
              continue;

            using (var asprda = new ActiveShipPositionRequestDataAccess())
            {
              if (asprda.IsStopRequired(ship, cg.LRITId.ToString()) == true)
              {
                log.Info("Stop Activo Para el Barco: " + ship.IMONum + " Requestor: " + cg.LRITId.ToString());
                continue;
              }
            }

            log.Info(string.Format("El barco {0} esta dentro de la standing order {1}", ship.IMONum, order.PlaceId));

            if (cgm.IsEntitled(cg, pos, ddpVersion, false) == false)
            {
              log.Info("no esta titulado para recibir la informacion, se envia recibo");
              ReceiptManager.SendReceipt(cg.LRITId.ToString(), "0", DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item0,
              string.Format("Not entitled to recieve"));

            }
            else
            {
              sprm.SendReport(cg.LRITId.ToString(), pos, "", DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item1, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1);
            }
          }
        }
      }
      catch (Exception ex)
      {
        log.Error("Error procesando StandingOrders", ex);
      }
      finally
      {
        sda.Dispose();
      }
    }
  }
}

