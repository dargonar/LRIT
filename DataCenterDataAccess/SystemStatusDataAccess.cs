using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acceso a datos de SystemStatusSent
  /// </summary>
  public class SystemStatusDataAccess : BaseDataAccess
  {
    public SystemStatusDataAccess() : base() { }
    public SystemStatusDataAccess(DBDataContext context) : base(context) { }

    /// <summary>
    /// Crea un nuevo SystemStatusSent en base de datos
    /// </summary>
    /// <param name="systemStatusSent">SystemStatusSent</param>
    public void Create(SystemStatus systemStatus, int inOut)
    {
        systemStatus.MsgInOut = new MsgInOut();
        systemStatus.MsgInOut.DDPVersion = systemStatus.DDPVersionNum;
        systemStatus.MsgInOut.Destination = "";
        systemStatus.MsgInOut.InOut = inOut;
        systemStatus.MsgInOut.MsgId = systemStatus.MessageId;
        systemStatus.MsgInOut.MsgType = systemStatus.MessageType;
        systemStatus.MsgInOut.RefId = "";
        systemStatus.MsgInOut.Source = systemStatus.Originator;
        systemStatus.MsgInOut.TimeStamp = systemStatus.TimeStamp;

        context.SystemStatus.InsertOnSubmit(systemStatus);
        context.SubmitChanges();
    }

    public SystemStatus GetLastIncomingStatus()
    {
        DataLoadOptions options = new DataLoadOptions();
        options.LoadWith<SystemStatus>(c => c.MsgInOut);
        var sysst = context.SystemStatus.Where(s => s.MsgInOut.InOut == 0 && s.MessageType == 11).OrderByDescending(s => s.TimeStamp).FirstOrDefault();
        return sysst;
    }

    public SystemStatus GetLastAspStatus()
    {
      var sysst = context.SystemStatus.OrderByDescending(s => s.TimeStamp).FirstOrDefault(s => s.Message == "ASP OK");
      return sysst;
    }

    public void Insert(SystemStatus sysst)
    {
      context.SystemStatus.InsertOnSubmit(sysst);
      context.SubmitChanges();
    }

    public IQueryable<SystemStatus> GetAll()
    {
        return context.SystemStatus;
    }


    public IQueryable<SystemStatus> GetAllBetween(DateTime fromDate, DateTime toDate)
    {
        return context.SystemStatus.Where(r => r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }

  }
}
