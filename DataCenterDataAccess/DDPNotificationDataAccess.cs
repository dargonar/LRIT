using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataCenterDataAccess
{
  /// <summary>
  /// Acesso a datos de DDPNotification
  /// </summary>
  public class DDPNotificationDataAccess : BaseDataAccess
  {
    public DDPNotificationDataAccess() : base() { }
    public DDPNotificationDataAccess(DBDataContext context) : base(context) { }
    
    /// <summary>
    /// Crea un nuevo DDPNotification en la base de datos
    /// </summary>
    /// <param name="ddpNotification">DDPNotification</param>
    public void Create( DDPNotification ddpNotification, int inOut )
    {

        ddpNotification.MsgInOut = new MsgInOut();
        ddpNotification.MsgInOut.DDPVersion = "";
        ddpNotification.MsgInOut.Destination = "";
        ddpNotification.MsgInOut.InOut = inOut;
        ddpNotification.MsgInOut.MsgId = ddpNotification.MessageId;
        ddpNotification.MsgInOut.MsgType = ddpNotification.MessageType;
        ddpNotification.MsgInOut.RefId = "";
        ddpNotification.MsgInOut.Source = "";
        ddpNotification.MsgInOut.TimeStamp = ddpNotification.TimeStamp;

        context.DDPNotifications.InsertOnSubmit(ddpNotification);
        context.SubmitChanges();
    }

    public IQueryable<DDPNotification> GetAll()
    {
        return context.DDPNotifications;
    }


    public IQueryable<DDPNotification> GetAllBetween(DateTime fromDate, DateTime toDate)
    {
        return context.DDPNotifications.Where(r => r.TimeStamp > fromDate && r.TimeStamp < toDate);
    }

  }
}
