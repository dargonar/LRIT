using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;
using DataCenterDataAccess;
using DataCenterLogic;
using log4net;
using log4net.Config;

namespace DataCenterLogic
{
  public enum PollAction { Report, Reprogram, Stop }

  public class AspManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(AspManager));
    /// <summary>
    /// Envia el comando al ASP que sera transmitido al ASP
    /// </summary>
    /// <param name="startTime"></param>
    /// <param name="minutes">El periodo del reporte en minutos</param>
    /// <param name="ship"></param>
    /// <param name="poll"></param>
    public void CreatePollMessage(DateTime startTime, int minutes, Ship ship, PollAction poll)
    {
      //Only for satamatics
      if (ship.issata != 0)
      {
        var msg2 = new DataCenterLogic.DCASP.PollMessageSata();
        msg2.equipmentId = ship.Mobile;
        
        if (poll == PollAction.Reprogram)
        {
          msg2.reprog = true;
          msg2.minutes = minutes;
          log.Info("CreatePollMessage: enviando PollAction.Reprogram al ASP");
        }
        else if (poll == PollAction.Stop)
        {
          msg2.reprog = true;
          msg2.minutes = 0;
          log.Info("CreatePollMessage: enviando PollAction.Stop al ASP");
        }
        else if (poll == PollAction.Report)
        {
          msg2.reprog = false;
          msg2.minutes = 0;
          log.Info("CreatePollMessage: enviando PollAction.Report al ASP");
        }

        var cc2 = new DataCenterLogic.DCASP.LRITDCASPServiceSoapClient();
        if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
        {
          cc2.PollShipSata(msg2);
        }
        return;
      }
      
      var msg = new DataCenterLogic.DCASP.PollMessage();
      var spman = new ShipPositionManager();

      var pos = spman.GetLastShipPosition(ship.IMONum);
      if (pos != null)
      {
        msg.OceanRegion = (DCASP.EOceanRegion)pos.Region;
      }
      else msg.OceanRegion = DCASP.EOceanRegion.AORW;
      msg.DNID = ship.DNID;
      msg.PollType = DCASP.EPollType.IndividualPoll;
      msg.ResponseType = DCASP.EResponseType.DataReport;
      msg.SubAddress = DCASP.ESubAddress.Others;
      msg.Address = ship.Mobile;
      msg.MemberNumber = ship.Member;
      

      switch(poll)
      {
        case PollAction.Report:
          {
            msg.CommandType = DCASP.ECommandType.DataReport;
            msg.StartFrame = 0;
            msg.NumberOfReports = 0;
            ShipManager.ChangeShipStatus(ShipStatus.Polling, ship);
            log.Info("Enviando Solicitud de report a ASP");
            break;
          }

        case PollAction.Reprogram:
          {
            msg.CommandType = DCASP.ECommandType.ProgramDataReporting;
            msg.StartFrame = TimeToFrame(startTime.TimeOfDay);
            msg.NumberOfReports = MinutesToTimesPerDay(minutes);
            ShipManager.ChangeShipStatus(ShipStatus.Polling, ship);
            log.Info("Enviando Solicitud de report periodico a ASP");
            break;
          }
        case PollAction.Stop:
          {
            msg.CommandType = DCASP.ECommandType.StopDataReserving;
            msg.StartFrame = 0;
            msg.NumberOfReports = 0;
            ShipManager.ChangeShipStatus(ShipStatus.Polling, ship);
            log.Info("Enviando Solicitud de Stop a ASP");
            break;
          }
      }
      //UNCOMMENT!
      var cc = new DataCenterLogic.DCASP.LRITDCASPServiceSoapClient();
      if (System.Configuration.ConfigurationManager.AppSettings["send2servers"] != "False")
      {
        cc.PollShip(msg);
      }
    }



    /// <summary>
    /// Calcula la frecuencia diaria a partir de un intervalo definido en minutos
    /// </summary>
    /// <param name="minutes"></param>
    /// <returns>Cantidad de reportes por dia</returns>
    private int MinutesToTimesPerDay(int minutes)
    {
      return 1440 / minutes;
    }
    /// <summary>
    /// Convierte la hora del dia a TimeFrames (24hs = 10000 Frames)
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    private int TimeToFrame(TimeSpan timeSpan)
    {
      int frames = (Convert.ToInt32(timeSpan.TotalSeconds) * 10000 / 86400);
      return frames;
    }
  }
}
