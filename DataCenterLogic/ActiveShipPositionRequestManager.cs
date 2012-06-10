using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using System.Transactions;
using log4net.Config;
using log4net;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de los requerimientos activos de posicion
  /// </summary>
  public class ActiveShipPositionRequestManager
  {

    public string DCDebug { get; set; }
    private static readonly ILog log = LogManager.GetLogger(typeof(ActiveShipPositionRequestManager));
    /// <summary>
    /// Retorna el periodo minimo de poleo para un barco determinado en minutos
    /// </summary>
    /// <param name="IMONum">ship IMO number</param>
    /// <param name="sprId">Id del Ship Position Request, que debe ser omitido para hacer la verificacion</param>
    /// <returns>Periodo minimo para el barco, o menor que cero si no hay nada configurado</returns>
    public int GetMinPeriodForShip(string IMONum, int sprId)
    {
      log.Debug("Obteniendo el periodo minimo del barco");
      using (ActiveShipPositionRequestDataAccess da = new ActiveShipPositionRequestDataAccess())
      {
        int minReqType = da.GetMinPeriodicRequestTypeForShip(IMONum, sprId);
        return ShipPositionRequestHelper.GetMinutes(minReqType);
      }
    }

    /// <summary>
    /// Borra un requerimiento activo 
    /// </summary>
    /// <param name="spr"></param>
    public void Remove(ShipPositionRequest spr)
    {
      using (var asprda = new ActiveShipPositionRequestDataAccess())
      {
        asprda.Remove(spr);
      }
    }

    /// <summary>
    /// Esta es la funcion principal del manejador de pedidos activos.
    /// Itera y procesa todos los pedidos activo.
    /// Es encargada de enviar (cuando se cumplan las condiciones) los mensajes a la cola de salida.
    /// </summary>
    public void Process(int hour, int mins)  //hora: 0-23, minutes={0, 15, 30, 45}
    {
      
      //log.Info("Procesando Active Ship Position Request");
      using (var asprda = new ActiveShipPositionRequestDataAccess())
      {
        List<ActiveShipPositionRequest> asprlist = asprda.GetAll();
        foreach (ActiveShipPositionRequest aspr in asprlist)
        {

          try
          {
            //Verifica si el tiempo de inicio ya paso
            var spm = new ShipPositionManager();
            var spr = aspr.ShipPositionRequest;
            DateTime UtcNow = DateTime.UtcNow;
            //log.Debug("Verificando estado del ASPR");
            if (aspr.Status == 0)
            {
              //Condicion para activar el envio
              if (RequestReadyToActive(aspr, UtcNow) == true)
              {
                if (TryReprog(spr) == true)
                {
                  asprda.Update(aspr.Id, 1, UtcNow);
                  log.Debug("Reprogramado OK ==> estado ACTIVO");
                }
                else
                {
                  log.Error("Reprogramado ERROR");
                }
              }
            }
            else
            {
              //Status = 1
              if (spr.StopTime != null && UtcNow >= spr.StopTime)
              {
                var IMONum = aspr.ShipPositionRequest.IMONum;
                var currentReqType = ShipPositionRequestHelper.GetMinutes(aspr.ShipPositionRequest.RequestType);
                var minReqType = GetMinPeriodForShip(IMONum, 0);

                log.Debug("Se cumplio Stop Time del request eliminando request activo");

                asprda.Remove(aspr);

                //Verifica si el request tiene frecuencia distinta a la actual y reprograma si es necesario
                //si el request caduco reprograma para que reporte cada 6 horas
                log.Debug("Verificando si hay que reprogramar el equipo");
                if (minReqType == currentReqType)
                {
                  using (var sda = new ShipDataAccess())
                  {
                    int newMinPeriod = GetMinPeriodForShip(IMONum, 0);

                    if (newMinPeriod == -1)
                      newMinPeriod = 360;

                    //Reprogram ASP
                    try
                    {
                      var aspman = new AspManager();
                      aspman.CreatePollMessage(DateTime.UtcNow.AddMinutes(8), newMinPeriod, sda.getByIMONum(spr.IMONum), PollAction.Reprogram);
                    }
                    catch (Exception ex)
                    {
                      ShipManager.ChangeShipStatus(ShipStatus.Error, sda.getByIMONum(spr.IMONum));
                      log.Error("Hubo un error en la reprogramacion del barco" + Environment.NewLine + Environment.NewLine + ex + Environment.NewLine + Environment.NewLine);
                    }
                  }
                }
                //log.Debug("ASPR " + aspr.Id + " REMOVED: Status: " + aspr.Status + " Barco:" + spr.IMONum);
                //log.Debug("Requestor: " + spr.DataUserRequestor);
                //log.Debug("Access-Request Type: " + spr.AccessType + "-" + spr.RequestType);
                continue;

              }

              //log.Debug("Verificando si es request de unica vez");
              //ONE TIME POLL
              if (ShipPositionRequestHelper.IsOneTimePoll(spr.RequestType) == true)
              {
                //log.Debug("verificando si hay nueva posicion registrada");
                ShipPosition spos = spm.GetLastShipPosition(spr.IMONum);
                if (spos != null && spos.TimeStamp > aspr.LastTime)
                {
                  log.Debug("Nueva posicion con fecha > LastTime ... verificando titulacion");
                  CheckEntitlementAndSendReportOrReceipt(spr, spos);
                  asprda.Remove(aspr);
                  ShipManager.ChangeShipStatus(ShipStatus.Ok, ShipManager.getByIMONum(spr.IMONum));
                  log.Debug("ASPR " + aspr.Id + " REMOVED: Status: " + aspr.Status + " Barco:" + spr.IMONum);
                  log.Debug("Requestor: " + spr.DataUserRequestor);
                  log.Debug("Access-Request Type: " + spr.AccessType + "-" + spr.RequestType);
                  continue;
                }
              }
              //PERIODIC REPORT
              else if (ShipPositionRequestHelper.IsPeriodicRequest(spr.RequestType) == true)
              {
                if (spr.RequestType == 2 && new int[] { 0, 15, 30, 45 }.Contains(mins) == false) //15 minute periodic rate
                  continue;
                if (spr.RequestType == 3 && new int[] { 0, 30 }.Contains(mins) == false)   //30 minute periodic rate
                  continue;
                if (spr.RequestType == 4 && mins != 0)  //1 hour periodic rate
                  continue;
                if (spr.RequestType == 5 && (new int[] { 0, 3, 6, 9, 12, 15, 18, 21 }.Contains(hour) == false || mins != 0)) //3 hour periodic rate
                  continue;
                if (spr.RequestType == 6 && (new int[] { 0, 6, 12, 18 }.Contains(hour) == false || mins != 0))    //6 hour periodic rate
                  continue;
                if (spr.RequestType == 10 && (new int[] { 0, 12 }.Contains(hour) == false || mins != 0))    //12 hour periodic rate
                  continue;
                if (spr.RequestType == 11 && (hour != 0 || mins != 0))   //24 hour periodic rate
                  continue;

                int minutes = ShipPositionRequestHelper.GetMinutes(spr.RequestType);
                log.Debug("Procesando requerimiento periodico de " + minutes + " minutos");

                //TimeSpan delta = TimeSpan.FromMinutes(minutes);
                //if (DCDebug == "true")
                //  delta = TimeSpan.FromSeconds(minutes);
                //if (UtcNow >= aspr.LastTime.AddSeconds(delta.TotalSeconds))
                //{

                //log.Debug("se cumplio el periodo");
                ShipPosition spos = spm.GetLastShipPosition(spr.IMONum);
                if (spos != null)
                {
                  //La ultima posicion para el barco es mas nueva que la ultima que mande?
                  if (spos.TimeStamp > aspr.LastTime)
                  {
                    CheckEntitlementAndSendReportOrReceipt(spr, spos);
                    ShipManager.ChangeShipStatus(ShipStatus.Ok, ShipManager.getByIMONum(spr.IMONum));
                  }
                  else
                    if (spos.TimeStamp < aspr.LastTime)
                    {
                      //Feature-add: +30' estados del Active Ship Position Report
                      log.Info("No new position since last report");
                      ReceiptManager.SendReceipt(spr.DataUserRequestor, spr.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
                      string.Format("No new position since last report"));
                      return;
                    }
                  asprda.Update(aspr.Id, 1, spos.TimeStamp);
                  log.Debug("ASPR " + aspr.Id + " UPDATED: Status: " + aspr.Status + " Barco:" + spr.IMONum);
                  log.Debug("Requestor: " + spr.DataUserRequestor);
                  log.Debug("Access-Request Type: " + spr.AccessType + "-" + spr.RequestType);
                }
                else
                {
                  log.Error("Hubo un problema al verificar la titulacion y enviar la respuesta");
                }
              }
            }

          }
          catch (Exception ex)
          {
            log.Error("Error procesando ASPR", ex);
          }        
        } //foreach

      }
    }

    /// <summary>
    /// Verifica si se cumple alguna condicion de activacion para un request activo.
    /// Estas condiciones son:
    ///   * La fecha actual es mayor a la de inicio. (Si la fecha de inicio esta especificada)
    ///   * La distancia al port o portfacility es menor o igual que la especificada. (Si AccessType=3) 
    /// </summary>
    /// <param name="aspr"></param>
    /// <param name="UtcNow"></param>
    /// <returns></returns>
    private bool RequestReadyToActive(ActiveShipPositionRequest aspr, DateTime UtcNow)
    {
      var ddpm = new DDPVersionManager();
      var currentDDP = ddpm.GetCurrentDDPVersion();

      var spr = aspr.ShipPositionRequest;
      var spm = new ShipPositionManager();

      if (ShipPositionRequestHelper.IsOneTimePoll(spr.RequestType) == true)
        return true;

      if (spr.AccessType != 3 && spr.StartTimeSpecified != 0 && UtcNow >= spr.StartTime)
        return true;

      if (spr.AccessType == 3 && spm.IsShipInArea(spr.IMONum, spr.Item, double.Parse(spr.Distance), currentDDP.Id) == true)
        return true;

      return false;
    }


    public bool TryReprog(ShipPositionRequest spr)
    {
      bool reprogOk = true;

      /**************************************************************************************************/

      if (ShipPositionRequestHelper.IsOneTimePoll(spr.RequestType) == true)
      {
        var ship = ShipManager.getByIMONum(spr.IMONum);

        try
        {
          var aspman = new AspManager();
          // El 8 asegura un start frame posterior a la hora actual
          aspman.CreatePollMessage(DateTime.UtcNow.AddMinutes(8), 0, ship, PollAction.Report);
          log.Debug("Solitud Enviada (OneTimePoll) a IMO:" + ship.IMONum);
        }
        catch (Exception ex)
        {
          log.Error("Error al enviar la solicitud" + Environment.NewLine + Environment.NewLine + ex + Environment.NewLine + Environment.NewLine);
          reprogOk = false;
        }
                
        return reprogOk;
      }
      
      /**************************************************************************************************/

      int requestPeriod = ShipPositionRequestHelper.GetMinutes(spr.RequestType);
      log.Debug("Request de " + requestPeriod + " minutos");
      
      //Get faster active request in minutes
      int minPeriod = GetMinPeriodForShip(spr.IMONum, spr.Id);
      log.Debug("Request activo mas frecuente del barco de " + minPeriod + " minutos");
            
      
      //See if we need to reprogram the equipment on ship
      log.Debug("Verificando si es necesario reprogramar el equipo");
      if (requestPeriod < minPeriod || minPeriod == -1)
      {
        using (var sdao = new ShipDataAccess())
        {
          var ship = sdao.getByIMONum(spr.IMONum);
          //Reprogram ASP
          log.Debug("Se envia PollMessage");
          try
          {
            var aspman = new AspManager();
            // El 8 asegura un start frame posterior a la hora actual
            aspman.CreatePollMessage(DateTime.UtcNow.AddMinutes(8), requestPeriod, ship, PollAction.Reprogram);
          }
          catch (Exception ex)
          {
            log.Error("Hubo un problema al enviar la solicitud" + ex);
            reprogOk = false;
          }
          finally
          {

          }
        }
      }

      return reprogOk;
    }


    /// <summary>
    /// Verifica si el DataUserRequestor esta titulado para recibir un mensaje de posicion
    /// </summary>
    /// <param name="spr">Mensaje ShipPositionRequest</param>
    /// <param name="spos">Posicion del barco</param>
    private void CheckEntitlementAndSendReportOrReceipt(ShipPositionRequest spr, ShipPosition spos)
    {
      var cgm = new ContractingGovermentManager();
      var ddpm = new DDPVersionManager();
      
      //Febrero 2011
      var ddpVersion = ddpm.DDPFromDate(spos.TimeStamp);
      var contractingGoverment = cgm.GetContractingGovermentByLRITId(spr.DataUserRequestor, ddpVersion.Id);

      //Solo verifico aguas internas del que lo pide si el acceso es COASTAL o FLAG
      bool verifyWatersOf = true;
      if (spr.AccessType == 3 || spr.AccessType == 5)
        verifyWatersOf = false;

      //Solo verifico titulacion si es distinto a SAR
      if (spr.AccessType != 6 && cgm.IsEntitled(contractingGoverment, spos, ddpVersion, verifyWatersOf) == false)
      {
        log.Debug("No titulado");
        ReceiptManager.SendReceipt(spr.DataUserRequestor, spr.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item0,
        string.Format("Not entitled to recieve"));
        return;
      }

      var sprm = new ShipPositionReportManager();
      
      /*************************** MEDITAR *****************************/

      //Default Coastal
      var responseType = DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item1;
      //Periodic report
      var messageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1;
      
      //Flag
      if( spr.AccessType == 2 )
        responseType = DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item2;
      //Port
      else 
      if( spr.AccessType == 3 || spr.AccessType == 5 )
        responseType = DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item3;
      //Coastal
      else
      if (spr.AccessType == 1)
      {
        responseType = DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item1;
      }
      //SAR
      else
      {
        responseType = DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item4;
        messageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item3;
      }
      
      //si es onetimepoll => msgtype polled report
      if( spr.RequestType == 1 )
        messageType = DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item3;

      /*************************************************************/

      sprm.SendReport(spr.DataUserRequestor, spos, spr.MessageId, responseType,  messageType);
    }

    /// <summary>
    /// Borra todos los requerimientos activos para un requestor en particular
    /// </summary>
    /// <param name="spr">ShipPositionRequest</param>
    public void RemoveAllForRequestor(ShipPositionRequest spr)
    {
      using (var asprda = new ActiveShipPositionRequestDataAccess())
      {
        asprda.RemoveAllForRequestor(spr);
      }
    }

    public void AddOrReplace(ShipPositionRequest shipPositionRequest)
    {
      using (var da = new ActiveShipPositionRequestDataAccess())
      {
        da.AddOrReplace(shipPositionRequest);
      }
    }

    /// <summary>
    /// Crea un nuevo request activo
    /// </summary>
    /// <param name="requestId">ID del ShipPositionRequest al que hace referencia</param>
    public void AddNew(int requestId)
    {
      //Prepare new active request
      ActiveShipPositionRequest newActiveRequest = new ActiveShipPositionRequest();
      newActiveRequest.LastMessage = "Added";
      newActiveRequest.RequestId = requestId;
      newActiveRequest.Status = 0;
      newActiveRequest.LastTime = DateTime.UtcNow;

      using (var da = new ActiveShipPositionRequestDataAccess())
      {
        da.Create(newActiveRequest);
      }
    }


  }
}
