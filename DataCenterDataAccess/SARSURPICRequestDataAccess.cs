using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de SARSURPICRequest
  /// </summary>
  public class SARSURPICRequestDataAccess : BaseDataAccess
  {
    public SARSURPICRequestDataAccess() : base() { }
    public SARSURPICRequestDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Crea un nuevo SARSURPICRequest en base de datos
    /// </summary>
    /// <param name="request"></param>
    public void Create(SARSURPICRequest request, int inOut)
    {
        request.MsgInOut = new MsgInOut();
        request.MsgInOut.DDPVersion = request.DDPVersionNum;
        request.MsgInOut.Destination = "";
        request.MsgInOut.InOut = inOut;
        request.MsgInOut.MsgId = request.MessageId;
        request.MsgInOut.MsgType = request.MessageType;
        request.MsgInOut.RefId = "";
        request.MsgInOut.Source = request.DataUserRequestor;
        request.MsgInOut.TimeStamp = request.TimeStamp;

        context.SARSURPICRequests.InsertOnSubmit(request);
        context.SubmitChanges();
    }

    public IQueryable<SARSURPICRequest> GetAll(int msgInOut)
    {
      return context.SARSURPICRequests.Where(r => r.MsgInOut.InOut == msgInOut);
    }

    public IQueryable<SARSURPICRequest> GetAllBetween(int msgInOut, DateTime fromDate, DateTime toDate)
    {
      return context.SARSURPICRequests.Where(r => r.MsgInOut.InOut == msgInOut && r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }
  }
}
