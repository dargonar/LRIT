using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataCenterDataAccess;
using log4net;
using System.Messaging;
using DataCenterLogic.DataCenterTypes;

namespace DataCenterLogic
{
  /// <summary>
  /// Administrador de los requerimientos de posicion
  /// </summary>
  class ShipPositionRequestManager
  {
    private static readonly ILog log = LogManager.GetLogger(typeof(ShipPositionRequestManager));
    /// <summary>
    /// Procesa un mensaje de tipo ShipPositionRequest
    /// </summary>
    /// <param name="msg">Mensaje de la cola con ShipPositionRequest en el body</param>
    public void ProcessShipPositionRequest(ShipPositionRequestType shipPositionRequest)
    {
      log.Info("Procesando ShipPositionRequest");
      try
      {
        if (ValidateShipPositionRequest(shipPositionRequest) != true)
          return;
      }
      catch (Exception ex)
      {
        log.Debug(ex);
      }

      Ship ship = null;

      using (var sdao = new ShipDataAccess())
      {
        log.Debug("Verifica si existe el barco en la base de datos");
        ship = sdao.getByIMONum(shipPositionRequest.IMONum);

        if (ship == null)
        {
          //The ship is not in our system, send receipt CODE 7
          log.Debug("el barco no esta en nuestro sistema, se envia recibo codigo 7");
          ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
            string.Format("The ship {0} is not registered", shipPositionRequest.IMONum));
          return;
        }
      }

      //Save ship position request to obtain id
      log.Debug("Guardando SPR en base de datos para obtener id");
      var shipPositionRequestDB = TypeHelper.Map2DB(shipPositionRequest);
      try
      {
        using (var spreq = new ShipPositionRequestDataAccess())
        {
          spreq.Create(shipPositionRequestDB, 0);
        }
      }
      catch (Exception ex)
      {
        log.Error("Hubo un problema al guardar el mensaje");
        log.Debug(ex);
        return;
      }


      
      //log.Debug("Verificando si ya hay un request activo para el barco");

      //using (var asprDa = new ActiveShipPositionRequestDataAccess())
      //{
      //  List<ActiveShipPositionRequest> asprlist = asprDa.GetAll();
      //  foreach (ActiveShipPositionRequest aspr in asprlist)
      //  {
      //    if (shipPositionRequest.IMONum == aspr.ShipPositionRequest.IMONum &&
      //        Convert.ToInt32(shipPositionRequest.AccessType) == aspr.ShipPositionRequest.AccessType &&
      //        shipPositionRequest.DataUserRequestor == aspr.ShipPositionRequest.DataUserRequestor
      //       )
      //    {
      //      log.Debug("Se reemplaza request del barco:" + shipPositionRequest.IMONum);
      //      asprDa.Remove(aspr);
      //    }
      //  }
      //}

      #region Historics position report
      log.Debug("Verificando si es un request para datos archivados");
      if (ShipPositionRequestHelper.IsHistoricRequest(shipPositionRequest.RequestType))
      {
        ProcessHistoricShipPositionRequest(shipPositionRequest, ship);
        return;
      }
      #endregion
      #region Periodic position report
      log.Debug("Verificando si es un request periodico");
      if (ShipPositionRequestHelper.IsPeriodicRequest(shipPositionRequest.RequestType))
      {
        ActiveShipPositionRequestManager asprManager = new ActiveShipPositionRequestManager();
        asprManager.AddOrReplace(shipPositionRequestDB);
        log.Debug("ASPR REPLACED: Barco:" + shipPositionRequest.IMONum);
        log.Debug("Requestor: " + shipPositionRequest.DataUserRequestor);
        log.Debug("Access-Request Type: " + shipPositionRequest.AccessType + "-" + shipPositionRequest.RequestType);
        

        //Es periodico, y nos saca de la frecuencia estandard 6hs (RequestType==6), y no es SAR (msgtype==5)?
        //Debemos cobrarle la presunta reprogramacion PRICING
        if (shipPositionRequest.RequestType != requestTypeType.Item6 && 
            shipPositionRequest.MessageType == messageTypeType1.Item5)
        {
          /************************************************/
          var pmgr = new PricingManager();
          decimal? price = pmgr.AddASPReprogrMessage(0, shipPositionRequest.DataUserRequestor, shipPositionRequest.DataUserProvider);
          log.Info(string.Format("ProcessShipPositionRequest: Reprogramacion presunta par {0} => precio={1}", shipPositionRequest.MessageId,price));
          /************************************************/
        }
        
        return;
      }
      #endregion
      #region One Time poll
      log.Debug("Verificando si es un request para unica vez");
      if (ShipPositionRequestHelper.IsOneTimePoll(shipPositionRequest.RequestType))
      {
        ActiveShipPositionRequestManager asprManager = new ActiveShipPositionRequestManager();
        //Crea Active
        asprManager.AddNew(shipPositionRequestDB.Id);
        log.Debug("ASPR ADDED: Barco:"+ shipPositionRequest.IMONum);
        log.Debug("Requestor: "+ shipPositionRequest.DataUserRequestor);
        log.Debug("Access-Request Type: "+ shipPositionRequest.AccessType +"-"+shipPositionRequest.RequestType);
      }
      #endregion
      #region Reset Request
      log.Debug("Verificando si es un request reset");
      if (ShipPositionRequestHelper.IsResetRequest(shipPositionRequest.RequestType))
      {
        var asprManager = new ActiveShipPositionRequestManager();
        asprManager.RemoveAllForRequestor(shipPositionRequestDB);
      }
      #endregion
      #region Stop Request
      log.Debug("Verificando si es un request Stop");
      if (ShipPositionRequestHelper.IsStopRequest(shipPositionRequest.RequestType))
      {
        ActiveShipPositionRequestManager asprManager = new ActiveShipPositionRequestManager();
        asprManager.AddOrReplace(shipPositionRequestDB);
        log.Debug("ASPR REPLACED: Barco:" + shipPositionRequest.IMONum);
        log.Debug("Requestor: " + shipPositionRequest.DataUserRequestor);
        log.Debug("Access-Request Type: " + shipPositionRequest.AccessType + "-" + shipPositionRequest.RequestType);
      }
      #endregion

      return;
    }

    /// <summary>
    /// Procesa un pedido de posicion historica.
    /// Los pedidos de posicion historicas son RequestType 7 y RequestType 9 
    /// </summary>
    /// <param name="shipPositionRequest">Mensaje de ShipPositionRequest</param>
    /// <param name="ship">Barco al que se hace referencia</param>
    private void ProcessHistoricShipPositionRequest(DataCenterLogic.DataCenterTypes.ShipPositionRequestType shipPositionRequest, Ship ship)
    {
      log.Info("Procesando ShipPositionRequest Historico");
      log.Debug("Obteniendo CgId de LRITId");
      
      //Obtener Contracting goverment ID
      ContractingGovermentManager cgmgr = new ContractingGovermentManager();
      var ddpm = new DDPVersionManager();
      var currentDDP = ddpm.GetCurrentDDPVersion();

      var contractingGoverment = cgmgr.GetContractingGovermentByLRITId(shipPositionRequest.DataUserRequestor, currentDDP.Id);
      if (contractingGoverment == null)
      {
        log.Info("CGID no válido se envia recibo");
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
        string.Format("Invalid Contracting Government ID"));
        return;
      }
      
      ShipPositionManager spm = new ShipPositionManager();
      
      List<ShipPosition> positions = new List<ShipPosition>();

      log.Debug("Verificando si se pide ultima posicion");
      if (ShipPositionRequestHelper.IsMostRecentPosition(shipPositionRequest.RequestType))
      {
        ShipPosition pos = spm.GetLastShipPosition(shipPositionRequest.IMONum);
        if (pos != null)
        {
          positions.Add(pos);
        }
      }
      else
      {
        log.Debug("No, se piden mas posiciones");
        log.Debug("Verificando titulacion");
        positions = spm.GetShipPositionHistory(shipPositionRequest.IMONum,
        shipPositionRequest.RequestDuration.startTime,
        shipPositionRequest.RequestDuration.stopTime);
      }

      if (positions.Count == 0)
      {
        log.Info("No hay posiciones");
        //The ship didnt send message in that period
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item6,
          string.Format("No archived data for the request"));
        return;
      }

      var configMgr = new ConfigurationManager();
      log.Debug("Procesando Posiciones");

      int not_entitled_count = 0;
      
      var ddpVersion = ddpm.DDPFromDate(DateTime.UtcNow);
      var cgOld = cgmgr.GetContractingGovermentByLRITId(shipPositionRequest.DataUserRequestor, ddpVersion.Id);
      
      foreach (ShipPosition position in positions)
      {
        bool verifyWatersOf = true;
        if ( shipPositionRequest.AccessType == accessTypeType.Item3 || shipPositionRequest.AccessType == accessTypeType.Item5 )
          verifyWatersOf = false;

        //Solo valido titulacion si el access type es distinto a 6 (SAR)
        if (shipPositionRequest.AccessType != accessTypeType.Item6 && (ddpVersion == null || cgmgr.IsEntitled(cgOld, position, ddpVersion, verifyWatersOf) == false) )
        {
          not_entitled_count++;
          continue;
        }

        var sprm = new ShipPositionReportManager();

        if (ShipPositionRequestHelper.IsMostRecentPosition(shipPositionRequest.RequestType))
          sprm.SendReport(shipPositionRequest.DataUserRequestor, position, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item4, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item3);
        else
        {
          if (shipPositionRequest.AccessType == accessTypeType.Item3 || shipPositionRequest.AccessType == accessTypeType.Item5)
            sprm.SendReport(shipPositionRequest.DataUserRequestor, position, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item3, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1);
          else if (shipPositionRequest.AccessType == accessTypeType.Item1)
            sprm.SendReport(shipPositionRequest.DataUserRequestor, position, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item1, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1);
          else if (shipPositionRequest.AccessType == accessTypeType.Item2)
            sprm.SendReport(shipPositionRequest.DataUserRequestor, position, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item2, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1);
          else
            sprm.SendReport(shipPositionRequest.DataUserRequestor, position, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.responseTypeType.Item4, DataCenterLogic.DataCenterTypesIDE.messageTypeType.Item1);
        }
        
        log.Debug("Report Enviado");
      }

      //Hubo alguna posicion para la que no tuvo permiso
      if (not_entitled_count != 0)
      {
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
        string.Format("Not entitled to recieve {0} positions", not_entitled_count));
        log.Debug("Receipt Enviado");
      }

      return;
    }

    /// <summary>
    /// Valida la combinacion de accesType y requestType para un ShipPositionRequest.
    /// Si hay algun error esta funcion envia un Receipt al requisitor, informandole el problema.
    /// </summary>
    /// <param name="shipPositionRequest">El mensaje ShipPositionRequest a ser analizado.</param>
    /// <returns>Verdadero, si el mensaje es valido, False otra cosa</returns>
    private bool ValidateShipPositionRequest(DataCenterLogic.DataCenterTypes.ShipPositionRequestType shipPositionRequest)
    {
      var ddpm = new DDPVersionManager();
      var ddpVersion = ddpm.GetCurrentDDPVersion();

      log.Info("Validando SPR");
      #region Port state with time trigger could not be used with RequestType 1,7 or 9:
      //log.Debug("Port state with time trigger could not be used with RequestType 1,7 or 9");
      if (shipPositionRequest.AccessType == DataCenterLogic.DataCenterTypes.accessTypeType.Item3 &&
          (shipPositionRequest.RequestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item1 ||
            shipPositionRequest.RequestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item7 ||
            shipPositionRequest.RequestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item9))
      {
        log.Error("Validacion erronea, se envia receipt");
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
          string.Format("Invalid request parameters: Port could not be used with RequestType 1,7 or 9"));
        return false;
      }
      #endregion
      #region Access type 0 only with message 4 and request 0
      //log.Debug("AccessType 0 (reset) solo valido con RT 0 y MS 4");
      if (shipPositionRequest.AccessType == DataCenterLogic.DataCenterTypes.accessTypeType.Item0 || shipPositionRequest.RequestType == requestTypeType.Item0 )
      {
        //Invalid request parameters: AccessType 0 only valid with MessageType 4
        if (shipPositionRequest.MessageType != DataCenterLogic.DataCenterTypes.messageTypeType1.Item4)
        {
          ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
            string.Format("Invalid request parameters: AccessType 0 only valid with MessageType 4"));
          log.Error("Validacion erronea, se envia receipt");
          return false;
        }

        // Invalid request parameters: AccessType 0 only valid with RequestType 0
        if (shipPositionRequest.RequestType != DataCenterLogic.DataCenterTypes.requestTypeType.Item0 || shipPositionRequest.AccessType != DataCenterLogic.DataCenterTypes.accessTypeType.Item0 )
        {
          ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
            string.Format("Invalid request parameters: AccessType 0 only valid with RequestType 0"));
          log.Info("Validacion erronea, se envia receipt");
          return false;
        }
      }
      #endregion
      #region REVISAR
      /*
      log.Debug("Port State");
      //Invalid request parameters: Port state and RequestTypes 1,7 and 9 only valid with AccessType 5
      if( shipPositionRequest.ItemElementName == DataCenterLogic.DataCenterTypes.ItemChoiceType.Port ||
          shipPositionRequest.ItemElementName == DataCenterLogic.DataCenterTypes.ItemChoiceType.PortFacility
          && ( shipPositionRequest.RequestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item1 ||
               shipPositionRequest.RequestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item7 ||
               shipPositionRequest.RequestType == DataCenterLogic.DataCenterTypes.requestTypeType.Item9 ) 
          && shipPositionRequest.AccessType != DataCenterLogic.DataCenterTypes.accessTypeType.Item5 )
      {
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
          string.Format("Invalid request parameters: Port state and RequestTypes 1,7 and 9 only valid with AccessType 5"));
       
        return false;
      } */
      #endregion
      #region Invalid request parameters: RequestTypes 1,7 and 9 are the only valid for MessageType 5
      //log.Debug("Si es mensaje de tipo 5 (Sar) RT solo puede ser 1 o 9");
      if (shipPositionRequest.MessageType == DataCenterLogic.DataCenterTypes.messageTypeType1.Item5 &&
          shipPositionRequest.RequestType != DataCenterLogic.DataCenterTypes.requestTypeType.Item1 &&
          shipPositionRequest.RequestType != DataCenterLogic.DataCenterTypes.requestTypeType.Item7 &&
          shipPositionRequest.RequestType != DataCenterLogic.DataCenterTypes.requestTypeType.Item9)
      {
        log.Error("Validacion erronea, se envia receipt");
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
          string.Format("Invalid request parameters: RequestTypes 1,7 and 9 are the only valid for MessageType 5"));
        return false;
      }
      #endregion
      #region Distance is only valid with access type 3
      log.Debug("Distance is only valid with access type 3");
      if (shipPositionRequest.Distance != "0" && shipPositionRequest.AccessType != DataCenterLogic.DataCenterTypes.accessTypeType.Item3)
      {
        log.Info("Validacion erronea, se envia receipt");
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
         string.Format("Invalid request parameters: Distance is only valid with access type 3"));
        return false;
      }
      #endregion
      #region Request Duration not valid for one time poll
      log.Debug("Request Duration not valid for one time poll");
      if (ShipPositionRequestHelper.IsOneTimePoll(shipPositionRequest.RequestType) &&
          (shipPositionRequest.RequestDuration!=null && (shipPositionRequest.RequestDuration.startTimeSpecified == true || shipPositionRequest.RequestDuration.stopTimeSpecified == true)))
      {
        log.Info("Validacion erronea, se envia receipt");
        ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
        string.Format("Invalid request parameters: Request Duration not valid for one time poll"));
        return false;
      }
      #endregion
      #region Port State
      log.Debug("Validacion Para PortStates");
      if (shipPositionRequest.AccessType == DataCenterLogic.DataCenterTypes.accessTypeType.Item3 ||
          shipPositionRequest.AccessType == DataCenterLogic.DataCenterTypes.accessTypeType.Item5)
      {
        if (shipPositionRequest.ItemElementName != ItemChoiceType.Port && shipPositionRequest.ItemElementName != ItemChoiceType.PortFacility)
        {
          log.Info("invalid fields for port state request, ItemElementName not port or portfacility");
          ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
          string.Format("Invalid fields for port state request, ItemElementName not port or portfacility"));
          return false;
        }
        string portName = shipPositionRequest.Item.ToString();
        using (var pda = new PlaceDataAccess())
        {
          if (pda.PortExists(portName, ddpVersion.Id) == false && pda.PortFacilityExists(portName, ddpVersion.Id) == false)
          {
            log.Info("invalid fields for port state request, Item" + shipPositionRequest.Item.ToString() + " doesn't exists");
            ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
            string.Format("invalid fields for port state request, Item" + shipPositionRequest.Item.ToString() + " doesn't exists")
            );
            return false;
          }
        }
      }
      #endregion
      #region AccessType2
      log.Debug("Validacion Para AccessType2");
      if (shipPositionRequest.AccessType == DataCenterLogic.DataCenterTypes.accessTypeType.Item2 &&
          shipPositionRequest.RequestType != requestTypeType.Item7)
      {
          ReceiptManager.SendReceipt(shipPositionRequest.DataUserRequestor, shipPositionRequest.MessageId, DataCenterLogic.DataCenterTypesIDE.receiptCodeType.Item7,
          string.Format("Access type 2 only valid for archived data"));
          return false;
      }
      #endregion
      //Verifica si existe el Data User que requiere la información
      var cgm = new ContractingGovermentManager();
      
      var contractingGoverment = cgm.GetContractingGovermentByLRITId(shipPositionRequest.DataUserRequestor, ddpVersion.Id);
      if (contractingGoverment == null)
      {
        string strError = string.Format("Specified LDU '{0}' does not exist", shipPositionRequest.DataUserRequestor);

        //Arma mensaje de Recibo
        DataCenterLogic.DataCenterTypes.ReceiptType receipt = new DataCenterLogic.DataCenterTypes.ReceiptType();

        var cmgr = new ConfigurationManager();

        receipt.DDPVersionNum = DDPVersionManager.currentDDP();
        receipt.Destination = shipPositionRequest.DataUserRequestor;
        receipt.Message = strError;
        receipt.MessageId = MessageIdManager.Generate();
        receipt.MessageType = DataCenterLogic.DataCenterTypes.messageTypeType3.Item7;
        receipt.Originator = cmgr.Configuration.DataCenterID;
        receipt.ReceiptCode = DataCenterLogic.DataCenterTypes.receiptCodeType.Item7;
        receipt.ReferenceId = shipPositionRequest.MessageId;
        receipt.schemaVersion = decimal.Parse(cmgr.Configuration.SchemaVersion);
        receipt.test = DataCenterLogic.DataCenterTypes.testType.Item1;
        receipt.TimeStamp = DateTime.UtcNow;

        Message msgout = new Message(receipt);
        msgout.Label = "receipt";

        //Encola mensaje
        QueueManager.Instance().EnqueueOut(msgout);

        log.Error(strError);
        return false;
      }

      log.Info("Validacion Ok");
      return true;
    }

    public ShipPositionRequest GetByMessageID(string msgid)
    {
      using (var sprmdao = new ShipPositionRequestDataAccess())
      {
        return sprmdao.GetByMessageId(msgid);
      }
    }

  }
}
