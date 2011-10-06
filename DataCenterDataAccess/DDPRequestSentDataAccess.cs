using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de DDPRequest enviados
  /// </summary>
  public class DDPRequestSentDataAccess : BaseDataAccess
  {

    public DDPRequestSentDataAccess() : base() { }
    public DDPRequestSentDataAccess(DBDataContext context) : base(context) { } 

    /// <summary>
    /// Crea un nuevo DDPRequest enviado a la base de datos
    /// </summary>
    /// <param name="ddpRequest">DDPRequestSent</param>
    public void Create(DDPRequestSent ddpRequest, int inOut)
    {
        ddpRequest.MsgInOut = new MsgInOut();
        ddpRequest.MsgInOut.DDPVersion = ddpRequest.DDPVersionNum;
        ddpRequest.MsgInOut.Destination = "";
        ddpRequest.MsgInOut.InOut = inOut;
        ddpRequest.MsgInOut.MsgId = ddpRequest.MessageId;
        ddpRequest.MsgInOut.MsgType = ddpRequest.MessageType;
        ddpRequest.MsgInOut.RefId = "";
        ddpRequest.MsgInOut.Source = ddpRequest.Originator;
        ddpRequest.MsgInOut.TimeStamp = ddpRequest.TimeStamp;

        context.DDPRequestSents.InsertOnSubmit(ddpRequest);
        context.SubmitChanges();
     
    }

    public IQueryable<DDPRequestSent> GetAll()
    {
        return context.DDPRequestSents;
    }

    public IQueryable<DDPRequestSent> GetAllBetween(DateTime fromDate, DateTime toDate)
    {
        return context.DDPRequestSents.Where(r => r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }


  }
}
