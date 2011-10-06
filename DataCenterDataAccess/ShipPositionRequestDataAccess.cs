using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de ShipPositionRequest
  /// </summary>
  public class ShipPositionRequestDataAccess : BaseDataAccess
  {

    public ShipPositionRequestDataAccess() : base() { }
    public ShipPositionRequestDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Crea un nuevo ShipPositionRequest en base de datos
    /// </summary>
    /// <param name="shipPositionRequest">ShipPositionRequest</param>
    /// <returns>ID del nuevo registro creado</returns>
    public int Create(ShipPositionRequest shipPositionRequest, int inOut)
    {
      shipPositionRequest.MsgInOut = new MsgInOut();
      shipPositionRequest.MsgInOut.DDPVersion = shipPositionRequest.DDPVersionNum;
      shipPositionRequest.MsgInOut.Destination = shipPositionRequest.DataUserProvider;
      shipPositionRequest.MsgInOut.InOut = inOut;
      shipPositionRequest.MsgInOut.MsgId = shipPositionRequest.MessageId;
      shipPositionRequest.MsgInOut.MsgType = shipPositionRequest.MessageType;
      shipPositionRequest.MsgInOut.RefId = "";
      shipPositionRequest.MsgInOut.Source = shipPositionRequest.DataUserRequestor;
      shipPositionRequest.MsgInOut.TimeStamp = shipPositionRequest.TimeStamp;

      context.ShipPositionRequests.InsertOnSubmit(shipPositionRequest);
      context.SubmitChanges();
      return shipPositionRequest.Id;
    }

    public ActiveShipPositionRequest GetActiveShipPositionRequest(ShipPositionRequest spr)
    {
      return context.ActiveShipPositionRequests.SingleOrDefault(a => a.ShipPositionRequest.Id == spr.Id);
    }

    public IQueryable<ShipPositionRequest> GetAll(int msgInOut)
    {
      return context.ShipPositionRequests.Where(r => r.MsgInOut.InOut == msgInOut);
    }

    public IQueryable<ShipPositionRequest> GetActives()
    {
      return context.ShipPositionRequests.Where(r => r.ActiveShipPositionRequests.Count != 0);
    }

    public IQueryable<ShipPositionRequest> GetAllBetween(int msgInOut, DateTime fromDate, DateTime toDate)
    {
      return context.ShipPositionRequests.Where(r => r.MsgInOut.InOut == msgInOut && r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }

    public bool RequestExists(string msgid)
    {
      return (context.ShipPositionRequests.Where(r => r.MessageId == msgid).ToList().Count != 0);
    }


    public ShipPositionRequest GetByMessageId(string msgid)
    {
      return context.ShipPositionRequests.SingleOrDefault(r => r.MessageId == msgid);
    }
  }
}